using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace Economy {

	public class Industry {

        public int id;
        public string name;
        public IndustryGroup group;
        public int tier;
        public IndustryStatus status;
        public string statusMessage;
        public float cost;
        public bool playerOwned;

        public List<IndustryUpgrade> availableIndustryUpgrades;
        public List<IndustryUpgrade> appliedIndustryUpgrades;
        public List<Recipe> recipes;
        public int? activeRecipe;
        public List<int> forcedActiveRecipes;

        private int ticks;
		
		private Dictionary<int, int> resourcesProduced;

		private EconomySingleton economyStatics;

        // EconomySingleton is necessary as an argument or it gets stuck in the static initialization
        public Industry(EconomySingleton economyStatics, int id, string name, Recipe recipe, IndustryGroup group, int tier) {
			this.id = id;
			this.name = name;
			this.economyStatics = economyStatics;
			status = IndustryStatus.STARTUP;
			statusMessage = "Starting up " + name + ".";
			recipes = new List<Recipe>();
			recipes.Add(recipe);
			activeRecipe = 0;
			this.group = group;
			this.tier = tier;
			resourcesProduced = new Dictionary<int, int>();
			approximateCost();
            playerOwned = false;
		}
		
		public void approximateCost() {
			
			float avg = 0.0f;
			
			foreach(Recipe recipe in recipes) {
				float iterationInput = 0;
				foreach(int resourceId in recipe.getInputs()) {
                    iterationInput += economyStatics.getResource(resourceId).baseValue * recipe.getInputRatio(resourceId);
				}
				float iterationOutput = 0;
				foreach(int resourceId in recipe.getOutputs()) {
					iterationOutput += economyStatics.getResource(resourceId).baseValue * recipe.getOutputPercent(resourceId)/100 * recipe.getOutputRatio(resourceId);
				}
				avg += iterationOutput - iterationInput;
				if(iterationOutput < iterationInput) {
					//System.out.println("Warn: Industry output is less valuable than input! " + iterationOutput + " <- " + iterationInput);
				}
			}
			
			avg /= recipes.Count;
			
			avg *= group.getIndustryPriceModifier();
			
			avg *= (50.0f * (3.0f + tier));
			cost = Mathf.Round(avg);
			//System.out.println(name + "\n" + avg + " -- " + " Even: " + (50 * (3 + tier) * group.getIndustryPriceModifier()));
		}
		
		public List<IndustryUpgrade> listAvailableIndustryUpgrades() {
			if(availableIndustryUpgrades == null) {
				availableIndustryUpgrades = new List<IndustryUpgrade>();
			}
			List<IndustryUpgrade> visibleUpgrades = new List<IndustryUpgrade>();
			
			if(appliedIndustryUpgrades != null) {
                Dictionary<int, int> tierDict = new Dictionary<int, int>();
				
				foreach(IndustryUpgrade u in appliedIndustryUpgrades) {
					if(u.getTierCategory() != null) {
						int? t = null;
                        int tier;
                        if(tierDict.TryGetValue(u.getTierCategory().Value, out tier)) {
                            tierDict[u.getTierCategory().Value] = Mathf.RoundToInt(Mathf.Max((float)u.getTier(), (float)t));
                        } else {
                            tierDict[u.getTierCategory().Value] = u.getTier().Value;
                        }
					}
				}
				
				foreach(IndustryUpgrade u in availableIndustryUpgrades) {
					if(u.getTierCategory() != null) {
                        int value;
                        if(tierDict.TryGetValue(u.getTierCategory().Value, out value)) { 
							if(u.getTier() <= tierDict[u.getTierCategory().Value] + 1) {
								visibleUpgrades.Add(u);
							}
						} else {
							if(u.getTier() == 0) {
								visibleUpgrades.Add(u);
							}
						}
					} else {
						visibleUpgrades.Add(u);
					}
				}
			} else {
				foreach(IndustryUpgrade u in availableIndustryUpgrades) {
					if((u.getTierCategory() != null && u.getTier() == 0) || u.getTierCategory() == null) {
						visibleUpgrades.Add(u);
					}
				}
				
			}
			return visibleUpgrades;
		}
		
		public List<IndustryUpgrade> listAppliedIndustryUpgrades() {
			if(appliedIndustryUpgrades == null) {
				appliedIndustryUpgrades = new List<IndustryUpgrade>();
			}
			return appliedIndustryUpgrades;
		}
		
		public void applyUpgrade(IndustryUpgrade upgrade) {
			if(availableIndustryUpgrades == null) {
				return;
			}
			if(!availableIndustryUpgrades.Remove(upgrade)) {
				//System.out.println("Trying to apply an upgrade that doesn't match the available upgrades");
				return;
			}
			
			if(appliedIndustryUpgrades == null) {
				appliedIndustryUpgrades = new List<IndustryUpgrade>();
			}
			appliedIndustryUpgrades.Add(upgrade);
			
			return;
		}
		
		public void disable() {
			status = IndustryStatus.DISABLED;
			statusMessage = "Industry Disabled.";
			ticks = 0;
		}
		
		public void enable() {
			status = IndustryStatus.STARTUP;
			statusMessage = "Industry Restarting.";
		}

		public string toString() {
			StringBuilder sb = new StringBuilder(name);
			sb.Append(":\n");
			foreach(Recipe recipe in recipes) {
				sb.Append(recipe.tostring());
			}
			return sb.ToString();
		}

		public Dictionary<int, int> getAllInputs() {
            Dictionary<int, int> inputMap = new Dictionary<int, int>();
            foreach(Recipe recipe in recipes) {
				foreach(int resourceId in recipe.getInputs()) {
                    if(inputMap.ContainsKey(resourceId)) {
                        inputMap[resourceId]++;
                    } else {
                        inputMap.Add(resourceId, 1);
                    }
				}
			}
			return inputMap;
		}

        public Dictionary<int, int> getAllOutputs() {
            Dictionary<int, int> outputMap = new Dictionary<int, int>();
            foreach (Recipe recipe in recipes) {
				foreach(int resourceId in recipe.getOutputs()) {
                    if (outputMap.ContainsKey(resourceId)) {
                        outputMap[resourceId]++;
                    } else {
                        outputMap.Add(resourceId, 1);
                    }
                }
            }
			return outputMap;
		}
		
		/**
		 * Runs an industry. The industry will pull resources from and to the resourceStockMap.
		 * 
		 * @param resourceStockMap 
		 * @return 0 if the industry is not running
		 * 1 if the industry is running
		 * 2 if the industry has written to the resourceStockMap (retry other halted industries)
		 */
		public IndustryRunResult run(Dictionary<int,int> resourceStockDictionary) {
			switch(status) {
			case IndustryStatus.DISABLED:
				return IndustryRunResult.DISABLED;
			case IndustryStatus.RUNNING:
				if(activeRecipe == null) {
					Debug.LogError ("Severe: Active recipe is null as the industry is running!");
				}
				ticks++;
				if(ticks == recipes[activeRecipe.Value].getRate()) {
					tryStart(resourceStockDictionary);
				} else {
					return IndustryRunResult.RUNNING;
				}
				outputResources(resourcesProduced);
				outputResources(resourceStockDictionary);
				return IndustryRunResult.PRODUCED;
			case IndustryStatus.STARTUP:
			default:
				return tryStart(resourceStockDictionary);
			}
		}
		
		private IndustryRunResult tryStart(Dictionary<int, int> resourceStockDictionary) {
			if(canRun(resourceStockDictionary)) {
				ticks = 1;
				status = IndustryStatus.RUNNING;
				statusMessage = "Industry Running smoothly.";
				return IndustryRunResult.RUNNING;
			} else {
				status = IndustryStatus.OUT_OF_RESOURCES;
				return IndustryRunResult.HALTED;
			}
		}
		
		private bool canRun(Dictionary<int, int> resourceStockDictionary) {
			foreach(Recipe recipe in recipes) {
				bool canRun = true;
				foreach(int resourceId in recipe.getInputs()) {
					int resourceStock;
                    if(resourceStockDictionary.TryGetValue(resourceId, out resourceStock)) {
                        // Insufficient resources
                        if (resourceStock < recipe.getInputRatio(resourceId)) {
                            statusMessage = "Industry Halted. Missing " + resourceId + ".";
                            canRun = false;
                            break;
                        }
                    } else {
                        // Missing resources completely
                        canRun = false;
                        break;
                    }
				}
				if(canRun) {
					activeRecipe = recipes.IndexOf(recipe);
					if(forcedActiveRecipes != null && forcedActiveRecipes.Count > 0 && !forcedActiveRecipes.Contains(activeRecipe.Value)) {
						continue;
					}
					foreach(int resourceId in recipe.getInputs()) {
                        resourceStockDictionary[resourceId] -= recipe.getInputRatio(resourceId);
					}
					return true;
				}
			}
			return false;
		}
		
		/**
		 * Write the output of the industry into the resourceStockMap
		 * @param industryResourceStockMap
		 */
		private void outputResources(Dictionary<int, int> resourceStockDictionary) {
			//Random random = new Random();
			foreach(int resourceId in recipes[activeRecipe.Value].getOutputs()) {
				int resourceCount = 0;
				for(int i = 0; i < recipes[activeRecipe.Value].getOutputRatio(resourceId); i++) {
					if(Random.Range(0.0f, 100.0f) <= recipes[activeRecipe.Value].getOutputPercent(resourceId)) {
						resourceCount++;
					}
				}
                int value;
                if(resourceStockDictionary.TryGetValue(resourceId, out value)) {
                    resourceStockDictionary[resourceId] += resourceCount;
                } else {
                    resourceStockDictionary.Add(resourceId, resourceCount);
                }
            }
		}
		
		public void addActiveRecipe(int index) {
			if(forcedActiveRecipes == null) {
				forcedActiveRecipes = new List<int>();
			}
			forcedActiveRecipes.Add(index);
			if(!forcedActiveRecipes.Contains(activeRecipe.Value)) {
				ticks = 0;
				activeRecipe = index;
				status = IndustryStatus.STARTUP;
				statusMessage = "Industry Restarting.";
			}
		}
		
		public void removeActiveRecipe(int index) {
			if(forcedActiveRecipes == null) {
				return;
			} else {
				forcedActiveRecipes.Remove(index);
			}
		}
		
		public void clearActiveRecipes() {
			forcedActiveRecipes = null;
			ticks = 0;
			status = IndustryStatus.STARTUP;
			statusMessage = "Industry Restarting.";
		}
		
		public string getName() {
			return name;
		}
		
		public string getStatusMessage() {
			return statusMessage;
		}
		
		public void addRecipe(int index, Recipe recipe) {
			recipes.Add(recipe);
		}
		
		public int getId() {
			return id;
		}
	}	

}
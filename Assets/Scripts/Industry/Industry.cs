using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Industry {

	public class Industry {
		
		private int id;
		private string name;
		private IndustryGroup group;
		private int tier;
		private IndustryStatus status;
		private string statusMessage;
		private float cost;
		
		private List<IndustryUpgrade> availableIndustryUpgrades;
		private List<IndustryUpgrade> appliedIndustryUpgrades;
		private List<Recipe> recipes;
		private int? activeRecipe;
		private List<int> forcedActiveRecipes;
		
		private int ticks;
		
		private IntegerMap resourcesProduced;

		private EconomyStatics economyStatics;
		
		public Industry(EconomyStatics economyStatics, int id, string name, Recipe recipe, IndustryGroup group, int tier) {
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
			resourcesProduced = new IntegerMap();
			approximateCost();
		}
		
		public void approximateCost() {
			
			float avg = 0.0f;
			
			foreach(Recipe recipe in recipes) {
				float iterationInput = 0;
				foreach(int resourceId in recipe.getInputs()) {
					iterationInput += economyStatics.getResource(resourceId).getBaseValue() * recipe.getInputRatio(resourceId);
				}
				float iterationOutput = 0;
				foreach(int resourceId in recipe.getOutputs()) {
					iterationOutput += economyStatics.getResource(resourceId).getBaseValue() * recipe.getOutputPercent(resourceId)/100 * recipe.getOutputRatio(resourceId);
				}
				avg += iterationOutput - iterationInput;
				if(iterationOutput < iterationInput) {
					//System.out.println("Warn: Industry output is less valuable than input! " + iterationOutput + " <- " + iterationInput);
				}
			}
			
			avg /= recipes.Count;
			
			avg *= group.getIndustryPriceModifier();
			
			avg *= (50.0f * (3.0f + tier));
			this.cost = Mathf.Round(avg);
			//System.out.println(name + "\n" + avg + " -- " + " Even: " + (50 * (3 + tier) * group.getIndustryPriceModifier()));
		}
		
		public List<IndustryUpgrade> listAvailableIndustryUpgrades() {
			if(availableIndustryUpgrades == null) {
				availableIndustryUpgrades = new List<IndustryUpgrade>();
			}
			List<IndustryUpgrade> visibleUpgrades = new List<IndustryUpgrade>();
			
			if(appliedIndustryUpgrades != null) {
				IntegerMap tierMap = new IntegerMap();
				
				foreach(IndustryUpgrade u in appliedIndustryUpgrades) {
					if(u.getTierCategory() != null) {
						int? t = null;
						t = tierMap.get(u.getTierCategory().Value);
						if(t != null) {
							tierMap.put (u.getTierCategory().Value,  Mathf.RoundToInt(Mathf.Max((float) u.getTier(), (float) t)));
						} else {
							tierMap.put(u.getTierCategory().Value, u.getTier().Value);
						}
					}
				}
				
				foreach(IndustryUpgrade u in availableIndustryUpgrades) {
					if(u.getTierCategory() != null) {
						if(tierMap.get(u.getTierCategory().Value) != 0) {
							if(u.getTier() <= tierMap.get(u.getTierCategory().Value) + 1) {
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
		public IntegerMap getInputs() {
			IntegerMap map = new IntegerMap();
			foreach(Recipe recipe in recipes) {
				foreach(int resourceId in recipe.getInputs()) {
					map.add(resourceId, 1);
				}
			}
			return map;
		}
		
		public IntegerMap getOutputs() {
			IntegerMap map = new IntegerMap();
			foreach(Recipe recipe in recipes) {
				foreach(int resourceId in recipe.getOutputs()) {
					map.add(resourceId, 1);
				}
			}
			return map;
		}
		
		/**
		 * Runs an industry. The industry will pull resources from and to the resourceStockMap.
		 * 
		 * @param resourceStockMap 
		 * @return 0 if the industry is not running
		 * 1 if the industry is running
		 * 2 if the industry has written to the resourceStockMap (retry other halted industries)
		 */
		public IndustryRunResult run(IntegerMap resourceStockMap) {
			switch(status) {
			case IndustryStatus.DISABLED:
				return IndustryRunResult.DISABLED;
			case IndustryStatus.RUNNING:
				if(activeRecipe == null) {
					Debug.Log ("Severe: Active recipe is null as the industry is running!");
				}
				ticks++;
				if(ticks == recipes[activeRecipe.Value].getRate()) {
					tryStart(resourceStockMap);
				} else {
					return IndustryRunResult.RUNNING;
				}
				outputResources(resourcesProduced);
				outputResources(resourceStockMap);
				return IndustryRunResult.PRODUCED;
			case IndustryStatus.STARTUP:
			default:
				return tryStart(resourceStockMap);
			}
		}
		
		private IndustryRunResult tryStart(IntegerMap resourceStockMap) {
			if(canRun(resourceStockMap)) {
				ticks = 1;
				status = IndustryStatus.RUNNING;
				statusMessage = "Industry Running smoothly.";
				return IndustryRunResult.RUNNING;
			} else {
				status = IndustryStatus.OUT_OF_RESOURCES;
				return IndustryRunResult.HALTED;
			}
		}
		
		private bool canRun(IntegerMap resourceStockMap) {
			foreach(Recipe recipe in recipes) {
				bool canRun = true;
				foreach(int resourceId in recipe.getInputs()) {
					int resourceStock = resourceStockMap.get(resourceId);
					if(resourceStock < recipe.getInputRatio(resourceId)) {
						statusMessage = "Industry Halted. Missing " + resourceId + ".";
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
						resourceStockMap.add(resourceId, resourceStockMap.get(resourceId) - recipe.getInputRatio(resourceId));
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
		private void outputResources(IntegerMap industryResourceStockMap) {
			Random random = new Random();
			foreach(int resourceId in recipes[activeRecipe.Value].getOutputs()) {
				int resourceCount = 0;
				for(int i = 0; i < recipes[activeRecipe.Value].getOutputRatio(resourceId); i++) {
					if(Random.Range(0.0f, 100.0f) <= recipes[activeRecipe.Value].getOutputPercent(resourceId)) {
						resourceCount++;
					}
				}
				addIndustryResource(industryResourceStockMap, resourceId, resourceCount);
			}
		}
		
		private void addIndustryResource(IntegerMap industryResourceStockMap, int resourceId, int amount) {
			industryResourceStockMap.add(resourceId, amount);
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
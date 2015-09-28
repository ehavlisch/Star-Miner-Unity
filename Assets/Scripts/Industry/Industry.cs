﻿using UnityEngine;
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
		private int activeRecipe;
		private List<int> forcedActiveRecipes;
		
		private int ticks;
		
		private Dictionary<string, int> resourcesProduced;
		
		public Industry(int id, string name, Recipe recipe, IndustryGroup group, int tier) {
			this.id = id;
			this.name = name;
			status = IndustryStatus.STARTUP;
			statusMessage = "Starting up " + name + ".";
			recipes = new List<Recipe>();
			recipes.Add(recipe);
			activeRecipe = 0;
			this.group = group;
			this.tier = tier;
			resourcesProduced = new Dictionary<string, int>();
			approximateCost();
		}
		
		public void approximateCost() {
			
			float avg = 0.0f;
			
			foreach(Recipe recipe in recipes) {
				float iterationInput = 0;
				foreach(Resource resource in recipe.getInputs()) {
					iterationInput += resource.getBaseValue() * recipe.getInputRatio(resource);
				}
				float iterationOutput = 0;
				foreach(Resource resource in recipe.getOutputs()) {
					iterationOutput += resource.getBaseValue() * recipe.getOutputPercent(resource)/100 * recipe.getOutputRatio(resource);
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
				Dictionary<int, int> tierMap = new Dictionary<int, int>();
				
				foreach(IndustryUpgrade u in appliedIndustryUpgrades) {
					if(u.getTierCategory() != null) {
						int? t = null;
						t = tierMap.get_Item(u.getTierCategory);
						if(t != null) {
							tierMap.Item[u.getTierCategory] =  Mathf.Max(u.getTier(), t);
						} else {
							tierMap.put(u.getTierCategory(), u.getTier());
						}
					}
				}
				
				foreach(IndustryUpgrade u in availableIndustryUpgrades) {
					if(u.getTierCategory() != null) {
						if(tierMap.get(u.getTierCategory()) != null) {
							if(u.getTier() <= tierMap.get(u.getTierCategory()) + 1) {
								visibleUpgrades.add(u);
							}
						} else {
							if(u.getTier() == 0) {
								visibleUpgrades.add(u);
							}
						}
					} else {
						visibleUpgrades.add(u);
					}
				}
			} else {
				foreach(IndustryUpgrade u in availableIndustryUpgrades) {
					if((u.getTierCategory() != null && u.getTier() == 0) || u.getTierCategory() == null) {
						visibleUpgrades.add(u);
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
			if(!availableIndustryUpgrades.remove(upgrade)) {
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
		
		public string tostring() {
			StringBuilder sb = new StringBuilder(name);
			sb.Append(":\n");
			Iterator<Recipe> it = recipes.iterator();
			while(it.hasNext()) {
				Recipe recipe = it.next();
				sb.Append(recipe);
			}
			return sb.Tostring();
		}
		
		public HashSet<Resource> getInputs() {
			HashSet<Resource> resources = new HashSet<Resource>();
			foreach(Recipe recipe in recipes) {
				resources.addAll(recipe.getInputs());
			}
			return resources;
		}
		
		public HashSet<Resource> getOutputs() {
			HashSet<Resource> resources = new HashSet<Resource>();
			foreach(Recipe recipe in recipes) {
				resources.addAll(recipe.getOutputs());
			}
			return resources;
		}
		
		/**
		 * Runs an industry. The industry will pull resources from and to the resourceStockMap.
		 * 
		 * @param resourceStockMap 
		 * @return 0 if the industry is not running
		 * 1 if the industry is running
		 * 2 if the industry has written to the resourceStockMap (retry other halted industries)
		 */
		public IndustryRunResult run(Dictionary<string, int> resourceStockMap) {
			switch(status) {
			case IndustryStatus.DISABLED:
				return IndustryRunResult.DISABLED;
			case IndustryStatus.RUNNING:
				ticks++;
				if(ticks == recipes.get(activeRecipe).getRate()) {
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
		
		private IndustryRunResult tryStart(Dictionary<string, int> resourceStockMap) {
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
		
		private bool canRun(Dictionary<string, int> resourceStockMap) {
			foreach(Recipe recipe in recipes) {
				bool canRun = true;
				foreach(Resource r in recipe.getInputs()) {
					int resourceStock = resourceStockMap.get(r.getId());
					if(resourceStock == null || resourceStock < recipe.getInputRatio(r)) {
						statusMessage = "Industry Halted. Missing " + r.getName() + ".";
						canRun = false;
						break;
					}
				}
				if(canRun) {
					activeRecipe = recipes.indexOf(recipe);
					if(forcedActiveRecipes != null && forcedActiveRecipes.size() > 0 && !forcedActiveRecipes.contains(activeRecipe)) {
						continue;
					}
					foreach(Resource r in recipe.getInputs()) {
						resourceStockMap.Add(r.getId(), resourceStockMap.get(r.getId()) - recipe.getInputRatio(r));
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
		private void outputResources(Dictionary<string, int> industryResourceStockMap) {
			Random random = new Random();
			foreach(Resource r in recipes.get(activeRecipe).getOutputs()) {
				int resourceCount = 0;
				for(int i = 0; i < recipes.Find(activeRecipe).getOutputRatio(r); i++) {
					if(random.nextInt(100) <= recipes.Find(activeRecipe).getOutputPercent(r)) {
						resourceCount++;
					}
				}
				addIndustryResource(industryResourceStockMap, r.getId(), resourceCount);
			}
		}
		
		private void addIndustryResource(Dictionary<string, int> industryResourceStockMap, string resourceId, int amount) {
			if(industryResourceStockMap.TryGetValue(resourceId) == null) {
				industryResourceStockMap.Add(resourceId, amount);
			} else {
				industryResourceStockMap.put(resourceId, industryResourceStockMap.get(resourceId) + amount);
			}
		}
		
		public void addActiveRecipe(int index) {
			if(forcedActiveRecipes == null) {
				forcedActiveRecipes = new List<int>();
			}
			forcedActiveRecipes.Add(index);
			if(!forcedActiveRecipes.Contains(activeRecipe)) {
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
			recipes.Add(index, recipe);
		}
		
		public int getId() {
			return id;
		}
	}	

}
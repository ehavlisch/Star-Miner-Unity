using UnityEngine;

using System;
using System.Collections.Generic;
using System.Text;

using Util;

namespace Economy {
	public class EconomySingleton {

        private static EconomySingleton instance;

        public static EconomySingleton Instance {
            get {
                if(instance == null) {
                    //Debug.Log("Instantiating EconomySingleton.");
                    instance = new EconomySingleton();
                }
                return instance;
            }
        }

        private Resource[] resources;
        private Industry[] industries;
        private int maxTier;

        private Dictionary<int, List<Industry>> industryTierMap { get; set; }
        private List<Resource>[] rarityLists;

        private EconomySingleton() {
			resources = genResources();
			industries = genIndustries();

            industryTierMap = new Dictionary<int, List<Industry>>();

            foreach(Industry industry in industries) {
                List<Industry> industryList;
                if(industryTierMap.TryGetValue(industry.tier, out industryList)) {
                    industryList.Add(industry);
                } else {
                    industryList = new List<Industry>();
                    industryList.Add(industry);
                    industryTierMap.Add(industry.tier, industryList);
                }
            }

           rarityLists = new List<Resource>[10];
            for(int i = 0; i < 10; i++) {
                rarityLists[i] = new List<Resource>();
            }
            foreach(Resource resource in resources) {
                if (resource.rarity > 0) {
                    rarityLists[resource.rarity].Add(resource);
                }
            }

            // Create rarity buckets
            // Buckets
            // x -  1  2 |  3  4 |  5   6 |  7   8 |  9  avg 
            // 0 - 50 30 | 18  2 |  -   - |  -   - |  -  1.72
            // 1 -  5 20 | 30 30 | 10   5 |  -   - |  -  3.35
            // 2 -  -  - |  5 20 | 30  30 | 10   5 |  -  5.35  
            // 3 -  -  - |  -  - |  5  20 | 30  30 | 15  7.3                  
            // 4 -  -  - |  -  - |  -   5 | 10  35 | 50  8.3   
        }

        /*
            Resource drops
            based on the value, fall into one of the rarity buckets
            Bucket: Value
            0: .4 - .6
            1: .3 - .4, .6 - .7
            2: .2 - .3, .7 - .8
            3: .1 - .2, .8 - .9
            4: 0 - .1, .9 - 1.0

        */
        public Resource getRandomResource(float value) {
            float random = UnityEngine.Random.value;
            if(value > 0.4 && value < 0.6) {
                // 0 Lowest Rarity
                if(random >= 0.98) {
                    return getRandomResource(4);
                } else if(random >= .8) {
                    return getRandomResource(3);
                } else if(random >= .5) {
                    return getRandomResource(2);
                } else {
                    return getRandomResource(1);
                }
            } else if(value > 0.3 && value <= 0.4 || value > 0.6 && value <= 0.7) {
                // 1 Low Rarity
                if(random >= .95) {
                    return getRandomResource(6);
                } else if(random >= .85) {
                    return getRandomResource(5);
                } else if(random >= .55) {
                    return getRandomResource(4);
                } else if(random >= .25) {
                    return getRandomResource(3);
                } else if(random >= .05) {
                    return getRandomResource(2);
                } else {
                    return getRandomResource(1);
                }
            } else if(value > 0.2 && value <= 0.3 || value > 0.7 && value <= 0.8) {
                // 2 Medium Rarity
                // 2 -  -  - |  5 20 | 30  30 | 10   5 |  -  5.35  
                if(random >= .95) {
                    return getRandomResource(8);
                } else if(random >= .85) {
                    return getRandomResource(7);
                } else if(random >= .55) {
                    return getRandomResource(6);
                } else if(random >= .25) {
                    return getRandomResource(5);
                } else if(random >= .05) {
                    return getRandomResource(4);
                } else {
                    return getRandomResource(3);
                }
            } else if(value > 0.1 && value <= 0.2 || value > 0.8 && value <= 0.9) {
                // 3 High Rarity
                // 3 -  -  - |  -  - |  5  20 | 30  30 | 15  7.3                  
                if (random >= .85) {
                    return getRandomResource(9);
                } else if (random >= .55) {
                    return getRandomResource(8);
                } else if (random >= .25) {
                    return getRandomResource(7);
                } else if (random >= .05) {
                    return getRandomResource(6);
                } else {
                    return getRandomResource(5);
                }
            } else {
                // 4 Highest Rarity
                // 4 -  -  - |  -  - |  -   5 | 10  35 | 50  8.3   
                if (random >= .5) {
                    return getRandomResource(9);
                } else if (random >= .15) {
                    return getRandomResource(8);
                } else if (random >= .05) {
                    return getRandomResource(7);
                } else {
                    return getRandomResource(6);
                }
            }
        }

        private Resource getRandomResource(int rarity) {
            return rarityLists[rarity][Utils.randomInt(0, rarityLists[rarity].Count)];
        }

        // Gets a random industry in the specific tier
        public Industry getRandomIndustry(int tier) {
            return getRandomIndustry(getIndustries(tier));
        }

        // Gets a random industry in or below the selected tier
        public Industry getRandomIndustryBelow(int tier) {
            return getRandomIndustry(getIndustries(Utils.randomInt(tier)));
        }

        // Picks a random industry out of a list
        private Industry getRandomIndustry(List<Industry> industryList) {
            return industryList[Utils.randomInt(industryList.Count)];
        }

        public List<Industry> getIndustries(int tier) {
            List<Industry> industryList;
            industryTierMap.TryGetValue(tier, out industryList);
            if(industryList == null) {
                Debug.LogError("Unable to retrieve industry tier list out of industry tier map for tier: " + tier + ".");
            }
            return industryList;
        }

        public int getIndustryMaxTier() {
            return maxTier;
        }
		
		public Resource getResource(int resourceId) {
			return resources[resourceId];
		}
		
		public static float genLog(float x) {
			// a = lower asymptote
			float a = 0;
			// k = upper asymptote
			float k = 1;
			// c = typically 1 
			float c = 1;
			// b = growth rate
			float b = 1;
			// q = related to Y(0)
			float q = 1;
			// q > 0 more growth
			float n = 1;
			
			return genLog(a, k, c, b, n, q, x);
		}
		
		public static float genLog(float a, float k, float c, float b, float n, float q, float x) {
			return a + (k - a) / (Mathf.Pow(c + (Mathf.Pow((q * (float) Math.E), -b * x)), 1.0f / n));
		}
		
		public static float genLogSimple(float x) {
			return 1 / (1 + (Mathf.Pow(((float) Math.E), -x)));
			
		}
		
		public static float percentValueOfPercentStock(float percent) {
			return 1 - genLogSimple(10 * percent - 5);
		}
		
		public static float getSellValueOf(int amount, int value, int currentStock, int highStock) {
			float sum = 0.0f;
			for(int i = 1; i <= amount; i++) {
				sum += value * 0.8f * percentValueOfPercentStock((currentStock + i) / highStock);
			}
			return sum;
		}
		
		public static float getBuyValueOf(int amount, float value, float currentStock, float highStock) {
			float sum = 0.0f;
			for(int i = 1; i <= amount; i++) {
				sum += value * 1.2f * percentValueOfPercentStock((currentStock - i) / highStock);
			}
			return sum;
		}

        public static int GEM_STONES = 0;
		public static int IRON_ORE = 1;
		public static int RUTILE = 2;
		public static int TITANIUM_ORE = 3;
		public static int NICKEL_ORE = 4;
		public static int COPPER_ORE = 5;
		public static int PLATINUM_DUST = 6;
		public static int SALT = 7;
		public static int CHLORINE = 8;
		public static int BAUXITE = 9;
		public static int SILICON_ORE = 10;
		public static int ARGON = 11;
		public static int FERROSILICON = 12;
		public static int CARBON_MONOXIDE = 13;
		public static int SULFURIC_ACID = 14;
		public static int HYDROCHLORIC_ACID = 15;
		public static int COPPER_REFINED = 16;
		public static int TITANIUM_REFINED = 17;
		public static int NICKEL_REFINED = 18;
		public static int IRON_REFINED = 19;
		public static int STEEL = 20;
		public static int SODIUM_HYDROXIDE = 21;
		public static int PLATINUM_REFINED = 22;
		public static int SILICON_SOLAR = 23;
		public static int SILICON_ELECTRICAL = 24;
		public static int PLASTIC = 25;
		public static int TITAN_PLATINUM = 26;
		public static int BATTERIES = 27;
		public static int PLATINUM_BATTERIES = 28;
		public static int HYDROGEN = 29;
		public static int CRUDE_OIL = 30;
		public static int ALUMINUM_ORE = 31;
		public static int ALUMINUM_REFINED = 32;
		public static int LASER_SIMPLE = 33;
		
		// 29/9/15
		public static int SILVER_ORE = 34;
		public static int GOLD_ORE = 35;
		public static int CALCIUM_ORE = 36;
		public static int OXYGEN = 37;
		public static int MAGNESIUM_ORE = 38;
		public static int SULFUR = 39;
		public static int CARBON = 40;
		public static int CHROMIUM_ORE = 41;
		public static int MANGANESE_ORE = 42;
		public static int NITROGEN = 43;
		public static int COBALT_ORE = 44;
		public static int ZINC_ORE = 45;
		public static int LEAD_ORE = 46;
		public static int SILVER_REFINED = 47;
		public static int GOLD_REFINED = 48;
		public static int CHROMIUM_REFINED = 49;
		public static int MANGANESE_REFINED = 50;
		public static int ZINC_REFINED = 51;
		public static int LEAD_REFINED = 52;
		public static int COBALT_REFINED = 53;
		public static int MAGNESIUM_REFINED = 54;
		public static int CALCIUM_REFINED = 55;
		public static int PYROLUSITE = 56;
		public static int COBALTITE = 57;
		public static int SPHALERITE = 58;
		public static int GALENA = 59;
		public static int SUPERALLOY = 60;
		
		
		public static int[] resourcesList = {GEM_STONES, IRON_ORE, RUTILE, TITANIUM_ORE, NICKEL_ORE, COPPER_ORE, PLATINUM_DUST, SALT, CHLORINE, BAUXITE, SILICON_ORE, ARGON, FERROSILICON, CARBON_MONOXIDE, SULFURIC_ACID, HYDROCHLORIC_ACID, COPPER_REFINED, TITANIUM_REFINED,
			NICKEL_REFINED, IRON_REFINED, STEEL, SODIUM_HYDROXIDE, PLATINUM_REFINED, SILICON_SOLAR, SILICON_ELECTRICAL, PLASTIC, TITAN_PLATINUM, BATTERIES, PLATINUM_BATTERIES, HYDROGEN, CRUDE_OIL, ALUMINUM_ORE, ALUMINUM_REFINED, LASER_SIMPLE,
			
			SILVER_ORE, GOLD_ORE, CALCIUM_ORE, OXYGEN, MAGNESIUM_ORE, SULFUR, CARBON, CHROMIUM_ORE, MANGANESE_ORE, NITROGEN, COBALT_ORE, ZINC_ORE, LEAD_ORE, SILVER_REFINED, GOLD_REFINED, CHROMIUM_REFINED, MANGANESE_REFINED, ZINC_REFINED,
			LEAD_REFINED, COBALT_REFINED, MAGNESIUM_REFINED, CALCIUM_REFINED, PYROLUSITE, COBALTITE, SPHALERITE, GALENA, SUPERALLOY};
		
		// TODO
		// Carbon Nano Tubes
		// Wiring
		
		// Ship object production, lasers -> weapons, bombs, missiles, mines
		// ship upgrades production, titan plat -> armor upgrades
		// alloy forge, takes a bunch of different recipes to upgrade ores
		// industries that produce 'science'
		// science is used to purchase industry upgrades along with cash?
		
		// maybe ship building?
		// unlockable recipes?
		
		// Ship shielding types 
		// Type 1: Deflector 
		// Type 2: Radiation
		// Type 3: Deflector + Radiation

		private Resource[] genResources() {
            Resource[] genResources = new Resource[resourcesList.Length];

            Resource r = new Resource(GEM_STONES, "Gem Stones", "An assortment of rare gems.", 30, 4, 3000, 50, 50);
            genResources[r.id] = r;
			r = new Resource(IRON_ORE, "Iron Ore", "A common unrefined ore.", 10, 2, 5000, 50, 50);
            genResources[r.id] = r;
			r = new Resource(RUTILE, "Rutile", "A common unrefined mineral. Contains Titanium.", 10, 2, 5000, 50, 50);
			genResources[r.id] = r;
			r = new Resource(TITANIUM_ORE, "Titanium Ore", "An uncommon unrefined element.", 30, 7, 1300, 50, 50);
			genResources[r.id] = r;
			r = new Resource(NICKEL_ORE, "Nickel Ore", "A common unrefined ore.", 10, 1, 6000, 50, 50);
			genResources[r.id] = r;
			r = new Resource(COPPER_ORE, "Copper Ore", "A common unrefined ore.", 10, 1, 6000, 50, 50);
			genResources[r.id] = r;
			r = new Resource(PLATINUM_DUST, "Platinum Dust", "Traces of an uncommon unrefined element.", 30, 7, 1300, 50, 50);
			genResources[r.id] = r;
			r = new Resource(SALT, "Salt", "Good ol' Salt. Not necessarily table salt.", 10, 2, 5000, 50, 50);
			genResources[r.id] = r;
			r = new Resource(CHLORINE, "Chlorine", "A toxic gas.", 20, 5, 3000, 10, 50);
			genResources[r.id] = r;
			r = new Resource(BAUXITE, "Bauxite", "A common unrefined mineral. Contains Aluminum.", 10, 2, 5000, 50, 50);
			genResources[r.id] = r;
			r = new Resource(SILICON_ORE, "Impure Silicon", "A common unrefined element.", 10, 2, 5000, 50, 50);
			genResources[r.id] = r;
			r = new Resource(ARGON, "Argon", "A toxic gas.", 20, 5, 3000, 10, 50);
			genResources[r.id] = r;
			r = new Resource(FERROSILICON, "Ferrosilicon", "A common unrefined mineral. Contains Iron and Silicon.", 10, 2, 5000, 50, 50);
			genResources[r.id] = r;
			r = new Resource(CARBON_MONOXIDE, "Carbon Monoxide", "A toxic gas.", 10, 5, 3000, 10, 50);
			genResources[r.id] = r;
			r = new Resource(SULFURIC_ACID, "Sulfuric Acid", "A highly corrosive acid.", 50, 0, 100, 50, 25);
			genResources[r.id] = r;
			r = new Resource(HYDROCHLORIC_ACID, "Hydrochloric Acid", "A highly corrosive acid.", 50, 0, 100, 50, 25);
			genResources[r.id] = r;
			r = new Resource(COPPER_REFINED, "Refined Copper", "A refined element.", 30, 0, 5000, 25, 25);
			genResources[r.id] = r;
			r = new Resource(TITANIUM_REFINED, "Refined Titanium", "A refined element.", 90, 0, 1000, 25, 10);
			genResources[r.id] = r;
			r = new Resource(NICKEL_REFINED, "Refined Nickel", "A refined element.", 30, 0, 5000, 25, 25);
			genResources[r.id] = r;
			r = new Resource(IRON_REFINED, "Refined Iron", "A refined element.", 30, 0, 5000, 25, 20);
			genResources[r.id] = r;
			r = new Resource(STEEL, "Steel", "A durable metal alloy.", 70, 0, 3000, 25, 20);
			genResources[r.id] = r;
			r = new Resource(SODIUM_HYDROXIDE, "Sodium Hydroxide", "A caustic soda.", 30, 0, 2500, 25, 10);
			genResources[r.id] = r;
			r = new Resource(PLATINUM_REFINED, "Pure Platinum", "A refined element.", 90, 0, 1000, 10, 25);
			genResources[r.id] = r;
			r = new Resource(SILICON_SOLAR, "Solar Grade Silicon", "Silicon pure enough for solar applications.", 30, 0, 2500, 25, 5);
			genResources[r.id] = r;
			r = new Resource(SILICON_ELECTRICAL, "Electrical Grade Silicon", "Silicon pure enough for electronics.", 90, 0, 500, 25, 5);
			genResources[r.id] = r;
			r = new Resource(PLASTIC, "Plastic", "A durable, flexible material.", 30, 0, 5000, 25, 10);
			genResources[r.id] = r;
			r = new Resource(TITAN_PLATINUM, "Titan Platinum", "Super hard, light weight metal.", 600, 0, 500, 35, 25);
			genResources[r.id] = r;
			r = new Resource(BATTERIES, "Batteries", "Rechargable sources of power.", 150, 0, 1000, 10, 10);
			genResources[r.id] = r;
			r = new Resource(PLATINUM_BATTERIES, "Platinum Batteries", "Rechargeable high power sources.", 400, 0, 500, 10, 20);
			genResources[r.id] = r;
			r = new Resource(HYDROGEN, "Hydrogen Gas", "A light, explosive gas.", 15, 5, 4000, 10, 50);
			genResources[r.id] = r;
			r = new Resource(CRUDE_OIL, "Crude Oil", "Dark sludge, unusable in it's current state.", 50, 9, 2000, 50, 50);
			genResources[r.id] = r;
			r = new Resource(ALUMINUM_ORE, "Aluminum Ore", "A common element.", 10, 1, 6000, 50, 50);
			genResources[r.id] = r;
			r = new Resource(ALUMINUM_REFINED, "Refined Aluminum", "A refined element.", 25, 0, 3000, 25, 10);
			genResources[r.id] = r;
			r = new Resource(LASER_SIMPLE, "Simple Laser", "A laser usable in a wide variety of applications.", 400, 0, 500, 10, 20);
			genResources[r.id] = r;	
			
			r = new Resource(SILVER_ORE, "Silver Ore", "Argentite. A valuable unrefined element.", 30, 8, 1000, 50, 50);
			genResources[r.id] = r;
			r = new Resource(GOLD_ORE, "Gold Ore", "A very valuable unrefined element.", 50, 9, 1000, 50, 50);
			genResources[r.id] = r;
			r = new Resource(CALCIUM_ORE, "Calcium Ore", "A common unrefined element.", 10, 2, 5000, 50, 50);
			genResources[r.id] = r;
			r = new Resource(OXYGEN, "Oxygen", "A common gas necessary for most life.", 10, 3, 5000, 10, 50);
			genResources[r.id] = r;
			r = new Resource(MAGNESIUM_ORE, "Magnesium Ore", "A common unrefined element.", 10, 2, 5000, 50, 50);
			genResources[r.id] = r;
			r = new Resource(SULFUR, "Sulfur", "A common element with a wide variety of uses.", 15, 2, 5000, 25, 10);
			genResources[r.id] = r;
			r = new Resource(CARBON, "Carbon", "A common element with a wide variety of uses.", 15, 2, 5000, 25, 10);
			genResources[r.id] = r;
			r = new Resource(CHROMIUM_ORE, "Chromium Ore", "Chromite. An uncommon unrefined element.", 30, 7, 3000, 50, 50);
			genResources[r.id] = r;
			r = new Resource(MANGANESE_ORE, "Manganese Ore", "An uncommon, unrefined element.", 5, 6, 2, 50, 50);
			genResources[r.id] = r;
			r = new Resource(NITROGEN, "Nitrogen Gas", "A common gas necessary for most plant life.", 10, 3, 5000, 10, 50);
			genResources[r.id] = r;
			r = new Resource(COBALT_ORE, "Cobalt Ore", "A valuable unrefined element.", 30, 7, 2000, 50, 50);
			genResources[r.id] = r;
			r = new Resource(ZINC_ORE, "Zinc Ore", "A common unrefined element.", 10, 4, 5000, 50, 50);
			genResources[r.id] = r;
			r = new Resource(LEAD_ORE, "Lead Ore", "A common unrefined element.", 10, 5, 5000, 50, 50);
			genResources[r.id] = r;
			r = new Resource(SILVER_REFINED, "Refined Silver", "A shiny valuable element. Often used for decoration.", 90, 0, 1000, 25, 30);
			genResources[r.id] = r;
			r = new Resource(GOLD_REFINED, "Refined Gold", "A dense, shiny, valuable element. Often used for decoration.", 120, 0, 1000, 25, 50);
			genResources[r.id] = r;
			r = new Resource(CHROMIUM_REFINED, "Refined Chromium", "A resistant, shiny metal often used for decoration.", 90, 0, 2000, 25, 20);
			genResources[r.id] = r;
			r = new Resource(MANGANESE_REFINED, "Refined Manganese", "A mostly unuseful, uncommon refined element.", 25, 0, 800, 25, 20);
			genResources[r.id] = r;
			r = new Resource(ZINC_REFINED, "Refined Zinc", "A refined element.", 30, 0, 5000, 25, 20);
			genResources[r.id] = r;
			r = new Resource(LEAD_REFINED, "Refined Lead", "A heavy, refined element.", 30, 0, 5000, 25, 40);
			genResources[r.id] = r;
			r = new Resource(COBALT_REFINED, "Refined Cobalt", "A refined element.", 90, 0, 5000, 25, 25);
			genResources[r.id] = r;
			r = new Resource(MAGNESIUM_REFINED, "Refined Magnesium", "A refined element. Typically used in medicines.", 30, 0, 2500, 25, 5);
			genResources[r.id] = r;
			r = new Resource(CALCIUM_REFINED, "Refined Calcium", "A refined element. Used to create alloys.", 30, 0, 5000, 25, 5);
			genResources[r.id] = r;
			
			r = new Resource(PYROLUSITE, "Pyrolusite", "A common mineral. Contains Manganese.", 10, 6, 1000, 50, 50); 
			genResources[r.id] = r;
			r = new Resource(COBALTITE, "Cobaltite", "A common mineral. Contains Cobalt.", 15, 5, 2000, 50, 50);
			genResources[r.id] = r;
			r = new Resource(SPHALERITE, "Sphalerite", "A common mineral. Contains Zinc.", 10, 7, 5000, 50, 50);
			genResources[r.id] = r;
			r = new Resource(GALENA, "Galena", "A common mineral. Contains Lead.", 10, 6, 5000, 50, 50);
			genResources[r.id] = r;
			r = new Resource(SUPERALLOY, "Superalloy", "A super hard metal, protected against corrosion.", 9999, 0, 2000, 1, 1);
			genResources[r.id] = r;
			
			return genResources;
			
		}

		// TODO reverse the order of additional Recipes. The most complicated ones should go first
		private Industry[] genIndustries() {
			List<Industry> industryList = new List<Industry>();
			int industryId = 0;
			Industry industry = null;
			// Ferrosilicon -> Impure Silicon + Refined Iron
			Recipe recipe = new Recipe(10);
			recipe.addInputResource(FERROSILICON, 1);
			recipe.addOutputResource(SILICON_ORE, 1, 80);
			recipe.addOutputResource(IRON_REFINED, 1, 80);
			industry = new Industry(this, industryId++, "Ferrosilicon Mill", recipe, IndustryGroup.REFINERY, 0);
            industryList.Add(industry);
			
			// Iron Ore -> Refined Iron + Impure Silicon
			recipe = new Recipe(10);
			recipe.addInputResource(IRON_ORE, 1);
			recipe.addOutputResource(IRON_REFINED, 1, 80);
			recipe.addOutputResource(SILICON_ORE, 1, 80);
			industry = new Industry(this, industryId++, "Iron refinery", recipe, IndustryGroup.REFINERY, 0);
			industryList.Add(industry);
			
			// Rutile -> Titanium Ore
			recipe = new Recipe(20);
			recipe.addInputResource(RUTILE, 1);
			recipe.addOutputResource(TITANIUM_ORE, 1, 95);
			industry = new Industry(this, industryId++, "Rutile Processing Plant", recipe, IndustryGroup.REFINERY, 0);
			industryList.Add(industry);
			
			// Titatinium Ore + Argon + Chlorine -> Refined Titanium
			recipe = new Recipe(40);
			recipe.addInputResource(TITANIUM_ORE, 1);
			recipe.addInputResource(ARGON, 1);
			recipe.addInputResource(CHLORINE, 1);
			recipe.addOutputResource(TITANIUM_REFINED, 1, 90);
			recipe.addOutputResource(ARGON, 1, 20);
			recipe.addOutputResource(CHLORINE, 1, 20);
			industry = new Industry(this, industryId++, "Titanium Forge", recipe, IndustryGroup.REFINERY, 1);
			industryList.Add(industry);
			
			// Nickel Ore + Carbon Monoxide -> Refined Nickel
			recipe = new Recipe(10);
			recipe.addInputResource(NICKEL_ORE, 5);
			recipe.addInputResource(CARBON_MONOXIDE, 1);
			recipe.addOutputResource(NICKEL_REFINED, 5, 90);
			recipe.addOutputResource(CARBON_MONOXIDE, 1, 20);
			recipe.addOutputResource(COBALT_ORE, 1, 5);
			industry = new Industry(this, industryId++, "Nickel Foundry", recipe, IndustryGroup.REFINERY, 1);
			industryList.Add(industry);
			
			// Copper Ore -> Refined Copper + Sulfuric Acid
			// Copper Ore + Hydrochloric Acid -> Refined Copper + Sulfuric Acid + (low chance) Platinum Dust
			recipe = new Recipe(20);
			recipe.addInputResource(COPPER_ORE, 1);
			recipe.addOutputResource(COPPER_REFINED, 1, 90);
			recipe.addOutputResource(SULFURIC_ACID, 1, 60);
			recipe.addOutputResource(LEAD_ORE, 1, 5);
			recipe.addOutputResource(COBALT_ORE, 1, 5);
			
			
			industry = new Industry(this, industryId++, "Copper Refinery", recipe, IndustryGroup.REFINERY, 0);
			
			recipe = new Recipe(80);
			recipe.addInputResource(COPPER_ORE, 1);
			recipe.addInputResource(HYDROCHLORIC_ACID, 1);
			recipe.addOutputResource(SULFURIC_ACID, 1, 60);
			recipe.addOutputResource(COPPER_REFINED, 1, 90);
			recipe.addOutputResource(PLATINUM_DUST, 1, 5);
			recipe.addOutputResource(HYDROCHLORIC_ACID, 1, 80);
			recipe.addOutputResource(LEAD_ORE, 1, 5);
			recipe.addOutputResource(COBALT_ORE, 1, 5);
			industry.addRecipe(0, recipe);
			industryList.Add(industry);
			
			// Refined Nickel + Sulfuric Acid -> Steel
			// Refined Iron + Refined Manganese -> 2 Steel
			// Refined Zinc + Refined Iron -> 2 Steel
			recipe = new Recipe(10);
			recipe.addInputResource(NICKEL_REFINED, 3);
			recipe.addInputResource(SULFURIC_ACID, 1);
			recipe.addOutputResource(STEEL, 2, 85);
			recipe.addOutputResource(SULFURIC_ACID, 1, 80);
			industry = new Industry(this, industryId++, "Steel Mill", recipe, IndustryGroup.ALLOYS, 0);
			
			recipe = new Recipe(10);
			recipe.addInputResource(IRON_REFINED, 1);
			recipe.addInputResource(MANGANESE_REFINED, 1);
			recipe.addOutputResource(STEEL, 2, 85);
			industry.addRecipe(0, recipe);
			
			recipe = new Recipe(10);
			recipe.addInputResource(IRON_REFINED, 1);
			recipe.addInputResource(ZINC_REFINED, 1);
			recipe.addOutputResource(STEEL, 2, 85);
			industry.addRecipe(0, recipe);	
			industryList.Add(industry);
			
			// Refined Nickel + Refined Zinc + Sulfuric Acid -> Batteries
			recipe = new Recipe(30);
			recipe.addInputResource(NICKEL_REFINED, 1);
			recipe.addInputResource(SULFURIC_ACID, 1);
			recipe.addInputResource(ZINC_REFINED, 1);
			recipe.addOutputResource(BATTERIES, 1, 100);
			industry = new Industry(this, industryId++, "Battery Factory", recipe, IndustryGroup.PRODUCTION, 2);
			industryList.Add(industry);
			
			// Sulfuric Acid + Salt -> Hydrochloric Acid
			recipe = new Recipe(20);
			recipe.addInputResource(SULFURIC_ACID, 1);
			recipe.addInputResource(SALT, 2);
			recipe.addOutputResource(HYDROCHLORIC_ACID, 2, 100);
			industry = new Industry(this, industryId++, "Hydrochloric Acid Lab", recipe, IndustryGroup.MATERIALS, 2);
			industryList.Add(industry);
			
			// Salt -> Chlorine + Sodium Hydroxide + Hydrogen
			recipe = new Recipe(30);
			recipe.addInputResource(SALT, 3);
			recipe.addOutputResource(CHLORINE, 1, 90);
			recipe.addOutputResource(SODIUM_HYDROXIDE, 1, 90);
			recipe.addOutputResource(HYDROGEN, 1, 90);
			industry = new Industry(this, industryId++, "Hydrochloric Acid Lab", recipe, IndustryGroup.MATERIALS, 1);
			industryList.Add(industry);
			
			// Crude Oil + Salt -> Plastic
			recipe = new Recipe(40);
			recipe.addInputResource(CRUDE_OIL, 1);
			recipe.addInputResource(SALT, 1);
			recipe.addOutputResource(PLASTIC, 4, 90);
			industry = new Industry(this, industryId++, "Plastic Manufactory", recipe, IndustryGroup.OIL, 3);
			industryList.Add(industry);
			
			// Crude Oil + Chlorine -> Plastic
			recipe = new Recipe(40);
			recipe.addInputResource(CRUDE_OIL, 1);
			recipe.addInputResource(CHLORINE, 1);
			recipe.addOutputResource(PLASTIC, 5, 90);
			industry = new Industry(this, industryId++, "Plastic Manufactory", recipe, IndustryGroup.OIL, 3);
			industryList.Add(industry);
			
			// Aluminum Ore + Salt -> Refined Aluminum
			// Aluminum Ore + Manganese Ore -> 2 Refined Aluminum
			recipe = new Recipe(10);
			recipe.addInputResource(ALUMINUM_ORE, 1);
			recipe.addInputResource(SALT, 1);
			recipe.addOutputResource(ALUMINUM_REFINED, 1, 97);
			recipe.addOutputResource(SALT, 1, 33);
			industry = new Industry(this, industryId++, "Aluminum Foundry", recipe, IndustryGroup.REFINERY, 0);
			recipe = new Recipe(10);
			recipe.addInputResource(ALUMINUM_ORE, 1);
			recipe.addInputResource(MANGANESE_ORE, 1);
			recipe.addOutputResource(ALUMINUM_REFINED, 2, 95);
			industry.addRecipe(0, recipe);	
			industryList.Add(industry);
			
			// Silicon (Metalurgical Grade) -> Solar Grade Silicon
			recipe = new Recipe(30);
			recipe.addInputResource(SILICON_ORE, 1);
			recipe.addOutputResource(SILICON_SOLAR, 1, 95);
			industry = new Industry(this, industryId++, "Solar Silicon Lab", recipe, IndustryGroup.SCIENTIFIC, 3);
			industryList.Add(industry);
			
			// Solar Grade Silicon -> Electronic Grade Silicon
			recipe = new Recipe(50);
			recipe.addInputResource(SILICON_SOLAR, 1);
			recipe.addOutputResource(SILICON_ELECTRICAL, 1, 80);
			industry = new Industry(this, industryId++, "Electrical Silicon Lab", recipe, IndustryGroup.SCIENTIFIC, 4);
			industryList.Add(industry);
			
			// Bauxite + Sodium Hydroxide -> Aluminum
			recipe = new Recipe(20);
			recipe.addInputResource(BAUXITE, 1);
			recipe.addInputResource(SODIUM_HYDROXIDE, 1);
			recipe.addOutputResource(ALUMINUM_REFINED, 1, 97);
			recipe.addOutputResource(SODIUM_HYDROXIDE, 1, 20);
			industry = new Industry(this, industryId++, "Bauxite Mill", recipe, IndustryGroup.REFINERY, 0);
			industryList.Add(industry);
			
			// Platinum Dust -> Pure Platinum
			recipe = new Recipe(30);
			recipe.addInputResource(PLATINUM_DUST, 4);
			recipe.addOutputResource(PLATINUM_REFINED, 3, 90);
			industry = new Industry(this, industryId++, "Platinum Refinery", recipe, IndustryGroup.REFINERY, 1);
			industryList.Add(industry);
			
			// Refined Nickel + Refined Zinc + Pure Platinum + Sulfuric Acid -> Platinum Batteries
			recipe = new Recipe(50);
			recipe.addInputResource(NICKEL_REFINED, 1);
			recipe.addInputResource(ZINC_REFINED, 1);
			recipe.addInputResource(PLATINUM_REFINED, 1);
			recipe.addInputResource(SULFURIC_ACID, 1);
			recipe.addOutputResource(PLATINUM_BATTERIES, 1, 100);
			industry = new Industry(this, industryId++, "Platinum Batteries Industry", recipe, IndustryGroup.PRODUCTION, 4);
			industryList.Add(industry);
			
			// Batteries + Pure Platinum + Sulfuric Acid-> Platinum Batteries
			recipe = new Recipe(60);
			recipe.addInputResource(BATTERIES, 1);
			recipe.addInputResource(PLATINUM_REFINED,  2);
			recipe.addInputResource(SULFURIC_ACID, 2);
			recipe.addOutputResource(PLATINUM_BATTERIES, 2, 100);
			industry = new Industry(this, industryId++, "Battery Upgrade Plant", recipe, IndustryGroup.PRODUCTION, 3);
			industryList.Add(industry);
			
			// Pure Platinum + Refined Titanium + Sodium Hydroxide -> Titan Platinum
			recipe = new Recipe(60);
			recipe.addInputResource(PLATINUM_REFINED, 1);
			recipe.addInputResource(TITANIUM_REFINED, 1);
			recipe.addInputResource(SODIUM_HYDROXIDE, 1);
			recipe.addOutputResource(TITAN_PLATINUM, 2, 50);
			industry = new Industry(this, industryId++, "Titan Platinum Forge", recipe, IndustryGroup.ALLOYS, 1);
			industryList.Add(industry);
			
			// Argon + Aluminum + batteries + silicon slag -> Laser
			recipe = new Recipe(40);
			recipe.addInputResource(ARGON, 1);
			recipe.addInputResource(ALUMINUM_REFINED, 1);
			recipe.addInputResource(BATTERIES, 1);
			recipe.addInputResource(SILICON_ORE, 1);
			recipe.addOutputResource(LASER_SIMPLE, 1, 100);
			industry = new Industry(this, industryId++, "Laser Workshop", recipe, IndustryGroup.PRODUCTION, 4);	
			industryList.Add(industry);
			
			// Silver Ore -> Refined Silver + Sulfur
			recipe = new Recipe(30);
			recipe.addInputResource(SILVER_ORE, 1);
			recipe.addOutputResource(SILVER_REFINED, 1, 90);
			recipe.addOutputResource(SULFUR, 1, 75);
			industry = new Industry(this, industryId++, "Silver Smeltery", recipe, IndustryGroup.REFINERY, 1);
			industryList.Add(industry);
			
			// Gold Ore -> Refined Gold
			recipe = new Recipe(40);
			recipe.addInputResource(GOLD_ORE, 1);
			recipe.addOutputResource(GOLD_REFINED, 1, 80);
			industry = new Industry(this, industryId++, "Gold Smeltery", recipe, IndustryGroup.REFINERY, 1);
			industryList.Add(industry);
			
			// Chromium Ore + Refined Aluminum -> Refined Chromium + Refined Iron + Refined Aluminum + Oxygen
			recipe = new Recipe(30);
			recipe.addInputResource(CHROMIUM_ORE, 1);
			recipe.addInputResource(ALUMINUM_ORE, 1);
			recipe.addOutputResource(CHROMIUM_REFINED, 1, 95);
			recipe.addOutputResource(ALUMINUM_REFINED, 1, 60);
			recipe.addOutputResource(IRON_REFINED, 1, 60);
			recipe.addOutputResource(OXYGEN, 1, 95);
			industry = new Industry(this, industryId++, "Chromium Mill", recipe, IndustryGroup.REFINERY, 1);
			industryList.Add(industry);
			
			// Manganese Ore -> Refined Manganese
			recipe = new Recipe(10);
			recipe.addInputResource(MANGANESE_ORE, 1);
			recipe.addOutputResource(MANGANESE_REFINED, 1, 80);
			industry = new Industry(this, industryId++, "Manganese Refinery", recipe, IndustryGroup.REFINERY, 1);
			industryList.Add(industry);
			
			// PyroLusite + Hydrochloric acid -> Refined manganese + Chlorine
			// PyroLusice + Hydrochloric acid + refined iron -> 2 Steel + Chlorine 
			recipe = new Recipe(20);
			recipe.addInputResource(PYROLUSITE, 1);
			recipe.addInputResource(HYDROCHLORIC_ACID, 1);
			recipe.addOutputResource(MANGANESE_REFINED, 1, 80);
			recipe.addOutputResource(CHLORINE, 1, 80);
			recipe.addOutputResource(HYDROCHLORIC_ACID, 1, 80);
			industry = new Industry(this, industryId++, "Pyrolusite Mill", recipe, IndustryGroup.REFINERY, 1);
			
			recipe = new Recipe(25);
			recipe.addInputResource(PYROLUSITE, 1);
			recipe.addInputResource(HYDROCHLORIC_ACID, 1);
			recipe.addInputResource(IRON_REFINED, 1);
			recipe.addOutputResource(MANGANESE_REFINED, 2, 80);
			recipe.addOutputResource(CHLORINE, 1, 80);
			recipe.addOutputResource(HYDROCHLORIC_ACID, 1, 80);
			industry.addRecipe(0, recipe);	
			industryList.Add(industry);
			
			// Cobaltite -> Cobalt ore + sulfur + (50) iron ore + (50) nickel ore
			recipe = new Recipe(10);
			recipe.addInputResource(COBALTITE, 1);
			recipe.addOutputResource(COBALT_ORE, 1, 90);
			recipe.addOutputResource(SULFUR, 1, 75);
			recipe.addOutputResource(IRON_ORE, 1, 40);
			recipe.addOutputResource(NICKEL_ORE, 1, 40);
			industry = new Industry(this, industryId++, "Cobaltite Mill", recipe, IndustryGroup.REFINERY, 1);
			industryList.Add(industry);
			
			// Cobalt Ore + sulfuric acid -> Refined Cobalt + Sulfur + sulfuric acid
			recipe = new Recipe(10);
			recipe.addInputResource(COBALT_ORE, 1);
			recipe.addInputResource(SULFURIC_ACID, 1);
			recipe.addOutputResource(COBALT_REFINED, 1, 90);
			recipe.addOutputResource(SULFUR, 1, 75);
			recipe.addOutputResource(SULFURIC_ACID, 1, 80);
			industry = new Industry(this, industryId++, "Cobalt Refinery", recipe, IndustryGroup.REFINERY, 1);
			industryList.Add(industry);
			
			// Refined cobalt + refined nickel + refined copper-> superalloy
			// Refined cobalt + steel + refined calcium -> superalloy
			// refined cobalt + refined aluminum + refined chromium + refined calcium -> superalloy
			// refined cobalt + refined platinum + refined calcium -> superalloy
			recipe = new Recipe(40);
			recipe.addInputResource(COBALT_REFINED, 1);
			recipe.addInputResource(NICKEL_REFINED, 1);
			recipe.addInputResource(COPPER_REFINED, 1);
			recipe.addOutputResource(SUPERALLOY, 3, 75);
			industry = new Industry(this, industryId++, "Superalloy Forge", recipe, IndustryGroup.ALLOYS, 1);
			
			recipe = new Recipe(40);
			recipe.addInputResource(COBALT_REFINED, 1);
			recipe.addInputResource(STEEL, 1);
			recipe.addInputResource(CALCIUM_REFINED, 1);
			recipe.addOutputResource(SUPERALLOY, 3, 75);
			industry.addRecipe(0, recipe);
			
			recipe = new Recipe(40);
			recipe.addInputResource(COBALT_REFINED, 1);
			recipe.addInputResource(ALUMINUM_REFINED, 1);
			recipe.addInputResource(CHROMIUM_REFINED, 1);
			recipe.addOutputResource(SUPERALLOY, 3, 75);
			industry.addRecipe(0, recipe);
			
			recipe = new Recipe(40);
			recipe.addInputResource(COBALT_REFINED, 1);
			recipe.addInputResource(PLATINUM_DUST, 1);
			recipe.addInputResource(CALCIUM_REFINED, 1);
			recipe.addOutputResource(SUPERALLOY, 3, 75);
			industry.addRecipe(0, recipe);
			industryList.Add(industry);
			
			// Sphalerite -> Zinc Ore + Sulfur + (30) Copper Ore + (30) Lead Ore + (30) Iron Ore
			recipe = new Recipe(10);
			recipe.addInputResource(SPHALERITE, 1);
			recipe.addOutputResource(ZINC_ORE, 1, 90);
			recipe.addOutputResource(SULFUR, 1, 75);
			recipe.addOutputResource(COPPER_ORE, 1, 10);
			recipe.addOutputResource(LEAD_ORE, 1, 10);
			recipe.addOutputResource(IRON_ORE, 1, 10);
			
			industry = new Industry(this, industryId++, "Sphalerite Mill", recipe, IndustryGroup.REFINERY, 0);
			industryList.Add(industry);
			
			// Zinc Ore + sulfuric acid -> Refined Zinc + sulfuric acid
			recipe = new Recipe(10);
			recipe.addInputResource(ZINC_ORE, 1);
			recipe.addInputResource(SULFURIC_ACID, 1);
			recipe.addOutputResource(ZINC_REFINED, 1, 95);
			recipe.addOutputResource(SULFURIC_ACID, 1, 80);
			industry = new Industry(this, industryId++, "Zinc Refinery", recipe, IndustryGroup.REFINERY, 0);
			industryList.Add(industry);		
			
			// Lead Ore + Sulfur -> Refined Lead
			// Galena -> Refined Lead + Sulfur
			recipe = new Recipe(10);
			recipe.addInputResource(LEAD_ORE, 1);
			recipe.addInputResource(SULFUR, 1);
			recipe.addOutputResource(LEAD_REFINED, 1, 95);
			industry = new Industry(this, industryId++, "Lead Refinery", recipe, IndustryGroup.REFINERY, 0);
			
			recipe = new Recipe(10);
			recipe.addInputResource(GALENA, 1);
			recipe.addOutputResource(LEAD_REFINED, 1, 90);
			recipe.addOutputResource(SULFUR, 1, 75);
			industry.addRecipe(0, recipe);
			industryList.Add(industry);
			
			// Calcium Ore -> Refined Calcium
			recipe = new Recipe(10);
			recipe.addInputResource(CALCIUM_ORE, 1);
			recipe.addOutputResource(CALCIUM_REFINED, 1, 90);
			industry = new Industry(this, industryId++, "Calcium Mill", recipe, IndustryGroup.MATERIALS, 0);
			industryList.Add(industry);

            Industry[] industryArray = new Industry[industryId];
            foreach(Industry i in industryList) {
                if(maxTier < i.tier) {
                    maxTier = i.tier;
                }
                industryArray[i.id] = i;
            }
            return industryArray;
		}
		
		// Industries to add
		// Refined Lead + ? ? ? -> Type 2 shield
		// Refined Lead + (bunch of ship parts) -> Ship

        // Industries that increase planet tier, hospitable, livable values?
		
		public string listResourceUses(bool verbose) {
			List<Resource> unusedResources = new List<Resource>();
			StringBuilder s = new StringBuilder();
			
			foreach(int resourceId in resourcesList) {
				Resource resource = resources[resourceId];
				if(!verbose) {
                    string resourceName = resource.name;
					if(resourceName.Length > 15) {
						resourceName = resourceName.Substring(0, 15);
						s.Append(resourceName);
					} else if(resourceName.Length < 8) {
						s.Append(resourceName);
						s.Append("    ");
					} else {
						s.Append(resourceName);
					}
				} else {
					s.Append(resource.name);
				}
				int inputs = 0;
				int outputs = 0;
				List<Industry> inputIndustries = new List<Industry>();
				List<Industry> outputIndustries = new List<Industry>();
				foreach(Industry industry in industries) {
					Dictionary<int, int> resourceDictionary = industry.getAllInputs();
                    int value;
					if(resourceDictionary.TryGetValue(resource.id, out value)) {
                        if (value > 0) {
                            inputIndustries.Add(industry);
                            inputs += resourceDictionary[resource.id];
                        }
					}
                    resourceDictionary = industry.getAllOutputs();
					if(resourceDictionary.TryGetValue(resource.id, out value)) {
                        if (value > 0) {
                            outputIndustries.Add(industry);
                            outputs += resourceDictionary[resource.id];
                        }
					}
				}
				if(outputs + inputs == 0) {
					unusedResources.Add(resource);
				}
				
				if(verbose) {
					s.Append("\nInputs to (");
				} else {
					s.Append("\t");
				}
				
				s.Append(inputs);
				
				if(verbose) {
					s.Append("):\n"); 
					foreach(Industry industry in inputIndustries) {
						s.Append(industry).Append("\n");
					}
					s.Append("Outputs from (");
				} else {
					s.Append("\t");
				}
				
				s.Append(outputs);
				
				if(verbose) {
					s.Append("):\n");
					foreach(Industry industry in outputIndustries) {
						s.Append(industry).Append("\n");
					}
					s.Append("---------------------");
				} 
				s.Append("\n");
			}
			if(unusedResources.Count > 0) {
				s.Append("Unused Resources:\n");
				for(int i = 0; i < unusedResources.Count; i++) {
					s.Append(unusedResources[i].name);
					if(unusedResources.Count > i + 1) {
						s.Append("\n");
					}
				}
			}
			return s.ToString();
		}

        public string listResourceRarities() {
            StringBuilder sb = new StringBuilder();
            foreach (Resource resource in resources) {
                if (resource != null && resource.rarity > 0) {
                    sb.Append(resource.name);
                    sb.Append("-");
                    sb.Append(resource.rarity);
                    sb.Append("\n");
                }
            }

            return sb.ToString();
        }
	}
}
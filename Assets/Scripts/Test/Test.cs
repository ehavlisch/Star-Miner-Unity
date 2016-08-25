using UnityEngine;
using Events;
using Economy;
using System.Collections.Generic;
using Races;

namespace Tests {
    public class Test {

        public static void runTests() {
            int randomValue = Random.seed;
            Random.seed = (int)System.DateTime.Now.Ticks;

            //testEvents();
            //testEconomySingleton();
            //testPlanetInit();
            //testIndustry();
            //testRandomResource();
            //testRaceFactory();
            //testFactionFactory();
            testFactionRaceIndex();

            // Not quite tests
            //sortEventNames();
            //Debug.Log(EconomySingleton.Instance.listResourceRarities());

            Random.seed = randomValue;
        }

        private static void testFactionRaceIndex() {

            FactionFactory.getFactionIndex(AresCoalition.NAME);

            FactionFactory.getFactionIndex(UnitedPlanets.NAME);

            RaceFactory.getRaceIndex(Aug.NAME);

            RaceFactory.getRaceIndex(QeThil.NAME);

            RaceFactory.getRaceIndex(PEH.NAME);
        }

        private static void testFactionFactory() {
            Debug.Log("Starting testFactionFactory.");
            Dictionary<string, int> factionCounts = new Dictionary<string, int>();
            factionCounts.Add(UnitedPlanets.NAME, 0);
            factionCounts.Add(AresCoalition.NAME, 0);
            factionCounts.Add(NoFaction.NAME, 0);

            Race race = RaceFactory.getRace(PEH.NAME);

            for (int i = 0; i < 10000; i++) {
                Faction faction = FactionFactory.getFaction(race);
                factionCounts[faction.getName()] += 1;
            }

            if(factionCounts[UnitedPlanets.NAME] <= 0) {
                Debug.LogWarning("Faction Factory return 0 instances of United Planets for the PEH race.");
            }
            if (factionCounts[AresCoalition.NAME] <= 0) {
                Debug.LogWarning("Faction Factory return 0 instances of AresCoalition for the PEH race.");
            }
            if (factionCounts[NoFaction.NAME] <= 0) {
                Debug.LogWarning("Faction Factory return 0 instances of NoFaction for the PEH race.");
            }


            Debug.Log("Finished testFactionFactory.");
        }

        private static void testRaceFactory() {
            Debug.Log("Starting testRaceFactory.");
            Race qeThil = RaceFactory.getRace(QeThil.NAME);
            if(qeThil == null) {
                Debug.LogError("Test Failed: Unable to find known race.");
            }
            Race peh = RaceFactory.getRace(PEH.NAME);
            if(peh == null) {
                Debug.LogError("Test Failed: Unable to find known race.");
            }

            Race race = RaceFactory.getRace();
            if(race == null) {
                Debug.LogError("Test Failed: Unable to find random race.");
            }

            HashSet<string> uniqueNames = new HashSet<string>();

            int count = 0;
            while(count < 10000) {
                if (!uniqueNames.Add(race.generateName())) {
                    break;
                }
                count++;
            }
            Debug.Log("Reached " + count + " before generating a duplicate name for the " + race.getName() + " race.");
            Debug.Log("Finished testRaceFactory.");

        }

        private static void sortEventNames() {
            Debug.Log("Starting sortEventNames.");
            string ids = EventFactory.sortIds(AugStrings.names);

            Debug.Log(ids);

            ids = EventFactory.sortIds(AugStrings.designations);

            Debug.Log(ids);
            // System.IO.File.WriteAllText(@"WriteText.txt", ids);

            Debug.Log("Finished sortEventNames.");
        }

        private static void testEvents() {

            Debug.Log("Starting testEvents.");
            RandomEvent anEvent = EventFactory.generateEvent();
            anEvent.runEvent();

            for (int i = 0; i < 10; i++) {
                if (anEvent.isClosed()) {
                    break;
                }
                anEvent.selectAction(0);

            }
            Debug.Log("Finished testEvents.");
        }

        private static void testEconomySingleton() {
            Industry industry = null;
            Debug.Log("Starting testEconomySingleton.");
            int maxTier = EconomySingleton.Instance.getIndustryMaxTier();
            for (int i = 0; i < 100; i++) {
                industry = EconomySingleton.Instance.getRandomIndustry(i % maxTier);
                if (industry.tier != i % maxTier) {
                    Debug.LogWarning("Test Failed: Industry not in specified tier.");
                    break;
                }
            }

            for (int i = 0; i < 100; i++) {
                industry = EconomySingleton.Instance.getRandomIndustryBelow(i % maxTier);
                if (industry.tier > i % maxTier) {
                    Debug.LogWarning("Test Failed: Industry was above specified tier.");
                    break;
                }
            }

            Debug.Log("Finished testEconomySingleton.");
        }

        private static void testPlanetInit() {
            Debug.Log("Starting testPlanetInit.");
            
            GameObject gameObject = new GameObject();
            gameObject.AddComponent<PlanetController>();

            PlanetController pc = gameObject.GetComponent<PlanetController>();
            pc.visit();
            pc.debugPlanet();
            
            Debug.Log("Finished testPlanetInit.");
        }
        
        private static void testIndustry() {
            Debug.Log("Starting testIndustry.");
            Industry industry = EconomySingleton.Instance.getRandomIndustry(EconomySingleton.Instance.getIndustryMaxTier());
            Debug.Log("Testing: " + industry.name);
            
            if(industry.recipes.Count == 0) {
                Debug.LogWarning("Industry contains no recipes.");
            }
            if(!industry.activeRecipe.HasValue) {
                Debug.LogWarning("Industry has no active recipe.");
                return;
            }

            Dictionary<int, int> resourceStockDictionary = new Dictionary<int, int>();

            IndustryRunResult result = industry.run(resourceStockDictionary);
            if(result != IndustryRunResult.HALTED) {
                Debug.LogWarning("Industry not in halted status initially.");
            }

            Recipe activeRecipe = industry.recipes[industry.activeRecipe.Value];
            List<int> inputResourceIds = activeRecipe.getInputs();
            foreach(int resourceId in inputResourceIds) {
                resourceStockDictionary.Add(resourceId, 100000);
            }

            result = industry.run(resourceStockDictionary);
            if(result != IndustryRunResult.RUNNING) {
                Debug.LogWarning("Industry not running with sufficient resources.");
            }
            int maxTicks = activeRecipe.getRate();
            int ticks = 0;
            while(result != IndustryRunResult.PRODUCED && ticks < maxTicks) {
                result = industry.run(resourceStockDictionary);
                ticks++;
            }
            if(ticks == maxTicks) {
                Debug.LogWarning("Industry never produced despite running for " + maxTicks + " tick.");
            }
            if (result != IndustryRunResult.PRODUCED) {
                Debug.LogWarning("Industry never produced despite running for the appropriate number of ticks.");
            }

            Debug.Log("Finished testIndustry.");
        }

        public static void testRandomResource() {
            Debug.Log("Starting testRandomResource.");


            for (int i = 0; i < 100; i++) {
                Resource resource = EconomySingleton.Instance.getRandomResource(0.5f);

                if (resource.rarity < 0 || resource.rarity > 4) {
                    Debug.LogWarning("Random resource was out of expected range based on it's value.");
                }
            }

            for(int i = 0; i < 100; i++) {
                Resource resource = EconomySingleton.Instance.getRandomResource(0.99f);

                if(resource.rarity < 6 || resource.rarity > 9) {
                    Debug.LogWarning("Random resource was out of expected range based on it's value.");
                }
            }
            Debug.Log("Finished testRandomResource.");
        }
    }
}
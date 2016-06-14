
using UnityEngine;
using Events;
using Economy;
using Util;
using System.Collections.Generic;

namespace Tests {
    public class Test {

        public static void runTests() {
            int randomValue = Random.seed;
            Random.seed = (int)System.DateTime.Now.Ticks;

            //testEvents();
            //testEconomySingleton();
            //sortEventNames();
            //testPlanetInit();
            //testIndustry();

            Debug.Log(EconomySingleton.Instance.listResourceRarities());

            Random.seed = randomValue;
        }

        private static void sortEventNames() {
            EventFactory.sortIds();
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
    }
}
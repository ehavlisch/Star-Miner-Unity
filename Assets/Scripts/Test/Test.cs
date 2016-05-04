
using UnityEngine;
using Events;
using Economy;
using Util;

namespace Tests {
    public class Test {

        public static void runTests() {
            //testEvents();
            testEconomySingleton();
            //sortEventNames();
            //testPlanetInit();
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
            Debug.Log("Starting testEconomySingleton.");
            Industry industry = EconomySingleton.Instance.getRandomIndustry(Constants.INDUSTRY_MAX_TIER);

            Debug.Log("Got a random industry: " + industry.name);

            Debug.Log("Finished testEconomySingleton.");
        }

        private static void testPlanetInit() {
            Debug.Log("Starting testPlanetInit.");

            //Setup a random seed
            int seed = Random.seed;
            Random.seed = System.DateTime.Now.Millisecond;

            GameObject gameObject = new GameObject();
            gameObject.AddComponent<PlanetController>();

            PlanetController pc = gameObject.GetComponent<PlanetController>();
            pc.visit();
            pc.debugPlanet();

            Random.seed = seed;
            Debug.Log("Finished testPlanetInit.");
        }
    }
}
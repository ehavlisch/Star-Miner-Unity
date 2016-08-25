using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ship {
    class EngineSingleton {
        private static EngineSingleton instance;

        public static EngineSingleton Instance {
            get {
                if (instance == null) {
                    //Debug.Log("Instantiating EconomySingleton.");
                    instance = new EngineSingleton();
                }
                return instance;
            }
        }

        Engine[] engines;

        private EngineSingleton() {
            genEngines();
        }

        public static int TEST_ENGINE = 0;

        public static int S_FUSION = 1;
        public static int M_FUSION = 2;
        public static int L_FUSION = 3;
        public static int XL_FUSION = 4;
        public static int S_FUSION_BREEDER = 5;
        public static int M_FUSION_BREEDER = 6;
        public static int L_FUSION_BREEDER = 7;
        public static int XL_FUSION_BREEDER = 8;
        public static int S_TWIN_FUSION_RAMJET = 9;
        public static int M_TWIN_FUSION_RAMJET = 10;
        public static int L_TWIN_FUSION_RAMJET = 11;
        public static int XL_TWIN_FUSION_RAMJET = 12;
        public static int S_SOLAR_BOOSTER = 13;
        public static int M_SOLAR_BOOSTER = 14;
        public static int L_SOLAR_BOOSTER = 15;
        public static int XL_SOLAR_BOOSTER = 16;

        public static int[] enginesList = { TEST_ENGINE,
            S_FUSION, M_FUSION, L_FUSION, XL_FUSION,
            S_FUSION_BREEDER, M_FUSION_BREEDER, L_FUSION_BREEDER, XL_FUSION_BREEDER,
            S_TWIN_FUSION_RAMJET, M_TWIN_FUSION_RAMJET, L_TWIN_FUSION_RAMJET, XL_TWIN_FUSION_RAMJET,
            S_SOLAR_BOOSTER, M_SOLAR_BOOSTER, L_SOLAR_BOOSTER, XL_SOLAR_BOOSTER
        };

        private void genEngines() {
            engines = new Engine[enginesList.Length];

            Engine engine = new Engine(TEST_ENGINE, 100, 1600, 80, 800, 60, 100, 1000, "Test Engine");
            engines[engine.engineId] = engine;

            // Fusion
            engine = new Engine(S_FUSION, 100, 1600, 80, 800, 60, 100, 1000, "Small Fusion Engine");
            engines[engine.engineId] = engine;

            engine = new Engine(M_FUSION, 90, 3200, 160, 1600, 120, 250, 2500, "Medium Fusion Engine");
            engines[engine.engineId] = engine;

            engine = new Engine(L_FUSION, 80, 6400, 320, 3200, 240, 750, 7500, "Large Fusion Engine");
            engines[engine.engineId] = engine;
            
            engine = new Engine(XL_FUSION, 70, 12800, 640, 6400, 480, 2000, 23000, "XL Fusion Engine");
            engines[engine.engineId] = engine;

            // Fusion Breeder
            engine = new Engine(S_FUSION_BREEDER, 200, 1600, 80, 800, 60, 150, 1500, "Small Fusion Breeder Engine");
            engines[engine.engineId] = engine;

            engine = new Engine(M_FUSION_BREEDER, 190, 3200, 160, 1600, 120, 400, 4000, "Medium Fusion Breeder Engine");
            engines[engine.engineId] = engine;

            engine = new Engine(L_FUSION_BREEDER, 180, 6400, 320, 3200, 240, 1000, 10000, "Large Fusion Breeder Engine");
            engines[engine.engineId] = engine;

            engine = new Engine(XL_FUSION_BREEDER, 170, 12800, 640, 6400, 480, 3000, 30000, "XL Fusion Breeder Engine");
            engines[engine.engineId] = engine;

            // Fusion Ramjet
            engine = new Engine(S_TWIN_FUSION_RAMJET, 60, 4000, 160, 3000, 90, 150, 7000, "Small Fusion Ramjet Engine");
            engines[engine.engineId] = engine;

            engine = new Engine(M_TWIN_FUSION_RAMJET, 50, 8000, 320, 6000, 180, 400, 16000, "Medium Fusion Ramjet Engine");
            engines[engine.engineId] = engine;

            engine = new Engine(L_TWIN_FUSION_RAMJET, 40, 16000, 640, 12000, 360, 1000, 36000, "Large Fusion Ramet Engine");
            engines[engine.engineId] = engine;

            engine = new Engine(XL_TWIN_FUSION_RAMJET, 30, 32000, 1280, 24000, 720, 3000, 80000, "XL Fusion Ramjet Engine");
            engines[engine.engineId] = engine;

            // Solar Booster
            engine = new Engine(S_SOLAR_BOOSTER, 800, 400, 300, 100, 30, 300, 2000, "Small Fusion Ramjet Engine");
            engines[engine.engineId] = engine;

            engine = new Engine(M_SOLAR_BOOSTER, 750, 800, 600, 200, 60, 800, 5000, "Medium Fusion Ramjet Engine");
            engines[engine.engineId] = engine;

            engine = new Engine(L_SOLAR_BOOSTER, 600, 1600, 1000, 400, 120, 1600, 12000, "Large Fusion Ramet Engine");
            engines[engine.engineId] = engine;

            engine = new Engine(XL_SOLAR_BOOSTER, 400, 3200, 1500, 800, 240, 3200, 27000, "XL Fusion Ramjet Engine");
            engines[engine.engineId] = engine;
        }

        public Engine getEngine(int index) {
            if (index >= 0 && index < enginesList.Length) {
                return engines[index];
            }
            return null;
        }
    }
}

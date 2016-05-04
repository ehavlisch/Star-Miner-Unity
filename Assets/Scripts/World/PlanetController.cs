using UnityEngine;
using System.Collections.Generic;

using Economy;
using Util;


public class PlanetController : MonoBehaviour {

    private bool visited = false;

    private int tier;
    private Dictionary<int, List<Industry>> industryTierMap;

    // How many people live on the planet
    private long population;
    private long maxPopulation;
    // How physicially large is the planet, KM
    private int radius;
    // How easy is it to live on this planet
    private float hospitable;

    void Start () {
        
    }
	
	void Update () {
	
	}

    public void visit() {
        if(!visited) {
            initializePlanet();
        }
    }
    
    private void initializePlanet() {
        hospitable = Random.Range(0.0f, 1.0f);
        // Sizes range from pluto sized to half brown dwarf sized
        radius = Utils.randomInt(1000, 400000);
        tier = Utils.randomInt(0, 5);


        float surfaceArea = 4 * Mathf.PI * radius * radius;
        maxPopulation = System.Convert.ToInt64(surfaceArea * maxPopDensityByTier(tier) * hospitable);

        population = Utils.randomLong(0, (maxPopulation)/2);
        visited = true;
    }

    private int maxPopDensityByTier(int tier) {
        switch (tier) {
            case 0: return 1;
            case 1: return 2;
            case 2: return 4;
            case 3: return 8;
            case 4: return 16;
            case 5: return 32;
            default: return 32;
        }
    }

    // Methods for tests
    public void debugPlanet() {
        Debug.Log("Population: " + Utils.readableLong(population) + " - " + population);
        Debug.Log("Max Population: " + Utils.readableLong(maxPopulation) + " - " + maxPopulation);
        Debug.Log("Radius: " + radius + " - " + radius/6731 + " earths.");
        float surfaceArea = 4 * Mathf.PI * radius * radius;
        Debug.Log("People / Km2: " + (population / surfaceArea));
        Debug.Log("Max pop / km2: " + (maxPopulation / surfaceArea));
        Debug.Log("Hospitable: " + hospitable);
        Debug.Log("Tier: " + tier + " - Expected max pop density: " + maxPopDensityByTier(tier) * hospitable);
    }


}

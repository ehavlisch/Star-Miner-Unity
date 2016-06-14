using UnityEngine;
using System.Collections.Generic;

using Economy;
using Util;
using System.Text;

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
    // How much of the planet is livable
    // Not arctic, ocean, desert, mountain, etc
    private float livable;

    private int maxIndustries;

    private Dictionary<int, int> resourceStockMap;

    private float lastVisit;

    void Start () {
        
    }
	
	void Update () {
	
	}

    public void visit() {
        // To get the number of ticks, need the gameController
        GameController gc = GameObject.Find("GameController").GetComponent<GameController>();
        resourceStockMap = new Dictionary<int, int>();
        if (!visited) {
            initializePlanet();
            lastVisit = gc.getGameTime();
        } else {
            // Calculate any resource income and write to resourceStockMap

            runIndustries(Mathf.FloorToInt(gc.getGameTime() - lastVisit));
            lastVisit = gc.getGameTime();
        }
    }
    
    private void initializePlanet() {
        hospitable = Random.Range(0.0f, 1.0f);
        livable = Random.Range(0.1f, 0.5f);
        // Sizes range from pluto sized to half brown dwarf sized
        radius = Utils.randomInt(1000, 400000);
        tier = Utils.randomInt(0, 5);

        float surfaceArea = 4 * Mathf.PI * radius * radius;
        maxPopulation = System.Convert.ToInt64(surfaceArea * maxPopDensityByTier(tier) * hospitable * livable);

        population = Utils.randomLong(0, (maxPopulation/(EconomySingleton.Instance.getIndustryMaxTier() - tier + 1)));
        
        int upperIndustries = Mathf.CeilToInt((tier + 1) * 200 * hospitable * population / maxPopulation);

        maxIndustries = Utils.randomInt(tier * 2, upperIndustries);
        int industries = Utils.randomInt(0, maxIndustries / 2);

        industryTierMap = new Dictionary<int, List<Industry>>();
        for (int i = 0; i < industries; i++) {
            Industry industry = EconomySingleton.Instance.getRandomIndustryBelow(tier);
            List<Industry> tierList;
            if(industryTierMap.TryGetValue(industry.tier, out tierList)) {
                tierList.Add(industry);
            } else {
                tierList = new List<Industry>();
                tierList.Add(industry);
                industryTierMap.Add(industry.tier, tierList);
            }
        }

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

    private void runIndustries(int ticks) {
        if(ticks <= 0) {
            return;
        }

        List<Industry> playerIndustries = new List<Industry>();
        List<Industry> tierList;
        for (int i = EconomySingleton.Instance.getIndustryMaxTier(); i >= 0; i--) {
            if(industryTierMap.TryGetValue(i, out tierList)) {
                foreach(Industry industry in tierList) {
                    if(industry.playerOwned) {
                        playerIndustries.Add(industry);
                    }
                }
            }
        }

        List<Industry> haltedIndustries = new List<Industry>();
        bool industryProduced = false;

        if(playerIndustries.Count > 0) {
            for(int i = 0; i < ticks; i++) {
                for(int industryIndex = 0; industryIndex < playerIndustries.Count; industryIndex++) {
                    Industry industry = playerIndustries[industryIndex];
                    IndustryRunResult result = industry.run(resourceStockMap);
                    switch (result) {
                        case IndustryRunResult.HALTED: {
                            haltedIndustries.Add(industry);
                            playerIndustries.RemoveAt(industryIndex);
                            industryIndex--;
                            break;
                        }
                        case IndustryRunResult.DISABLED: {
                            playerIndustries.RemoveAt(industryIndex);
                            industryIndex--;
                            break;
                        }
                        case IndustryRunResult.PRODUCED: {
                            industryProduced = true;
                            break;
                        }
                        case IndustryRunResult.RUNNING: {
                            // Nothing
                            break;
                        }
                        default: {
                            Debug.LogWarning("Unrecognized industry run result!");
                            break;
                        }
                    }
                }

                if(industryProduced) {
                    foreach(Industry halted in haltedIndustries) {
                        playerIndustries.Add(halted);
                    }
                    haltedIndustries = new List<Industry>();
                    industryProduced = false;
                } else if(playerIndustries.Count == 0) {
                    return;
                }
            }
        }
        
    }

    // Methods for tests
    public void debugPlanet() {
        StringBuilder sb = new StringBuilder();
        sb.Append("Planet info:\n");
        sb.Append("Tier: ").Append(tier).Append(" Radius: ").Append(radius).Append(".\n");
        sb.Append("Hospitable: ").Append(hospitable).Append(" Livable: ").Append(livable).Append(".\n");

        sb.Append("Population: ").Append(Utils.readableLong(population)).Append(" - ").Append(population).Append(".\n");
        sb.Append("Max Population: ").Append(Utils.readableLong(maxPopulation)).Append(" - ").Append(maxPopulation).Append(".\n");

        float surfaceArea = 4 * Mathf.PI * radius * radius;
        sb.Append("People / Km2: ").Append(population / (surfaceArea * livable)).Append(".\n");
        sb.Append("Max People / Km2: ").Append(maxPopulation / (surfaceArea * livable)).Append(".\n");

        sb.Append("MaxIndustries: ").Append(maxIndustries).Append(".\n");

        Debug.Log(sb.ToString());
    }


}

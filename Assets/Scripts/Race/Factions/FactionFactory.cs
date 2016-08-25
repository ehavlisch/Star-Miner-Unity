using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Races {
    public static class FactionFactory {

        /*
        Adding a new Faction
            1. Add the factionName to the factionNames static array.
            2. Add a static instance of the Faction to this class.
            3. Add an entry in the factionName -> instance map.
            Optional:
            4. Add a case to the getFaction(Race race) method
            5. Add a case to the RaceFactory.getRace(Faction faction) method
            6. Setup default faction relations.
            7. Setup default player relation.

        */
        private static string[] factionNames = { UnitedPlanets.NAME, AresCoalition.NAME, NoFaction.NAME };
        
        // TODO these will eventually have to be loaded or updated somehow to make sure their attributes can change over time.
        // otherwise they will just reset to the defaults on startup
        private static AresCoalition aresCoalition = new AresCoalition();
        private static UnitedPlanets unitedPlanets = new UnitedPlanets();
        private static NoFaction noFaction = new NoFaction();

        private static readonly Dictionary<string, Faction> factions = new Dictionary<string, Faction> {
            { UnitedPlanets.NAME, aresCoalition },
            { AresCoalition.NAME, unitedPlanets },
            { NoFaction.NAME, noFaction }
        };

        public static Faction getFaction(string name) {
            Faction output = null;
            factions.TryGetValue(name, out output);
            return output;
        }

        // Returns a faction that a member of the race might be in
        public static Faction getFaction(Race race) {
            Faction faction = noFaction;
            switch (race.getName()) {
                case Aug.NAME:
                    return Utils.getItemByPercents(new Faction[] { noFaction }, new int[] { 100 });
                case PEH.NAME:
                    return Utils.getItemByPercents(new Faction[] { aresCoalition, unitedPlanets, noFaction }, new int[] { 30, 60, 10 });
                case QeThil.NAME:
                    return Utils.getItemByPercents(new Faction[] { noFaction }, new int[] { 100 });
                default:
                    return faction;
            }
        }

        public static int getFactionCount() {
            return factionNames.Length;
        }

        public static Faction getFaction(int index) {
            return getFaction(factionNames[index]);
        }

        public static int getFactionIndex(string name) {
            for(int i = 0; i < factionNames.Length; i++) {
                if(string.ReferenceEquals(name, factionNames[i])) {
                    return i;
                }
            }
            Debug.LogError("Unable to find the faction index of " + name + ".");
            return -1;
        }
    }
}
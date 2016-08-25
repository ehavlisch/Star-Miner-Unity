using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Races {
    public static class RaceFactory {

        /*
        Adding a new Race:
            1. Add static declaration of the race in this class.
            2. Add the name of the race to the raceNames static string.
            3. Add the race to the raceName -> raceInstance map.
            Optional:
            4. Add the race into the getRace(Faction faction) method.
            5. Add the race into the FactionFactory.getFaction(Race race) method.
            6. Setup the default relations for the race and other races by overriding the getDefaultRelation method in the race.
            7. Setup default player relation
        */
        private static QeThil qeThil = new QeThil();
        private static PEH peh = new PEH();
        private static Aug aug = new Aug();

        private static readonly Dictionary<string, Race> races = new Dictionary<string, Race> {
            { QeThil.NAME, qeThil },
            { PEH.NAME, peh },
            { Aug.NAME, aug }
        };

        private static string[] raceNames = { QeThil.NAME, PEH.NAME, Aug.NAME };

        public static int getRaceCount() {
            return raceNames.Length;
        }

        public static Race getRace() {
            return getRace(Utils.getMember(raceNames));
        }

        public static Race getRace(string race) {
            Race output = null;
            races.TryGetValue(race, out output);
            return output;
        }
        
        public static Race getRace(Faction faction) {
            if (faction == null) {
                Debug.LogWarning("Null faction passed to getRace.");
                return RaceFactory.getRace(PEH.NAME);
            }

            switch (faction.getName()) {
                case UnitedPlanets.NAME:
                    return Utils.getItemByPercents(new Race[] { peh }, new int[] { 100 });
                case AresCoalition.NAME:
                    return Utils.getItemByPercents(new Race[] { peh }, new int[] { 100 });
                case NoFaction.NAME:
                    return Utils.getItemByPercents(new Race[] { peh, aug }, new int[] { 90, 10 });
                default:
                    Debug.LogWarning("Default case of getRace. This needs to handle any faction properly!");
                    return RaceFactory.getRace(PEH.NAME);
            }
        }

        public static Race getRace(int i) {
            return getRace(raceNames[i]);
        }

        public static int getRaceIndex(string name) {
            for(int i = 0; i < raceNames.Length; i++) {
                if(string.ReferenceEquals(name, raceNames[i])) {
                    return i;
                }
            }
            Debug.LogError("Cannot find race index of " + name + ".");
            return -1;
        }
    }
}
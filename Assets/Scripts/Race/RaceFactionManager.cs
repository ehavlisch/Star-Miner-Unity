using UnityEngine;

namespace Races {
    public class RaceFactionManager {

        /*
            Race / Faction Relations
            Race and faction relations are on a scale from 0 to 10,000
            9000 - 10000 Revered, allies
            8000 - 8999 Loved
            7000 - 7999 Liked
            6000 - 6999 Warm Relationship
            5000 - 5999 Neutral
            4000 - 4999 Neutral
            3000 - 3999 Chilly Relationship
            2000 - 2999 Unliked
            1000 - 1999 Hated
            0    -  999 Despised, enemies
        */
        private int[,] globalRaceRelations;
        private int[] playerRaceRelations;

        private int[,] globalFactionRelations;
        private int[] playerFactionRelations;

        public RaceFactionManager() {
            // Setup global race relations
            int races = RaceFactory.getRaceCount();
            globalRaceRelations = new int[races, races];
            for (int i = 0; i < races; i++) {
                Race race = RaceFactory.getRace(i);

                for(int j = 0; j < races; j++) {

                    if(i == j) {
                        globalRaceRelations[i, j] = -1;
                    } else {
                        int relation = race.getDefaultRelationship(RaceFactory.getRace(j).getName());
                        globalRaceRelations[i, j] = relation;
                    }
                }
            }

            // Setup player race relations
            playerRaceRelations = new int[races];
            for(int i = 0; i < races; i++) {
                playerRaceRelations[i] = RaceFactory.getRace().getDefaultPlayerRelationship();
            }


            // Setup global faction relations
            int factions = FactionFactory.getFactionCount();
            globalFactionRelations = new int[factions, factions];
            for (int i = 0; i < factions; i++) {
                Faction faction = FactionFactory.getFaction(i);

                for(int j = 0; j < factions; j++) {
                    if(i == j) {
                        globalFactionRelations[i, j] = -1;
                    } else {
                        Race other = RaceFactory.getRace(j);
                        int relation = faction.getDefaultFactionRelation(other.getName());
                        globalFactionRelations[i, j] = relation;
                    }
                }
            }

            // Setup player faction relations
            playerFactionRelations = new int[factions];
            for(int i = 0; i < factions; i++) {
                playerFactionRelations[i] = FactionFactory.getFaction(i).getDefaultPlayerRelation();
            }
            Debug.Log("Finished setting up default relations.");
        }
        
        public int getGlobalRaceRelationship(string first, string second) {
            return globalRaceRelations[RaceFactory.getRaceIndex(first), RaceFactory.getRaceIndex(second)];
        }

        public int getPlayerRaceRelationship(string race) {
            return playerRaceRelations[RaceFactory.getRaceIndex(race)];
        }

        public int getGlobalFactionRelationship(string first, string second) {
            return globalFactionRelations[FactionFactory.getFactionIndex(first), FactionFactory.getFactionIndex(second)];
        }

        public int getPlayerFactionRelationship(string faction) {
            return playerFactionRelations[FactionFactory.getFactionIndex(faction)];
        }

        // Maybe unused ? Wrote them to have them handy
        public int getGlobalRelationShip(Race first, Race second) {
            return getGlobalRaceRelationship(first.getName(), second.getName());
        }

        public int getPlayerRelationship(Race race) {
            return getPlayerRaceRelationship(race.getName());
        }

        public int getGlobalRelationship(Faction first, Faction second) {
            return getGlobalFactionRelationship(first.getName(), second.getName());
        }

        public int getPlayerRelationship(Faction faction) {
            return getPlayerFactionRelationship(faction.getName());
        }
    }
}
namespace Races {
    public class UnitedPlanets : Faction {
        public const string NAME = "United Planets of America";

        public UnitedPlanets() {
            agression = 55;
            size = 65;
            power = 80;

            spacetech = 50;
            biotech = 70;
            robotech = 75;
        }

        public override string getDescription() {
            return "The United Planets is one of the stronges Post Earth Human factions. Formerly known as the United States of America, the United Planets lead the Post Earth Humans away from Earth on gargantuan colony ships. The United Planets has established colonies and planets across the universe. Some of the planets and colonies have fragmented away from the United Planets, but because the faction is so massive, little can be done about the rogue groups. The central government is slow to anger, but once angered can become incredibly poweful and resiliant.";
        }

        public override string getMemberName() {
            return "American";
        }

        public override string getName() {
            return NAME;
        }

        public override string getPluralMemberName() {
            return "Americans";
        }

        public override int getDefaultFactionRelation(string name) {
            switch (name) {
                default:
                    return 5000;
            }
        }

        public override int getDefaultPlayerRelation() {
            return 5000;
        }
    }
}
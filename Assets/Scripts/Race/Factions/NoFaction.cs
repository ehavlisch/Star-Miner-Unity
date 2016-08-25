namespace Races {
    public class NoFaction : Faction {

        public const string NAME = "Factionless";

        public NoFaction() {
            agression = 20;
            size = 5;
            power = 20;
            
            robotech = 50;
            spacetech = 50;
            biotech = 50;
        }

        public override string getDescription() {
            return "The group of wanderes that have no home. They hold no allegiances and have no one looking out for them.";
        }

        public override string getMemberName() {
            return "Factionless";
        }

        public override string getName() {
            return NAME;
        }

        public override string getPluralMemberName() {
            return "Factionless";
        }
    }
}
namespace Races {
    public class AresCoalition : Faction {

        public const string NAME = "Ares Coalition";

        public AresCoalition() {
            agression = 85;
            size = 20;
            power = 20;

            spacetech = 35;
            biotech = 60;
            robotech = 35;
        }

        public override string getDescription() {
            return "The Ares Coalition is a organized pirate organization of mostly Humans. The group was formed by early Mars colonists who felt abandoned by the Post Earth Humans. They rely heavily on massively overpowering their prey with many small ships in order to maintain their headcount and superiority. Many factions have tried to eliminate the Ares Coalition, but the group has proven difficult to eradicate completely.";
        }

        public override string getMemberName() {
            return "Son of Ares";
        }

        public override string getName() {
            return NAME;
        }

        public override string getPluralMemberName() {
            return "Children of Ares";
        }

        public override int getDefaultFactionRelation(string name) {
            switch (name) {
                default:
                    return 2000;
            }
        }

        public override int getDefaultPlayerRelation() {
            return 2000;
        }
    }
}
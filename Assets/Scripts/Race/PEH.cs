using Util;

namespace Races {
    public class PEH : Race {

        public const string NAME = "Post-Earth Human";

        public override string generateName(bool masculine) {
            if(masculine) {
                return Utils.getMember(PEHstrings.mascBase) + " " + Utils.getMember(PEHstrings.surnames);
            } else {
                return Utils.getMember(PEHstrings.femBase) + " " + Utils.getMember(PEHstrings.surnames);
            }
        }

        public override string getMemberName() {
            return "Human";
        }

        public override string getMemberName(bool masculine) {
            if(masculine) {
                return "Man";
            } else {
                return "Woman";
            }
        }

        public override string getName() {
            return NAME;
        }

        public override string getPluralMemberName() {
            return "Humans";
        }

        public override string getPluralMemberName(bool masculine) {
            if(masculine) {
                return "Men";
            } else {
                return "Women";
            }
        }

        public override string getPluralName() {
            return "Humans";
        }

        public override bool hasMasculine() {
            return true;
        }

        public override string getShortDescription() {
            return PEHstrings.shortDescription;
        }

        public override string getDetailedDescription() {
            return PEHstrings.detailedDescription;
        }

        public override int getDefaultRelationship(string name) {
            switch (name) {
                default:
                    return 5000;
            }
        }

        public override int getDefaultPlayerRelationship() {
            return 5000;
        }
    }
}
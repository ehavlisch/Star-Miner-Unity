using Util;

namespace Races {
    public class QeThil : Race {
        
        public const string NAME = "Qe'Thil";

        public override string generateName(bool masculine) {
            if(masculine) {
                return Utils.getMember(QeThilStrings.mascBase) + "'" + Utils.getMember(QeThilStrings.mascEnds);
            } else {
                return Utils.getMember(QeThilStrings.femBase) + "'" + Utils.getMember(QeThilStrings.femEnds);
            }
        }

        public override string getName() {
            return NAME;
        }

        public override string getPluralName() {
            return "Qe'Thil";
        }

        public override string getMemberName() {
            return "Qe'Thillian";
        }

        public override string getPluralMemberName() {
            return "Qe'Thillians";
        }

        public override string getMemberName(bool masculine) {
            if(masculine) {
                return "Qe'Thilo";
            } else {
                return "Qe'Thilu";
            }
        }

        public override string getPluralMemberName(bool masculine) {
            if(masculine) {
                return "Qe'Thiloe";
            } else {
                return "Qe'Thilue";
            }
        }

        public override bool hasMasculine() {
            return true;
        }

        public override string getShortDescription() {
            return QeThilStrings.shortDescription;
        }

        public override string getDetailedDescription() {
            return QeThilStrings.detailedDescription;
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
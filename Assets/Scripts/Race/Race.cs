using System;
using Util;

namespace Races {
    public abstract class Race {
        public virtual bool hasMasculine() {
            return false;
        }
        
        public virtual string generateName() {
            return generateName(Utils.nextBoolean());
        }

        public abstract string generateName(bool masculine);

        public abstract string getName();
        public abstract string getPluralName();
        public abstract string getMemberName();

        public virtual string getMemberName(bool masculine) {
            return getMemberName();
        }

        public abstract string getPluralMemberName();

        public virtual string getPluralMemberName(bool masculine) {
            return getPluralMemberName();
        }

        public abstract string getShortDescription();
        public abstract string getDetailedDescription();

        public virtual int getDefaultRelationship(string name) {
            return 5000;
        }

        public virtual int getDefaultPlayerRelationship() {
            return 5000;
        }
    }
}
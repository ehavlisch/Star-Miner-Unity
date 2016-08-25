namespace Races {
    public abstract class Faction {

        // Agression scale:

        // Hostile
        // 90 - 100: Attack on sight
        // 80 - 89: Attack on sight of weaker
        // 70 - 79: Attack on sight of enemy
        // 60 - 69: Attack on sight of weaker enemy
        // 50 - 59: Attack on sight of much weaker enemy
        // Defensive
        // 40 - 49: Defend when attacked
        // 30 - 39: Defend when attacked by weaker, otherwise flee
        // 20 - 29: Defend when attacked by much weaker, otherwise flee
        // 10 - 19: Defend if cannot flee
        // Flee
        // 0  -  9: Flee, will not attack

        protected int agression = -1;
        protected int size = -1;
        protected int power = -1;

        protected int robotech = -1;
        protected int biotech = -1;
        protected int spacetech = -1;
        
        public int getAgression() {
            return agression;
        }

        public int getSize() {
            return size;
        }

        public int getPower() {
            return power;
        }

        public int getRobotech() {
            return robotech;
        }

        public int getBiotech() {
            return biotech;
        }

        public int getSpacetech() {
            return spacetech;
        }

        public abstract string getDescription();
        public abstract string getName();
        public abstract string getMemberName();
        public abstract string getPluralMemberName();

        public virtual int getDefaultFactionRelation(string name) {
            return 5000;
        }

        public virtual int getDefaultPlayerRelation() {
            return 5000;
        }
    }
}

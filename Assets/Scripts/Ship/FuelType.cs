namespace Ship {
    public class FuelType {
        public float density;
        public float efficiency;

        public FuelType(float density, float efficiency) {
            this.density = density;
            this.efficiency = efficiency;
        }

        public float getMass(float volume) {
            return density * volume;
        }
    }
}
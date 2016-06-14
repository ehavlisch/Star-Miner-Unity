namespace Economy {
    public class Cargo {
        public float mass;
        public float volume;
        public string name;
        public float value;
        public string pickupName;

        public CargoType cargoType;

        public Cargo(CargoType cargoType, float mass, float volume, string name, float value, string pickupName) {
            this.cargoType = cargoType;
            this.mass = mass;
            this.volume = volume;
            this.name = name;
            this.value = value;
            this.pickupName = pickupName;
        }
    }
}
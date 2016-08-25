using Economy;

namespace Ship {
    public class Generator : Cargo {

        public float maxEnergy;
        public float rechargeRate;

        public Generator(float mass, float maxEnergy, float rechargeRate, float volume, float value, string name) : base(CargoType.GENERATOR, mass, volume, name, value, "generator") {
            this.maxEnergy = maxEnergy;
            this.rechargeRate = rechargeRate;
        }
    }
}
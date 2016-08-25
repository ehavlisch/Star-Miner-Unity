using Economy;

namespace Ship {
    public class FuelTank : Cargo {
        //d = mass / volume;
        public FuelType fuelType;

        public float fuelVolume;
        public float fuelEfficiency;

        public float tankVolume;

        public float tankDurability;

        public FuelTank(FuelType fuelType, float fuelVolume, float tankVolume, float mass, float tankDurability, float volume, string name, float value) : base(CargoType.FUEL_TANK, mass, volume, name, value, "fueltank") {
            this.fuelType = fuelType;
            this.fuelVolume = fuelVolume;
            this.tankVolume = tankVolume;
            this.tankDurability = tankDurability;
        }

        public bool hasFuel() {
            return fuelVolume > 0;
        }

        public float subtractFuel(float power) {
            float volume = power / fuelType.efficiency;
            if (fuelVolume - volume < 0) {
                // Drain the tank
                float remainingPower = fuelVolume * fuelType.efficiency;
                fuelVolume = 0;
                return remainingPower;
            } else {
                fuelVolume -= volume;
                return power;
            }
        }

        public float getTotalMass() {
            return fuelType.getMass(fuelVolume) + mass;
        }
    }
}
using Economy;
using System.Collections.Generic;

namespace Ship {
    public class CargoBay : Cargo {

        public float filledVolume;
        public List<Cargo> cargo;

        public CargoBay(float volume, float mass, float value, string name) : base(CargoType.CARGO_BAY, mass, volume, name, value, "cargobay") {
            this.volume = volume;
            this.mass = mass;
            this.value = value;
            this.name = name;

            cargo = new List<Cargo>();
        }

        public bool addCargo(Cargo c) {
            if (filledVolume + c.volume > volume) {
                return false;
            } else {
                cargo.Add(c);
                filledVolume += c.volume;
                return true;
            }
        }

        public float calculateMass() {
            float totalMass = mass;
            foreach (Cargo c in cargo) {
                totalMass += c.mass;
            }
            return totalMass;
        }
    }
}
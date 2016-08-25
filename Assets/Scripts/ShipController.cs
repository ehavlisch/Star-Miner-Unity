using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Ship;
using Economy;
using ship;

public class ShipController : MonoBehaviour {

	public float mass;

	public int maxFuelTanks;
	private List<FuelTank> fuelTanks;

	private float maxFuel;

	public int maxEngines;
	private List<Engine> engines;

	public int maxGenerators;
	private List<Generator> generators;

	public int maxWeapons;
	private Weapon[] weapons;
	private Transform[] shotSpawns;

	private int maxCargoBays;
	private List<CargoBay> cargoBays;

	private float energy;
	private float maxEnergy;
	private float rechargeRate;

	public float maxHull;
	private float hull;

	private List<AudioSource> engineAudioSources;

	void Start() {
		maxFuelTanks = 4;
		maxEngines = 2;
		maxGenerators = 2;
		maxCargoBays = 4;
		maxWeapons = 2;
		fuelTanks = new List<FuelTank> (maxFuelTanks);
		engines = new List<Engine> (maxEngines);
		generators = new List<Generator> (maxGenerators);
		cargoBays = new List<CargoBay> (maxCargoBays);

		fuelTanks.Add (new FuelTank (new FuelType (1.0f, 100.0f), 100.0f, 100.0f, 5.0f, 100.0f, 100.0f, "Basic Fuel Tank", 100.0f));	
		maxFuel = calculateMaxFuelVolume ();

		engines.Add (EngineSingleton.Instance.getEngine(0));
        
        engineAudioSources = new List<AudioSource> (maxEngines);
		foreach(Engine engine in engines) {
			engineAudioSources.Add(engine.getMainSound(gameObject));
		}

		generators.Add (new Generator (20, 30, 1f, 100, 1000, "Basic Generator"));
		maxEnergy = calculateTotalMaxEnergy ();
		rechargeRate = calculateTotalEnergyRecharge ();
		energy = 0;

		cargoBays.Add (new CargoBay (2000, 100, 100, "Basic Cargo Bay"));

		hull = maxHull;
		weapons = new Weapon[maxWeapons];
		shotSpawns = new Transform[maxWeapons];

		for (int i = 0; i < maxWeapons; i++) {
			shotSpawns[i] = (transform.FindChild("ShotSpawn" + (i+1)));
		}

		weapons[0] = new SimpleLaser ();

		weapons[1] = new SimpleLaser ();

		GetComponent<Rigidbody>().mass = calculateMass();

	}

	void OnCollisionEnter(Collision col) {
		if (col.gameObject.tag == "World" || col.gameObject.tag == "Enemy") {
			Debug.Log ("Collision Magnitude: " + col.relativeVelocity.magnitude);
			if (col.relativeVelocity.magnitude > .1) {
				energy -= col.relativeVelocity.magnitude * 10;
				if (energy < 0) {
					hull += energy;
					energy = 0;
				}
			}
		} else if (col.gameObject.tag == "Pickup") {
			if(addCargo(col.gameObject.GetComponent<PickupController>().cargo)) {
				Destroy (col.gameObject);
				GetComponent<Rigidbody>().mass = calculateMass ();
			}
		} else {
			Debug.Log ("Non world/enemy/projectile/pickup collision");
		} 
	}

	public bool addCargo(Cargo c) {
		foreach (CargoBay cb in cargoBays) {
			if(cb.addCargo (c)) {
				return true;
			}
		}
		return false;
	}

	public void fire() {
		float angle = GetComponent<Rigidbody> ().rotation.eulerAngles.y * Mathf.Deg2Rad;
		for (int i = 0; i < maxWeapons; i++) {
			Weapon weapon = weapons[i];
			if(weapon != null) {
				if(weapon.ready()) {
					if(energy > weapon.getEnergyCost()) {
						GameObject projectileObject = (GameObject)Instantiate (Resources.Load(weapon.getProjectileName()), shotSpawns[i].position, shotSpawns[i].rotation);
						
						weapon.initProjectile(projectileObject, angle, GetComponent<Rigidbody>().velocity);
						
						// The projectile can not hit the player. Also lights cannot hit the player either
						Physics.IgnoreCollision(projectileObject.GetComponent<Collider>(), GetComponent<Collider>());
						
						if(weapon.hasRecoil()) {
							GetComponent<Rigidbody>().AddForce(weapon.getRecoil(angle));
						}

						weapon.fire();
						energy -= weapon.getEnergyCost();
					}
				}
			}

		}
	}

	public void setEngineVolume(float volume) {
		foreach (AudioSource audioSource in engineAudioSources) {
            if (audioSource != null) {
                if (audioSource.volume > volume) {
                    audioSource.volume = audioSource.volume - 0.05f;
                } else {
                    audioSource.volume = volume;
                }
            }
		}
	}

	public void updateEnergy() {
		energy += rechargeRate;
		if (energy > maxEnergy) {
			energy = maxEnergy;
		}
	}

	public float subtractFuel(float power) {
		foreach(FuelTank ft in fuelTanks) {
			power -= ft.subtractFuel(power);
			if(power == 0) {
				return 0;
			}
		}

		return power;
	}
	
	public bool hasFuel() {
		foreach(FuelTank ft in fuelTanks) {
			if(ft.hasFuel()) {
				return true;
			}
		}
		return false;
	}

	public float calculateMass() {
		float totalMass = mass;

		foreach(FuelTank ft in fuelTanks) {
			totalMass += ft.getTotalMass();
		}

		foreach (Engine e in engines) {
			totalMass += e.mass;
		}

		foreach (Generator g in generators) {
			totalMass += g.mass;
		}

		foreach (CargoBay cb in cargoBays) {
			totalMass += cb.calculateMass();
		}

		return totalMass;
	}
	
	public float calculateTotalMaxEnergy() {
		float totalEnergy = 0;
		foreach (Generator g in generators) {
			totalEnergy += g.maxEnergy;
		}
		return totalEnergy;
	}

	
	public float calculateTotalEnergyRecharge() {
		float totalRecharge = 0;
		foreach (Generator g in generators) {
			totalRecharge += g.rechargeRate;
		}
		return totalRecharge;
	}

	public float calculateEngineForce() {
		float totalForce = 0.0f;
		foreach (Engine e in engines) {
            totalForce += e.force;
		}
		return totalForce;
	}

	public float calculateEngineLatForce() {
		float totalForce = 0.0f;
		foreach (Engine e in engines) {
            totalForce += e.forceLat;
		}
		return totalForce;
	}

	public float calculateEngineEfficiency() {
		float totalEff = 0.0f;
		foreach (Engine e in engines) {
            totalEff += e.efficiency;
		}
		return totalEff;
	}

	public float calculateEngineRotate() {
		float totalForce = 0.0f;
		foreach (Engine e in engines) {
            totalForce += e.forceRotate;
		}
		return totalForce;
	}

	public float calculateFuelVolume() {
		float totalVolume = 0;
		foreach (FuelTank ft in fuelTanks) {
			totalVolume += ft.fuelVolume;
		}
		return totalVolume;
	}

	public float calculateMaxFuelVolume() {
		float totalVolume = 0;
		foreach (FuelTank ft in fuelTanks) {
			totalVolume += ft.tankVolume;
		}
		return totalVolume;
	}

	public float getEnergy() {
		return energy;
	}

	public float getMaxEnergy() {
		return maxEnergy;
	}

	public float getHull() {
		return hull;
	}

	public float getMaxFuel() {
		return maxFuel;
	}
}

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Ship;

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

		fuelTanks.Add (new FuelTank (new FuelType (1, 100), 100, 100, 5, 100));	
		maxFuel = calculateMaxFuelVolume ();

		engines.Add (new GenericEngine());

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
				if(Time.time > weapon.getNextFire()) {
					if(energy > weapon.energyCost) {
						GameObject projectileObject = (GameObject)Instantiate (Resources.Load(weapon.projectileName), shotSpawns[i].position , shotSpawns[i].rotation);
						Projectile projectile = projectileObject.GetComponent <Projectile>();

						Physics.IgnoreCollision(projectileObject.GetComponent<Collider>(), GetComponent<Collider>());

						float projectileVelocityX = Mathf.Sin (angle) * weapon.projectileSpeed;
						float projectileVelocityZ = Mathf.Cos (angle) * weapon.projectileSpeed;
						projectile.damage = weapon.damage;
						projectile.force = weapon.force;

						if(weapon.recoil) {
							float recoilX = - Mathf.Sin (angle) * weapon.recoilForce;
							float recoilZ = - Mathf.Cos (angle) * weapon.recoilForce;

							GetComponent<Rigidbody>().AddForce(new Vector3(recoilX, 0.0f, recoilZ));
						}

						Vector3 projectileVelocity = new Vector3(projectileVelocityX, 0, projectileVelocityZ);
						projectile.GetComponent<Rigidbody>().velocity = (projectileVelocity + GetComponent<Rigidbody>().velocity);
						weapon.setNextFire(Time.time);
						energy -= weapon.energyCost;
					}
				}
			}

		}
	}

	public void setEngineVolume(float volume) {
		foreach (AudioSource audioSource in engineAudioSources) {
			if(audioSource.volume > volume) {
				audioSource.volume = audioSource.volume - 0.05f;
			} else {
				audioSource.volume = volume;
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

public class Generator : Cargo {

	public float maxEnergy;
	public float rechargeRate;

	public Generator(float mass, float maxEnergy, float rechargeRate, float volume, float value, string name) {
		this.mass = mass;
		this.maxEnergy = maxEnergy;
		this.rechargeRate = rechargeRate;

		this.volume = volume;
		this.value = value;
		this.name = name;
	}
}

public class CargoBay : Cargo {

	public float filledVolume;
	public List<Cargo> cargo;

	public CargoBay(float volume, float mass, float value, string name) {
		this.volume = volume;
		this.mass = mass;
		this.value = value;
		this.name = name;

		cargo = new List<Cargo> ();
	}

	public bool addCargo(Cargo c) {
		if (filledVolume + c.volume > volume) {
			return false;
		} else {
			cargo.Add (c);
			filledVolume += c.volume;
			return true;
		}
	}

	public float calculateMass() {
		float totalMass = mass;
		foreach(Cargo c in cargo) {
			totalMass += c.mass;
		}
		return totalMass;
	}
}

public class FuelTank : Cargo{
	//d = mass / volume;
	public FuelType fuelType;

	public float fuelVolume;
	public float fuelEfficiency;

	public float tankVolume;

	public float tankDurability;

	public FuelTank(FuelType fuelType, float fuelVolume, float tankVolume, float mass, float tankDurability) {
		this.fuelType = fuelType;
		this.fuelVolume = fuelVolume;
		this.tankVolume = tankVolume;
		this.mass = mass;
		this.tankDurability = tankDurability;
	}

	public float subtractFuel(float power) {
		float volume = power / fuelType.efficiency;
		if (fuelVolume - volume < 0) {
			float temp = fuelVolume * fuelType.efficiency;
			fuelVolume = 0;
			return temp;
		} else {
			fuelVolume -= volume;
			return power;
		}
	}

	public float getTotalMass() {
		return fuelType.getMass (fuelVolume) + mass;
	}
}

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

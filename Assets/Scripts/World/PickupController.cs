using UnityEngine;
using System.Collections;

public class PickupController : MonoBehaviour {
	public Cargo cargo;

	void Start() {
		if (cargo == null) {
			cargo = new Cargo();
			cargo.mass = 100;
			cargo.volume = 100;
			cargo.name = "Basic Ore";
			cargo.value = 100;
		}
	}
}

public class Cargo {
	public float mass;
	public float volume;
	public string name;
	public float value;

	public float getMass() {
		return mass;
	}

	public float getVolume() {
		return volume;
	}

	public string getName() {
		return name;
	}

	public float getValue() {
		return value;
	}

	public Cargo() {

	}

	public Cargo(float mass, float volume, string name, float value) {
		this.mass = mass;
		this.volume = volume;
		this.name = name;
		this.value = value;
	}
}

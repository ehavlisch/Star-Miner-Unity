using UnityEngine;
using System.Collections;

namespace Ship {

public class Weapon {

	public float energyCost;
	
	public bool ammo;
	public int ammoType;
	
	public float damage;
	
	public float reloadSpeed;
	
	public float force;
	public bool recoil;
	public float recoilForce;
	
	public GameObject laserBolt;
	
	public float nextFire;
	
	public float projectileSpeed;

	public string projectileName;

	public Weapon(float energyCost, float damage, float reloadSpeed, float force, float recoilForce) {
		this.energyCost = energyCost;
		this.damage = damage;
		this.reloadSpeed = reloadSpeed;
		this.force = force;
		this.recoilForce = recoilForce;
		if(recoilForce != 0.0f) {
			recoil = true;
		}
	}

	public float getNextFire () {
		return nextFire;
	}
	
	public void setNextFire(float time) {
		nextFire = time + reloadSpeed;
	}
}

	public class SimpleLaser : Weapon {

		public SimpleLaser() : base (5, 30, 0.25f, 40, 20) {
			projectileSpeed = 8;
			ammo = false;
			ammo = false;
			projectileName = "LaserBolt";
		}
	}

}

using UnityEngine;
using System.Collections;

public class SimpleLaser : MonoBehaviour, WeaponController {

	public float energyCost;
	
	public bool ammo;
	public int ammoType;
	
	public float damage;
	
	public float reloadSpeed;

	public float force;
	public float recoilForce;

	public GameObject laserBolt;

	public float nextFire;

	public float projectileSpeed;

	void Start() {
		energyCost = 5;
		ammo = false;
		damage = 30;
		reloadSpeed = 0.25f;
		force = 40;
		recoilForce = 20;
		projectileSpeed = 8;
	}

	public float getEnergyCost ()
	{
		return energyCost;
	}

	public bool usesAmmo ()
	{
		return ammo;
	}

	public int getAmmoType ()
	{
		return ammoType;
	}

	public float getDamage ()
	{
		return damage;
	}

	public float getReloadSpeed ()
	{
		return reloadSpeed;
	}

	public float getRecoilForce ()
	{
		return recoilForce;
	}
	
	public float getNextFire ()
	{
		return nextFire;
	}

	public void setNextFire(float time) {
		nextFire = time + reloadSpeed;
	}

	public GameObject getProjectile ()
	{
		return laserBolt;
	}

	public float getProjectileSpeed() {
		return projectileSpeed;
	}

	public float getForce() {
		return force;
	}
}
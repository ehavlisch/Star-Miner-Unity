using UnityEngine;
using Economy;

namespace Ship {

public class Weapon : Cargo {

	protected float energyCost, damage, reloadSpeed, force, recoilForce, projectileSpeed, nextFire;
	protected bool ammo, recoil;
	protected int ammoType;
	protected string projectileName;

	public Weapon(float energyCost, bool ammo, float damage, float reloadSpeed, float force, float recoilForce, 
        float projectileSpeed, string projectileName, float mass, float volume, string name, float value) : base(CargoType.WEAPON, mass, volume, name, value, "weapon") {
        
		this.energyCost = energyCost;
		this.ammo = ammo;
		this.damage = damage;
		this.reloadSpeed = reloadSpeed;
		this.force = force;
		this.recoilForce = recoilForce;
		if(recoilForce != 0.0f) {
			recoil = true;
		}
		this.projectileSpeed = projectileSpeed;
		this.projectileName = projectileName;

        
	}
	
	public string getProjectileName() {
		return projectileName;
	}
	
	public void initProjectile(GameObject projectileObject, float angle, Vector3 shipVelocity) {
		Projectile projectile = projectileObject.GetComponent <Projectile>();
		
		float projectileVelocityX = Mathf.Sin (angle) * projectileSpeed;
		float projectileVelocityZ = Mathf.Cos (angle) * projectileSpeed;
		projectile.damage = damage;
		projectile.force = force;
		
		Vector3 projectileVelocity = new Vector3(projectileVelocityX, 0, projectileVelocityZ);
		projectile.GetComponent<Rigidbody>().velocity = (projectileVelocity + shipVelocity);
			
	}
	
	public bool hasRecoil() {
		return recoil;
	}
	
	public Vector3 getRecoil(float angle) {
		if(recoil) {
			float recoilX = - Mathf.Sin (angle) * recoilForce;
			float recoilZ = - Mathf.Cos (angle) * recoilForce;	
			return new Vector3(recoilX, 0.0f, recoilZ);
		}
		Debug.LogError("Calculating recoil vector when weapon has no recoil.");
		return new Vector3(0.0f, 0.0f, 0.0f);
	}
	
	public float getEnergyCost() {
		return energyCost;
	}
	
	public void fire() {
		nextFire = Time.time + reloadSpeed;
	}
	
	public bool ready() {
		return Time.time > nextFire;
	}
}

	public class SimpleLaser : Weapon {

		public SimpleLaser() : base (5, false, 30, 0.25f, 40, 20, 8, "LaserBolt", 10, 10, "Simple Laser", 100) {
		}
	}

}

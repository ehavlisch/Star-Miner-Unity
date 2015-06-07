using UnityEngine;
using System.Collections;

public interface WeaponController {

	float getEnergyCost();
	bool usesAmmo();
	int getAmmoType();
	float getDamage();
	float getReloadSpeed();
	float getRecoilForce();
	float getNextFire();
	GameObject getProjectile();
	void setNextFire(float time);
	float getProjectileSpeed();
	float getForce();
}

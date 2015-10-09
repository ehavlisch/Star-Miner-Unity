using UnityEngine;
using System.Collections;

public interface Projectile {
	float getDamage();
	void setDamage(float d);

	float getForce();
	void setForce(float f);

	void setVelocity(Vector3 velocity);
}

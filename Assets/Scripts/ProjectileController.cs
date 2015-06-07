using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour, Projectile {
	private float force;
	private float damage;
	private bool active;

	public float getDamage ()
	{
		return damage;
	}

	public void setDamage (float d)
	{
		damage = d;
	}

	public float getForce ()
	{
		return force;
	}

	public void setForce (float f)
	{
		force = f;
	}

	public void setVelocity(Vector3 velocity) {
		GetComponent<Rigidbody> ().velocity = velocity;
	}

	void Start() {
		Destroy (gameObject, 5.0f);
	}
}

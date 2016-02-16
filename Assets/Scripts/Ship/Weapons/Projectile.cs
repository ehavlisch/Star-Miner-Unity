using UnityEngine;
using System.Collections;

namespace Ship {

public class Projectile : MonoBehaviour {
	public float force;
	public float damage;
	public bool active;
	public float lifetime;

	public float createTime;

	void Start() {
		createTime = Time.time;
		lifetime = 5.0f;
	}

	void FixedUpdate() {
		Destroy(gameObject, lifetime);
	}
}

}

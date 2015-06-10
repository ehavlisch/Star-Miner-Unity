using UnityEngine;
using System.Collections;

public class AsteroidController : MonoBehaviour {

	public float health;
	private float maxHealth;

	public float value;

	public WorldController world;

	private bool firstFrame;

	private IntVector2 location;

	public void Start() {
		firstFrame = true;

		if (health == 0) {
			initialize();	
		}
	}

	public void initialize() {
		float scale = calculateScale (value);



		health = 300 * scale + 10 + Random.Range (0, 50);
		maxHealth = health;
		GetComponent <Rigidbody> ().mass = health;

		transform.localScale = new Vector3 (scale, scale, scale);
		
		firstFrame = true;
		
		transform.rotation = new Quaternion (Random.value, Random.value, Random.value, Random.value);
	}

	private float calculateScale(float value) {
		if (value >= 0.45 && value <= .55) {
			return - Mathf.Abs (value - .5f) * 20.0f + 2.0f;
		} else {
			return 1.0f;
		}
	}

	public void fragment(float value, int splits, float oldMaxHealth, WorldController world) {
		this.value = value;
		this.world = world;
		firstFrame = true;

		maxHealth = oldMaxHealth / splits / 2;
		health = maxHealth;

		float scale = calculateScale (value) / splits;
		transform.localScale = new Vector3 (scale, scale, scale);

		transform.rotation = new Quaternion (Random.value, Random.value, Random.value, Random.value);
		GetComponent <Rigidbody> ().mass = health;

	}

	public void FixedUpdate() {
		if (health < 0) {
			// If the object has a world location delete it,
			if(location != null) {
				world.destroy(location);
			} else {
				//TODO this should delete from the worlds miscObjects buckets
			}
			if(maxHealth > 200) {
				int splits = Mathf.FloorToInt (maxHealth/200);
				for(int i = 0; i <= splits; i++) {
					int theta = Random.Range (0, 360);
					int thetaOffset = 360 / (splits + 1) * i;
					Vector3 updatedPosition = new Vector3(Mathf.Sin (theta + thetaOffset) * Random.value, 0, Mathf.Cos (theta + thetaOffset)* Random.value);
					//AsteroidController asteroid = (AsteroidController) Instantiate (this, gameObject.transform.position + updatedPosition , gameObject.transform.rotation);

					GameObject asteroidObject = (GameObject) Instantiate (world.getAsteroid(), gameObject.transform.position + updatedPosition , gameObject.transform.rotation);
					asteroidObject.transform.parent = this.transform.parent;
					AsteroidController asteroid = (AsteroidController) asteroidObject.GetComponent ("AsteroidController");
					asteroid.fragment(value, splits, maxHealth, world);
					//Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), asteroid.GetComponent<Collider>());
				}
			} else {
				Debug.Log ("IMPLEMENT: Asteroid destroyed - spawn loot");
			}
			Destroy (gameObject);
		}

		// Slow the object when they're first spawned after splitting
		if (firstFrame) {
			firstFrame = false;
			Rigidbody rb = GetComponent<Rigidbody>();
			rb.velocity = rb.velocity * 0.1f;
			rb.angularVelocity = rb.angularVelocity * 0.8f;
		}
	}

	void OnCollisionEnter(Collision col) {
		Projectile projectile = col.transform.GetComponent<Projectile> ();
		if (projectile != null) {
			health -= projectile.getDamage();
			//GetComponent<Rigidbody>().AddForce
			Destroy(col.gameObject);
		} else {
			//Debug.Log ("IMPLEMENT: Asteroid hit by something else");
		}
	}

	public void setValue(float value) {
		this.value = value;
	}

	public void setWorld(WorldController world) {
		this.world = world;
	
	}

	public Rigidbody getRigidbody() {
		return GetComponentInChildren<Rigidbody> ();
	}

	public void setLocation(IntVector2 location) {
		this.location = location;
	}
}

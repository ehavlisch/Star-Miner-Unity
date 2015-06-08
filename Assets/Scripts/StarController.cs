using UnityEngine;
using System.Collections;

public class StarController : MonoBehaviour {

	public float collisionRadius;

	private float scale;
	private float gravityWellSize;

	public void Start() {
		GetComponent<SphereCollider> ().radius = collisionRadius;
		scale = this.transform.localScale.x;
		gravityWellSize = scale * collisionRadius;
	}

	public void OnTriggerEnter(Collider collider) {
		//Debug.Log ("Star Col!");
	}

	public void OnTriggerStay(Collider collider) {

		float force = -0.05f;
		// Can this just be this.transform? Or is it shifted?
		Vector3 planetPosition = new Vector3 (0, 0, 0);

		Rigidbody rb = collider.GetComponent<Rigidbody> ();
		Vector3 position = collider.GetComponent<Transform> ().position;
		if (rb == null) {
			if(collider.transform.parent.gameObject != null && collider.transform.parent.gameObject.GetComponent<Rigidbody>() != null) {
				rb = collider.transform.parent.gameObject.GetComponent<Rigidbody>();
				position = collider.transform.parent.transform.position;
			}
		}
	

		if (rb != null) {
			float distance = Vector3.Distance(planetPosition, position);

			float wellStrength = (gravityWellSize - distance) / distance;

			rb.AddForce (wellStrength * (position - planetPosition).normalized * force * rb.mass, ForceMode.Impulse);
		} 

		// Again, this shouldn't be 0, 0, 0, should be the center of the star on the Y = 0 axis
		if(Vector3.Distance (collider.GetComponent<Transform>().position, new Vector3(0, 0, 0)) < 2) {
			Destroy(collider.gameObject);
		}
	}
}

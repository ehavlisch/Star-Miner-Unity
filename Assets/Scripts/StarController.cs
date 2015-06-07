using UnityEngine;
using System.Collections;

public class StarController : MonoBehaviour {

	public void OnTriggerEnter(Collider collider) {
		//Debug.Log ("Star Col!");
	}

	public void OnTriggerStay(Collider collider) {

		float force = -0.01f;
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
			rb.AddForce ((position - planetPosition).normalized * force * rb.mass, ForceMode.Impulse);
		} 

		if(Vector3.Distance (collider.GetComponent<Transform>().position, new Vector3(0, 0, 0)) < 5) {
			Destroy(collider.gameObject);
		}
	}
}

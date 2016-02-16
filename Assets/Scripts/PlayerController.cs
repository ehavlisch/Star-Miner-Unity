using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public ShipController ship;

	void Start() {
		initShip ();
	}

	void initShip() {
		ship.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
		ship.GetComponent<Rigidbody>().maxAngularVelocity = 30;
	}

	void Update() {	
		if(Input.GetButton ("PrimaryFire")) {
			ship.fire ();
		}
	}

	void FixedUpdate() {
	
		float latThruster = Input.GetAxis ("LatThruster");
		float mainThruster = Input.GetAxis ("MainThruster");
		float rotate = Input.GetAxis ("Rotate");
		float dampening = Input.GetAxis ("Dampening");
		float engineVolume = 0.1f;

		bool precise = false;
		if(Input.GetAxis ("Precise") != 0) {
			precise = true;
		}
		Rigidbody rigidBody = ship.GetComponent<Rigidbody> ();
		//TODO the only constant change here is the fuel weight. 
		// Need to change this to keep most of the ship mass constant
		rigidBody.mass = ship.calculateMass ();

		float angle = rigidBody.rotation.eulerAngles.y;
		float latAngle = angle * Mathf.Deg2Rad + Mathf.PI / 2;

		float latThrusterX = Mathf.Sin (latAngle) * latThruster * ship.calculateEngineLatForce ();
		float latThrusterZ = Mathf.Cos (latAngle) * latThruster * ship.calculateEngineLatForce ();

		float mainThrusterX = Mathf.Sin (Mathf.Deg2Rad * angle) * mainThruster * ship.calculateEngineForce();
		float mainThrusterZ = Mathf.Cos (Mathf.Deg2Rad * angle) * mainThruster * ship.calculateEngineForce();

		if (Mathf.Abs(latThruster) > 0) {
			engineVolume = 0.9f;
		}

		if (Mathf.Abs(mainThruster) > 0) {
			engineVolume = 1.0f;
		}

		mainThrusterX += latThrusterX;
		mainThrusterZ += latThrusterZ;

		Vector3 dampenAngularVector = new Vector3(0,0,0);
		Vector3 dampenVector = new Vector3 (0, 0, 0);
		if (dampening > 0) {
			engineVolume = 0.7f;
			float aVY = rigidBody.angularVelocity.y;
			if(aVY > 0.0) {
				dampenAngularVector = new Vector3(0, - ship.calculateEngineRotate(),0);
			} else if(aVY < -0.0) {
				dampenAngularVector = new Vector3(0, ship.calculateEngineRotate(),0);
			}

			dampenVector = rigidBody.velocity;

			dampenVector *= -1;
			dampenVector.Normalize();

			dampenVector *= ship.calculateEngineLatForce();
		}
		
		Vector3 movement = new Vector3 (mainThrusterX, 0, mainThrusterZ);

		bool dampen = (dampenVector.magnitude > 0.0f ? true : false);
		
		if(precise) {
			engineVolume = engineVolume * 0.5f;
			movement *= 0.5f;
		}
	
		if (movement.magnitude > 0 && dampen) {
			movement *= 0.8f; 
			dampenVector *= 0.9f;
			movement = movement + dampenVector;
		} else if (dampen) {
			movement = dampenVector;
		} 

		Vector3 angularVector;
		if (dampenAngularVector.magnitude > 0) {
			angularVector = dampenAngularVector;
		} else if (rotate != 0) {
			angularVector = new Vector3 (0, ship.calculateEngineRotate () * rotate, 0);
		} else {
			//Apply a tiny bit of friction
			angularVector = new Vector3(0, 0, 0);
		}
		
		//Bug - after moving, the player will be able to dampen and burn fuel even while sitting still
		//Fix - when the speed falls below a threshold, zero it out 
		//Temp fix - when they player's velocity is slow, free fuel (does not consider angular)

		//Check if there's enough fuel to do it
		float forceTotal = Mathf.Abs (movement.x) + Mathf.Abs (movement.z) + Mathf.Abs (angularVector.y);
		//Debug.Log (forceTotal + " X: " + movement.x + " Z: " + movement.z + " Y: " + angularVector.y);

		if (rigidBody.velocity.magnitude > .05) {
			//Debug.Log ("High velocity - subtract fuel");
			float fuelCost = forceTotal / ship.calculateEngineEfficiency ();
			float fuelShortage = ship.subtractFuel (fuelCost);

			if (fuelShortage > 0) {
				float fuelScalar = ((fuelCost - fuelShortage) / fuelCost);
				movement *= fuelScalar;
				angularVector *= fuelScalar;
				engineVolume *= fuelScalar;
			}
		} else {
			//Debug.Log("Low Velocity (" + rigidBody.velocity.magnitude + ") - free fuel");
		}

		ship.setEngineVolume (engineVolume);
		rigidBody.AddForce (movement);
		rigidBody.AddTorque (angularVector);


		// Testing pull force
		//float force = -1.0f;
		//Vector3 planetPosition = new Vector3 (0, 0, 0);
		//ship.rigidbody.AddForce ((ship.transform.position - planetPosition).normalized * force * ship.rigidbody.mass, ForceMode.Impulse);

		//Energy Updates
		ship.updateEnergy ();

	}
}

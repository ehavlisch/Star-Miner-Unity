using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public Text shieldText;
	public Text hullText;
	public Text fuelText;

	public Camera mainCamera;

	public PlayerController player;

	void Start() {
		//TODO programatically create player, starting ship, and attach main camera;
		//player = new PlayerController ();
	}

	void FixedUpdate() {
		shieldText.text = Mathf.FloorToInt(player.ship.getEnergy ()) + " / " + player.ship.getMaxEnergy ();
		hullText.text = Mathf.FloorToInt(player.ship.getHull ()) + " / " + player.ship.maxHull;
		fuelText.text = Mathf.FloorToInt (player.ship.calculateFuelVolume ()) + " / " + player.ship.getMaxFuel();
	}
}

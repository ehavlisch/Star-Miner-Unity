using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public Text shieldText;
	public Text hullText;
	public Text fuelText;

	public void update(PlayerController player) {
		shieldText.text = Mathf.FloorToInt (player.ship.getEnergy ()) + " / " + player.ship.getMaxEnergy ();
		hullText.text = Mathf.FloorToInt (player.ship.getHull ()) + " / " + player.ship.maxHull;
		fuelText.text = Mathf.FloorToInt (player.ship.calculateFuelVolume ()) + " / " + player.ship.getMaxFuel ();
	}

	public void disable() {
		shieldText.text = "";
		hullText.text = "";
		fuelText.text = "";
	}
}

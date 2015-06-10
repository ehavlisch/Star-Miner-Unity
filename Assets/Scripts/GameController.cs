using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public Text shieldText;
	public Text hullText;
	public Text fuelText;

	public Camera mainCamera;

	public PlayerController player;

	bool showMenu = false;

	void Start() {
		//TODO this should be a landing menu for the game, then put into debug which will automatically start a new game

		//TODO programatically create player, starting ship, and attach main camera;
		//player = new PlayerController ();
	}

	void FixedUpdate() {
		shieldText.text = Mathf.FloorToInt(player.ship.getEnergy ()) + " / " + player.ship.getMaxEnergy ();
		hullText.text = Mathf.FloorToInt(player.ship.getHull ()) + " / " + player.ship.maxHull;
		fuelText.text = Mathf.FloorToInt (player.ship.calculateFuelVolume ()) + " / " + player.ship.getMaxFuel();
	}

	void Update() {
		if(Input.GetButtonDown("menu"))
			showMenu = toggleMenu();
	}

	void OnGUI() {

		if(showMenu) {
			if(GUILayout.Button("Resume")) {
				showMenu = toggleMenu();
			}
		}
	}

	bool toggleMenu() {
		if(Time.timeScale == 0) {
			Time.timeScale = 1;
			return(false);
		} else {
			Time.timeScale = 0;
			shieldText.text = "";
			hullText.text = "";
			fuelText.text = "";
			return true;  
		}
	}
}

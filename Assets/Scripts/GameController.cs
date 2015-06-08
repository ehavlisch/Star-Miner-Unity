using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public Text shieldText;
	public Text hullText;
	public Text fuelText;

	public Camera mainCamera;

	public PlayerController player;

	private bool showMenu;
	private bool showingMenu;

	private int menuPresses = 0;


	void Start() {
		//TODO programatically create player, starting ship, and attach main camera;
		//player = new PlayerController ();
	}

	void FixedUpdate() {
		shieldText.text = Mathf.FloorToInt(player.ship.getEnergy ()) + " / " + player.ship.getMaxEnergy ();
		hullText.text = Mathf.FloorToInt(player.ship.getHull ()) + " / " + player.ship.maxHull;
		fuelText.text = Mathf.FloorToInt (player.ship.calculateFuelVolume ()) + " / " + player.ship.getMaxFuel();
	//}


	//void FixedUpdate() {
		float menuInput = Input.GetAxisRaw ("menu");
		if (menuInput != 0) {
			menuPresses++;
		}

		if (menuInput == 1 && !showingMenu) {
			showMenu = true;
			Time.timeScale = 0;
		}
	}

	void Update() {
		// Close the menu
		if (!showMenu && !showingMenu) {
			Time.timeScale = 1;
		}
	}

	void OnGUI() {
		if (!showMenu && showingMenu) {
			showingMenu = false;
		}

		if (showMenu || showingMenu) {
			showingMenu = true;

			if (GUI.Button (new Rect (100, 25, 150, 30), "Resume Game: " + menuPresses)) {
				// This code is executed when the Button is clicked
				Debug.Log ("Button clicked" + menuPresses);
				showMenu = false;
				showingMenu = false;
			}
		}
	}
}

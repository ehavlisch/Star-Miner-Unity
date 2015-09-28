using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public Camera mainCamera;

	public PlayerController player;
	public WorldController worldController;
	public UIController uiController;

	bool showMenu = false;

	void Start() {
		//TODO this should be a landing menu for the game, then put into debug which will automatically start a new game

		//TODO programatically create player, starting ship, and attach main camera;
		//player = new PlayerController ();

		UnityEngine.Random.seed = 150;

		worldController.generateWorld ();

		worldController.printWorld ();
		worldController.printWorldNodes ();
	}

	void FixedUpdate() {
		uiController.update (player);
	}

	void Update() {
		if (Input.GetButtonDown ("menu")) {
			showMenu = toggleMenu ();
		}
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
			uiController.disable();
			return true;  
		}
	}
}

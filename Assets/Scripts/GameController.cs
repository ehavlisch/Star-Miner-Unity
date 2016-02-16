using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using Events;

public class GameController : MonoBehaviour {

	public Camera mainCamera;

	public PlayerController player;
	public WorldController worldController;
	public UIController uiController;

	bool showMenu = false;

	// Debugging tools
	private bool testMode = true;
	private bool staticRandomSeed = false;

	void Start() {
		Debug.Log ("Starting");
		if (staticRandomSeed) {
			UnityEngine.Random.seed = 150;
		}
		if (testMode) {
			testEvents();
			//sortEventNames();
		}
		//TODO this should be a landing menu for the game, then put into debug which will automatically start a new game

		//TODO programatically create player, starting ship, and attach main camera;
		//player = new PlayerController ();

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

	void testEvents() {
		Debug.Log ("Starting testEvents");
		RandomEvent anEvent = EventFactory.generateEvent ();
		anEvent.runEvent ();

		for(int i = 0; i < 10; i ++) {
			if(anEvent.isClosed()) {
				break;
			}
			anEvent.selectAction (0);

		}
		Debug.Log ("Finished testEvents");
	}

	void sortEventNames() {
		EventFactory.sortIds ();
	}
}

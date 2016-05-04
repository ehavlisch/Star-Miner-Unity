using UnityEngine;
using Tests;

public class GameController : MonoBehaviour {

	public Camera mainCamera;

	public PlayerController player;
	public WorldController worldController;
	public UIController uiController;

	bool showMenu = false;

	// Debugging tools
	private bool testMode = true;
	private bool staticRandomSeed = false;

    // For generating map chunks the player moves towards
    private Vector3 lastCheckedPosition;
    private float travelThreshold = 10;


    void Start() {
		Debug.Log ("Starting");
		if (staticRandomSeed) {
			Random.seed = 151;
		}
		if (testMode) {
            Test.runTests();
		}
		//TODO this should be a landing menu for the game, then put into debug which will automatically start a new game

		//TODO programatically create player, starting ship, and attach main camera;
		//player = new PlayerController ();

		worldController.generateWorld ();

		worldController.printWorld ();
		worldController.printWorldNodes ();

        lastCheckedPosition = player.GetComponent<Transform>().position;
        Debug.Log(lastCheckedPosition);
	}

	void FixedUpdate() {
		uiController.update (player);
        Vector3 currentPosition = player.GetComponent<Transform>().position;
        if(Vector3.Distance(currentPosition, lastCheckedPosition) >= travelThreshold) {
            Debug.Log(Vector3.Distance(currentPosition, lastCheckedPosition));
            Debug.Log(currentPosition);
            Debug.Log(lastCheckedPosition);
            lastCheckedPosition = currentPosition;
            worldController.playerMovementCheck(new IntVector2(lastCheckedPosition));
        }
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

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

    private float gameTime = 0;
    private float startTime = 0;
    
    void Start() {
		Debug.Log ("GameController Starting");
		if (staticRandomSeed) {
            Debug.LogWarning("Setting static random seed.");
			Random.seed = 151;
		} else {
            Random.seed = System.DateTime.Now.Millisecond;
        }
		if (testMode) {
            TestUtils.Instance.startTimer("tests");
            Debug.Log("GameController Running Tests.");
            try {
                Test.runTests();
            } catch (System.Exception e) {
                Debug.LogWarning("Errors in tests.");
                Debug.LogWarning(e.Message + e.StackTrace);
            }
            Debug.Log("GameController Finished Tests. " + TestUtils.Instance.endTimer("tests") + "ms elapsed.");
		}
		//TODO this should be a landing menu for the game, then put into debug which will automatically start a new game

		//TODO programatically create player, starting ship, and attach main camera;
		//player = new PlayerController ();

		worldController.generateWorld ();

		worldController.printWorld ();
		worldController.printWorldNodes ();

        lastCheckedPosition = player.GetComponent<Transform>().position;

        gameTime = 0;
        startTime = Time.time;
        Debug.Log(lastCheckedPosition);
        Debug.Log("GameController Start completed.");
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
        gameTime = Time.time - startTime;
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

    public float getGameTime() {
        return gameTime;
    }
}

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class WorldController : MonoBehaviour {

    public WorldNode[,] worldArray;

    private int chunkSize;
    private int chunkSpread;
    private int worldNodeSize;

    private float flux;

    private WorldNode root;

    private int worldSize;

    private float shiftFactor = 0.25f;

    private int worldNodeCount;
    private int randomSeed;

    private IntVector2 playerChunk;
    private IntVector2 worldShift;

    private Direction needToUnload = Direction.NONE;

    private int loadSize = 1;

    // Lists of the objects - 
    // TODO should be in a higher class and shared
    private ArrayList asteroids { get; set; }
    private ArrayList planets { get; set; }
    private ArrayList stars { get; set; }

    public void generateWorld() {
        Debug.Log("generateWorld()");
		init ();
		generateSystem ();
		newWorld (25, 10, 0.5f, 0.02f);
	}

	private void init () {
		asteroids = new ArrayList ();
		asteroids.Add (Resources.Load ("Asteroid01"));
		asteroids.Add (Resources.Load ("Asteroid02"));
		asteroids.Add (Resources.Load ("Asteroid03"));
		asteroids.Add (Resources.Load ("Asteroid04"));
		asteroids.Add (Resources.Load ("Asteroid05"));
		asteroids.Add (Resources.Load ("Asteroid06"));
		asteroids.Add (Resources.Load ("Asteroid07"));
		//asteroids.Add (Resources.Load ("Asteroid08"));
		
		planets = new ArrayList ();
		planets.Add (Resources.Load ("Planet01"));
		
		stars = new ArrayList ();
		stars.Add (Resources.Load ("Star01"));

	}
	
	// Generates Planets
	private void generateSystem() {
		//GameObject starObject = (GameObject)Instantiate (getStar (), new Vector3 (0, 0, 0), Quaternion.identity);
		//starObject.transform.parent = this.transform;
		
		int distanceFromSun = 100;
	
		int planetCount = Mathf.FloorToInt (UnityEngine.Random.Range (3, 12));
		for (int i = 0; i < planetCount; i++) {
			float angle = UnityEngine.Random.Range(0, 360);
			distanceFromSun = Mathf.FloorToInt (UnityEngine.Random.Range(distanceFromSun + 10, distanceFromSun + 200));
			
			float distanceX = Mathf.Sin (angle) * distanceFromSun;
			float distanceZ = Mathf.Cos (angle) * distanceFromSun;
			
			GameObject planetObject = (GameObject) Instantiate (getPlanet (), new Vector3(distanceX, 0, distanceZ), Quaternion.identity);
			planetObject.transform.parent = this.transform;
			
		}
	}

	// Generates asteroid map
	public void newWorld(int chunkSize, int chunkSpread, float startValue, float flux) {
		this.chunkSize = chunkSize;
		this.chunkSpread = chunkSpread;
		worldNodeSize = chunkSize * chunkSpread;
		this.flux = flux;

		worldSize = 2 * (loadSize * 2 + 1) + 1;
		int worldCenter = (int)Mathf.Floor (worldSize / 2);
		
		worldArray = new WorldNode[worldSize,worldSize];
		worldShift = new IntVector2 (worldCenter, worldCenter);

		root = getChunk (new IntVector2 (worldCenter, worldCenter));

		worldArray = new WorldNode[worldSize, worldSize];
		worldArray [worldCenter, worldCenter] = root;
		root.setLocation (new IntVector2 (worldCenter, worldCenter));

		playerChunk = new IntVector2 (0, 0);
		loadAdjacentChunks(Direction.NONE);
	}

	public GameObject getAsteroid() {
		return (GameObject) asteroids[Mathf.FloorToInt (UnityEngine.Random.Range (0, asteroids.Count))];
	}
	
	public GameObject getPlanet() {
		return (GameObject) planets[Mathf.FloorToInt (UnityEngine.Random.Range (0, planets.Count))];
	}
	
	public GameObject getStar() {
		return (GameObject) stars[Mathf.FloorToInt (UnityEngine.Random.Range (0, stars.Count))];
	}

	public void playerMovementCheck(IntVector2 location) {
		Debug.Log("PlayerMovementCheck. WorldNodeSize: " + worldNodeSize);
		IntVector2 newPlayerChunk = new IntVector2 (location.x / worldNodeSize, location.y / worldNodeSize);

		if(!newPlayerChunk.Equals(playerChunk)) {
			// Player moved in which direction:
			Direction direction = Direction.NONE;
			if(playerChunk.x == newPlayerChunk.x) {
				if(newPlayerChunk.y > playerChunk.y) {
					direction = Direction.RIGHT;
				} else {
					direction = Direction.LEFT;
				}
			} else {
				if(newPlayerChunk.x > playerChunk.x) {
					direction = Direction.DOWN;
				} else {
					direction = Direction.UP;
				}
			}
			playerChunk = newPlayerChunk;
			loadAdjacentChunks(direction);
			needToUnload = direction.opposite();
		} else if(needToUnload != Direction.NONE) {
			Debug.Log("Unloading chunks while not moving between chunks");
			unloadNonAdjacentChunks(needToUnload);
		}

	}

	private void loadAdjacentChunks(Direction direction) {
		if(direction != Direction.NONE) {
			expandInDirection(direction); 
		} else {
			IntVector2 location = getChunk(new IntVector2(playerChunk.x + worldShift.x, playerChunk.y + worldShift.y)).getLocation();
			for(int i = 1; i <= loadSize; i++) {
				// Load the cardinal directions, should only have one pre existing neighbor
				foreach(Direction dir in Enum.GetValues(typeof(Direction))) {
					if(dir == Direction.NONE) {
						continue;
					}
					IntVector2 newChunk = new IntVector2(location.x + dir.getPair().x * i, location.y + dir.getPair().y * i);
					getChunk(newChunk);
				}
				
				// for each direction
				foreach(Direction dir in Enum.GetValues(typeof(Direction))) {
					if(dir == Direction.NONE) {
						continue;
					}
					WorldNode cardinalNode = getChunk(new IntVector2(dir.getPair().x * i + worldShift.x, dir.getPair().y * i + worldShift.y));
					WorldNode targetNode = cardinalNode;
					int skip = 0;
					for(int j = 0; j < i; j++) {
						for(int k = 0; k < skip; k++) {
							targetNode = targetNode.get(dir.perpOne());
						}
						
						getChunk(new IntVector2(targetNode.getLocation().x + dir.perpOne().getPair().x, targetNode.getLocation().y + dir.perpOne().getPair().y));
						
						targetNode = cardinalNode;
						if(i - j > 1) {
							for(int k = 0; k < skip; k++) {
								targetNode = targetNode.get(dir.perpTwo());
							}
							
							getChunk(new IntVector2(targetNode.getLocation().x + dir.perpTwo().getPair().x, targetNode.getLocation().y + dir.perpTwo().getPair().y));
							
							targetNode = cardinalNode;
							skip++;
						}
					}
				}
			}
		}
	}

	private void expandInDirection(Direction direction) {
		WorldNode currentNode = getChunk(new IntVector2(playerChunk.x + worldShift.x,playerChunk.y + worldShift.y));
		WorldNode trailingNode = currentNode;
		currentNode = currentNode.get(direction);
		
		for(int i = 0; i < loadSize; i++) {
			if(currentNode == null) {
				currentNode = getChunk(new IntVector2(trailingNode.getLocation().x + direction.getPair().x, trailingNode.getLocation().y + direction.getPair().y));
				trailingNode = currentNode;
			} else if(!currentNode.isLoaded()) {
				currentNode.load();
			}
			expandPerpendicular(currentNode, direction);
			
			trailingNode = currentNode;
			currentNode = currentNode.get(direction);
		}
	}

	private void expandPerpendicular(WorldNode startNode, Direction direction) {
		Direction perpOne = direction.perpOne();
		Direction perpTwo = direction.perpTwo();
		WorldNode lastExpandOne = startNode.get(perpOne);
		WorldNode trailOne = startNode;
		WorldNode lastExpandTwo = startNode.get(perpTwo);
		WorldNode trailTwo = startNode;
		for(int i = 0; i < loadSize; i++) {
			if(lastExpandOne != null) {
				lastExpandOne.load();
				trailOne = lastExpandOne;
				lastExpandOne = lastExpandOne.get(direction.perpOne());
			} else {
				lastExpandOne = getChunk(new IntVector2(trailOne.getLocation().x + perpOne.getPair().x, trailOne.getLocation().y + perpOne.getPair().y));
				trailOne = lastExpandOne;
				lastExpandOne = lastExpandOne.get(direction.perpOne());
			}
			
			if(lastExpandTwo != null) {
				lastExpandTwo.load();
				trailTwo = lastExpandTwo;
				lastExpandTwo = lastExpandTwo.get(direction.perpTwo());
			} else {
				lastExpandTwo = getChunk(new IntVector2(trailTwo.getLocation().x + perpTwo.getPair().x, trailTwo.getLocation().y + perpTwo.getPair().y));
				trailTwo = lastExpandTwo;
				lastExpandTwo = lastExpandTwo.get(direction.perpTwo());
			}
		}
	}

	private void unloadNonAdjacentChunks(Direction direction) {
		IntVector2 playerWorldNode = new IntVector2(playerChunk.x + worldShift.x, playerChunk.y + worldShift.y);
		
		if(direction != Direction.NONE) {
			// Unload the single row that shouldn't be shown anymore
			List<IntVector2> clearList = new List<IntVector2>();
			WorldNode node = null;
			
			node = getChunk(playerWorldNode);
			
			for(int i = 0; i <= loadSize; i++) {
				node = node.get(direction);
			}
			
			for(int i = -loadSize ; i <= loadSize; i++) {
				if(direction == Direction.DOWN || direction == Direction.UP) {
					clearList.Add(new IntVector2(node.getLocation().x, node.getLocation().y + i));
				} else {
					clearList.Add(new IntVector2(node.getLocation().x + i, node.getLocation().y));
				}
			}
			
			foreach(IntVector2 p in clearList) {
				if(worldArray[p.x, p.y] != null) {
					worldArray[p.x, p.y].unload();
				}
			}
		} else {
			// Unload everything but what should be shown
			// Anything not in the save region can be removed
			// Maybe do this on pauses just to make sure we don't miss anything
			int saveRowLow = playerWorldNode.x - loadSize;
			int saveRowHigh = playerWorldNode.x + loadSize;
			
			int saveColLow = playerWorldNode.y - loadSize;
			int saveColHigh = playerWorldNode.y + loadSize;
			
			for(int i = 0; i < worldSize; i++) {
				for(int j = 0; j < worldSize; j++) {
					if(!(i >= saveRowLow && i <= saveRowHigh && j >= saveColLow && j <= saveColHigh)) {
						unload(new IntVector2(i, j));
					}
				}
			}
		}
		
		needToUnload = Direction.NONE;
	}

	public WorldNode getChunk(IntVector2 location) {
		//Note: Does making the world a square still make sense? 
		if(location.x < 0) {
			int shiftRight = worldSize - (int) Mathf.Floor(worldSize * shiftFactor);
			int shiftDown = (int) Mathf.Floor(worldSize / 2.0f);
			location = shift(shiftRight, shiftDown, location);
		} else if(location.x >= worldSize) {
			int shiftRight = (int) Mathf.Floor(worldSize * shiftFactor);
			int shiftDown = (int) Mathf.Floor(worldSize / 2);
			location = shift(shiftRight, shiftDown, location);
		}
		
		if(location.y < 0) {
			int shiftRight = (int) Mathf.Floor(worldSize / 2);
			int shiftDown = worldSize - (int) Mathf.Floor(worldSize * shiftFactor);
			location = shift(shiftRight, shiftDown, location);
		} else if(location.y >= worldSize) {
			int shiftRight = (int) Mathf.Floor(worldSize / 2);
			int shiftDown = (int) Mathf.Floor(worldSize * shiftFactor);
			location = shift(shiftRight, shiftDown, location);
		}
		WorldNode worldNode = worldArray [location.x, location.y];
		if (worldNode != null) {
			if(worldNode.isLoaded ()) {
				return worldNode;
			} else {
				worldNode.load ();
				instantiateWorldNodeObjects(worldNode);
				return worldNode;
			}
		} else {
			int worldNodeId = worldNodeCount++;
			WorldNode newWorldNode = new WorldNode(worldNodeId);
			newWorldNode.setLocation(location);

			List<MapChunk> neighbors = new List<MapChunk>();
			List<int> neighborLocations = new List<int>();
			// X: Row, Y: Column
			if(location.y - 1 >= 0 && worldArray[location.x, location.y - 1] != null) {
				//Debug.Log ("Neighbor Left - loading from row:" + location.x + ", col:" + (location.y - 1) + ".");
				neighborLocations.Add(1);
				neighbors.Add(worldArray[location.x, location.y - 1].getMapChunk());
				worldArray[location.x, location.y - 1].setRight(newWorldNode);
				newWorldNode.setLeft(worldArray[location.x, location.y - 1]);
			}
			
			if(location.x - 1 >= 0 && worldArray[location.x - 1, location.y] != null) {
				//Debug.Log ("Neighbor Above - loading from row:" + (location.x - 1) + ", col:" + location.y + ".");
				neighborLocations.Add(2);
				neighbors.Add(worldArray[location.x - 1, location.y].getMapChunk());
				worldArray[location.x - 1, location.y].setDown(newWorldNode);
				newWorldNode.setUp(worldArray[location.x - 1, location.y]);
			}
			
			if(location.x + 1 < worldSize && worldArray[location.x + 1, location.y] != null) {
				//Debug.Log ("Neighbor Below - loading from row:" + (location.x + 1) + ", col:" + location.y + ".");
				neighborLocations.Add(3);
				neighbors.Add(worldArray[location.x + 1, location.y].getMapChunk());
				worldArray[location.x + 1, location.y].setUp(newWorldNode);
				newWorldNode.setDown(worldArray[location.x + 1, location.y]);
			}
			
			if(location.y + 1 < worldSize && worldArray[location.x, location.y + 1] != null) {
				//Debug.Log ("Neighbor Right - loading from row:" + location.x + ", col:" + (location.y + 1) + ".");
				neighborLocations.Add(4);
				neighbors.Add(worldArray[location.x, location.y + 1].getMapChunk());
				worldArray[location.x, location.y + 1].setLeft(newWorldNode);
				newWorldNode.setRight(worldArray[location.x, location.y + 1]);
			}
			
			MapChunk newMapChunk = new MapChunk(chunkSize, flux, neighbors.ToArray(), neighborLocations.ToArray(), randomSeed, worldNodeId);
			
			newWorldNode.setMapChunk(newMapChunk);
			
			worldArray[location.x, location.y] = newWorldNode;
			instantiateWorldNodeObjects(newWorldNode);
			return newWorldNode;
		}
	}

	private void instantiateWorldNodeObjects(WorldNode worldNode) {
		IntVector2 worldNodeLocation = worldNode.getLocation();
		IntVector2 relativeLocation = new IntVector2(worldNodeLocation.x - worldShift.x, worldNodeLocation.y - worldShift.y);
		int halfSpreadSize = chunkSize * chunkSpread / 2;
		//		System.out.println("Relative Center: " + relativeLocation);
		IntVector2 actualCenter = new IntVector2(relativeLocation.x * worldNodeSize, relativeLocation.y * worldNodeSize);
		//		System.out.println("Actual Center: " + actualCenter);
		//		System.out.println("Generating objects from: " + (actualCenter.x - halfSpreadSize) + " to " + (actualCenter.x + halfSpreadSize) + ".");
		//		System.out.println("Generating objects from: " + (actualCenter.y - halfSpreadSize) + " to " + (actualCenter.y + halfSpreadSize) + ".");

		for(int i = 0; i < chunkSize; i++) {
			for(int j = 0; j < chunkSize; j++) {
				float value = worldNode.getMapChunk().map[i, j];
					// Instantiate or something
				if(value >= .45 && value <= .55) {

					float randomX = UnityEngine.Random.Range(-1.0f, 1.0f) * chunkSpread / 2;
					float randomZ = UnityEngine.Random.Range(-1.0f, 1.0f) * chunkSpread / 2;
					Vector3 location = new Vector3(actualCenter.x - halfSpreadSize + i * chunkSpread + randomX, 0, actualCenter.y - halfSpreadSize + j * chunkSpread + randomZ);

					Quaternion rotation = new Quaternion(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

					GameObject asteroidObject = (GameObject) Instantiate (getAsteroid(), location , rotation);
					asteroidObject.transform.parent = this.transform;
					AsteroidController asteroid = (AsteroidController) asteroidObject.GetComponent ("AsteroidController");
					asteroid.setLocation(new IntVector2(i, j));
					asteroid.setValue(value);
					asteroid.setWorld (this);
				}
			}
		}
	}

	private IntVector2 shift(int shiftRight, int shiftDown, IntVector2 location) {
		worldShift = new IntVector2(worldShift.x + shiftRight, worldShift.y + shiftDown);
		int newWorldSize = worldSize * 2;
		WorldNode[,] newWorldArray = new WorldNode[newWorldSize, newWorldSize];
		
		for(int i = 0; i < worldSize; i++) {
			for(int j = 0; j < worldSize; j++) {
				if(worldArray[i, j] != null) {
					newWorldArray[i + shiftRight, j + shiftDown] = worldArray[i, j];
					newWorldArray[i + shiftRight, j + shiftDown].setLocation(new IntVector2(i + shiftRight, j + shiftDown));
				}
			}
		}
		worldArray = newWorldArray;
		worldSize = newWorldSize;
		return new IntVector2(location.x + shiftRight, location.y + shiftDown);
	}

	public void unloadWorld() {
		for(int i = 0; i < worldSize; i++) {
			for(int j = 0; j < worldSize; j++) {
				WorldNode worldNode = worldArray[i, j];
				if(worldNode != null) {
					worldNode.unload();
				}
			}
		}
	}
	
	public void unload(IntVector2 location) {
		WorldNode worldNode = worldArray[location.x, location.y];
		if(worldNode != null) {
			worldNode.unload();
		}
	}
	
	public void printWorldNodes() {
		StringBuilder worldstring = new StringBuilder ();
		for(int i = 0; i < worldSize; i++) {
			StringBuilder sb = new StringBuilder();
			for(int j = 0; j < worldSize; j++) {
				if(worldArray[i, j] == null) {
					sb.Append("-");
				} else {
					sb.Append("O");
				}
			}
			worldstring.Append (sb.ToString());
			worldstring.Append ("\n");
		}
		Debug.Log (worldstring);
	}
	
	public void printWorld() {
		StringBuilder worldstring = new StringBuilder ();
		for(int i = 0; i < worldSize; i++) {
			
			if(emptyRow(i)) {
				continue;
			}
			
			StringBuilder[] sbs = new StringBuilder[chunkSize];
			for(int j = 0; j < chunkSize; j++) {
				sbs[j] = new StringBuilder();
			}
			
			for(int j = 0; j < worldSize; j++) {
				
				if(emptyCol(j)) {
					continue;
				}
				
				if(worldArray[i, j] != null) {
					if(worldArray[i, j].isLoaded()) {
						worldArray[i, j].printRows(sbs);
					} else {
						for(int k = 0; k < chunkSize; k++) {
							for(int l = 0; l < chunkSize; l++) {
								sbs[l].Append("Z");
							}
						}
					}
				} else {
					for(int k = 0; k < chunkSize; k++) {
						for(int l = 0; l < chunkSize; l++) {
							sbs[l].Append(" ");
						}
					}
				}
			}
			for(int j = 0; j < chunkSize; j++) {
				if(sbs[j].Length > 0) {
					worldstring.Append (sbs[j].ToString());
					worldstring.Append ("\n");
				}
			}
		}
		Debug.Log (worldstring.ToString ());
	}

	private bool emptyRow(int row) {
		for(int i = 0; i < worldSize; i++) {
			if(worldArray[row, i] != null) {
				return false;
			}
		}
		return true;
	}
	
	private bool emptyCol(int col) {
		for(int i = 0; i < worldSize; i++) {
			if(worldArray[i, col] != null) {
				return false;
			}
		}
		return true;
	}
}



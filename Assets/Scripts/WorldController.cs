using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class WorldController : MonoBehaviour {

	private WorldNode[][] worldArray;
	
	private int chunkSize;
	private int flux;
	
	private WorldNode root;
	
	private int worldSize;
	
	private double shiftFactor = .25;
	
	private int worldNodeCount;
	private long randomSeed;

	// Lists of the objects - 
	// TODO should be in a higher class and shared
	public ArrayList asteroids;
	public ArrayList planets;
	public ArrayList stars;

	// Use this for initialization
	void Start () {
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

	// Update is called once per frame
	void Update () {
	
	}

	private void generateSystem() {
		GameObject starObject = (GameObject)Instantiate (getStar (), new Vector3 (0, 0, 0), Quaternion.identity);
		starObject.transform.parent = this.transform;
		
		int distanceFromSun = 100;
		
		int planetCount = Mathf.FloorToInt (Random.Range (3, 12));
		for (int i = 0; i < planetCount; i++) {
			float angle = Random.Range(0, 360);
			distanceFromSun = Mathf.FloorToInt (Random.Range(distanceFromSun + 10, distanceFromSun + 200));
			
			float distanceX = Mathf.Sin (angle) * distanceFromSun;
			float distanceZ = Mathf.Cos (angle) * distanceFromSun;
			
			GameObject planetObject = (GameObject) Instantiate (getPlanet (), new Vector3(distanceX, 0, distanceZ), Quaternion.identity);
			planetObject.transform.parent = this.transform;
			
		}
	}

	public void newWorld(int chunkSize, float startValue, int flux, long randomSeed) {
		this.chunkSize = chunkSize;
		this.flux = flux;
		this.randomSeed = randomSeed;

		int worldNodeId = worldNodeCount++;
		MapChunk rootChunk = new MapChunk (chunkSize, startValue, flux, randomSeed, worldNodeId);
		root = new WorldNode (worldNodeId);
		root.setMapChunk (rootChunk);

		worldSize = 9;
		worldArray = new WorldNode[worldSize, worldSize];
		int worldCenter = (int)Mathf.Floor (worldSize / 2);
		worldArray [worldCenter, worldCenter] = root;
		root.setLocation (new IntVector2 (worldCenter, worldCenter));
	}

	public GameObject getAsteroid() {
		return (GameObject) asteroids[Mathf.FloorToInt (Random.Range (0, asteroids.Count))];
	}
	
	public GameObject getPlanet() {
		return (GameObject) planets[Mathf.FloorToInt (Random.Range (0, planets.Count))];
	}
	
	public GameObject getStar() {
		return (GameObject) stars[Mathf.FloorToInt (Random.Range (0, stars.Count))];
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
		
		if(worldArray[location.x][location.y] != null) {
			//TODO re add loading non null chunks
			//return worldArray[location.x][location.y].load();
			Debug.Log ("FIX THIS!!");
			return null;
		} else {
			int worldNodeId = worldNodeCount++;
			WorldNode newWorldNode = new WorldNode(worldNodeId);
			newWorldNode.setLocation(location);

			List<MapChunk> neighbors = new List<MapChunk>();
			List<int> neighborLocations = new List<int>();
			
			if(location.y - 1 >= 0 && worldArray[location.x][location.y - 1] != null) {
				neighborLocations.Add(1);
				neighbors.Add(worldArray[location.x][location.y - 1].getMapChunk());
				worldArray[location.x][location.y - 1].setRight(newWorldNode);
				newWorldNode.setLeft(worldArray[location.x][location.y - 1]);
			}
			
			if(location.x - 1 >= 0 && worldArray[location.x - 1][location.y] != null) {
				neighborLocations.Add(2);
				neighbors.Add(worldArray[location.x - 1][location.y].getMapChunk());
				worldArray[location.x - 1][location.y].setDown(newWorldNode);
				newWorldNode.setUp(worldArray[location.x - 1][location.y]);
			}
			
			if(location.x + 1 < worldSize && worldArray[location.x + 1][location.y] != null) {
				neighborLocations.Add(3);
				neighbors.Add(worldArray[location.x + 1][location.y].getMapChunk());
				worldArray[location.x + 1][location.y].setUp(newWorldNode);
				newWorldNode.setDown(worldArray[location.x + 1][location.y]);
			}
			
			if(location.y + 1 < worldSize && worldArray[location.x][location.y + 1] != null) {
				neighborLocations.Add(4);
				neighbors.Add(worldArray[location.x][location.y + 1].getMapChunk());
				worldArray[location.x][location.y + 1].setLeft(newWorldNode);
				newWorldNode.setRight(worldArray[location.x][location.y + 1]);
			}
			
			MapChunk newMapChunk = new MapChunk(chunkSize, flux, neighbors.ToArray(new MapChunk[neighbors.Count]), neighborLocations.ToArray(new int[neighborLocations.Count]), randomSeed, worldNodeId);
			
			newWorldNode.setMapChunk(newMapChunk);
			
			worldArray[location.x][location.y] = newWorldNode;
			return newWorldNode;
		}
	}

	private IntVector2 shift(int shiftRight, int shiftDown, IntVector2 location) {
		int newWorldSize = worldSize * 2;
		WorldNode[,] newWorldArray = new WorldNode[newWorldSize, newWorldSize];
		
		for(int i = 0; i < worldSize; i++) {
			for(int j = 0; j < worldSize; j++) {
				if(worldArray[i][j] != null) {
					newWorldArray[i + shiftRight, j + shiftDown] = worldArray[i][j];
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
		for(int i = 0; i < worldSize; i++) {
			StringBuilder sb = new StringBuilder();
			for(int j = 0; j < worldSize; j++) {
				if(worldArray[i][j] == null) {
					sb.Append("-");
				} else {
					sb.Append("O");
				}
			}
			Debug.Log(sb.ToString());
		}
	}
	
	public void printWorld() {
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
				
				if(worldArray[i][j] != null) {
					if(worldArray[i][j].isLoaded()) {
						worldArray[i][j].printRows(sbs);
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
					//System.out.println(sbs[j].toString());
					Debug.Log (sbs[j].ToString());
				}
			}
		}
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapChunk {

	// Required for regeneration
	private long randomSeed;
	private int worldNodeId;


	private float[,] map;

	public int size;
	private float flux;

	// Values for root node
	private bool isRoot = false;
	private int startValue;

	// Edges for neighboring maps
	private int[] left;
	private int[] right;
	private int[] top;
	private int[] bot;

	// Safe to empty
	private Queue<IntVector2> toDo;
	private bool[,] mapWritten;
	private bool[,] mapPending;
	private Random random;

	// should default to false
	private bool filled = false;

	// Unsure if we can empty
	float[] buckets;

	int count;

	// Lists of the objects - 
	// TODO should be in a higher class and shared
	public ArrayList asteroids;
	public ArrayList planets;
	public ArrayList stars;

	public MapChunk(int size, float startValue, float flux, long randomSeed, int worldNodeId) {
		this.size = size;
		this.flux = flux;
		this.startValue = startValue;
		int start = (int) Mathf.Floor(size/2);
		this.randomSeed = randomSeed;
		this.worldNodeId = worldNodeId;
		isRoot = true;
		
		init();
		
		map[start][start] = startValue;
		mapPending [start] [start] = true;
		mapWritten[start][start] = true;
		
		addAdjacent(start, start);

		// Use the other fill logic below to populate a map based on percentages of materials
		fill();

		//printMap();
	}

	public MapChunk(int size, int flux, MapChunk[] neighbors, Integer[] locations, long randomSeed, int worldNodeId) {
		this.randomSeed = randomSeed;
		this.worldNodeId = worldNodeId;
		this.size = size;
		this.flux = flux;
		
		init();
		
		if(neighbors.Length == 0 || neighbors.Length != locations.Length) {
			//System.out.println("Invalid MapChunk Neighbors");
			Debug.Log ("Invalid MapChunk Neighbors");
			return;
		}
		
		for(int i = 0; i < neighbors.GetLength; i++) {
			MapChunk neighbor = neighbors[i];
			switch(locations[i]) {
			case 1: {
				top = new int[size];
				for(int j = 0; j < size; j++) {
					top[j] = neighbor.getMap()[j, size - 1];
					toDo.Enqueue(new IntVector2(j, 0));
				}
				break;
			}
			case 2: {
				left = new int[size];
				for(int j = 0; j < size; j++) {
					left[j] = neighbor.getMap()[0, j];
					toDo.Enqueue(new IntVector2(0, j));
				}
				break;
			}
			case 3: {
				right = new int[size];
				for(int j = 0; j < size; j++) {
					right[j] = neighbor.getMap()[size - 1, j];
					toDo.Enqueue(new IntVector2(size - 1, j));
				}
				break;
			}
			case 4: {
				bot = new int[size];
				for(int j = 0; j < size; j++) {
					bot[j] = neighbor.getMap()[0][j];
					toDo.Enqueue(new IntVector2(j, size - 1));
				}
				break;
			}
			default: {
				Debug.Log ("Invalid location value in locations array");
				return;
				//throw new Exception("Invalid location value in locations array");
			}
			}
		}	
		fill();
	}

	private void init() {
		toDo = new Queue<IntVector2>();
		map = new int[size][size];
		mapWritten = new bool[size,size];
		mapPending = new bool[size,size];
		buckets = new float[10];
		filled = false;
		count = 0;
	}

	void Start() {
		flux = 0.06f;

		/*
		 * Populating list of space objects
		 */
		//TODO these lists of objects should be at higher levels
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

		int seed = Mathf.FloorToInt(Random.Range(0, 1000000));
		//int seed = 306891;
		Random.seed = seed;
		Debug.Log ("Seed: " + seed);

		//TODO move calls to higher levels for instantiation
		//generateSystem ();

		//generateMap ();


		/*
		int b0 = 0, b1 = 0, b2 = 0, b3 = 0, b4 = 0, b5 = 0, b6 = 0, b7 = 0, b8 = 0, b9 = 0, b10 = 0;

		for (int i = 0; i < size; i++) {
			for(int j = 0; j < size; j++) {
				float var = map[i,j];
				if(var == 1.0) {
					b10++;
				} else if(var >= 0.9) {
					b9++;
				} else if(var >= 0.8) {
					b8++;
				} else if(var >= 0.7) {
					b7++;
				} else if(var >= 0.6) {
					b6++;
				} else if(var >= 0.5) {
					b5++;
				} else if(var >= 0.4) {
					b4++;
				} else if(var >= 0.3) {
					b3++;
				} else if(var >= 0.2) {
					b2++;
				} else if(var >= 0.1) {
					b1++;
				} else if(var >= 0) {
					b0++;
				} 
			}
		}
		//float total = size * size;

		Debug.Log ("b10: " + b10/total * 100 + " b9 " + b9/total * 100 + " b8 " + b8/total * 100 + " b7 " + b7/total * 100 + " b6 " + b6/total * 100 + " b5 " + b5/total * 100 + " b4 " + b4/total * 100 + " b3 " + b3/total * 100 + " b2 " + b2/total * 100 + " b1 " + b1/total * 100 + " b0 " + b0/total * 100 );
	*/


		//TODO
		// This map instantiation should be done at a higher level
		// Also, this generates a chunk centered at 0,0 
		// This raises a good point though, the world is going to need to generate
		// an area based off it's location and the spread, so this logic is good, 
		//but needs to be extended for multiple chunks
		/*
		int spread = 10;

		for (int i = - size / 2; i < size / 2; i++) {
			int mapI = i + size/2;
			for(int j = - size / 2; j < size / 2; j++) {
				int mapJ = j + size / 2;
				if(i == 0 && j == 0) continue;

				if(map[mapI,mapJ] >= .45 && map[mapI,mapJ] <= .55) {
					float randomX = Random.Range(-1.0f, 1.0f) * spread / 2;
					float randomZ = Random.Range(-1.0f, 1.0f) * spread / 2;
					Quaternion rotation = new Quaternion(Random.value, Random.value, Random.value, Random.value);
					Vector3 position = new Vector3(i * spread + randomX, 0, j * spread + randomZ);

					GameObject asteroidObject = (GameObject) Instantiate (getAsteroid(), position , rotation);
					asteroidObject.transform.parent = this.transform;
					AsteroidController asteroid = (AsteroidController) asteroidObject.GetComponent ("AsteroidController");
					asteroid.setLocation(new IntVector2(mapI, mapJ));
					asteroid.setValue(map[mapI, mapJ]);
					asteroid.setWorld (this);
				}
			}
		}
		*/
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

	public float getMap(int x, int y) {
		return map [x, y];
	}

	// TODO the world should handle instantiating objects, not this lower class
	/*
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
	*/

	private float generateMap() {
		//IntVector2 start = new IntVector2 (150, 150);

		float count = 0;
		Queue positions = new Queue ();
		//bool[,] enqueuedPositions = new bool[size, size];

		for(int i = 0; i < 10; i++) {

			IntVector2  start = new IntVector2  (Mathf.FloorToInt (Random.Range (1, size - 1)), Mathf.FloorToInt (Random.Range (1, size - 1)));

			addAdjacent (start.x, start.y);
			//Debug.Log ("Start Position: " + start);
			/*
			if(!enqueuedPositions[start.x + 1, start.y]) {
				positions.Enqueue (new IntVector2  (start.x + 1, start.y));
				enqueuedPositions [start.x + 1, start.y] = true;
			}

			if(!enqueuedPositions[start.x, start.y + 1]) {
				positions.Enqueue (new IntVector2 (start.x, start.y + 1));
				enqueuedPositions [start.x, start.y + 1] = true;
			}

			if(!enqueuedPositions[start.x - 1, start.y]) {
				positions.Enqueue (new IntVector2 (start.x - 1, start.y));
				enqueuedPositions [start.x - 1, start.y] = true;
			}

			if(!enqueuedPositions[start.x, start.y - 1]) {
				positions.Enqueue (new IntVector2(start.x, start.y - 1));
				enqueuedPositions [start.x, start.y - 1] = true;
			}
			*/

			enqueuedPositions[start.x, start.y] = true;
			//map [start.x, start.y] = Random.value;
			map [start.x, start.y] = 0.451f;

			//Debug.Log (map [start.x, start.y]);
			count ++;

		}

		//Equivalent to fill
		while (positions.Count > 0) {

			IntVector2 pos = (IntVector2) positions.Dequeue ();
			if(map[pos.x, pos.y] > 0) {
				Debug.Log ("Position value already set!!!");
				continue;
			}
			count++;
			int neighbors = 0;
			float neighborAvg = 0;

			//Debug.Log (pos);

			if(pos.x + 1 < size) {
				if(map[pos.x + 1, pos.y] != 0) {
					neighbors ++;
					neighborAvg += map[pos.x + 1, pos.y];
				} else {
					if(!enqueuedPositions[pos.x + 1, pos.y]) {
						positions.Enqueue(new IntVector2 (pos.x + 1, pos.y));
						enqueuedPositions[pos.x + 1, pos.y] = true;
					}
				}
			}

			if(pos.y + 1 < size) {
				if(map[pos.x, pos.y + 1] != 0) {
					neighbors ++;
					neighborAvg += map[pos.x, pos.y + 1];
				} else {
					if(!enqueuedPositions[pos.x, pos.y + 1]) {
						positions.Enqueue(new IntVector2 (pos.x, pos.y + 1));
						enqueuedPositions[pos.x, pos.y + 1] = true;
					}
				}
			}

			if(pos.x - 1 >= 0) {
				if(map[pos.x - 1, pos.y] != 0) {
					neighbors ++;
					neighborAvg += map[pos.x - 1, pos.y];
				} else {
					if(!enqueuedPositions[pos.x - 1, pos.y]) {
						positions.Enqueue(new IntVector2 (pos.x - 1, pos.y));
						enqueuedPositions[pos.x - 1, pos.y] = true;
					}				}
			}

			if(pos.y - 1 >= 0) {
				if(map[pos.x, pos.y - 1] != 0) {
					neighbors ++;
					neighborAvg += map[pos.x, pos.y - 1];
				} else {
					if(!enqueuedPositions[pos.x, pos.y - 1]) {
						positions.Enqueue(new IntVector2 (pos.x, pos.y - 1));
						enqueuedPositions[pos.x, pos.y - 1] = true;
					}
				}
			}

			neighborAvg /= neighbors;
			int distributionOk = 0;

			int loopCounts = 0;

			float lowerBound = neighborAvg - flux;
			float upperBound = neighborAvg + flux;

			while(map[pos.x, pos.y] <= 0.0f || map[pos.x, pos.y] >= 1.0f || (distributionOk = this.checkPercentages (map[pos.x, pos.y],buckets,count)) > 0) {
				loopCounts ++;
				if(loopCounts > size * size) {
					Debug.Log ("Reached loopCounts!");
					return count;
				}
				if(distributionOk == 1) {
					// High end - increase negative flux, decrease positive flux
					lowerBound += flux;
					upperBound -= flux;
				} else if(distributionOk == 2) {
					// Low end - increase positive flux, decrease negative flux
					lowerBound -= flux;
					upperBound += flux;
				}

				if(lowerBound < 0.0f) {
					//Debug.Log ("Resetting a lower bound to 0");
					lowerBound = 0;
				}
				if(upperBound > 1.0f) {
					upperBound = 1;
					//Debug.Log ("Resetting an upper bound to 1.0f");
				}

				if(distributionOk > 0) {
					//Debug.Log ("Distribution not ok");
					//return count;
				}

				map[pos.x, pos.y] = Random.Range(lowerBound, upperBound);
			}
			
			//Debug.Log (neighbors);
			//Debug.Log (neighborAvg);
		}


		Debug.Log ("Done Generating map - " + count + " created.");
		return size * size;
	}

	private void addAdjacent(IntVector2 v) {
		addAdjacent (v.x, v.y);
	}

	private void addAdjacent(int x, int y) {
		// left
		if(x - 1 >= 0 && !mapPending[x - 1][y]) {
			mapPending[x - 1][y] = true;
			toDo.Enqueue(new IntVector2(x - 1, y));
		}
		// right
		if(x + 1 < size && !mapPending[x + 1][y]) {
			mapPending[x + 1][y] = true;
			toDo.Enqueue(new IntVector2(x + 1, y));
		}
		// top
		if(y - 1 >= 0 && !mapPending[x][y - 1]) {
			mapPending[x][y - 1] = true;
			toDo.Enqueue(new IntVector2(x, y - 1));
		}
		// bot
		if(y + 1 < size && !mapPending[x][y + 1]) {
			mapPending[x][y + 1] = true;
			toDo.Enqueue(new IntVector2(x, y + 1));
		}
	}

	private int checkPercentages(float var, float[] b, float count) {

		if(var == 1.0) {
			if((1.0f + b[10])/count * 100 > 1 && count > size) {
				return 1;
			}
			b[10]++;
			return 0;
		} else if(var >= 0.9) {
			if((1.0f +  b[9])/count * 100 > 5 && count > size) {
				return 1;
			}
			b[9]++;
			return 0;
		} else if(var >= 0.8) {
			if((1.0f + b[8])/count * 100 > 10 && count > size) {
				return 1;
			}
			b[8]++;
			return 0;
		} else if(var >= 0.7) {
			if((1.0f + b[7])/count * 100 > 40 && count > size) {
				return 1;
			}
			b[7]++;
			return 0;
		} else if(var >= 0.6) {
			if((1.0f + b[6])/count * 100 > 80 && count > size) {
				return 3;
			}
			b[6]++;
			return 0;
		} else if(var >= 0.5) {
			if((1.0f + b[5])/count * 100 > 100 && count > size) {
				return 3;
			}
			b[5]++;
			return 0;
		} else if(var >= 0.4) {
			if((1.0f + b[4])/count * 100 > 80 && count > size) {
				return 3;
			}
			b[4]++;
			return 0;
		} else if(var >= 0.3) {
			if((1.0f + b[3])/count * 100 > 40 && count > size) {
				return 2;
			}
			b[3]++;
			return 0;
		} else if(var >= 0.2) {
			if((1.0f + b[2])/count * 100 > 10 && count > size) {
				return 2;
			}
			b[2]++;
			return 0;
		} else if(var >= 0.1) {
			if((1.0f + b[1])/count * 100 > 5 && count > size) {
				return 2;
			}
			b[1]++;
			return 0;
		} else if(var >= 0) {
			if((1.0f + b[0])/count * 100 > 1 && count > size) {
				return 2;
			}
			b[0]++;
			return 0;
		} 
		Debug.Log ("Shouldn't get here?");
		return 0;
	}
}

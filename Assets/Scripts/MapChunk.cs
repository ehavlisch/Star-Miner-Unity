using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class MapChunk {

	// Required for regeneration
	private int randomSeed;
	private int worldNodeId;


	private float[,] map;

	public int size;
	private float flux;

	// Values for root node
	private bool isRoot = false;
	private float startValue;

	// Edges for neighboring maps
	private float[] left;
	private float[] right;
	private float[] top;
	private float[] bot;

	// Safe to empty
	private Queue<IntVector2> toDo;
	private bool[,] mapWritten;
	private bool[,] mapPending;
	private UnityEngine.Random random;

	// should default to false
	private bool filled = false;

	// Unsure if we can empty
	float[] buckets;

	int count;

	/*
	public MapChunk(int size, float startValue, float flux, int randomSeed, int worldNodeId) {
		this.size = size;
		this.flux = flux;
		this.startValue = startValue;
		int start = (int) Mathf.Floor(size/2);
		this.randomSeed = randomSeed;
		UnityEngine.Random.seed = randomSeed;
		this.worldNodeId = worldNodeId;
		isRoot = true;
		
		init();
		
		map[start, start] = startValue;
		mapPending [start, start] = true;
		mapWritten[start, start] = true;
		
		addAdjacent(start, start);

		// Use the other fill logic below to populate a map based on percentages of materials
		generateMap();
	}
	*/

	public MapChunk(int size, float flux, MapChunk[] neighbors, int[] locations, int randomSeed, int worldNodeId) {
		this.size = size;
		this.flux = flux;
		this.randomSeed = randomSeed;
		this.worldNodeId = worldNodeId;
		init ();

		if (worldNodeId == 0) {
			int start = (int) Mathf.Floor(size/2);
			UnityEngine.Random.seed = randomSeed;

			map[start, start] = startValue;
			mapPending [start, start] = true;
			mapWritten[start, start] = true;

			addAdjacent(start, start);
			isRoot = true;
			generateMap();
		} else {
			
			if (neighbors.Length == 0 || neighbors.Length != locations.Length) {
				//System.out.println("Invalid MapChunk Neighbors");
				Debug.Log ("Invalid MapChunk Neighbors");
				return;
			}
			
			for (int i = 0; i < neighbors.Length; i++) {
				MapChunk neighbor = neighbors [i];
				switch (locations [i]) {
				case 1:
					{
						//Debug.Log ("Adding right col of neighbor to the left, enque left col into toDo");
						left = new float[size];
						for (int j = 0; j < size; j++) {
							left [j] = neighbor.getMap () [j, size - 1];
							toDo.Enqueue (new IntVector2 (j, 0));
						}
						break;
					}
				case 2:
					{
						//Debug.Log ("Adding bot row of neighbor on top, enque top row into toDo");
						top = new float[size];
						for (int j = 0; j < size; j++) {
							top [j] = neighbor.getMap () [size - 1, j];
							toDo.Enqueue (new IntVector2 (0, j));
						}
						break;
					}
				case 3:
					{
						//Debug.Log ("Adding top row of neighbor below, enque bot row into toDo");
						bot = new float[size];
						for (int j = 0; j < size; j++) {
							bot [j] = neighbor.getMap () [0, j];
							toDo.Enqueue (new IntVector2 (size - 1, j));
						}
						break;
					}
				case 4:
					{
						//Debug.Log ("Adding left col of neighbor to the right, enque the right col into toDo");
						right = new float[size];
						for (int j = 0; j < size; j++) {
							right [j] = neighbor.getMap () [j, 0];
							toDo.Enqueue (new IntVector2 (j, size - 1));
						}
						break;
					}
				default:
					{
						Debug.Log ("Invalid location value in locations array");
						return;
						//throw new Exception("Invalid location value in locations array");
					}
				}
			}	
			generateMap ();
		}
	}

	private void init() {
		toDo = new Queue<IntVector2>();
		map = new float[size, size];
		mapWritten = new bool[size, size];
		mapPending = new bool[size, size];
		buckets = new float[10];
		filled = false;
		count = 0;
	}

	//FIXME this method won't be called ever, keeping it around for reference for a bit
	void Start() {
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

	public void reFill() {
		Debug.Log ("refilling?");
		init();
		if(toDo.Count == 0) {
			if(isRoot) {
				int start = (int) Mathf.Floor(size/2);
				
				map[start, start] = startValue;
				mapWritten[start, start] = true;
				
				addAdjacent(start, start);
			} else {
				
				if(top != null) {
					for(int j = 0; j < size; j++) {
						toDo.Enqueue(new IntVector2(j, 0));
					}
				}
				if(bot != null) {
					for(int j = 0; j < size; j++) {
						toDo.Enqueue(new IntVector2(j, size - 1));
					}
				}
				if(left != null) {
					for(int j = 0; j < size; j++) {
						toDo.Enqueue(new IntVector2(0, j));
					}
				}
				if(right != null) {
					for(int j = 0; j < size; j++) {
						toDo.Enqueue(new IntVector2(size - 1, j));
					}
				}
			}
			if(toDo.Count == 0) {
				Debug.Log("Cannot fill MapChunk, toDo list is empty");
			}
			generateMap();
		}
	}

	private float generateMap() {
		/*
		if (top != null) {
			Debug.Log ("Top top set.");
		}
		if (bot != null) {
			Debug.Log ("Bot row set.");
		}
		if (left != null) {
			Debug.Log ("Left Col set.");
		}
		if (right != null) {
			Debug.Log ("Right col set.");
		}
		*/
		if(!isRoot && top == null && bot == null && left == null && right == null) {
			Debug.Log ("SEVERE: Trying to generate a map chunk with no neighbors.");
		}

		Debug.Log ("Filling a mapchunk");

		while (toDo.Count > 0) {

			IntVector2 pos = (IntVector2)toDo.Dequeue ();

			if (mapWritten [pos.x, pos.y]) {
				//Debug.Log ("Position value already set!!!");
				continue;
			}

			count++;
			addAdjacent (pos);
			float avg = avgAdjacent (pos.x, pos.y);

			getDistributionOkValue(pos, avg);
			mapWritten[pos.x, pos.y] = true;
		}

		if (isRoot) {
			count++;
		}
		
		if (count != size * size) {
			Debug.Log ("Done Generating map - " + count + " created.");
			for (int i = 0; i < size; i++) {
				for (int j = 0; j < size; j++) {
					if (!mapWritten [i, j]) {
						Debug.Log ("Not written: (" + i + ", " + j + ")");
					}
				}
			}
		}
		filled = true;
		return size * size;
	}



	private void getDistributionOkValue(IntVector2 pos, float avg) {
		int distributionOk = 0;
		
		int loopCounts = 0;
		
		float lowerBound = avg - flux;
		float upperBound = avg + flux;
		
		while (map[pos.x, pos.y] <= 0.0f || map[pos.x, pos.y] >= 1.0f 
		       //Some issues with this right now, we hit loop counts a bunch even with check percentages running only after a certain period of time
		       || (distributionOk = checkPercentages (map[pos.x, pos.y],buckets,count)) > 0
		       ) {
			loopCounts ++;
			if (loopCounts > size * size) {
				Debug.Log ("SEVERE: Reached loopCounts! " + distributionOk);
				return;
			}
			if (distributionOk == 1) {
				// High end - increase negative flux, decrease positive flux
				lowerBound += flux;
				upperBound -= flux;
			} else if (distributionOk == 2) {
				// Low end - increase positive flux, decrease negative flux
				lowerBound -= flux;
				upperBound += flux;
			}
			
			if (lowerBound < 0.0f) {
				//Debug.Log ("Resetting a lower bound to 0");
				lowerBound = 0;
			}
			if (upperBound > 1.0f) {
				upperBound = 1;
				//Debug.Log ("Resetting an upper bound to 1.0f");
			}
			
			if (distributionOk > 0) {
				//Debug.Log ("Distribution not ok");
				//return count;
			}
			
			map [pos.x, pos.y] = UnityEngine.Random.Range (lowerBound, upperBound);
		}
	}

	private void addAdjacent(IntVector2 v) {
		addAdjacent (v.x, v.y);
	}

	private void addAdjacent(int x, int y) {
		if(x - 1 >= 0 && !mapPending[x - 1, y]) {
			mapPending[x - 1, y] = true;
			toDo.Enqueue(new IntVector2(x - 1, y));
		}
		if(x + 1 < size && !mapPending[x + 1, y]) {
			mapPending[x + 1, y] = true;
			toDo.Enqueue(new IntVector2(x + 1, y));
		}
		if(y - 1 >= 0 && !mapPending[x, y - 1]) {
			mapPending[x, y - 1] = true;
			toDo.Enqueue(new IntVector2(x, y - 1));
		}
		if(y + 1 < size && !mapPending[x, y + 1]) {
			mapPending[x, y + 1] = true;
			toDo.Enqueue(new IntVector2(x, y + 1));
		}
	}

	// ROW, COLUMN 
	private float avgAdjacent(int x, int y) {
		float avg = 0.0f;
		int count = 0;

		// looking at row above
		if(x > 0 && mapWritten[x - 1, y]) {
			avg += map[x - 1, y];
			count++;
		} else if(x == 0 && top != null) {
			// x == 0 redundant?
			avg += top[y];
			count++;
		}

		// looking at row below
		if(x + 1 < size - 1 && mapWritten[x + 1, y]) {
			avg += map[x + 1, y];
			count++;
		} else if(bot != null) {
			avg += bot[y];
			count++;
		}

		// looking at column left
		if(y - 1 >= 0 && mapWritten[x, y - 1]) {
			avg += map[x, y - 1];
			count++;
		} else if(right != null) {
			avg += right[x];
			count++;
		}

		// looking at column right
		if(y + 1 < size - 1 && mapWritten[x, y + 1]) {
			avg += map[x, y + 1];
			count++;
		} else if(left != null) {
			avg += left[x];
			count++;
		}
		
		return avg/count;
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

	public static string getChars(float value) {
		if(value > 0.90000f) {
			return ".";
		} else if(value > 0.80000f) {
			return "$";
		} else if(value > 0.70000f) {
			return "\\";
		} else if(value > 0.60000f){
			return "|";
		} else if(value > 0.50000f) {
			return "+";
		} else if(value > 0.40000f) {
			return "-";
		} else if(value > 0.30000f) {
			return "/";
		} else if(value > 0.20000f) {
			return "#";
		} else if(value > 0.10000f) {
			return ".";
		} else if(value > 0.0f) {
			return ":";
		} else {
			return "?";
		}
	}

	public void printRows(StringBuilder[] sbs) {
		for(int i = 0; i < size; i++) {
			for(int j = 0; j < size; j++) {
				sbs[i].Append(getChars(map[i, j]));
			}
		}
	}

	public void empty() {
		toDo = null;
		map = null;
		mapWritten = null;
		mapPending = null;
		random = null;
		filled = false;
	}

	public float getMap(IntVector2 v) {
		return map [v.x, v.y];
	}

	public float getMap(int x, int y) {
		return map [x, y];
	}

	public float[,] getMap() {
		return map;
	}

	public Boolean isFilled() {
		return filled;
	}
}

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class MapChunk {

    // Required for regeneration
    private int randomSeed;
    private int worldNodeId;

    public float[,] map { get; set; }

    private int size;
    private float flux;

    // Values for root node
    private bool isRoot = false;
    // public to avoid warning
    public float startValue = 0.5f;

    // Edges for neighboring maps
    private float[] left;
    private float[] right;
    private float[] top;
    private float[] bot;

    // Safe to empty
    private Queue<IntVector2> toDo;
    private bool[,] mapWritten;
    public bool[,] mapPending;

    //TODO this should be used for regenerating chunks, but it seems like it can't be instantiated...
    //Might have to change this to reset the seed before regenerating in the UnityEngine...

    // should default to false
    public bool filled { get; set; }

	// Unsure if we can empty
	float[] buckets;

	int count;

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
							left [j] = neighbor.map[j, size - 1];
							toDo.Enqueue (new IntVector2 (j, 0));
						}
						break;
					}
				case 2:
					{
						//Debug.Log ("Adding bot row of neighbor on top, enque top row into toDo");
						top = new float[size];
						for (int j = 0; j < size; j++) {
							top [j] = neighbor.map[size - 1, j];
							toDo.Enqueue (new IntVector2 (0, j));
						}
						break;
					}
				case 3:
					{
						//Debug.Log ("Adding top row of neighbor below, enque bot row into toDo");
						bot = new float[size];
						for (int j = 0; j < size; j++) {
							bot [j] = neighbor.map[0, j];
							toDo.Enqueue (new IntVector2 (size - 1, j));
						}
						break;
					}
				case 4:
					{
						//Debug.Log ("Adding left col of neighbor to the right, enque the right col into toDo");
						right = new float[size];
						for (int j = 0; j < size; j++) {
							right [j] = neighbor.map[j, 0];
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
		buckets = new float[9];
		filled = false;
		count = 0;
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

        //Debug.Log ("Filling a mapchunk");
        int revertSeed = UnityEngine.Random.seed;
		UnityEngine.Random.seed = randomSeed + worldNodeId; 

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
			Debug.LogWarning ("Done Generating map - " + count + " created.");
			for (int i = 0; i < size; i++) {
				for (int j = 0; j < size; j++) {
					if (!mapWritten [i, j]) {
						Debug.LogWarning ("Not written: (" + i + ", " + j + ")");
					}
				}
			}
		}
		filled = true;
        UnityEngine.Random.seed = revertSeed;

        //StringBuilder sb = new StringBuilder();
        //for(int i = 0; i < 9; i++) {
        //   sb.Append("Bucket[" + i + "]=" + buckets[i] + "\n");
        //}
        //Debug.Log(sb.ToString());

        return size * size;
	}



	private void getDistributionOkValue(IntVector2 pos, float avg) {
		int distributionOk = 0;
		
		int loopCounts = 0;
		
		float lowerBound = avg - flux;
		float upperBound = avg + flux;

        if(lowerBound < 0) {
            lowerBound = 0.0f;
            upperBound = 0.0f + flux;
        }
        if(upperBound > 1.0f) {
            upperBound = 1.0f;
            lowerBound = 1.0f - flux;
        }

		if (count < size) {
			map [pos.x, pos.y] = UnityEngine.Random.Range (lowerBound, upperBound);
		} else {
            float value = UnityEngine.Random.Range(lowerBound, upperBound);
            distributionOk = checkPercentages(value, buckets, count);
            while (value < 0.0f || value > 1.0f || distributionOk != 0) {
				loopCounts ++;
				if (loopCounts > 10) {
					Debug.LogWarning ("Reached loopCounts! " + distributionOk + " Failed to generate in range: " + lowerBound + " to " + upperBound + " Based on avg: " + avg);
					map[pos.x, pos.y] = upperBound - lowerBound / 2.0f;
					return;
				}
				if (distributionOk == 1) {
					// High end - increase negative flux, decrease positive flux
					lowerBound -= flux;
					upperBound -= flux;
				} else if (distributionOk == 2) {
					// Low end - increase positive flux, decrease negative flux
					lowerBound += flux;
					upperBound += flux;
				} else if(distributionOk == 3) {
                    // Shift to one of the random buckets above or below the middle one
                    if(UnityEngine.Random.value > 0.5) {
                        lowerBound += .1f;
                        upperBound += .1f;
                    } else {
                        lowerBound -= .1f;
                        upperBound -= .1f;
                    }
                }


                if (lowerBound < 0) {
                    lowerBound = 0.0f;
                    upperBound = 0.0f + flux;
                }
                if (upperBound > 1.0f) {
                    upperBound = 1.0f;
                    lowerBound = 1.0f - flux;
                }
				
				value = UnityEngine.Random.Range (lowerBound, upperBound);
                distributionOk = checkPercentages(value, buckets, count);
			}
            map[pos.x, pos.y] = value;
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

    // Check if the var fits into the buckets without violating the limits
    // Return 1 if it overflows a bucket and should be lowered
    // Return 2 if it overflows a bucket and should be raised
    // Return 3 if it overflows a bucket near the center of the distribution
    // Return 0 if it does not overflow a bucket
	private int checkPercentages(float var, float[] b, float count) {
        if(var >= 0.9) {
			if((1.0f + b[8] + b[0])/count * 100 > 1 && count > size) {
				return 1;
			}
			b[8]++;
			return 0;
		} else if(var >= 0.8) {
			if((1.0f + b[7] + b[1])/count * 100 > 5 && count > size) {
				return 1;
			}
			b[7]++;
			return 0;
		} else if(var >= 0.7) {
			if((1.0f + b[6] + b[2])/count * 100 > 10 && count > size) {
				return 1;
			}
			b[6]++;
			return 0;
		} else if(var >= 0.6) {
			if((1.0f + b[5] + b[3])/count * 100 > 40 && count > size) {
				return 1;
			}
			b[5]++;
			return 0;
		} else if(var >= 0.4) {
			if((1.0f + b[4])/count * 100 > 90 && count > size) {
				return 3;
			}
			b[4]++;
			return 0;
		} else if(var >= 0.3) {
			if((1.0f + b[3] + b[5])/count * 100 > 40 && count > size) {
				return 2;
			}
			b[3]++;
			return 0;
		} else if(var >= 0.2) {
			if((1.0f + b[2] + b[6])/count * 100 > 10 && count > size) {
				return 2;
			}
			b[2]++;
			return 0;
		} else if(var >= 0.1) {
			if((1.0f + b[1] + b[7])/count * 100 > 5 && count > size) {
				return 2;
			}
			b[1]++;
			return 0;
		} else if(var >= 0) {
			if((1.0f + b[0] + b[8])/count * 100 > 1 && count > size) {
				return 2;
			}
			b[0]++;
			return 0;
		} 
		Debug.Log ("Shouldn't get here?");
		return 0;
	}

	public static string getChars(float value) {
		if(value > 0.90f) {
			return "0";
		} else if(value > 0.8f) {
			return "-";
		} else if(value > 0.7f) {
			return "*";
		} else if(value > 0.6f){
			return "+";
		} else if(value > 0.5f) {
			return "A";
		} else if(value > 0.4f) {
			return "A";
		} else if(value > 0.3f) {
			return "+";
		} else if(value > 0.2f) {
			return "*";
		} else if(value > 0.1f) {
			return "-";
		} else if(value > 0.0f) {
			return "0";
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
		filled = false;
	}

	public float getMap(IntVector2 v) {
		return map [v.x, v.y];
	}

	public float getMap(int x, int y) {
		return map [x, y];
	}
}

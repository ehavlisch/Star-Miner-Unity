using System;

public enum Direction
{
	UP, RIGHT, DOWN, LEFT, NONE
};

public static class Directions {
	public static Direction perpOne(this Direction dir) {
		switch (dir) {
		case Direction.UP: return Direction.RIGHT;
		case Direction.DOWN: return Direction.LEFT;
		case Direction.LEFT: return Direction.UP;
		case Direction.RIGHT: return Direction.DOWN;
		default: return Direction.NONE;
		}
	}

	public static Direction perpTwo(this Direction dir) {
		switch (dir) {
		case Direction.UP: return Direction.LEFT;
		case Direction.DOWN: return Direction.RIGHT;
		case Direction.LEFT: return Direction.DOWN;
		case Direction.RIGHT: return Direction.UP;
		default: return Direction.NONE;
		}
	}

	public static Direction opposite(this Direction dir) {
		switch(dir) {
		case Direction.UP: return Direction.DOWN;
		case Direction.DOWN: return Direction.UP;
		case Direction.LEFT: return Direction.RIGHT;
		case Direction.RIGHT: return Direction.LEFT;
		default: return Direction.NONE;
		}
	}

	public static IntVector2 getPair(this Direction dir) {
		switch (dir) {
		case Direction.UP: return new IntVector2(-1,0);
		case Direction.DOWN: return new IntVector2(1,0);
		case Direction.LEFT: return new IntVector2(0,-1);
		case Direction.RIGHT: return new IntVector2(0,1); 
		default: return new IntVector2(0,0);
		}
	}
}


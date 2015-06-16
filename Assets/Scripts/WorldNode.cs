using System;

public class WorldNode {
	/*
	 * Locations:
	 * 		  Up
	 * Left	mapChunk Right
	 * 		 Down
	 */
	private MapChunk mapChunk;
	private WorldNode left;
	private WorldNode right;
	private WorldNode up;
	private WorldNode down;
	private IntVector2 location;
	
	private int worldNodeId;

	public WorldNode(int worldNodeId) {
		this.worldNodeId = worldNodeId;
	}
	
	public String toString() {
		return "World Node: " + location;
	}
	
	public MapChunk getMapChunk() {
		return mapChunk;
	}
	
	public void setMapChunk(MapChunk mapChunk) {
		this.mapChunk = mapChunk;
	}
	
	public WorldNode getLeft() {
		return left;
	}
	
	public void setLeft(WorldNode left) {
		this.left = left;
	}
	
	public WorldNode getRight() {
		return right;
	}
	
	public void setRight(WorldNode right) {
		this.right = right;
	}
	
	public WorldNode getUp() {
		return up;
	}
	
	public void setUp(WorldNode up) {
		this.up = up;
	}
	
	public WorldNode getDown() {
		return down;
	}
	
	public void setDown(WorldNode down) {
		this.down = down;
	}
	
	public IntVector2 getLocation() {
		return location;
	}
	
	public void setLocation(IntVector2 location) {
		this.location = location;
	}
	
	public int getWorldNodeId() {
		return worldNodeId;
	}
	
	public void fill() {
		mapChunk.reFill();
	}

	//TODO this is supposed to return this instance	
	public void load() {
		if(!mapChunk.isFilled()) {
			mapChunk.reFill();
		}
		//return this;
	}

	
	public void unload() {
		if(mapChunk.isFilled()) {
			mapChunk.empty();
		}
	}
	
	public bool isLoaded() {
		if(mapChunk == null || !mapChunk.isFilled()) {
			return false;
		} 
		return true;
	}
	
	public void printRows(StringBuilder[] sbs) {
		mapChunk.printRows(sbs);
	}
}

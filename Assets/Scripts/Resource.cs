using UnityEngine;
using System.Collections;

namespace Industry {
	public class Resource {
		private string id;
		private string name;
		private string description;
		private int baseValue;
		private int rarity;
		private int highStock;

		//TODO this needs to work more closely like the cargo class in the pickup controller
		// specifically, need to add mass and volume
		
		// Resource id needs to be unique among resources or they will be replaced
		public Resource createResource(string id, string name, string description, int baseValue, int rarity, int highStock) {
			this.id = id;
			this.name = name;
			this.description = description;
			this.baseValue = baseValue;
			this.rarity = rarity;
			this.highStock = highStock;
		}
		
		public Resource createResource(string id, string name, string description, int baseValue, int highStock) {
			this.id = id;
			this.name = name;
			this.description = description;
			this.baseValue = baseValue;
			this.rarity = 0;
			this.highStock = highStock;
		}
		
		public string getId() {
			return id;
		}
		
		public string getName() {
			return name;
		}
		
		public string getDescription() {
			return description;
		}
		
		public int getRarity() {
			return rarity;
		}
		
		public int getBaseValue() {
			return baseValue;
		}
		
		public int getHighStock() {
			return highStock;
		}
	}
}

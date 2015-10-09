using UnityEngine;
using System.Collections;

namespace Industry {
	public class Resource {
		private int id;
		private string name;
		private string description;
		private int baseValue;
		private int rarity;
		private int highStock;
		private int volume;
		private int mass;

		//TODO this needs to work more closely like the cargo class in the pickup controller
		// specifically, need to add mass and volume
		
		// Resource id needs to be unique among resources or they will be replaced
		public Resource (int id, string name, string description, int baseValue, int rarity, int highStock, int volume, int mass) {
			this.id = id;
			this.name = name;
			this.description = description;
			this.baseValue = baseValue;
			this.rarity = rarity;
			this.highStock = highStock;
			this.volume = volume;
			this.mass = mass;
		}
		
		public int getId() {
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

namespace Economy {
	public class Resource {
        public int id;
        public string name;
        public int baseValue;

        // Public to avoid warnings
        public string description;
        public int rarity;
        public int highStock;
        public int volume;
        public int mass;

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
	}
}


namespace Economy {
    class ResourceCargo : Cargo {

        Resource resource;
        float count;
        
        public ResourceCargo(Resource resource, float count) : base(CargoType.RESOURCE, count * resource.mass, count * resource.volume, resource.name, count * resource.baseValue, "resource" + resource.id) {
            this.resource = resource;
            this.count = count;
        }
    }
}

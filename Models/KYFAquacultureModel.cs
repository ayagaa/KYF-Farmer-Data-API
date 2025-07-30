using Newtonsoft.Json;

namespace Farmer.Data.API.Models
{
    public class KYFAquacultureModel
    {
        [JsonProperty("aquacultureTypes")]
        public List<AquacultureType> AquacultureTypes { get; set; } = new List<AquacultureType>();

        [JsonProperty("fishSpecies")]
        public List<FishSpecies> FishSpecies { get; set; } = new List<FishSpecies>();

        [JsonProperty("productionLevel")]
        public string ProductionLevel { get; set; }

        [JsonProperty("productionSystem")]
        public List<AquaProductionSystem> ProductionSystems { get; set; } = new List<AquaProductionSystem>();
    }

    public class AquacultureType
    {
        [JsonProperty("aquacultureType")]
        public string TypeName { get; set; }
    }

    public class FishSpecies
    {
        [JsonProperty("speciesName")]
        public string SpeciesName { get; set; }

        [JsonProperty("numberOfFingerlings")]
        public string NumberOfFingerlings { get; set; }
    }
    public class AquaProductionSystem
    {
        [JsonProperty("productionSystem")]
        public string ProductionSystem { get; set; }

        [JsonProperty("productionSystemStatus")]
        public string ProductionSystemStatus { get; set; }

        [JsonProperty("productionSystemDimensions")]
        public List<AquaProductionSystemDimensions> ProductionSystemDimenions { get; set; } = new List<AquaProductionSystemDimensions>();
    }

    public class AquaProductionSystemDimensions
    {
        //public string Dimension { get; set; }
        //public string DimensionDescription { get; set; }
        //public string DimensionUnit { get; set; }
        [JsonProperty("length")]
        public string Length { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("diameter")]
        public string Diameter { get; set; }
    }
}

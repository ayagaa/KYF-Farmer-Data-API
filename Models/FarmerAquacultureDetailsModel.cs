using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Farmer.Data.API.Models
{
    public class FarmerAquacultureDetailsModel
    {
        [JsonProperty("_id")]
        [JsonPropertyName("_id")]
        public string Id;

        [JsonProperty("farmer_id")]
        [JsonPropertyName("farmer_id")]
        public int FarmerId;

        [JsonProperty("fish_type_id")]
        [JsonPropertyName("fish_type_id")]
        public int FishTypeId;

        [JsonProperty("fish_production_type")]
        [JsonPropertyName("fish_production_type")]
        public string FishProductionType;

        [JsonProperty("fish_type")]
        [JsonPropertyName("fish_type")]
        public string FishType;

        [JsonProperty("fish_category_id")]
        [JsonPropertyName("fish_category_id")]
        public int FishCategoryId;

        [JsonProperty("fish_category")]
        [JsonPropertyName("fish_category")]
        public string FishCategory;

        [JsonProperty("farmer_farm_id")]
        [JsonPropertyName("farmer_farm_id")]
        public int FarmerFarmId;

        [JsonProperty("production_status")]
        [JsonPropertyName("production_status")]
        public int ProductionStatus;

        [JsonProperty("active_area")]
        [JsonPropertyName("active_area")]
        public string ActiveArea;

        [JsonProperty("no_of_active_units")]
        [JsonPropertyName("no_of_active_units")]
        public int NoOfActiveUnits;

        [JsonProperty("inactive_area")]
        [JsonPropertyName("inactive_area")]
        public string InactiveArea;

        [JsonProperty("no_of_inactive_units")]
        [JsonPropertyName("no_of_inactive_units")]
        public int NoOfInactiveUnits;

        [JsonProperty("unit_of_measure_id")]
        [JsonPropertyName("unit_of_measure_id")]
        public int UnitOfMeasureId;

        [JsonProperty("no_of_fingerlings")]
        [JsonPropertyName("no_of_fingerlings")]
        public int NoOfFingerlings;

        [JsonProperty("unit_of_measure")]
        [JsonPropertyName("unit_of_measure")]
        public string UnitOfMeasure;
    }
}

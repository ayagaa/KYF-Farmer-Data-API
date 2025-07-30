using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Farmer.Data.API.Models
{
    public class FarmerCropDetailsModel
    {
        [JsonProperty("_id")]
        [JsonPropertyName("_id")]
        public string Id;

        [JsonProperty("farmer_id")]
        [JsonPropertyName("farmer_id")]
        public int FarmerId;

        [JsonProperty("crop_id")]
        [JsonPropertyName("crop_id")]
        public int CropId;

        [JsonProperty("crop")]
        [JsonPropertyName("crop")]
        public string Crop;

        [JsonProperty("crop_cat_id")]
        [JsonPropertyName("crop_cat_id")]
        public int CropCatId;

        [JsonProperty("crop_category")]
        [JsonPropertyName("crop_category")]
        public string CropCategory;

        [JsonProperty("area_unit_id")]
        [JsonPropertyName("area_unit_id")]
        public int AreaUnitId;

        [JsonProperty("area_unit")]
        [JsonPropertyName("area_unit")]
        public string AreaUnit;

        [JsonProperty("usage_of_certified_Seeds")]
        [JsonPropertyName("usage_of_certified_Seeds")]
        public int UsageOfCertifiedSeeds;

        [JsonProperty("crop_motive_id")]
        [JsonPropertyName("crop_motive_id")]
        public int CropMotiveId;

        [JsonProperty("crop_motive")]
        [JsonPropertyName("crop_motive")]
        public string CropMotive;

        [JsonProperty("crop_system_id")]
        [JsonPropertyName("crop_system_id")]
        public int CropSystemId;

        [JsonProperty("cropping_system")]
        [JsonPropertyName("cropping_system")]
        public string CroppingSystem;

        [JsonProperty("water_source_id")]
        [JsonPropertyName("water_source_id")]
        public int WaterSourceId;

        [JsonProperty("water_source")]
        [JsonPropertyName("water_source")]
        public string WaterSource;

        [JsonProperty("pesticide_use")]
        [JsonPropertyName("pesticide_use")]
        public int PesticideUse;

        [JsonProperty("crop_area")]
        [JsonPropertyName("crop_area")]
        public double CropArea;

        [JsonProperty("fungicides")]
        [JsonPropertyName("fungicides")]
        public int Fungicides;

        [JsonProperty("insecticides")]
        [JsonPropertyName("insecticides")]
        public int Insecticides;

        [JsonProperty("herbicides")]
        [JsonPropertyName("herbicides")]
        public int Herbicides;

        [JsonProperty("rodenticides")]
        [JsonPropertyName("rodenticides")]
        public int Rodenticides;

        [JsonProperty("molluscides")]
        [JsonPropertyName("molluscides")]
        public int Molluscides;
    }
}

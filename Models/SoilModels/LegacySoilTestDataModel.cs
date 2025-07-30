using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Farmer.Data.API.Models.SoilModels
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class E
    {
        [JsonProperty("C mmhcs_per_cm", NullValueHandling = NullValueHandling.Ignore)]
        [BsonElement("C mmhcs_per_cm")]
        public string Cmmhcs_per_cm { get; set; }

        [JsonProperty("C mmhcs_per_cm_Class", NullValueHandling = NullValueHandling.Ignore)]
        [BsonElement("C mmhcs_per_cm_Class")]
        public object Cmmhcs_per_cm_Class { get; set; }
    }

    public class LegacySoilTestDataModel
    {
        [JsonIgnore]
        [JsonProperty("_id", NullValueHandling = NullValueHandling.Ignore)]
        public string _id { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string id { get; set; }

        [JsonProperty("Lab_No", NullValueHandling = NullValueHandling.Ignore)]
        public string Lab_No { get; set; }

        [JsonProperty("Lab_No_Year", NullValueHandling = NullValueHandling.Ignore)]
        public string Lab_No_Year { get; set; }

        [JsonProperty("County", NullValueHandling = NullValueHandling.Ignore)]
        public string County { get; set; }

        [JsonProperty("Constituency", NullValueHandling = NullValueHandling.Ignore)]
        public object Constituency { get; set; }

        [JsonProperty("Ward", NullValueHandling = NullValueHandling.Ignore)]
        public object Ward { get; set; }

        [JsonProperty("Location", NullValueHandling = NullValueHandling.Ignore)]
        public object Location { get; set; }

        [JsonProperty("Sub_location", NullValueHandling = NullValueHandling.Ignore)]
        public object Sub_location { get; set; }

        [JsonProperty("Village", NullValueHandling = NullValueHandling.Ignore)]
        public object Village { get; set; }

        [JsonProperty("Description_of_the_Farm", NullValueHandling = NullValueHandling.Ignore)]
        public string Description_of_the_Farm { get; set; }

        [JsonProperty("Latitude", NullValueHandling = NullValueHandling.Ignore)]
        public object Latitude { get; set; }

        [JsonProperty("Longitude", NullValueHandling = NullValueHandling.Ignore)]
        public object Longitude { get; set; }

        [JsonProperty("Year", NullValueHandling = NullValueHandling.Ignore)]
        public string Year { get; set; }

        [JsonIgnore]
        [JsonProperty("Farmers_Name", NullValueHandling = NullValueHandling.Ignore)]
        public string Farmers_Name { get; set; }

        [JsonIgnore]
        [JsonProperty("Telephone_No", NullValueHandling = NullValueHandling.Ignore)]
        public object Telephone_No { get; set; }

        [JsonProperty("Years_Under_Cultivation", NullValueHandling = NullValueHandling.Ignore)]
        public object Years_Under_Cultivation { get; set; }

        [JsonProperty("Project_Name", NullValueHandling = NullValueHandling.Ignore)]
        public string Project_Name { get; set; }

        [JsonProperty("Sample_description", NullValueHandling = NullValueHandling.Ignore)]
        public object Sample_description { get; set; }

        [JsonProperty("Soil_depth_cm", NullValueHandling = NullValueHandling.Ignore)]
        public string Soil_depth_cm { get; set; }

        [JsonProperty("Soil_pH", NullValueHandling = NullValueHandling.Ignore)]
        public string Soil_pH { get; set; }

        [JsonProperty("Soil_pH_Class", NullValueHandling = NullValueHandling.Ignore)]
        public string Soil_pH_Class { get; set; }

        [JsonProperty("Exch_Acidity_me_percent_", NullValueHandling = NullValueHandling.Ignore)]
        public string Exch_Acidity_me_percent_ { get; set; }

        [JsonProperty("Exch_Acidity_Class", NullValueHandling = NullValueHandling.Ignore)]
        public object Exch_Acidity_Class { get; set; }

        [JsonProperty("Total_Nitrogen_percent_", NullValueHandling = NullValueHandling.Ignore)]
        public string Total_Nitrogen_percent_ { get; set; }

        [JsonProperty("Total_Nitrogen_percent_Class", NullValueHandling = NullValueHandling.Ignore)]
        public string Total_Nitrogen_percent_Class { get; set; }

        [JsonProperty("Total_Org_Carbon_percent_", NullValueHandling = NullValueHandling.Ignore)]
        public string Total_Org_Carbon_percent_ { get; set; }

        [JsonProperty("Total_Org_Carbon_percent_Class", NullValueHandling = NullValueHandling.Ignore)]
        public string Total_Org_Carbon_percent_Class { get; set; }

        [JsonProperty("Phosphorus_Olsen_ppm", NullValueHandling = NullValueHandling.Ignore)]
        public string Phosphorus_Olsen_ppm { get; set; }

        [JsonProperty("Phosphorus_Olsen_ppm_Class", NullValueHandling = NullValueHandling.Ignore)]
        public string Phosphorus_Olsen_ppm_Class { get; set; }

        [JsonProperty("Potassium_meq_percent_", NullValueHandling = NullValueHandling.Ignore)]
        public string Potassium_meq_percent_ { get; set; }

        [JsonProperty("Potassium_meq_percent_Class", NullValueHandling = NullValueHandling.Ignore)]
        public string Potassium_meq_percent_Class { get; set; }

        [JsonProperty("Calcium_meq_percent_", NullValueHandling = NullValueHandling.Ignore)]
        public string Calcium_meq_percent_ { get; set; }

        [JsonProperty("Calcium_meq_percent_Class", NullValueHandling = NullValueHandling.Ignore)]
        public string Calcium_meq_percent_Class { get; set; }

        [JsonProperty("Magnesium_meq_percent_", NullValueHandling = NullValueHandling.Ignore)]
        public string Magnesium_meq_percent_ { get; set; }

        [JsonProperty("Magnesium_meq_percent_Class", NullValueHandling = NullValueHandling.Ignore)]
        public string Magnesium_meq_percent_Class { get; set; }

        [JsonProperty("Manganese_meq_percent_", NullValueHandling = NullValueHandling.Ignore)]
        public string Manganese_meq_percent_ { get; set; }

        [JsonProperty("Manganese_meq_percent_Class", NullValueHandling = NullValueHandling.Ignore)]
        public string Manganese_meq_percent_Class { get; set; }

        [JsonProperty("Copper_ppm", NullValueHandling = NullValueHandling.Ignore)]
        public string Copper_ppm { get; set; }

        [JsonProperty("Copper_ppm_Class", NullValueHandling = NullValueHandling.Ignore)]
        public string Copper_ppm_Class { get; set; }

        [JsonProperty("Iron_ppm", NullValueHandling = NullValueHandling.Ignore)]
        public string Iron_ppm { get; set; }

        [JsonProperty("Iron_ppm_Class", NullValueHandling = NullValueHandling.Ignore)]
        public string Iron_ppm_Class { get; set; }

        [JsonProperty("Zinc_ppm", NullValueHandling = NullValueHandling.Ignore)]
        public string Zinc_ppm { get; set; }

        [JsonProperty("Zinc_ppm_Class", NullValueHandling = NullValueHandling.Ignore)]
        public string Zinc_ppm_Class { get; set; }

        [JsonProperty("Sodium_meq_percent_", NullValueHandling = NullValueHandling.Ignore)]
        public string Sodium_meq_percent_ { get; set; }

        [JsonProperty("Sodium_meq_percent_Class", NullValueHandling = NullValueHandling.Ignore)]
        public string Sodium_meq_percent_Class { get; set; }

        [JsonProperty("Electr_Conductivity_mS_per_cm", NullValueHandling = NullValueHandling.Ignore)]
        public string Electr_Conductivity_mS_per_cm { get; set; }

        [JsonProperty("Electr_Conductivity_mS_per_cm_Class", NullValueHandling = NullValueHandling.Ignore)]
        public object Electr_Conductivity_mS_per_cm_Class { get; set; }

        [JsonProperty("Alluminium_ppm                            ", NullValueHandling = NullValueHandling.Ignore)]
        public string Alluminium_ppm { get; set; }

        [JsonProperty("Alluminium_ppm_Class", NullValueHandling = NullValueHandling.Ignore)]
        public object Alluminium_ppm_Class { get; set; }

        [JsonProperty("Available_Nitrogen", NullValueHandling = NullValueHandling.Ignore)]
        public string Available_Nitrogen { get; set; }

        [JsonProperty("Available_Nitrogen_Class", NullValueHandling = NullValueHandling.Ignore)]
        public object Available_Nitrogen_Class { get; set; }

        [JsonProperty("Boron_ppm", NullValueHandling = NullValueHandling.Ignore)]
        public string Boron_ppm { get; set; }

        [JsonProperty("Boron_ppm_Class", NullValueHandling = NullValueHandling.Ignore)]
        public object Boron_ppm_Class { get; set; }

        [JsonProperty("Cadmiun_ppm", NullValueHandling = NullValueHandling.Ignore)]
        public string Cadmiun_ppm { get; set; }

        [JsonProperty("Cadmiun_ppm_Class", NullValueHandling = NullValueHandling.Ignore)]
        public object Cadmiun_ppm_Class { get; set; }

        [JsonProperty("Cobalt_ppm", NullValueHandling = NullValueHandling.Ignore)]
        public string Cobalt_ppm { get; set; }

        [JsonProperty("Cobalt_ppm_Class", NullValueHandling = NullValueHandling.Ignore)]
        public object Cobalt_ppm_Class { get; set; }

        [JsonProperty("Gallium_ppm", NullValueHandling = NullValueHandling.Ignore)]
        public string Gallium_ppm { get; set; }

        [JsonProperty("Gallium_ppm_Class", NullValueHandling = NullValueHandling.Ignore)]
        public object Gallium_ppm_Class { get; set; }

        [JsonProperty("Silver_ppm", NullValueHandling = NullValueHandling.Ignore)]
        public string Silver_ppm { get; set; }

        [JsonProperty("Silver_ppm_Class", NullValueHandling = NullValueHandling.Ignore)]
        public object Silver_ppm_Class { get; set; }

        [JsonProperty("Lead_ppm", NullValueHandling = NullValueHandling.Ignore)]
        public string Lead_ppm { get; set; }

        [JsonProperty("Lead_ppm_Class", NullValueHandling = NullValueHandling.Ignore)]
        public object Lead_ppm_Class { get; set; }

        [JsonProperty("Chromium_ppm", NullValueHandling = NullValueHandling.Ignore)]
        public string Chromium_ppm { get; set; }

        [JsonProperty("Chromium_ppm_Class", NullValueHandling = NullValueHandling.Ignore)]
        public object Chromium_ppm_Class { get; set; }

        [JsonProperty("sulphur_ppm", NullValueHandling = NullValueHandling.Ignore)]
        public string sulphur_ppm { get; set; }

        [JsonProperty("Sulphur_ppm_Class", NullValueHandling = NullValueHandling.Ignore)]
        public object Sulphur_ppm_Class { get; set; }

        [JsonProperty("E", NullValueHandling = NullValueHandling.Ignore)]
        public E E { get; set; }

        [JsonProperty("gps_tagged_from", NullValueHandling = NullValueHandling.Ignore)]
        public string gps_tagged_from { get; set; }

        [JsonProperty("final_Latitude", NullValueHandling = NullValueHandling.Ignore)]
        public object final_Latitude { get; set; }

        [JsonProperty("final_Longitude", NullValueHandling = NullValueHandling.Ignore)]
        public object final_Longitude { get; set; }

        [JsonProperty("data_source", NullValueHandling = NullValueHandling.Ignore)]
        public string data_source { get; set; }

        [JsonProperty("Crop", NullValueHandling = NullValueHandling.Ignore)]
        public string Crop { get; set; }

        [JsonProperty("Fertilizer_Recommendation", NullValueHandling = NullValueHandling.Ignore)]
        public string Fertilizer_Recommendation { get; set; }
    }

}

using Farmer.Data.API.Utils;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Farmer.Data.API.Models.SoilModels
{
    #region Changed Class
    //[Collection("legacy_data")]
    //public class LegacySoilDataModel
    //{
    //    [Newtonsoft.Json.JsonIgnore]
    //    [JsonProperty("_id")]
    //    [JsonPropertyName("_id")]
    //    [BsonId]
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    //[Newtonsoft.Json.JsonConverter(typeof(ObjectIdConverter))]
    //    public string Id;

    //    //[JsonProperty("id")]
    //    //[JsonPropertyName("id")]
    //    //public string SampleId;

    //    [JsonProperty("Lab_No")]
    //    [JsonPropertyName("Lab_No")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? LabNo;

    //    [JsonProperty("County")]
    //    [JsonPropertyName("County")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? County;

    //    [JsonProperty("Constituency")]
    //    [JsonPropertyName("Constituency")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? Constituency;

    //    [JsonProperty("Ward")]
    //    [JsonPropertyName("Ward")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? Ward;

    //    [JsonProperty("Location")]
    //    [JsonPropertyName("Location")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? Location;

    //    [JsonProperty("Sub_location")]
    //    [JsonPropertyName("Sub_location")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? SubLocation;

    //    [JsonProperty("Village")]
    //    [JsonPropertyName("Village")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? Village;

    //    [JsonProperty("Description_of_the_Farm")]
    //    [JsonPropertyName("Description_of_the_Farm")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? DescriptionOfTheFarm;

    //    [JsonProperty("Latitude")]
    //    [JsonPropertyName("Latitude")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? Latitude;

    //    [JsonProperty("Longitude")]
    //    [JsonPropertyName("Longitude")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? Longitude;

    //    [JsonProperty("Year")]
    //    [JsonPropertyName("Year")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? Year;

    //    [JsonProperty("Farmers_Name")]
    //    [JsonPropertyName("Farmers_Name")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? FarmersName;

    //    [JsonProperty("Telephone_No")]
    //    [JsonPropertyName("Telephone_No")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? TelephoneNo;

    //    [JsonProperty("Years_Under_Cultivation")]
    //    [JsonPropertyName("Years_Under_Cultivation")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? YearsUnderCultivation;

    //    [JsonProperty("Project_Name")]
    //    [JsonPropertyName("Project_Name")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? ProjectName;

    //    [JsonProperty("Sample_description")]
    //    [JsonPropertyName("Sample_description")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? SampleDescription;

    //    [JsonProperty("Soil_depth_cm")]
    //    [JsonPropertyName("Soil_depth_cm")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? SoilDepthCm;

    //    [JsonProperty("Soil_pH")]
    //    [JsonPropertyName("Soil_pH")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? SoilPH;

    //    [JsonProperty("Soil_pH_Class")]
    //    [JsonPropertyName("Soil_pH_Class")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? SoilPHClass;

    //    [JsonProperty("Exch_Acidity_me_percent_")]
    //    [JsonPropertyName("Exch_Acidity_me_percent_")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? ExchAcidityMePercent;

    //    [JsonProperty("Exch_Acidity_Class")]
    //    [JsonPropertyName("Exch_Acidity_Class")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? ExchAcidityClass;

    //    [JsonProperty("Total_Nitrogen_percent_")]
    //    [JsonPropertyName("Total_Nitrogen_percent_")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? TotalNitrogenPercent;

    //    [JsonProperty("Total_Nitrogen_percent_Class")]
    //    [JsonPropertyName("Total_Nitrogen_percent_Class")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? TotalNitrogenPercentClass;

    //    [JsonProperty("Total_Org_Carbon_percent_")]
    //    [JsonPropertyName("Total_Org_Carbon_percent_")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? TotalOrgCarbonPercent;

    //    [JsonProperty("Total_Org_Carbon_percent_Class")]
    //    [JsonPropertyName("Total_Org_Carbon_percent_Class")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? TotalOrgCarbonPercentClass;

    //    [JsonProperty("Phosphorus_Olsen_ppm")]
    //    [JsonPropertyName("Phosphorus_Olsen_ppm")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? PhosphorusOlsenPpm;

    //    [JsonProperty("Phosphorus_Olsen_ppm_Class")]
    //    [JsonPropertyName("Phosphorus_Olsen_ppm_Class")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? PhosphorusOlsenPpmClass;

    //    [JsonProperty("Potassium_meq_percent_")]
    //    [JsonPropertyName("Potassium_meq_percent_")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? PotassiumMeqPercent;

    //    [JsonProperty("Potassium_meq_percent_Class")]
    //    [JsonPropertyName("Potassium_meq_percent_Class")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? PotassiumMeqPercentClass;

    //    [JsonProperty("Calcium_meq_percent_")]
    //    [JsonPropertyName("Calcium_meq_percent_")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? CalciumMeqPercent;

    //    [JsonProperty("Calcium_meq_percent_Class")]
    //    [JsonPropertyName("Calcium_meq_percent_Class")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? CalciumMeqPercentClass;

    //    [JsonProperty("Magnesium_meq_percent_")]
    //    [JsonPropertyName("Magnesium_meq_percent_")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? MagnesiumMeqPercent;

    //    [JsonProperty("Magnesium_meq_percent_Class")]
    //    [JsonPropertyName("Magnesium_meq_percent_Class")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? MagnesiumMeqPercentClass;

    //    [JsonProperty("Manganese_meq_percent_")]
    //    [JsonPropertyName("Manganese_meq_percent_")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? ManganeseMeqPercent;

    //    [JsonProperty("Manganese_meq_percent_Class")]
    //    [JsonPropertyName("Manganese_meq_percent_Class")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? ManganeseMeqPercentClass;

    //    [JsonProperty("Copper_ppm")]
    //    [JsonPropertyName("Copper_ppm")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? CopperPpm;

    //    [JsonProperty("Copper_ppm_Class")]
    //    [JsonPropertyName("Copper_ppm_Class")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? CopperPpmClass;

    //    [JsonProperty("Iron_ppm")]
    //    [JsonPropertyName("Iron_ppm")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? IronPpm;

    //    [JsonProperty("Iron_ppm_Class")]
    //    [JsonPropertyName("Iron_ppm_Class")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? IronPpmClass;

    //    [JsonProperty("Zinc_ppm")]
    //    [JsonPropertyName("Zinc_ppm")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? ZincPpm;

    //    [JsonProperty("Zinc_ppm_Class")]
    //    [JsonPropertyName("Zinc_ppm_Class")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? ZincPpmClass;

    //    [JsonProperty("Sodium_meq_percent_")]
    //    [JsonPropertyName("Sodium_meq_percent_")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? SodiumMeqPercent;

    //    [JsonProperty("Sodium_meq_percent_Class")]
    //    [JsonPropertyName("Sodium_meq_percent_Class")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? SodiumMeqPercentClass;

    //    [JsonProperty("Electr_Conductivity_mS_per_cm")]
    //    [JsonPropertyName("Electr_Conductivity_mS_per_cm")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? ElectrConductivityMSPerCm;

    //    [JsonProperty("Electr_Conductivity_mS_per_cm_Class")]
    //    [JsonPropertyName("Electr_Conductivity_mS_per_cm_Class")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? ElectrConductivityMSPerCmClass;

    //    [JsonProperty("Crop")]
    //    [JsonPropertyName("Crop")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? Crop;

    //    [JsonProperty("Fertilizer_Recommendation")]
    //    [JsonPropertyName("Fertilizer_Recommendation")]
    //    [BsonRepresentation(BsonType.String)]
    //    //[BsonSerializer(typeof(StringOrDoubleSerializer))]
    //    public string? FertilizerRecommendation;
    //}
    #endregion

    #region Changed 2
    [Collection("legacy_data")]
    [BsonIgnoreExtraElements]
    public class LegacySoilDataModel
    {
        [Newtonsoft.Json.JsonIgnore]
        [JsonProperty("_id")]
        [JsonPropertyName("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        //[Newtonsoft.Json.JsonConverter(typeof(ObjectIdConverter))]
        public string Id;

        [JsonProperty("id")]
        [JsonPropertyName("id")]
        [BsonElement("id")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? SampleId;

        [JsonProperty("Lab_No")]
        [JsonPropertyName("Lab_No")]
        [BsonElement("Lab_No")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? LabNo;

        [JsonProperty("County")]
        [JsonPropertyName("County")]
        [BsonElement("County")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? County;

        [JsonProperty("Constituency")]
        [JsonPropertyName("Constituency")]
        [BsonElement("Constituency")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? Constituency;

        [JsonProperty("Ward")]
        [JsonPropertyName("Ward")]
        [BsonElement("Ward")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? Ward;

        [JsonProperty("Location")]
        [JsonPropertyName("Location")]
        [BsonElement("Location")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? Location;

        [JsonProperty("Sub_location")]
        [JsonPropertyName("Sub_location")]
        [BsonElement("Sub_location")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? SubLocation;

        [JsonProperty("Village")]
        [JsonPropertyName("Village")]
        [BsonElement("Village")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? Village;

        [JsonProperty("Description_of_the_Farm")]
        [JsonPropertyName("Description_of_the_Farm")]
        [BsonElement("Description_of_the_Farm")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? DescriptionOfTheFarm;

        [JsonProperty("Latitude")]
        [JsonPropertyName("Latitude")]
        [BsonElement("Latitude")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? Latitude;

        [JsonProperty("Longitude")]
        [JsonPropertyName("Longitude")]
        [BsonElement("Longitude")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? Longitude;

        [JsonProperty("Year")]
        [JsonPropertyName("Year")]
        [BsonElement("Year")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? Year;

        [JsonProperty("Farmers_Name")]
        [JsonPropertyName("Farmers_Name")]
        [BsonElement("Farmers_Name")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? FarmersName;

        [JsonProperty("Telephone_No")]
        [JsonPropertyName("Telephone_No")]
        [BsonElement("Telephone_No")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? TelephoneNo;

        [JsonProperty("Years_Under_Cultivation")]
        [JsonPropertyName("Years_Under_Cultivation")]
        [BsonElement("Years_Under_Cultivation")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? YearsUnderCultivation;

        [JsonProperty("Project_Name")]
        [JsonPropertyName("Project_Name")]
        [BsonElement("Project_Name")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? ProjectName;

        [JsonProperty("Sample_description")]
        [JsonPropertyName("Sample_description")]
        [BsonElement("Sample_description")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? SampleDescription;

        [JsonProperty("Soil_depth_cm")]
        [JsonPropertyName("Soil_depth_cm")]
        [BsonElement("Soil_depth_cm")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? SoilDepthCm;

        [JsonProperty("Soil_pH")]
        [JsonPropertyName("Soil_pH")]
        [BsonElement("Soil_pH")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? SoilPH;

        [JsonProperty("Soil_pH_Class")]
        [JsonPropertyName("Soil_pH_Class")]
        [BsonElement("Soil_pH_Class")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? SoilPHClass;

        [JsonProperty("Exch_Acidity_me_percent_")]
        [JsonPropertyName("Exch_Acidity_me_percent_")]
        [BsonElement("Exch_Acidity_me_percent_")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? ExchAcidityMePercent;

        [JsonProperty("Exch_Acidity_Class")]
        [JsonPropertyName("Exch_Acidity_Class")]
        [BsonElement("Exch_Acidity_Class")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? ExchAcidityClass;

        [JsonProperty("Total_Nitrogen_percent_")]
        [JsonPropertyName("Total_Nitrogen_percent_")]
        [BsonElement("Total_Nitrogen_percent_")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? TotalNitrogenPercent;

        [JsonProperty("Total_Nitrogen_percent_Class")]
        [JsonPropertyName("Total_Nitrogen_percent_Class")]
        [BsonElement("Total_Nitrogen_percent_Class")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? TotalNitrogenPercentClass;

        [JsonProperty("Total_Org_Carbon_percent_")]
        [JsonPropertyName("Total_Org_Carbon_percent_")]
        [BsonElement("Total_Org_Carbon_percent_")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? TotalOrgCarbonPercent;

        [JsonProperty("Total_Org_Carbon_percent_Class")]
        [JsonPropertyName("Total_Org_Carbon_percent_Class")]
        [BsonElement("Total_Org_Carbon_percent_Class")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? TotalOrgCarbonPercentClass;

        [JsonProperty("Phosphorus_Olsen_ppm")]
        [JsonPropertyName("Phosphorus_Olsen_ppm")]
        [BsonElement("Phosphorus_Olsen_ppm")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? PhosphorusOlsenPpm;

        [JsonProperty("Phosphorus_Olsen_ppm_Class")]
        [JsonPropertyName("Phosphorus_Olsen_ppm_Class")]
        [BsonElement("Phosphorus_Olsen_ppm_Class")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? PhosphorusOlsenPpmClass;

        [JsonProperty("Potassium_meq_percent_")]
        [JsonPropertyName("Potassium_meq_percent_")]
        [BsonElement("Potassium_meq_percent_")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? PotassiumMeqPercent;

        [JsonProperty("Potassium_meq_percent_Class")]
        [JsonPropertyName("Potassium_meq_percent_Class")]
        [BsonElement("Potassium_meq_percent_Class")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? PotassiumMeqPercentClass;

        [JsonProperty("Calcium_meq_percent_")]
        [JsonPropertyName("Calcium_meq_percent_")]
        [BsonElement("Calcium_meq_percent_")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? CalciumMeqPercent;

        [JsonProperty("Calcium_meq_percent_Class")]
        [JsonPropertyName("Calcium_meq_percent_Class")]
        [BsonElement("Calcium_meq_percent_Class")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? CalciumMeqPercentClass;

        [JsonProperty("Magnesium_meq_percent_")]
        [JsonPropertyName("Magnesium_meq_percent_")]
        [BsonElement("Magnesium_meq_percent_")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? MagnesiumMeqPercent;

        [JsonProperty("Magnesium_meq_percent_Class")]
        [JsonPropertyName("Magnesium_meq_percent_Class")]
        [BsonElement("Magnesium_meq_percent_Class")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? MagnesiumMeqPercentClass;

        [JsonProperty("Manganese_meq_percent_")]
        [JsonPropertyName("Manganese_meq_percent_")]
        [BsonElement("Manganese_meq_percent_")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? ManganeseMeqPercent;

        [JsonProperty("Manganese_meq_percent_Class")]
        [JsonPropertyName("Manganese_meq_percent_Class")]
        [BsonElement("Manganese_meq_percent_Class")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? ManganeseMeqPercentClass;

        [JsonProperty("Copper_ppm")]
        [JsonPropertyName("Copper_ppm")]
        [BsonElement("Copper_ppm")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? CopperPpm;

        [JsonProperty("Copper_ppm_Class")]
        [JsonPropertyName("Copper_ppm_Class")]
        [BsonElement("Copper_ppm_Class")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? CopperPpmClass;

        [JsonProperty("Iron_ppm")]
        [JsonPropertyName("Iron_ppm")]
        [BsonElement("Iron_ppm")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? IronPpm;

        [JsonProperty("Iron_ppm_Class")]
        [JsonPropertyName("Iron_ppm_Class")]
        [BsonElement("Iron_ppm_Class")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? IronPpmClass;

        [JsonProperty("Zinc_ppm")]
        [JsonPropertyName("Zinc_ppm")]
        [BsonElement("Zinc_ppm")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? ZincPpm;

        [JsonProperty("Zinc_ppm_Class")]
        [JsonPropertyName("Zinc_ppm_Class")]
        [BsonElement("Zinc_ppm_Class")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? ZincPpmClass;

        [JsonProperty("Sodium_meq_percent_")]
        [JsonPropertyName("Sodium_meq_percent_")]
        [BsonElement("Sodium_meq_percent_")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? SodiumMeqPercent;

        [JsonProperty("Sodium_meq_percent_Class")]
        [JsonPropertyName("Sodium_meq_percent_Class")]
        [BsonElement("Sodium_meq_percent_Class")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? SodiumMeqPercentClass;

        [JsonProperty("Electr_Conductivity_mS_per_cm")]
        [JsonPropertyName("Electr_Conductivity_mS_per_cm")]
        [BsonElement("Electr_Conductivity_mS_per_cm")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? ElectrConductivityMSPerCm;

        [JsonProperty("Electr_Conductivity_mS_per_cm_Class")]
        [JsonPropertyName("Electr_Conductivity_mS_per_cm_Class")]
        [BsonElement("Electr_Conductivity_mS_per_cm_Class")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? ElectrConductivityMSPerCmClass;

        [JsonProperty("Crop")]
        [JsonPropertyName("Crop")]
        [BsonElement("Crop")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? Crop;

        [JsonProperty("Fertilizer_Recommendation")]
        [JsonPropertyName("Fertilizer_Recommendation")]
        [BsonElement("Fertilizer_Recommendation")]
        //[BsonRepresentation(BsonType.String)]
        [BsonSerializer(typeof(StringOrDoubleSerializer))]
        public string? FertilizerRecommendation;
    }
    #endregion

    #region Initial Class
    //[Collection("legacy_data")]
    //public class LegacySoilDataModel
    //{
    //    [Newtonsoft.Json.JsonIgnore]
    //    [JsonProperty("_id")]
    //    [JsonPropertyName("_id")]
    //    [BsonId]
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    //[Newtonsoft.Json.JsonConverter(typeof(ObjectIdConverter))]
    //    public string Id;

    //    //[JsonProperty("id")]
    //    //[JsonPropertyName("id")]
    //    //public double SampleId;

    //    [JsonProperty("Lab_No")]
    //    [JsonPropertyName("Lab_No")]
    //    public int LabNo;

    //    [JsonProperty("County")]
    //    [JsonPropertyName("County")]
    //    public string? County;

    //    [JsonProperty("Constituency")]
    //    [JsonPropertyName("Constituency")]
    //    public string? Constituency;

    //    [JsonProperty("Ward")]
    //    [JsonPropertyName("Ward")]
    //    public string? Ward;

    //    [JsonProperty("Location")]
    //    [JsonPropertyName("Location")]
    //    public string? Location;

    //    [JsonProperty("Sub_location")]
    //    [JsonPropertyName("Sub_location")]
    //    public string? SubLocation;

    //    [JsonProperty("Village")]
    //    [JsonPropertyName("Village")]
    //    public string? Village;

    //    [JsonProperty("Description_of_the_Farm")]
    //    [JsonPropertyName("Description_of_the_Farm")]
    //    public string? DescriptionOfTheFarm;

    //    [JsonProperty("Latitude")]
    //    [JsonPropertyName("Latitude")]
    //    public string? Latitude;

    //    [JsonProperty("Longitude")]
    //    [JsonPropertyName("Longitude")]
    //    public string? Longitude;

    //    [JsonProperty("Year")]
    //    [JsonPropertyName("Year")]
    //    public double Year;

    //    [JsonProperty("Farmers_Name")]
    //    [JsonPropertyName("Farmers_Name")]
    //    public string? FarmersName;

    //    [JsonProperty("Telephone_No")]
    //    [JsonPropertyName("Telephone_No")]
    //    public string? TelephoneNo;

    //    [JsonProperty("Years_Under_Cultivation")]
    //    [JsonPropertyName("Years_Under_Cultivation")]
    //    public string? YearsUnderCultivation;

    //    [JsonProperty("Project_Name")]
    //    [JsonPropertyName("Project_Name")]
    //    public string? ProjectName;

    //    [JsonProperty("Sample_description")]
    //    [JsonPropertyName("Sample_description")]
    //    public string? SampleDescription;

    //    [JsonProperty("Soil_depth_cm")]
    //    [JsonPropertyName("Soil_depth_cm")]
    //    public string? SoilDepthCm;

    //    [JsonProperty("Soil_pH")]
    //    [JsonPropertyName("Soil_pH")]
    //    public double SoilPH;

    //    [JsonProperty("Soil_pH_Class")]
    //    [JsonPropertyName("Soil_pH_Class")]
    //    public string? SoilPHClass;

    //    [JsonProperty("Exch_Acidity_me_percent_")]
    //    [JsonPropertyName("Exch_Acidity_me_percent_")]
    //    public string? ExchAcidityMePercent;

    //    [JsonProperty("Exch_Acidity_Class")]
    //    [JsonPropertyName("Exch_Acidity_Class")]
    //    public string? ExchAcidityClass;

    //    [JsonProperty("Total_Nitrogen_percent_")]
    //    [JsonPropertyName("Total_Nitrogen_percent_")]
    //    public double TotalNitrogenPercent;

    //    [JsonProperty("Total_Nitrogen_percent_Class")]
    //    [JsonPropertyName("Total_Nitrogen_percent_Class")]
    //    public string? TotalNitrogenPercentClass;

    //    [JsonProperty("Total_Org_Carbon_percent_")]
    //    [JsonPropertyName("Total_Org_Carbon_percent_")]
    //    public double TotalOrgCarbonPercent;

    //    [JsonProperty("Total_Org_Carbon_percent_Class")]
    //    [JsonPropertyName("Total_Org_Carbon_percent_Class")]
    //    public string? TotalOrgCarbonPercentClass;

    //    [JsonProperty("Phosphorus_Olsen_ppm")]
    //    [JsonPropertyName("Phosphorus_Olsen_ppm")]
    //    public double PhosphorusOlsenPpm;

    //    [JsonProperty("Phosphorus_Olsen_ppm_Class")]
    //    [JsonPropertyName("Phosphorus_Olsen_ppm_Class")]
    //    public string? PhosphorusOlsenPpmClass;

    //    [JsonProperty("Potassium_meq_percent_")]
    //    [JsonPropertyName("Potassium_meq_percent_")]
    //    public double PotassiumMeqPercent;

    //    [JsonProperty("Potassium_meq_percent_Class")]
    //    [JsonPropertyName("Potassium_meq_percent_Class")]
    //    public string? PotassiumMeqPercentClass;

    //    [JsonProperty("Calcium_meq_percent_")]
    //    [JsonPropertyName("Calcium_meq_percent_")]
    //    public double CalciumMeqPercent;

    //    [JsonProperty("Calcium_meq_percent_Class")]
    //    [JsonPropertyName("Calcium_meq_percent_Class")]
    //    public string? CalciumMeqPercentClass;

    //    [JsonProperty("Magnesium_meq_percent_")]
    //    [JsonPropertyName("Magnesium_meq_percent_")]
    //    public double MagnesiumMeqPercent;

    //    [JsonProperty("Magnesium_meq_percent_Class")]
    //    [JsonPropertyName("Magnesium_meq_percent_Class")]
    //    public string? MagnesiumMeqPercentClass;

    //    [JsonProperty("Manganese_meq_percent_")]
    //    [JsonPropertyName("Manganese_meq_percent_")]
    //    public string? ManganeseMeqPercent;

    //    [JsonProperty("Manganese_meq_percent_Class")]
    //    [JsonPropertyName("Manganese_meq_percent_Class")]
    //    public string? ManganeseMeqPercentClass;

    //    [JsonProperty("Copper_ppm")]
    //    [JsonPropertyName("Copper_ppm")]
    //    public double CopperPpm;

    //    [JsonProperty("Copper_ppm_Class")]
    //    [JsonPropertyName("Copper_ppm_Class")]
    //    public string? CopperPpmClass;

    //    [JsonProperty("Iron_ppm")]
    //    [JsonPropertyName("Iron_ppm")]
    //    public double IronPpm;

    //    [JsonProperty("Iron_ppm_Class")]
    //    [JsonPropertyName("Iron_ppm_Class")]
    //    public string? IronPpmClass;

    //    [JsonProperty("Zinc_ppm")]
    //    [JsonPropertyName("Zinc_ppm")]
    //    public double ZincPpm;

    //    [JsonProperty("Zinc_ppm_Class")]
    //    [JsonPropertyName("Zinc_ppm_Class")]
    //    public string? ZincPpmClass;

    //    [JsonProperty("Sodium_meq_percent_")]
    //    [JsonPropertyName("Sodium_meq_percent_")]
    //    public string? SodiumMeqPercent;

    //    [JsonProperty("Sodium_meq_percent_Class")]
    //    [JsonPropertyName("Sodium_meq_percent_Class")]
    //    public string? SodiumMeqPercentClass;

    //    [JsonProperty("Electr_Conductivity_mS_per_cm")]
    //    [JsonPropertyName("Electr_Conductivity_mS_per_cm")]
    //    public string? ElectrConductivityMSPerCm;

    //    [JsonProperty("Electr_Conductivity_mS_per_cm_Class")]
    //    [JsonPropertyName("Electr_Conductivity_mS_per_cm_Class")]
    //    public string? ElectrConductivityMSPerCmClass;

    //    [JsonProperty("Crop")]
    //    [JsonPropertyName("Crop")]
    //    public string? Crop;

    //    [JsonProperty("Fertilizer_Recommendation")]
    //    [JsonPropertyName("Fertilizer_Recommendation")]
    //    public string? FertilizerRecommendation;
    //}
    #endregion
}

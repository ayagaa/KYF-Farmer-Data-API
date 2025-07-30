using Farmer.Data.API.Models.SoilModels;
using Farmer.Data.API.Utils;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.Serialization;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Farmer.Data.API.Databases
{
    public class LegacySoilDataDbContext : DbContext
    {
        public DbSet<LegacySoilDataModel> LegacySoilData { get; init; }

        public LegacySoilDataDbContext(DbContextOptions options) : base(options)
        {
            BsonClassMap.RegisterClassMap<LegacySoilDataModel>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.CalciumMeqPercent).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.CalciumMeqPercentClass).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.CopperPpm).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.CopperPpmClass).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.ElectrConductivityMSPerCm).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.ElectrConductivityMSPerCmClass).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.ExchAcidityClass).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.ExchAcidityMePercent).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.IronPpm).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.IronPpmClass).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.LabNo).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.Latitude).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.Longitude).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.MagnesiumMeqPercent).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.MagnesiumMeqPercentClass).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.ManganeseMeqPercent).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.ManganeseMeqPercentClass).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.PhosphorusOlsenPpm).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.PhosphorusOlsenPpmClass).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.PotassiumMeqPercent).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.PotassiumMeqPercentClass).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.SodiumMeqPercent).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.SodiumMeqPercentClass).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.SoilDepthCm).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.SoilPH).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.SoilPHClass).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.TelephoneNo).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.TotalNitrogenPercent).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.TotalNitrogenPercentClass).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.TotalOrgCarbonPercent).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.TotalOrgCarbonPercentClass).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.ZincPpm).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.ZincPpmClass).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.Year).SetSerializer(new StringOrDoubleSerializer());
                cm.MapMember(c => c.YearsUnderCultivation).SetSerializer(new StringOrDoubleSerializer());
            });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LegacySoilDataModel>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasElementName("_id");
                //entity.Property(e => e.SampleId).HasElementName("id");
                entity.Property(e => e.LabNo).HasElementName("Lab_No");
                entity.Property(e => e.County).HasElementName("County");
                entity.Property(e => e.Constituency).HasElementName("Constituency");
                entity.Property(e => e.Ward).HasElementName("Ward");
                entity.Property(e => e.Location).HasElementName("Location");
                entity.Property(e => e.SubLocation).HasElementName("Sub_location");
                entity.Property(e => e.Village).HasElementName("Village");
                entity.Property(e => e.DescriptionOfTheFarm).HasElementName("Description_of_the_Farm");
                entity.Property(e => e.Latitude).HasElementName("Latitude");
                entity.Property(e => e.Longitude).HasElementName("Longitude");
                entity.Property(e => e.Year).HasElementName("Year");
                entity.Property(e => e.FarmersName).HasElementName("Farmers_Name");
                entity.Property(e => e.TelephoneNo).HasElementName("Telephone_No");
                entity.Property(e => e.YearsUnderCultivation).HasElementName("Years_Under_Cultivation");
                entity.Property(e => e.ProjectName).HasElementName("Project_Name");
                entity.Property(e => e.SampleDescription).HasElementName("Sample_description");
                entity.Property(e => e.SoilDepthCm).HasElementName("Soil_depth_cm");
                entity.Property(e => e.SoilPH).HasElementName("Soil_pH");
                entity.Property(e => e.SoilPHClass).HasElementName("Soil_pH_Class");
                entity.Property(e => e.ExchAcidityMePercent).HasElementName("Exch_Acidity_me_percent_");
                entity.Property(e => e.ExchAcidityClass).HasElementName("Exch_Acidity_Class");
                entity.Property(e => e.TotalNitrogenPercent).HasElementName("Total_Nitrogen_percent_");
                entity.Property(e => e.TotalNitrogenPercentClass).HasElementName("Total_Nitrogen_percent_Class");
                entity.Property(e => e.TotalOrgCarbonPercent).HasElementName("Total_Org_Carbon_percent_");
                entity.Property(e => e.TotalOrgCarbonPercentClass).HasElementName("Total_Org_Carbon_percent_Class");
                entity.Property(e => e.PhosphorusOlsenPpm).HasElementName("Phosphorus_Olsen_ppm");
                entity.Property(e => e.PhosphorusOlsenPpmClass).HasElementName("Phosphorus_Olsen_ppm_Class");
                entity.Property(e => e.PotassiumMeqPercent).HasElementName("Potassium_meq_percent_");
                entity.Property(e => e.PotassiumMeqPercentClass).HasElementName("Potassium_meq_percent_Class");
                entity.Property(e => e.CalciumMeqPercent).HasElementName("Calcium_meq_percent_");
                entity.Property(e => e.CalciumMeqPercentClass).HasElementName("Calcium_meq_percent_Class");
                entity.Property(e => e.MagnesiumMeqPercent).HasElementName("Magnesium_meq_percent_");
                entity.Property(e => e.MagnesiumMeqPercentClass).HasElementName("Magnesium_meq_percent_Class");
                entity.Property(e => e.CopperPpm).HasElementName("Copper_ppm");
                entity.Property(e => e.CalciumMeqPercentClass).HasElementName("Copper_ppm_Class");
                entity.Property(e => e.IronPpm).HasElementName("Iron_ppm");
                entity.Property(e => e.IronPpmClass).HasElementName("Iron_ppm_Class");
                entity.Property(e => e.ZincPpm).HasElementName("Zinc_ppm");
                entity.Property(e => e.ZincPpmClass).HasElementName("Zinc_ppm_Class");
                entity.Property(e => e.SodiumMeqPercent).HasElementName("Sodium_meq_percent_");
                entity.Property(e => e.SodiumMeqPercentClass).HasElementName("Sodium_meq_percent_Class");
                entity.Property(e => e.ElectrConductivityMSPerCm).HasElementName("Electr_Conductivity_mS_per_cm");
                entity.Property(e => e.ElectrConductivityMSPerCmClass).HasElementName("Electr_Conductivity_mS_per_cm_Class");
                entity.Property(e => e.Crop).HasElementName("Crop");
                entity.Property(e => e.FertilizerRecommendation).HasElementName("Fertilizer_Recommendation");

                entity.ToCollection("legacy_data");
                //entity.ToJson();
            });

            modelBuilder.Entity<LegacySoilDataModel>();
        }
    }
}

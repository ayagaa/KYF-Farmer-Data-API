using CsvHelper.Configuration.Attributes;

namespace Farmer.Data.API.Models
{
    public class CropTreeConversionFactorModel
    {
        [Name("crop_name")]
        public string CropName { get; set; }

        [Name("convertion_factor_units_per_acre")]
        public float ConverstionFactor { get; set; }
    }
}

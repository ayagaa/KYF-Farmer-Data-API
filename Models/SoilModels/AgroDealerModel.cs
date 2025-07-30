using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;

namespace Farmer.Data.API.Models.SoilModels
{
    public class AgroDealerModel
    {
        [Name("county")]
        [JsonProperty("county")]
        public string County { get; set; }

        [Name("subcounty")]
        [JsonProperty("subcounty")]
        public string Subcounty { get; set; }

        [Name("ward")]
        [JsonProperty("ward")]
        public string Ward { get; set; }

        [Name("agrodealer_name")]
        [JsonProperty("agroDealerName")]
        public string AgrodealerName { get; set; }

        [Name("market")]
        [JsonProperty("market")]
        public string Market { get; set; }

        [Name("gps_latitude")]
        [JsonProperty("latitude")]
        public string GpsLatitude { get; set; }

        [Name("gps_longitude")]
        [JsonProperty("longitude")]
        public string GpsLongitude { get; set; }

        [Name("gps_altitude")]
        [JsonProperty("altitude")]
        public string GpsAltitude { get; set; }

        [Name("agrodealer_phone")]
        [JsonProperty("agroDealerPhone")]
        public string AgrodealerPhone { get; set; }
    }
}

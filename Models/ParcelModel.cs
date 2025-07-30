using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;

namespace Farmer.Data.API.Models
{
    public class ParcelModel
    {
        [JsonProperty("parcelAcreage")]
        public string Acreage { get; set; }

        [JsonProperty("parcelOwnshipStatus")]
        public string OwnershipStatus { get; set; } = "Owned by self";
    }

    public class ParcelExract : ParcelModel
    {
        [Name("parcelId")]
        [JsonProperty("parcel_id")]
        public string ParcelId { get; set; }
    }
}

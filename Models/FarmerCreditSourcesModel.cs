using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Farmer.Data.API.Models
{
    public class FarmerCreditSourcesModel
    {
        [JsonProperty("_id")]
        [JsonPropertyName("_id")]
        public string Id;

        [JsonProperty("farmer_id")]
        [JsonPropertyName("farmer_id")]
        public int FarmerId;

        [JsonProperty("credit_source_id")]
        [JsonPropertyName("credit_source_id")]
        public int CreditSourceId;

        [JsonProperty("credit_source")]
        [JsonPropertyName("credit_source")]
        public string CreditSource;
    }
}

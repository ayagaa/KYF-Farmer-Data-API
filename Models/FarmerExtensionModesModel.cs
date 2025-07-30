using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Farmer.Data.API.Models
{
    public class FarmerExtensionModesModel
    {
        [JsonProperty("_id")]
        [JsonPropertyName("_id")]
        public string Id;

        [JsonProperty("farmer_id")]
        [JsonPropertyName("farmer_id")]
        public int FarmerId;

        [JsonProperty("extension_mode_id")]
        [JsonPropertyName("extension_mode_id")]
        public int ExtensionModeId;

        [JsonProperty("source_mode")]
        [JsonPropertyName("source_mode")]
        public string SourceMode;
    }
}

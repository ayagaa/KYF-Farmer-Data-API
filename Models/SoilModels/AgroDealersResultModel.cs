using Newtonsoft.Json;

namespace Farmer.Data.API.Models.SoilModels
{
    public class AgroDealersResultModel
    {
        [JsonProperty("county")]
        public string county { get; set; }

        [JsonProperty("subcounty")]
        public string subcounty { get; set; }

        [JsonProperty("ward")]
        public string ward { get; set; }

        [JsonProperty("agroDealers")]
        public List<AgroDealerModel> dealers { get; set; } = new List<AgroDealerModel>();
    }
}

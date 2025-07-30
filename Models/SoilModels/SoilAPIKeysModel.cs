using CsvHelper.Configuration.Attributes;

namespace Farmer.Data.API.Models.SoilModels
{
    public class SoilAPIKeysModel
    {

        [Name("ORGANIZATION")]
        public string Organization { get; set; }

        [Name("API_KEY")]
        public string Key { get; set; }

    }
}

using CsvHelper.Configuration.Attributes;

namespace Farmer.Data.API.Models.MBTileModels
{
    public class MbTilesRawData
    {
        [Name("Name")]
        public string Name { get; set; }

        [Name("Bounds")]
        public string Bounds { get; set; }

        [Name("Latitude")]
        public float Latitude { get; set; }

        [Name("Longitude")]
        public float Longitude { get; set; }
    }
}

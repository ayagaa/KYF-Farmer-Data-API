using Farmer.Data.API.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

namespace Farmer.Data.API.Utils
{
    public static class GeoJson
    {
        static string inputAPZoneFolder = @"/home/kproxy2/soilMap/GEOJSON"; // Folder to pick Geojson files from
        static string inputAPZoneFile = @"/home/kproxy2/soilMap/AP-Zones.csv";
        public static void PopulateGeoJsonDictionary(ref Dictionary<string, string> geoJsonDictionary)
        {
            var apZones = Csv.ReadRootObjectFile<APZoneModel>(inputAPZoneFile);

            var directory = new DirectoryInfo(inputAPZoneFolder);

            if (directory.Exists)
            {
                var geojsonFiles = directory.GetFiles("*.geojson");

                var apZonesBagDictionary = new ConcurrentDictionary<string, string>();

                Parallel.ForEach(apZones, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, apZone =>
                {
                    if (!string.IsNullOrEmpty(apZone.AgpzoneCode))
                    {
                        var zoneGeojsonFile = geojsonFiles.FirstOrDefault(zf => zf.Name.StartsWith(apZone.AgpzoneCode, StringComparison.InvariantCultureIgnoreCase));

                        if (zoneGeojsonFile != null)
                        {
                            try
                            {
                                var zoneGeoJsonString = File.ReadAllText(zoneGeojsonFile.FullName);
                                //apZonesBagDictionary.TryAdd(apZone.AgpzoneCode, zoneGeoJsonString);
                                var geoJson = JObject.Parse(zoneGeoJsonString);

                                //var geoJson = zoneGeoJsonString;

                                if (geoJson != null)
                                {
                                    var geoJsonString = geoJson.ToString(Newtonsoft.Json.Formatting.Indented);
                                    //var geoJsonString = Regex.Replace(geoJson.ToString(), @"[\r\n]", "");
                                    apZonesBagDictionary.TryAdd(apZone.AgpzoneCode, geoJsonString);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error getting geojson from file: {zoneGeojsonFile.Name}\n{ex.Message}");
                            }
                        }
                    }

                });

                geoJsonDictionary = apZonesBagDictionary.ToDictionary();
            }
            else
            {
                Console.WriteLine($"Folder not found: {inputAPZoneFolder}");
            }

        }
    }
}

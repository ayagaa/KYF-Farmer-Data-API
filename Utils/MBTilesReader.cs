namespace Farmer.Data.API.Utils
{
    public static class MBTilesReader
    {
        public static void GetTileData(string filePath)
        {
            //var connString = $"Data Source={filePath};Mode=ReadOnly;Version=3";

            string connString = $"Data Source={filePath};Version=3;";




            //var mbTileSource = new MbTilesTileSource(filePath, false);

            //var metaData = mbTileSource.Json;

            //Console.WriteLine($"Data: {0}", metaData);
        }
    }
}

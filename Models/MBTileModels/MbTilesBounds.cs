namespace Farmer.Data.API.Models.MBTileModels
{
    public class MbTilesBounds
    {
        public Coordinates BottomLeftCorner { get; set; }
        public Coordinates TopRightCorner { get; set; }
    }

    public class Coordinates
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}

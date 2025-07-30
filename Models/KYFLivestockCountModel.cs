namespace Farmer.Data.API.Models
{
    public class KYFLivestockCountModel
    {
        public string Category { get; set; }  // e.g. "Cattle"
        public string SubCategory { get; set; }  // e.g. "Indigenous"
        public string Breed { get; set; }  // e.g. "Zebu"
        //public string Gender { get; set; }  // e.g. "male"
        public string LivestockDetail { get; set; }  // e.g. "adult"
        public int Count { get; set; }  // e.g. 5

        public string ProductionSystem { get; set; }

        public string BreedingSystem { get; set; }
    }
}

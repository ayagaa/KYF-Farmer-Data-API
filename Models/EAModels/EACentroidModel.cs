using CsvHelper.Configuration;

namespace Farmer.Data.API.Models.EAModels
{
    public class EACentroidModel
    {
        public int Sn { get; set; }
        public string CouName { get; set; }
        public string SCouName { get; set; }
        public string DivName { get; set; }
        public string LocName { get; set; }
        public string SLName { get; set; }
        public string EAName { get; set; }
        public int Num { get; set; }
        public int ConstCode { get; set; }
        public string ConstName { get; set; }
        public int WardCode { get; set; }
        public string WardName { get; set; }
        public long EACodeFull { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class ModelClassMap : ClassMap<EACentroidModel>
    {
        public ModelClassMap()
        {
            Map(m => m.Sn).Name("Sn");
            Map(m => m.CouName).Name("CouName");
            Map(m => m.SCouName).Name("SCouName");
            Map(m => m.DivName).Name("DivName");
            Map(m => m.LocName).Name("LocName");
            Map(m => m.SLName).Name("SLName");
            Map(m => m.EAName).Name("EAName");
            Map(m => m.Num).Name("Num");
            Map(m => m.ConstCode).Name("ConstCode");
            Map(m => m.ConstName).Name("ConstName");
            Map(m => m.WardCode).Name("WardCode");
            Map(m => m.WardName).Name("WardName");
            Map(m => m.EACodeFull).Name("EACodeFull");
            Map(m => m.Latitude).Name("Latitude");
            Map(m => m.Longitude).Name("Longitude");
        }
    }
}

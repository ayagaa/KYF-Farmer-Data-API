using CsvHelper.Configuration.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace Farmer.Data.API.Models
{
    public class APZoneModel
    {
        [Name("county_id")]
        public string CountyId { get; set; }

        [Name("county_name")]
        public string CountyName { get; set; }

        [Name("subcounty_id")]
        public string SubcountyId { get; set; }

        [Name("subcounty_name")]
        public string SubcountyName { get; set; }

        [Name("ward_name")]
        public string WardName { get; set; }

        [Name("ward_id")]
        public string WardId { get; set; }

        [Name("agpzone_name")]
        public string AgpzoneName { get; set; }

        [Name("agpzone_code")]
        public string AgpzoneCode { get; set; }

        [Ignore]
        [Name("latitide")]
        public string Latitude { get; set; }

        [Ignore]
        [Name("longitude")]
        public string Longitude { get; set; }
    }

    public class APZoneAdminComparer : IEqualityComparer<APZoneModel>
    {
        public bool Equals(APZoneModel? x, APZoneModel? y)
        {
            return (x.CountyId == y.CountyId && x.SubcountyId == y.SubcountyId && x.WardId == y.WardId);
        }

        public int GetHashCode([DisallowNull] APZoneModel obj)
        {
            return (obj.CountyId.ToString() + obj.SubcountyId.ToString() + obj.WardId.ToString()).GetHashCode();
        }
    }
}

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Farmer.Data.API.Models
{
    public class KYFQueryAdminModel
    {

        [JsonPropertyName("county")]
        public string County { get; set; }

        [JsonPropertyName("subcounty")]
        public string Subcounty { get; set; }

        [JsonPropertyName("ward")]
        public string Ward { get; set; }

        public override string ToString()
        {
            return County + Subcounty + Ward;
        }

        public override int GetHashCode()
        {
            return (County.ToUpper() + Subcounty.ToUpper() + Ward.ToUpper()).GetHashCode();
        }
    }

    public class KYFQueryAdminComparer : IEqualityComparer<KYFQueryAdminModel>
    {
        public bool Equals(KYFQueryAdminModel? x, KYFQueryAdminModel? y)
        {
            //return (x.County.ToLower().Trim()
            //    + x.Subcounty.ToLower().Trim()
            //    + x.Ward.ToLower().Trim()).GetHashCode() ==
            //    (y.County.ToLower().Trim()
            //    + y.Subcounty.ToLower().Trim().GetHashCode()
            //    + y.Ward.ToLower().Trim()).GetHashCode();

            return x.County.ToLower().Trim() == y.County.ToLower().Trim()
                && x.Subcounty.ToLower().Trim() == y.Subcounty.ToLower().Trim()
                && x.Ward.ToLower().Trim() == y.Ward.ToLower().Trim();
        }

        public int GetHashCode([DisallowNull] KYFQueryAdminModel obj)
        {
            return obj.GetHashCode(); ;
        }
    }


}

using Farmer.Data.API.Models;
using Farmer.Data.API.Models.SoilModels;
using Microsoft.Extensions.Caching.Memory;

namespace Farmer.Data.API.Utils
{
    public static class APZoneCache
    {
        public static IMemoryCache APZoneMemoryCache;

        public static Dictionary<string, string> APZoneMaps = new Dictionary<string, string>();

        public static List<AgroDealerModel> AgroDealers = new List<AgroDealerModel>();

        public static List<KYFQueryAdminModel> APZones = new List<KYFQueryAdminModel>();

        public static List<CropTreeConversionFactorModel> CropTreeConversionFactors = new List<CropTreeConversionFactorModel>();
    }
}

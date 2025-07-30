using Farmer.Data.API.Models;
using Farmer.Data.API.Models.SoilModels;
using Farmer.Data.API.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;

namespace Farmer.Data.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SoilDataController : ControllerBase
    {
        //[HttpGet("legacy/all")]
        //public async Task<ActionResult<List<LegacySoilDataModel>>> GetAllLegacySoilData()
        //{
        //    var data = await DataAccess.GetLegacySoilData();
        //    return Ok(data);
        //}

        private bool IsAuthorized(string authToken)
        {
            return DataAccess.SoilAPIKeys.ContainsKey(authToken);
            //return !string.IsNullOrEmpty(authToken) &&
            //       _apiKey.Equals(authToken, StringComparison.OrdinalIgnoreCase);
        }

        private readonly string _apiKey;

        public SoilDataController(IMemoryCache memoryCache)
        {
            KYFFarmerDataCache.KyfFarmerMemoryCache = memoryCache;

            var credentials = new CredentialsManager.Supplier("Credentials.json");

            _apiKey = credentials.GetSecret("APIKeys:KYFMaps");
        }


        [HttpGet("legacy/all")]
        [APIKeyLogRequest]
        public async Task<ActionResult<string>> GetAllLegacySoilData()
        {
            var authToken = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).Parameter;
            if (IsAuthorized(authToken))
            {
                var data = await DataAccess.GetLegacySoilData();
                if (!string.IsNullOrEmpty(data))
                {
                    var resultData = Newtonsoft.Json.JsonConvert.DeserializeObject<LegacySoilTestDataModel[]>(data);
                    return Ok(resultData);
                }
                return NotFound($"No result found");
            }
            return BadRequest();
        }


        [HttpGet("legacy/sample/{labNo}")]
        [APIKeyLogRequest]
        public async Task<ActionResult<string>> GetLegacySoilDataByLabNo(string labNo)
        {
            var authToken = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).Parameter;
            if (IsAuthorized(authToken))
            {
                var data = await DataAccess.GetLegacySoilDataByLab(labNo);
                if (!string.IsNullOrEmpty(data))
                {
                    var resultData = Newtonsoft.Json.JsonConvert.DeserializeObject<LegacySoilTestDataModel[]>(data);
                    return Ok(resultData);
                }
                return NotFound($"No result found for lab {labNo}");
            }
            return BadRequest();
        }

        [HttpGet("legacy/county/{county}")]
        [APIKeyLogRequest]
        public async Task<ActionResult<string>> GetLegacySoilDataByCounty(string county)
        {
            var authToken = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).Parameter;
            if (IsAuthorized(authToken))
            {
                var dataKey = county.GetHashCode();
                var data = new List<LegacySoilDataModel>();

                if (KYFFarmerDataCache.KyfFarmerMemoryCache.TryGetValue(dataKey, out data))
                {
                    return Ok(data);
                }
                else
                {
                    var dataString = await DataAccess.GetLegacySoilDataByCounty(county);
                    if (!string.IsNullOrEmpty(dataString))
                    {
                        var resultData = Newtonsoft.Json.JsonConvert.DeserializeObject<LegacySoilTestDataModel[]>(dataString);
                        KYFFarmerDataCache.KyfFarmerMemoryCache.Set(dataKey, resultData?.ToList());
                        return Ok(resultData);
                    }
                }


                return NotFound($"No result found for county {county}");
            }
            return BadRequest();
        }

        [HttpGet("agrodealers/{county}")]
        [APIKeyLogRequest]
        public async Task<ActionResult> GetAgroDealers(string county)
        {
            var authToken = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).Parameter;
            if (IsAuthorized(authToken))
            {
                var agrodealers = APZoneCache.AgroDealers.Where(ag => ag.County.Trim().ToLower() == county.Trim().ToLower())?.ToList();

                if (agrodealers != null && agrodealers.Any())
                {
                    var result = new AgroDealersResultModel
                    {
                        county = county,
                        dealers = agrodealers
                    };

                    return Ok(result);
                }

                return NotFound($"No result found for county {county}");
            }
            return BadRequest();
        }

        [HttpGet("agrodealers/{county}/{subcounty}/{ward}")]
        [APIKeyLogRequest]
        public async Task<ActionResult> GetAgroDealers(string county, string subcounty, string ward)
        {
            var authToken = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).Parameter;
            if (IsAuthorized(authToken))
            {
                var agrodealers = APZoneCache.AgroDealers.Where(ag => ag.County.Trim().ToLower() == county.Trim().ToLower()
                && ag.Subcounty.Trim().ToLower() == subcounty.Trim().ToLower()
                && ag.Ward.Trim().ToLower() == ward.Trim().ToLower())?.ToList();

                if (agrodealers != null && agrodealers.Any())
                {
                    var result = new AgroDealersResultModel
                    {
                        county = county,
                        dealers = agrodealers
                    };

                    return Ok(result);
                }

                return NotFound($"No result found for county {county} subcounty {subcounty} ward {ward}");
            }
            return BadRequest();
        }

    }
}

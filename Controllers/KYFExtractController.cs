using Farmer.Data.API.Models;
using Farmer.Data.API.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Net.Http.Headers;

namespace Farmer.Data.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KYFExtractController : ControllerBase
    {
        private bool IsAuthorized(string authToken)
        {

            return !string.IsNullOrEmpty(authToken) &&
                   _apiKey.Equals(authToken, StringComparison.OrdinalIgnoreCase);
        }

        private readonly string _apiKey;

        private readonly IUriService _uriService;
        //private readonly IMemoryCache _kyfFarmerDataMemoryCache;
        public KYFExtractController(IMemoryCache memoryCache, IUriService uriService)
        {
            KYFFarmerDataCache.KyfFarmerMemoryCache = memoryCache;
            _uriService = uriService;

            var credentials = new CredentialsManager.Supplier("Credentials.json");
            _apiKey = credentials.GetSecret("APIKeys:KYFEvoucher");
        }

        [HttpPost("farmers")]
        public async Task<IActionResult> GetWardFarmers([FromBody] KYFQueryAdminModel admin,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var authToken = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).Parameter;
            if (IsAuthorized(authToken))
            {
                var dataKey = admin.GetHashCode();
                var data = new List<KYFFarmerDetailModel>();
                if (KYFFarmerDataCache.KyfFarmerMemoryCache.TryGetValue(dataKey, out data))
                {
                    if (data != null && data.Any())
                    {
                        if (pageNumber > 0 && pageSize > 0)
                        {
                            var route = Request.Path.Value;
                            var validFilter = new PaginationFilterModel(pageNumber, pageSize);

                            var pagedResponse = Helpers.GeneratePagedResponse<KYFFarmerDetailModel>(data.OrderBy(d => d.NationalID).ToList(), validFilter, _uriService, route);
                            return Ok(pagedResponse);
                        }

                        return Ok(data.OrderBy(d => d.NationalID).ToList());
                    }
                }
                else
                {
                    data = await DataAccess.GetKYFWardFarmerDetails(admin.County.ToUpper(), admin.Subcounty.ToUpper(), admin.Ward.ToUpper());

                    if (data != null && data.Any())
                    {
                        KYFFarmerDataCache.KyfFarmerMemoryCache.Set(dataKey, data);
                        if (pageNumber > 0 && pageSize > 0)
                        {
                            var route = Request.Path.Value;
                            var validFilter = new PaginationFilterModel(pageNumber, pageSize);

                            var pagedResponse = Helpers.GeneratePagedResponse<KYFFarmerDetailModel>(data.OrderBy(d => d.NationalID).ToList(), validFilter, _uriService, route);
                            return Ok(pagedResponse);
                        }

                        return Ok(data.OrderBy(d => d.NationalID).ToList());
                    }
                }


                return NotFound($"No new or updated farmer for County: {admin.County}; Subcounty:{admin.Subcounty}; Ward: {admin.Ward}");
            }
            else
            {
                return BadRequest("The token provided is not authorized");
            }
            return BadRequest("The request made was contrary to API documentation. Please review the documentation and implement accordingly.");
        }


        [HttpGet("countyfarmers/crops/{county}")]
        public async Task<IActionResult> GetCountyCropFarmers(string county)
        {
            var authToken = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).Parameter;
            if (IsAuthorized(authToken))
            {
                var dataKey = county.GetHashCode();
                var data = new List<KYFAPExtractModel>();
                data = await DataAccess.GetKYFCountyExtract(county, KYFFarmerDataCache.KyfFarmerMemoryCache);

                if (data != null && data.Any())
                {
                    if (data != null && data.Any())
                    {
                        Console.WriteLine($"Generating crops table for county: {county}");
                        using var cropsTable = await DataAccess.CropsDataFormatter(data);
                        cropsTable.ToCSV($"{county}_crops_export.csv");
                        return Ok($"File {county}_crops_export.csv has been written to the server");
                    }
                }


                return NotFound($"No new or updated farmer for County: {county.ToUpper()};");
            }
            else
            {
                return BadRequest("The token provided is not authorized");
            }
            return BadRequest("The request made was contrary to API documentation. Please review the documentation and implement accordingly.");
        }

        [HttpGet("countyfarmers/livestock/{county}")]
        public async Task<IActionResult> GetCountyLivestockFarmers(string county)
        {
            var authToken = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).Parameter;
            if (IsAuthorized(authToken))
            {
                var dataKey = county.GetHashCode();
                var data = new List<KYFAPExtractModel>();
                data = await DataAccess.GetKYFCountyExtract(county, KYFFarmerDataCache.KyfFarmerMemoryCache);

                if (data != null && data.Any())
                {

                    if (data != null && data.Any())
                    {
                        Console.WriteLine($"Generating livestock table for county: {county}");
                        var livestockTable = await DataAccess.LivestockDataFormatter(data);
                        Console.WriteLine($"Generating livestock table for county: {county}");
                        livestockTable.ToCSV($"{county}_livestock_export.csv");
                        Console.WriteLine($"File {county}_livestock_export.csv has been written to the server");
                        return Ok($"File {county}_livestock_export.csv has been written to the server");
                    }
                }


                return NotFound($"No new or updated farmer for County: {county.ToUpper()};");
            }
            else
            {
                return BadRequest("The token provided is not authorized");
            }
            return BadRequest("The request made was contrary to API documentation. Please review the documentation and implement accordingly.");
        }

        [HttpGet("countyfarmers/all/{county}")]
        public async Task<IActionResult> GetCountyCropsLivestockFarmers(string county)
        {
            var authToken = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).Parameter;
            if (IsAuthorized(authToken))
            {
                var dataKey = county.GetHashCode();
                var data = new List<KYFAPExtractModel>();
                data = await DataAccess.GetKYFCountyExtract(county, KYFFarmerDataCache.KyfFarmerMemoryCache);

                if (data != null && data.Any())
                {

                    if (data != null && data.Any())
                    {
                        //Console.WriteLine($"Generating livestock table for county: {county}");
                        //var livestockTable = await DataAccess.LivestockDataFormatter(data);
                        //Console.WriteLine($"Writing livestock table for county: {county}");
                        //livestockTable.DataTableToCsv($"{county}_livestock_TODAY_export.csv");

                        //Console.WriteLine($"Generating crops table for county: {county}");
                        //using var cropsTable = await DataAccess.CropsDataFormatter(data);
                        //Console.WriteLine($"Writing crops table for county: {county}");
                        //cropsTable.DataTableToCsv($"{county}_crops_TODAY_export.csv");

                        var output = JsonConvert.SerializeObject(data, Formatting.Indented);
                        System.IO.File.WriteAllText($"{county}_crops_livestock.json", output);

                        //await DataAccess.SendAtlert($"File {county}_livestock_TODAY_export.csv and {county}_crops_TODAY_export.csv have been written to the server", county);

                        await DataAccess.SendAtlert($"File {county}_crops_livestock.json has been written to the server", county);

                        //return Ok($"File {county}_livestock_TODAY_export.csv and {county}_crops_TODAY_export.csv have been written to the server");

                        return Ok($"File {county}_crops_livestock.json has been written to the server");
                    }
                }


                return NotFound($"No new or updated farmer for County: {county.ToUpper()};");
            }
            else
            {
                return BadRequest("The token provided is not authorized");
            }
            return BadRequest("The request made was contrary to API documentation. Please review the documentation and implement accordingly.");
        }

        [HttpGet("countyfarmers/bio/{county}")]
        public async Task<IActionResult> GetCountyBioData(string county)
        {
            var authToken = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).Parameter;
            if (IsAuthorized(authToken))
            {
                var dataKey = county.GetHashCode();
                var data = new List<KYFAPExtractModel>();
                data = await DataAccess.GetKYFCountyExtractBio(county);

                if (data != null && data.Any())
                {

                    if (data != null && data.Any())
                    {
                        //Console.WriteLine($"Generating livestock table for county: {county}");
                        //var livestockTable = await DataAccess.LivestockDataFormatter(data);
                        Console.WriteLine($"Generating farmer bio table for county: {county}");
                        using var farmerBioTable = await DataAccess.FarmerBiosFormatter(data);
                        //Console.WriteLine($"Writing: {livestockTable.Rows.Count} records for county: {county}");
                        //livestockTable.ToCSV($"{county}_livestock_export.csv");
                        //Console.WriteLine($"Completed writing: {livestockTable.Rows.Count} records for county: {county}");
                        farmerBioTable.ToCSV($"{county}_farmer_bio_export.csv");
                        //return Ok($"File {county}_livestock_export.csv has been written to the server");
                        return Ok($"File {county}_farmer_bio_export.csv has been written to the server");
                    }
                }


                return NotFound($"No new or updated farmer for County: {county.ToUpper()};");
            }
            else
            {
                return BadRequest("The token provided is not authorized");
            }
        }

        [HttpGet("countyfarmers/aqua/{county}")]
        public async Task<IActionResult> GetCountyAquaData(string county)
        {
            var authToken = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).Parameter;
            if (IsAuthorized(authToken))
            {
                var dataKey = county.GetHashCode();
                var data = new List<KYFAPExtractModel>();
                data = await DataAccess.GetKYFCountyExtractAquaculture(county);

                if (data != null && data.Any())
                {
                    var output = JsonConvert.SerializeObject(data, Formatting.Indented);
                    System.IO.File.WriteAllText($"{county}_aqaudata.json", output);
                    return Ok($"{county}_aquadata.json written to file");
                }
                return NotFound($"No new or updated farmer for County: {county.ToUpper()};");
            }
            else
            {
                return BadRequest("The token provided is not authorized");
            }
        }

        [HttpPost("farmers/dated")]
        public async Task<IActionResult> GetWardFarmersByDate([FromBody] KYFQueryAdminModel admin,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string start = "", [FromQuery] string end = "")
        {
            var authToken = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).Parameter;
            if (IsAuthorized(authToken))
            {
                // Parse the input dates using ParseExact
                DateTime startDate = DateTime.Now.Date;
                DateTime endDate = DateTime.Now.Date;
                try
                {

                    // Parse the input dates using ParseExact
                    startDate = DateTime.ParseExact(start, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    endDate = DateTime.ParseExact(end, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                }
                catch (ArgumentNullException)
                {
                    return BadRequest("No valid dates were provided");
                }
                catch (FormatException)
                {
                    return BadRequest($"Dates startDate: {startDate} or endDate: {endDate} are not in the correct format (dd-MM-yyyy) e.g. {DateTime.Today.Date.ToString("dd-MM-yyyy")}");
                }
                catch (Exception)
                {
                }
                if (endDate > startDate)
                {

                    var dataKey = admin.GetHashCode();
                    var data = new List<KYFFarmerDetailModel>();
                    if (KYFFarmerDataCache.KyfFarmerMemoryCache.TryGetValue(dataKey, out data))
                    {
                        if (data != null && data.Any())
                        {
                            var dateResult = data.Where(d => !string.IsNullOrEmpty(d.RecordDate)
                            && DateTime.TryParse(d.RecordDate, null, DateTimeStyles.RoundtripKind, out DateTime date)
                            && (date >= startDate && date <= endDate))?.ToList();

                            if (dateResult != null && dateResult.Any())
                            {
                                if (pageNumber > 0 && pageSize > 0)
                                {
                                    var route = Request.Path.Value;
                                    var validFilter = new PaginationFilterModel(pageNumber, pageSize);

                                    var pagedResponse = Helpers.GeneratePagedResponse<KYFFarmerDetailModel>(dateResult.OrderBy(d => d.NationalID).ToList(), validFilter, _uriService, route);
                                    return Ok(pagedResponse);
                                }

                                return Ok(dateResult.OrderBy(d => d.NationalID).ToList());
                            }
                            else
                            {
                                //var rangeData = await DataAccess.GetKYFFarmerDetailModelsByDate(admin.County.ToUpper(),
                                //    admin.Subcounty.ToUpper(), admin.Ward.ToUpper(), startDate, endDate);

                                var rangeData = await DataAccess.GetKYFWardFarmerDetails(admin.County.ToUpper(),
                                    admin.Subcounty.ToUpper(), admin.Ward.ToUpper());

                                if (rangeData != null && rangeData.Any())
                                {
                                    data.AddRange(rangeData);

                                    KYFFarmerDataCache.KyfFarmerMemoryCache.Remove(dataKey);

                                    KYFFarmerDataCache.KyfFarmerMemoryCache.Set(dataKey, rangeData);

                                    if (pageNumber > 0 && pageSize > 0)
                                    {
                                        var route = Request.Path.Value;
                                        var validFilter = new PaginationFilterModel(pageNumber, pageSize);

                                        var pagedResponse = Helpers.GeneratePagedResponse<KYFFarmerDetailModel>(rangeData.OrderBy(d => d.NationalID).ToList(), validFilter, _uriService, route);
                                        return Ok(pagedResponse);
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        data = await DataAccess.GetKYFWardFarmerDetails(admin.County.ToUpper(), admin.Subcounty.ToUpper(), admin.Ward.ToUpper());

                        if (data != null && data.Any())
                        {
                            KYFFarmerDataCache.KyfFarmerMemoryCache.Set(dataKey, data);

                            var dateResult = data.Where(d => !string.IsNullOrEmpty(d.RecordDate)
                           && DateTime.TryParse(d.RecordDate, null, DateTimeStyles.RoundtripKind, out DateTime date)
                           && (date >= startDate && date <= endDate))?.ToList();

                            if (dateResult != null && dateResult.Any())
                            {
                                if (pageNumber > 0 && pageSize > 0)
                                {
                                    var route = Request.Path.Value;
                                    var validFilter = new PaginationFilterModel(pageNumber, pageSize);

                                    var pagedResponse = Helpers.GeneratePagedResponse<KYFFarmerDetailModel>(dateResult.OrderBy(d => d.NationalID).ToList(), validFilter, _uriService, route);
                                    return Ok(pagedResponse);
                                }

                                return Ok(dateResult.OrderBy(d => d.NationalID).ToList());
                            }
                        }
                    }

                }
                else
                {
                    return BadRequest($"Invalid date range: StartDate: {start} EndDate: {end}");
                }

                return NotFound($"No new or updated farmer for County: {admin.County}; Subcounty:{admin.Subcounty}; Ward: {admin.Ward}");
            }
            else
            {
                return BadRequest("The token provided is not authorized");
            }
            return BadRequest("The request made was contrary to API documentation. Please review the documentation and implement accordingly.");
        }

        [HttpPost("farmer/{id}")]
        public async Task<IActionResult> GetWardFarmer(string id, [FromBody] KYFQueryAdminModel admin)
        {
            var authToken = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).Parameter;
            if (IsAuthorized(authToken))
            {
                var dataKey = admin.GetHashCode();
                var data = new List<KYFFarmerDetailModel>();
                if (KYFFarmerDataCache.KyfFarmerMemoryCache.TryGetValue(dataKey, out data))
                {
                    if (data != null && data.Any())
                    {
                        var result = data.Where(d => d.NationalID == id)?.ToList();

                        if (result != null && result.Any())
                            return Ok(result);
                        else return NotFound($"The farmer of Id Number: {id} was not found in County: {admin.County}; Subcounty: {admin.Subcounty}; Ward: {admin.Ward}");
                    }
                }
                else
                {
                    data = await DataAccess.GetKYFWardFarmerDetails(admin.County.ToUpper(), admin.Subcounty.ToUpper(), admin.Ward.ToUpper());

                    if (data != null && data.Any())
                    {
                        KYFFarmerDataCache.KyfFarmerMemoryCache.Set(dataKey, data);

                        var result = data.Where(d => d.NationalID == id)?.ToList();

                        if (result != null && result.Any())
                            return Ok(result);
                        else return NotFound($"The farmer of Id Number: {id} was not found in County: {admin.County}; Subcounty: {admin.Subcounty}; Ward: {admin.Ward}");
                    }
                }
            }
            else
            {
                return BadRequest("The token provided is not authorized");
            }
            return BadRequest("The request made was contrary to API documentation. Please review the documentation and implement accordingly.");
        }
    }
}

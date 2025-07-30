using Farmer.Data.API.Models;
using Farmer.Data.API.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using System.Net.Http.Headers;

namespace Farmer.Data.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KYFController : ControllerBase
    {
        private bool IsAuthorized(string authToken)
        {

            return !string.IsNullOrEmpty(authToken) &&
                   _apiKey.Equals(authToken, StringComparison.OrdinalIgnoreCase);
        }

        private readonly string _apiKey;

        private readonly IUriService _uriService;
        //private readonly IMemoryCache _kyfFarmerDataMemoryCache;
        public KYFController(IMemoryCache memoryCache, IUriService uriService)
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

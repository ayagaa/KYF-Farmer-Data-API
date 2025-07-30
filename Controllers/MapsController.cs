using Farmer.Data.API.Models.MBTileModels;
using Farmer.Data.API.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;



namespace Farmer.Data.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapsController : ControllerBase
    {
        private bool IsAuthorized(string authToken)
        {

            return !string.IsNullOrEmpty(authToken) &&
                   _apiKey.Equals(authToken, StringComparison.OrdinalIgnoreCase);
        }

        private readonly string _apiKey;

        public MapsController()
        {
            var credentials = new CredentialsManager.Supplier("Credentials.json");
            _apiKey = credentials.GetSecret("APIKeys:KYFMaps");
        }

        [HttpHead("EA/{eacode}")]
        [HttpGet("EA/{eacode}")]
        public async Task<ActionResult<object>> GetAPZone(string eacode)
        {
            var authToken = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).Parameter;
            Console.WriteLine($"Token Value: {authToken}");
            if (IsAuthorized(authToken))
            {
                try
                {
                    // Base URL for the remote server
                    //string baseUrl = "http://10.101.100.51/maps";
                    var credentials = new CredentialsManager.Supplier("Credentials.json");

                    string baseUrl = credentials.GetSecret("APIKeys:KYFMapsUrl");

                    var _eacode = eacode + ".png";
                    //var _eacodeMbTile = eacode + ".mbtiles";

                    var easData = Csv.ReadRootObjectFile<MbTilesRawData>("EA_Data.csv");



                    // Construct the full URL for the file
                    string fileUrl = $"{baseUrl}/{_eacode}";

                    using HttpClient _httpClient = new();
                    // Fetch the file from the remote URL
                    HttpResponseMessage response = await _httpClient.GetAsync(fileUrl);

                    if (!response.IsSuccessStatusCode)
                    {
                        return NotFound($"File not found on the server: {_eacode}");
                    }

                    var eaData = easData.FirstOrDefault(ea => ea.Name == eacode);

                    if (eaData != null)
                    {
                        // Read the file content
                        var fileBytes = await response.Content.ReadAsByteArrayAsync();

                        //var maxImageSize = 100;

                        //var imageSizeMB = fileBytes.Length / (1024.0 * 1024.0);

                        var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/png";


                        HttpContext.Response.Headers.Add("EA-Bounds", eaData.Bounds);
                        HttpContext.Response.Headers.Add("EA-Centroid-Latitude", eaData.Latitude.ToString());
                        HttpContext.Response.Headers.Add("EA-Centroid-Longitude", eaData.Longitude.ToString());

                        try
                        {
                            if (APZoneCache.APZoneMaps.TryGetValue(eacode, out string eaPolygonString)
                                && !string.IsNullOrEmpty(eaPolygonString))
                            {
                                var geoJson = JObject.Parse(eaPolygonString);
                                if (geoJson != null)
                                {
                                    try
                                    {
                                        var coordinates = geoJson["features"][0]["geometry"]["coordinates"][0];

                                        var jsonCoordinates = coordinates.ToString(Newtonsoft.Json.Formatting.None);

                                        jsonCoordinates = Regex.Replace(jsonCoordinates, @"[\r\n]", "");
                                        //Console.WriteLine(jsonCoordinates);
                                        //HttpContext.Response.Headers.Add("AP-Zone-Polygon", Uri.EscapeDataString(jsonCoordinates));
                                    }
                                    catch (Exception ex)
                                    {

                                        Console.WriteLine($"Error1111 adding the polygon header: {ex.Message}");
                                    }


                                }
                                //HttpContext.Response.Headers.Add("AP-Zone-Polygon", eaPolygonString);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error adding the polygon header: {ex.Message}");
                        }



                        return File(fileBytes, contentType, _eacode);
                        //return Ok(Convert.ToBase64String(fileBytes));
                    }
                }
                catch (HttpRequestException ex)
                {
                    return StatusCode(500, $"Error fetching file from server: {ex.Message}");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
            return BadRequest("Bad request. Please read documentation.");
        }

        [HttpGet("EA/polygon/{eacode}")]
        public async Task<ActionResult<object>> GetAPZonePolygon(string eacode)
        {
            var authToken = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).Parameter;
            Console.WriteLine($"AP Zone Polygon Token Value: {authToken}");
            if (IsAuthorized(authToken))
            {
                try
                {

                    var easData = Csv.ReadRootObjectFile<MbTilesRawData>("EA_Data.csv");

                    var eaData = easData.FirstOrDefault(ea => ea.Name == eacode);

                    if (eaData != null)
                    {
                        HttpContext.Response.Headers.Add("EA-Bounds", Uri.EscapeDataString(eaData.Bounds));
                        HttpContext.Response.Headers.Add("EA-Centroid-Latitude", eaData.Latitude.ToString());
                        HttpContext.Response.Headers.Add("EA-Centroid-Longitude", eaData.Longitude.ToString());

                        try
                        {
                            if (APZoneCache.APZoneMaps.TryGetValue(eacode, out string eaPolygonString)
                                && !string.IsNullOrEmpty(eaPolygonString))
                            {
                                return Ok(eaPolygonString);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error sending the polygon: {ex.Message}");
                        }


                        return NotFound($"Polygon not found for AP zone: {eacode}");

                    }
                }
                catch (HttpRequestException ex)
                {
                    return StatusCode(500, $"Error fetching file from server: {ex.Message}");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
            return BadRequest("Bad request. Please read documentation.");
        }

        [HttpGet()]
        public async Task<ActionResult<string>> Test()
        {
            return Ok("We are hitting this");
        }
    }
}

using Farmer.Data.API.Models;
using Farmer.Data.API.Models.SoilModels;
using Farmer.Data.API.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IUriService>(o =>
{
    var accessor = o.GetRequiredService<IHttpContextAccessor>();
    var request = accessor?.HttpContext?.Request;
    var credentials = new CredentialsManager.Supplier("Credentials.json");

    var uri = string.Concat(request?.Scheme, "://", request?.Host.ToUriComponent()).Replace(credentials.GetSecret("AppSettings:KYFUrlIP"), credentials.GetSecret("AppSettings:farmerdb.kalro.org")).Replace("http", "https");
    return new UriService(uri);
});


var app = builder.Build();

//Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseRouting();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
//app.UseSwagger();
//app.UseSwaggerUI();
app.UseAuthorization();

app.MapControllers();

GeoJson.PopulateGeoJsonDictionary(ref APZoneCache.APZoneMaps);
var agrodealers = @"/home/kproxy2/soilMap/Agrodealers_data.csv";
//var agrodealers = "Agrodealers_data.csv";

APZoneCache.AgroDealers = Csv.ReadRootObjectFile<AgroDealerModel>(agrodealers);
Console.WriteLine($"Number of Agrodealers: {APZoneCache.AgroDealers.Count}");

var soilAPIKeys = @"/home/kproxy2/soilMap/Soil_API_KEYS.csv";
//var soilAPIKeys = "Soil_API_KEYS.csv";

var readKeys = Csv.ReadRootObjectFile<SoilAPIKeysModel>(soilAPIKeys);
Console.WriteLine($"Number of Soil Challenge Keys: {readKeys.Count}");

var convertionFactorsFile = @"/home/kproxy2/soilMap/crop_tree_conversion_factor.csv";
//var convertionFactorsFile = "crop_tree_conversion_factor.csv";
APZoneCache.CropTreeConversionFactors = Csv.ReadRootObjectFile<CropTreeConversionFactorModel>(convertionFactorsFile);
Console.WriteLine($"Number of convertion factors: {APZoneCache.CropTreeConversionFactors?.Count}");

var apZoneComparer = new KYFQueryAdminComparer();
var inputAPZoneFile = @"/home/kproxy2/soilMap/AP-Zones.csv";
//var inputAPZoneFile = "AP-Zones.csv";
APZoneCache.APZones = Csv.ReadRootObjectFile<APZoneModel>(inputAPZoneFile)
    .Select(a => new KYFQueryAdminModel
    {
        County = a.CountyName,
        Subcounty = a.SubcountyName,
        Ward = a.WardName,
    }).Distinct(apZoneComparer).ToList();

Console.WriteLine($"Number of zones: {APZoneCache.APZones.Count}");


foreach (var key in readKeys)
{
    DataAccess.SoilAPIKeys.TryAdd(key.Key, key.Organization);
}
Console.WriteLine($"Number of Soil Challenge Keys in Keys Dictionary:{DataAccess.SoilAPIKeys.Count}");
Console.WriteLine($"Number of AP Zone Maps: {APZoneCache.APZoneMaps.Count}");

app.Run();

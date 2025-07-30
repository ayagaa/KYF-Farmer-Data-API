namespace Farmer.Data.API.Utils
{
    //using BruTile.Wms;
    using Farmer.Data.API.Databases;
    using Farmer.Data.API.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using MongoDB.Bson;
    using MongoDB.Bson.IO;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Collections.Concurrent;
    using System.Data;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Text;
    using System.Text.Json;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Defines the <see cref="DataAccess" />
    /// </summary>
    public static class DataAccess
    {

        public static Dictionary<string, string> SoilAPIKeys = new Dictionary<string, string>();

        /// <summary>
        /// Defines the _jsonSerializerSettings
        /// </summary>
        private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
        };

        /// <summary>
        /// Defines the _legacySoilDataContext
        /// </summary>
        private static LegacySoilDataDbContext _legacySoilDataContext;

        /// <summary>
        /// The LegacySoilDataCollection
        /// </summary>
        /// <returns>The <see cref="IMongoCollection{BsonDocument}"/></returns>
        private static IMongoCollection<BsonDocument> LegacySoilDataCollection()
        {
            var credentials = new CredentialsManager.Supplier("Credentials.json");
            var settings = MongoClientSettings.FromUrl(new MongoUrl(credentials.GetSecret("Database:KYFMongo")));
            settings.MaxConnectionPoolSize = 200;           // default is 100
            // default is 500
            settings.WaitQueueTimeout = TimeSpan.FromMinutes(60);
            var client = new MongoClient(settings); // Replace with your MongoDB connection string
            var database = client.GetDatabase(credentials.GetSecret("Database:SoilMongoDatabase")); // Replace with your database name
            var collection = database.GetCollection<BsonDocument>(credentials.GetSecret("Database:SoilMongoCollection"));

            return collection;
        }

        /// <summary>
        /// The KYFDataCollection
        /// </summary>
        /// <returns>The <see cref="IMongoCollection{BsonDocument}"/></returns>
        private static IMongoCollection<BsonDocument> KYFDataCollection()
        {
            var credentials = new CredentialsManager.Supplier("Credentials.json");
            var settings = MongoClientSettings.FromUrl(new MongoUrl(credentials.GetSecret("Database:KYFMongo")));
            settings.MaxConnectionPoolSize = 200;           // default is 100
            // default is 500
            settings.WaitQueueTimeout = TimeSpan.FromMinutes(60);
            var client = new MongoClient(settings); // Replace with your MongoDB connection string
            var database = client.GetDatabase(credentials.GetSecret("Database:KYFMongoDatabase")); // Replace with your database name
            var collection = database.GetCollection<BsonDocument>(credentials.GetSecret("Database:KYFMongoCollection"));

            return collection;
        }

        private static IMongoCollection<BsonDocument> KYFDataExtraction()
        {
            var credentials = new CredentialsManager.Supplier("Credentials.json");

            var settings = MongoClientSettings.FromUrl(new MongoUrl(credentials.GetSecret("Database:KYFMongo")));
            settings.MaxConnectionPoolSize = 200;           // default is 100
            // default is 500
            settings.WaitQueueTimeout = TimeSpan.FromMinutes(60);
            var client = new MongoClient(settings); // Replace with your MongoDB connection string
            var database = client.GetDatabase(credentials.GetSecret("Database:KYFMongoDatabase")); // Replace with your database name
            var collection = database.GetCollection<BsonDocument>(credentials.GetSecret("Database:KYFMongoCollection"));

            return collection;
        }

        /// <summary>
        /// The ToJsonString
        /// </summary>
        /// <param name="bson">The bson<see cref="BsonDocument"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string ToJsonString(BsonDocument bson)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BsonBinaryWriter(stream))
                {
                    BsonSerializer.Serialize(writer, typeof(BsonDocument), bson);
                }
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new Newtonsoft.Json.Bson.BsonReader(stream))
                {
                    var sb = new StringBuilder();
                    var sw = new StringWriter(sb);
                    using (var jWriter = new JsonTextWriter(sw))
                    {
                        jWriter.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                        jWriter.WriteToken(reader);
                    }
                    return sb.ToString();
                }
            }
        }

        /// <summary>
        /// The GetLegacySoilData
        /// </summary>
        /// <returns>The <see cref="Task{string}"/></returns>
        public static async Task<string> GetLegacySoilData()
        {
            var resultString = new StringBuilder();

            try
            {
                resultString.Append('[');
                var soilData = LegacySoilDataCollection();
                var filter = Builders<BsonDocument>.Filter.Empty;

                var bsonDocuments = await soilData.Find(filter).ToListAsync();

                foreach (var bsonDocument in bsonDocuments)
                {
                    var json = ToJsonString(bsonDocument);
                    resultString.Append(json).Append(',');
                }

                resultString.Append(']');
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Format Error message: {ex.Message}");
                Console.WriteLine($"Format Error fetching data: {ex.StackTrace}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Error fetching data: {ex.StackTrace}");
            }

            return resultString.ToString();
        }

        /// <summary>
        /// The GetLegacySoilDataByLab
        /// </summary>
        /// <param name="labNo">The labNo<see cref="string"/></param>
        /// <returns>The <see cref="Task{string}"/></returns>
        public static async Task<string> GetLegacySoilDataByLab(string labNo)
        {
            var resultString = new StringBuilder();

            try
            {
                resultString.Append('[');
                var soilData = LegacySoilDataCollection();
                var filter = Builders<BsonDocument>.Filter.Eq("lab_No", labNo);

                var bsonDocuments = await soilData.Find(filter).ToListAsync();

                foreach (var bsonDocument in bsonDocuments)
                {
                    var json = ToJsonString(bsonDocument);
                    resultString.Append(json).Append(',');
                }

                resultString.Append(']');
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Format Error message: {ex.Message}");
                Console.WriteLine($"Format Error fetching data: {ex.StackTrace}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Error fetching data: {ex.StackTrace}");
            }

            return resultString.ToString();
        }

        /// <summary>
        /// The GetLegacySoilDataByCounty
        /// </summary>
        /// <param name="county">The county<see cref="string"/></param>
        /// <returns>The <see cref="Task{string}"/></returns>
        public static async Task<string> GetLegacySoilDataByCounty(string county)
        {
            var resultString = new StringBuilder();

            try
            {
                resultString.Append('[');
                var soilData = LegacySoilDataCollection();
                var filter = Builders<BsonDocument>.Filter.Eq("County", county);

                var bsonDocuments = await soilData.Find(filter).ToListAsync();

                foreach (var bsonDocument in bsonDocuments)
                {
                    var json = ToJsonString(bsonDocument);
                    resultString.Append(json).Append(',');
                }

                resultString.Append(']');
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Format Error message: {ex.Message}");
                Console.WriteLine($"Format Error fetching data: {ex.StackTrace}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Error fetching data: {ex.StackTrace}");
            }

            return resultString.ToString();
        }

        #region GetKYFWardFarmerDetails Old
        ///// <summary>
        ///// The GetKYFWardFarmerDetails
        ///// </summary>
        ///// <param name="county">The county<see cref="string"/></param>
        ///// <param name="subcounty">The subcounty<see cref="string"/></param>
        ///// <param name="ward">The ward<see cref="string"/></param>
        ///// <returns>The <see cref="Task{List{KYFFarmerDetailModel}}"/></returns>
        //public static async Task<List<KYFFarmerDetailModel>> GetKYFWardFarmerDetails(
        //    string county,
        //    string subcounty,
        //    string ward
        //)
        //{
        //    var rawResultsBag = new ConcurrentBag<KYFFarmerDetailModel>();
        //    try
        //    {
        //        var collection = KYFDataCollection();

        //        #region Build Filter
        //        // Build a filter that matches documents whose Section A contains:
        //        // county = "KITUI", sub county = "KITUI CENTRAL", and ward = "KYANGWITHYA WEST".
        //        // Since these fields are nested within an array element ("data"),
        //        // we use $elemMatch on the "data" field.
        //        var filter = Builders<BsonDocument>.Filter.And(
        //            // Filter for county in Section A
        //            Builders<BsonDocument>.Filter.ElemMatch<BsonValue>(
        //                "data",
        //                new BsonDocument
        //                {
        //                    { "name", "Section A" },
        //                    {
        //                        "content",
        //                        new BsonDocument(
        //                            "$elemMatch",
        //                            new BsonDocument { { "label", "county" }, { "value", county } }
        //                        )
        //                    },
        //                }
        //            ),
        //            // Filter for sub county in Section A
        //            Builders<BsonDocument>.Filter.ElemMatch<BsonValue>(
        //                "data",
        //                new BsonDocument
        //                {
        //                    { "name", "Section A" },
        //                    {
        //                        "content",
        //                        new BsonDocument(
        //                            "$elemMatch",
        //                            new BsonDocument
        //                            {
        //                                { "label", "sub county" },
        //                                { "value", subcounty },
        //                            }
        //                        )
        //                    },
        //                }
        //            ),
        //            // Filter for ward in Section A
        //            Builders<BsonDocument>.Filter.ElemMatch<BsonValue>(
        //                "data",
        //                new BsonDocument
        //                {
        //                    { "name", "Section A" },
        //                    {
        //                        "content",
        //                        new BsonDocument(
        //                            "$elemMatch",
        //                            new BsonDocument { { "label", "ward" }, { "value", ward } }
        //                        )
        //                    },
        //                }
        //            ),
        //            // Filter for ward in Section A
        //            Builders<BsonDocument>.Filter.ElemMatch<BsonValue>(
        //                "data",
        //                new BsonDocument
        //                {
        //                    { "name", "Section A" },
        //                    {
        //                        "content",
        //                        new BsonDocument(
        //                            "$elemMatch",
        //                            new BsonDocument { { "label", "ward" }, { "value", ward } }
        //                        )
        //                    },
        //                }
        //            )
        //        );

        //        // Define a projection to return only the fields you require.
        //        // For example, here we return the top-level status fields along with the entire "data" array.
        //        // You can narrow this further (e.g. to specific nested fields) using dot notation.
        //        //var projection = Builders<BsonDocument>.Projection
        //        //    .Include("registration_status")
        //        //    .Include("farmer_status")
        //        //    //.Include("data")
        //        //    .Include("data.parcels");

        //        //var documents = collection.Find(filter).Project(projection).ToList();
        //        #endregion

        //        #region Query Processing

        //        var options = new FindOptions<BsonDocument>
        //        {
        //            NoCursorTimeout = true
        //        };

        //        using (var cursor = await collection.FindAsync(filter, options))
        //        {
        //            while (await cursor.MoveNextAsync())
        //            {
        //                #region Data processing
        //                // Fetch all documents (you can add filters if needed)
        //                //var documents = collection.Find(new BsonDocument()).ToList();

        //                Parallel.ForEach(
        //                cursor.Current,
        //                //new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
        //                document =>
        //                {
        //                    var rawResult = new KYFFarmerDetailModel()
        //                    {
        //                        County = county,
        //                        Subcounty = subcounty,
        //                        Ward = ward,
        //                    };
        //                    try
        //                    {
        //                        // Convert the document to a JSON string and parse into a JObject.
        //                        string jsonString = document.ToJson();
        //                        JObject jObject = JObject.Parse(jsonString);

        //                        // The JSON is expected to have a "data" array with multiple sections.
        //                        JArray sections = jObject["data"] as JArray;

        //                        rawResult.RegistrationStatus = jObject["registration_status"]
        //                            ?.ToString();

        //                        rawResult.FarmerStatus = jObject["farmer_status"]?.ToString();

        //                        if (sections != null)
        //                        {
        //                            // Extract administrative details from Section A.
        //                            JObject sectionA =
        //                                sections.FirstOrDefault(s =>
        //                                    s["name"]?.ToString().Contains("Section A") == true
        //                                ) as JObject;

        //                            if (sectionA != null)
        //                            {
        //                                // Extract administrative details from Section A.
        //                                JArray contentA = sectionA["content"] as JArray;
        //                                if (contentA != null)
        //                                {
        //                                    rawResult.RecordDate =
        //                                        contentA
        //                                            .FirstOrDefault(c =>
        //                                                c["label"]
        //                                                    ?.ToString()
        //                                                    .ToLower()
        //                                                    .Contains("date") == true
        //                                            )
        //                                            ?["value"]?.ToString() ?? "";

        //                                    try
        //                                    {
        //                                        // Extract household GPS from the field with name "gps" and type "gps"
        //                                        JObject gpsField =
        //                                            contentA.FirstOrDefault(obj =>
        //                                                (string)obj["name"] == "gps"
        //                                                && (string)obj["type"] == "gps"
        //                                            ) as JObject;

        //                                        if (gpsField != null)
        //                                        {
        //                                            JObject coords =
        //                                                gpsField["value"]["coords"] as JObject;
        //                                            rawResult.Latitude = coords["latitude"]?.ToString();
        //                                            rawResult.Longitude = coords["longitude"]
        //                                                ?.ToString();

        //                                            //Console.WriteLine($"Coordinates =>\nLatitude: {latitude}\nLongitude: {longitude}");
        //                                        }
        //                                    }
        //                                    catch (Exception) { }
        //                                }
        //                            }

        //                            // Extract farmer personal details from Section B.
        //                            JObject sectionB =
        //                                sections.FirstOrDefault(s =>
        //                                    s["name"]?.ToString().Contains("Section B") == true
        //                                ) as JObject;

        //                            string farmerName = "",
        //                                farmerId = "",
        //                                farmerPhone = "",
        //                                farmerGender = "";
        //                            if (sectionB != null)
        //                            {
        //                                JArray contentB = sectionB["content"] as JArray;
        //                                if (contentB != null)
        //                                {
        //                                    rawResult.Name =
        //                                        contentB
        //                                            .FirstOrDefault(c =>
        //                                                c["q_code"]?.ToString() == "B02"
        //                                            )
        //                                            ?["value"]?.ToString() ?? "";
        //                                    rawResult.NationalID =
        //                                        contentB
        //                                            .FirstOrDefault(c =>
        //                                                c["q_code"]?.ToString() == "B03"
        //                                            )
        //                                            ?["value"]?.ToString() ?? "";
        //                                    rawResult.MobileNumber =
        //                                        contentB
        //                                            .FirstOrDefault(c =>
        //                                                c["q_code"]?.ToString() == "B04"
        //                                            )
        //                                            ?["value"]?.ToString() ?? "";
        //                                    rawResult.Sex =
        //                                        contentB
        //                                            .FirstOrDefault(c =>
        //                                                c["q_code"]?.ToString() == "B05"
        //                                            )
        //                                            ?["value"]?.ToString() ?? "";

        //                                    rawResult.YOB =
        //                                        sectionB["content"]
        //                                            .FirstOrDefault(obj =>
        //                                                (string)obj["name"] == "farmer_yob"
        //                                            )
        //                                            ?["value"]?.ToString() ?? "";
        //                                }
        //                            }

        //                            // Extract parcel details from Section C.
        //                            JObject sectionC =
        //                                sections.FirstOrDefault(s =>
        //                                    s["name"]?.ToString().Contains("Section C") == true
        //                                ) as JObject;

        //                            if (sectionC != null)
        //                            {
        //                                JArray parcels = sectionC["parcels"] as JArray;
        //                                if (parcels != null && parcels.Any())
        //                                {
        //                                    foreach (JObject parcel in parcels)
        //                                    {
        //                                        JArray parcelContent = parcel["content"] as JArray;

        //                                        try
        //                                        {
        //                                            try
        //                                            {
        //                                                //JArray parcelContent = parcel["content"] as JArray;
        //                                                string acreage =
        //                                                    parcelContent
        //                                                        ?.FirstOrDefault(c =>
        //                                                            c["name"]
        //                                                                ?.ToString()
        //                                                                .Equals(
        //                                                                    "parcel_total_acreage",
        //                                                                    StringComparison.OrdinalIgnoreCase
        //                                                                ) == true
        //                                                        )
        //                                                        ?["value"]?.ToString() ?? "";
        //                                                string parcelOwnershipStatus =
        //                                                    parcelContent
        //                                                        ?.FirstOrDefault(c =>
        //                                                            c["name"]
        //                                                                ?.ToString()
        //                                                                .Equals(
        //                                                                    "ownership_status",
        //                                                                    StringComparison.OrdinalIgnoreCase
        //                                                                ) == true
        //                                                        )
        //                                                        ?["value"]?.FirstOrDefault()["value"]
        //                                                        ?.ToString() ?? "";

        //                                                rawResult.Parcels.Add(
        //                                                    new ParcelModel
        //                                                    {
        //                                                        Acreage = acreage,
        //                                                        OwnershipStatus = parcelOwnershipStatus,
        //                                                    }
        //                                                );
        //                                            }
        //                                            catch (Exception) { }

        //                                            // Extract crop details from the key "crops" under the parcel.
        //                                            JArray cropsArray = parcel["crops"] as JArray;
        //                                            if (cropsArray != null && cropsArray.Any())
        //                                            {
        //                                                var crops = new List<KYFCropDetail>();
        //                                                foreach (JObject cropObject in cropsArray)
        //                                                {
        //                                                    JArray content =
        //                                                        cropObject["content"] as JArray;

        //                                                    if (content != null && content.Any())
        //                                                    {
        //                                                        // Extract the crop name.
        //                                                        string cropName = content
        //                                                            .FirstOrDefault(item =>
        //                                                                item["name"] != null
        //                                                                && item["name"].ToString()
        //                                                                    == "crop_name"
        //                                                            )
        //                                                            ?["value"]?.ToString();

        //                                                        // Extract the acreage for this crop.
        //                                                        string cropAcreageStr = content
        //                                                            .FirstOrDefault(item =>
        //                                                                item["name"] != null
        //                                                                && item["name"].ToString()
        //                                                                    == "acreage_annual_crop"
        //                                                            )
        //                                                            ?["value"]?.ToString();

        //                                                        if (
        //                                                            float.TryParse(
        //                                                                cropAcreageStr,
        //                                                                out float cropAcreage
        //                                                            )
        //                                                        )
        //                                                        {
        //                                                            crops.Add(
        //                                                                new KYFCropDetail
        //                                                                {
        //                                                                    CropName = cropName,
        //                                                                    TotalCropAcreage =
        //                                                                        cropAcreage,
        //                                                                }
        //                                                            );
        //                                                        }
        //                                                    }
        //                                                }
        //                                                if (crops != null && crops.Any())
        //                                                {
        //                                                    var summedCropDetails = crops
        //                                                        .GroupBy(c => c.CropName)
        //                                                        .Select(g => new KYFCropDetail
        //                                                        {
        //                                                            CropName = g.Key,
        //                                                            TotalCropAcreage = g.Sum(c =>
        //                                                                c.TotalCropAcreage
        //                                                            ),
        //                                                        })
        //                                                        .ToList();

        //                                                    rawResult.Crops.AddRange(summedCropDetails);
        //                                                }
        //                                            }
        //                                        }
        //                                        catch (Exception ex)
        //                                        {
        //                                            Console.WriteLine(
        //                                                $"Error getting crops in parcel: {ex.Message}"
        //                                            );
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }

        //                        if (rawResult.Crops != null && rawResult.Crops.Any())
        //                        {
        //                            rawResultsBag.Add(rawResult);
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Console.WriteLine($"Error processing document: {ex.Message}");
        //                    }
        //                }
        //            );
        //                #endregion
        //            }
        //        }

        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(
        //            $"Error fetching and processing farmer data: {ex.Message}\nStack Trace: {ex.StackTrace}"
        //        );
        //    }

        //    return rawResultsBag.ToList();
        //}
        #endregion

        private static void DumpToken(JToken token, string indent = "")
        {
            Console.WriteLine($"{indent}- {token.Type}: {token.Path}");
            if (token is JValue) return;

            foreach (JToken child in token.Children())
            {
                DumpToken(child, indent + "  ");
            }
        }
        private static List<KYFLivestockCountModel> ProcessLivestockData(JArray livestockSectionContent, JArray mainSections)
        {
            var results = new ConcurrentBag<KYFLivestockCountModel>();

            try
            {
                if (livestockSectionContent != null && livestockSectionContent.Any())
                {
                    var livestockTypes = new List<string>();
                    var livestockTypeObject = livestockSectionContent
                        .OfType<JObject>()                              // only take object-tokens
    .Select(o => new { Name = o.Value<string>("name"), Value = o.Value<JToken>("value") }).Where(t => t.Name == "livestock_type")?.FirstOrDefault();


                    if (livestockTypeObject != null && livestockTypeObject.Value != null
                        && (livestockTypeObject.Value is JArray))
                    {
                        try
                        {
                            var livestockTypesArray = livestockTypeObject.Value as JArray;

                            if (livestockTypesArray != null && livestockTypesArray.Any())
                            {
                                foreach (JToken animalType in livestockTypesArray)
                                {
                                    var livestockType = animalType["name"].ToString();
                                    if (!string.IsNullOrEmpty(livestockType))
                                        livestockTypes.Add(livestockType);
                                }
                            }

                            if (livestockTypes.Any())
                            {
                                try
                                {

                                    Parallel.ForEach(livestockTypes, livestockType =>
                                    {

                                        try
                                        {
                                            JObject livestockTypeSection =
    mainSections.FirstOrDefault(s =>
    s["name"]?.ToString() == livestockType
    ) as JObject;
                                            if (livestockTypeSection != null)
                                            {
                                                var breedingSystem = string.Empty;
                                                var herdStructure = livestockTypeSection["herd_structure"] as JArray;
                                                var livestockContent = livestockTypeSection["content"] as JArray;

                                                //if(livestockContent != null && livestockContent.Any())
                                                //{
                                                //    var breedingServicesContent = livestockContent.FirstOrDefault(c => c["name"]?.ToString()
                                                //    .Contains("_production_breeding_services") == true)?["value"] as JArray;

                                                //    if(breedingServicesContent != null && breedingServicesContent.Any())
                                                //    {

                                                //    }
                                                //}

                                                if (herdStructure != null && herdStructure.Any())
                                                {
                                                    foreach (var herd in herdStructure)
                                                    {
                                                        var herdStructureResult = new KYFLivestockCountModel();

                                                        var herdContent = herd["content"] as JArray;
                                                        var subcategory = string.Empty;
                                                        var breedsKept = string.Empty;
                                                        var productionSystem = string.Empty;
                                                        if (herdContent != null && herdContent.Any())
                                                        {
                                                            try
                                                            {
                                                                var subTypeContent = herdContent
                    .FirstOrDefault(c =>
                        c["name"]?.ToString().Contains("type") == true
                    )
                    ?["value"] as JArray;
                                                                if (subTypeContent != null && subTypeContent.Any())
                                                                {
                                                                    subcategory = subTypeContent[0]["value"].ToString();
                                                                }
                                                            }
                                                            catch (Exception)
                                                            {

                                                            }


                                                            try
                                                            {
                                                                var productionSystemElement = herdContent.FirstOrDefault(c =>
(c["name"]?.ToString().Contains("production_system") == true ||
 c["name"]?.ToString().Contains("prod_system") == true)
//&&
//c["type"]?.ToString() == "radio" || c["type"]?.ToString() == "select"
)
?["value"] as JArray;
                                                                if (productionSystemElement != null && productionSystemElement.Any())
                                                                {
                                                                    productionSystem = productionSystemElement[0]?["value"]?.ToString();
                                                                }
                                                                else
                                                                {
                                                                    try
                                                                    {
                                                                        var productionSystemContent = livestockContent.FirstOrDefault(c =>
                                                                                           (c["name"]?.ToString().Contains("production_system") == true ||
                                                                                            c["name"]?.ToString().Contains("prod_system") == true))?["value"];

                                                                        if (productionSystemContent != null && productionSystemContent is JArray)
                                                                        {
                                                                            productionSystem = productionSystemContent[0]?["value"]?.ToString();
                                                                        }
                                                                        else if (productionSystemContent != null && !(productionSystemContent is JArray))
                                                                        {
                                                                            productionSystem = productionSystemContent?.ToString();
                                                                        }
                                                                    }
                                                                    catch (Exception)
                                                                    {
                                                                    }

                                                                }
                                                            }
                                                            catch (Exception)
                                                            {

                                                            }


                                                            try
                                                            {
                                                                var breedsKeptContent = herdContent[2]?["value"] as JArray;
                                                                if (breedsKeptContent != null && breedsKeptContent.Any() && !livestockType.Contains("Duck") && !livestockType.Contains("Pigs"))
                                                                {
                                                                    breedsKept = breedsKeptContent[0]["value"].ToString();
                                                                }
                                                            }
                                                            catch (Exception)
                                                            {
                                                            }

                                                            try
                                                            {
                                                                var livestockNumbersContent = herdContent.Where(c => c["keyboardType"]?.ToString() == "numeric")?.ToList();
                                                                //Console.WriteLine($"Livestock before numbers: {livestockType} subcategory: {subcategory}");
                                                                if (livestockNumbersContent != null && livestockNumbersContent.Any())
                                                                {
                                                                    foreach (var livestockNumber in livestockNumbersContent)
                                                                    {

                                                                        try
                                                                        {
                                                                            var detail = !string.IsNullOrEmpty(livestockNumber["placeholder"]?.ToString()) ? livestockNumber["placeholder"]?.ToString() : livestockNumber["label"]?.ToString();
                                                                            var detailCount = livestockNumber["value"]?.ToString();
                                                                            //Console.WriteLine($"Livestock type at numbers: {livestockType}");
                                                                            if (float.TryParse(detailCount, out float animalCount) && animalCount > 0f)
                                                                            {

                                                                                var livestockDetail = new KYFLivestockCountModel()
                                                                                {
                                                                                    Category = livestockType,
                                                                                    SubCategory = subcategory,
                                                                                    Breed = breedsKept,
                                                                                    LivestockDetail = detail,
                                                                                    Count = (int)animalCount,
                                                                                    ProductionSystem = productionSystem
                                                                                };

                                                                                //Console.WriteLine($"Added livestock: {livestockType}");
                                                                                results.Add(livestockDetail);
                                                                            }
                                                                        }
                                                                        catch (Exception)
                                                                        {


                                                                        }


                                                                    }
                                                                }
                                                            }
                                                            catch (Exception)
                                                            {

                                                            }

                                                        }

                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    });
                                }
                                catch (Exception)
                                {
                                }
;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error trying to process livestock: {ex.Message}\n{ex.StackTrace}");
                        }

                    }
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error trying to target livestockTypes: {ex.Message}\n{ex.StackTrace}");
            }


            return results.ToList();
        }

        private static List<KYFAquacultureModel> ProcessAquacultureData(JArray aquaContent, JObject aquacultureSection)
        {
            var resultData = new List<KYFAquacultureModel>();

            try
            {
                var aquaTypesContent = aquaContent.FirstOrDefault(a => a["name"]?.ToString().Contains("aquaculture_type") == true
                && a["type"]?.ToString().Contains("multi_select") == true)?["value"] as JArray;

                var fishTypesContent = aquaContent.FirstOrDefault(a => a["name"]?.ToString().Contains("fish_types") == true
&& a["type"]?.ToString().Contains("multi_select") == true)?["value"] as JArray;

                var productionLevelContent = aquaContent.FirstOrDefault(a => a["name"]?.ToString().Contains("aqua_production_level") == true
&& a["type"]?.ToString().Contains("radio") == true)?["value"] as JArray;

                var aquaData = new KYFAquacultureModel();

                if (productionLevelContent != null && productionLevelContent.Any())
                {
                    aquaData.ProductionLevel = productionLevelContent[0]?["value"]?.ToString();
                }
                if (fishTypesContent != null && fishTypesContent.Any())
                {
                    foreach (var fishType in fishTypesContent)
                    {
                        var fishSpecies = fishType["value"]?.ToString();
                        var numberFilter = $"number_of_{fishSpecies.Replace(" ", "_").ToLower()}";
                        var fishSpeciesNumber = aquaContent.FirstOrDefault(a => a["name"]?.ToString().Contains(numberFilter) == true
&& a["type"]?.ToString().Contains("text") == true)?["value"]?.ToString();
                        aquaData.FishSpecies.Add(new FishSpecies
                        {
                            SpeciesName = fishSpecies,
                            NumberOfFingerlings = fishSpeciesNumber
                        });
                        //Console.WriteLine($"Species: {fishSpecies}; Number of fingerlings: {fishSpeciesNumber}");
                    }
                }

                if (aquaTypesContent != null && aquaTypesContent.Any())
                {
                    foreach (var type in aquaTypesContent)
                    {
                        var aquaType = type["value"].ToString();
                        aquaData.AquacultureTypes.Add(new AquacultureType { TypeName = aquaType });

                        //var aquaTypeArray = aquacultureSection[aquaType] as JArray;

                        //if (aquaTypeArray != null && aquaTypeArray.Any())
                        //{
                        //    foreach (var typeContent in aquaTypeArray)
                        //    {
                        //        var content = typeContent["content"] as JArray;
                        //    }
                        //}
                        //Console.WriteLine($"AquaType: {aquaType}");
                    }
                }

                var pondsContent = aquacultureSection["Ponds"] as JArray;

                if (pondsContent != null && pondsContent.Any())
                {
                    foreach (var pond in pondsContent)
                    {
                        var systemContent = pond["content"] as JArray;

                        if (systemContent != null && systemContent.Any())
                        {

                            var productionSystemName = "Ponds";
                            var productionSystemStatus = string.Empty;
                            var statusContent
    = systemContent.FirstOrDefault(p => p["name"]?.ToString().Contains("status_of_pond") == true)?["value"] as JArray;

                            var dimensionsContent = systemContent.Where(p => p["name"]?.ToString().Contains("_active_units_") == true
                            && p["keyboardType"]?.ToString() == "numeric")?.ToArray();

                            if (statusContent != null && statusContent.Any())
                            {
                                productionSystemStatus = statusContent[0]?["value"].ToString();
                            }

                            var prodSystem = new AquaProductionSystem();
                            prodSystem.ProductionSystem = productionSystemName;
                            prodSystem.ProductionSystemStatus = productionSystemStatus;

                            if (dimensionsContent != null && dimensionsContent.Any())
                            {
                                var dimension = new AquaProductionSystemDimensions();
                                dimension.Length = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("length") == true)?["value"]?.ToString();
                                dimension.Width = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("width") == true)?["value"]?.ToString();
                                dimension.Height = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("heigth") == true)?["value"]?.ToString();
                                dimension.Diameter = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("diameter") == true)?["value"]?.ToString();

                                //Console.WriteLine($"Production system: {productionSystemName}; Status: {productionSystemStatus}\nLength: {dimension.Length};\nWidth:{dimension.Width}\nHeight:{dimension.Height}\nDiameter:{dimension.Diameter}");
                                prodSystem.ProductionSystemDimenions.Add(dimension);
                            }
                            aquaData.ProductionSystems.Add(prodSystem);
                        }
                    }
                }

                var cagesContent = aquacultureSection["Cages"] as JArray;

                if (cagesContent != null && cagesContent.Any())
                {
                    foreach (var cage in cagesContent)
                    {
                        var systemContent = cage["content"] as JArray;

                        if (systemContent != null && systemContent.Any())
                        {

                            var productionSystemName = "Cages";
                            var productionSystemStatus = string.Empty;
                            var statusContent
    = systemContent.FirstOrDefault(p => p["name"]?.ToString().Contains("status_of_") == true)?["value"] as JArray;

                            var dimensionsContent = systemContent.Where(p => p["name"]?.ToString().Contains("_active_units_") == true
                            && p["keyboardType"]?.ToString() == "numeric")?.ToArray();

                            if (statusContent != null && statusContent.Any())
                            {
                                productionSystemStatus = statusContent[0]?["value"].ToString();
                            }

                            var prodSystem = new AquaProductionSystem();
                            prodSystem.ProductionSystem = productionSystemName;
                            prodSystem.ProductionSystemStatus = productionSystemStatus;

                            if (dimensionsContent != null && dimensionsContent.Any())
                            {
                                var dimension = new AquaProductionSystemDimensions();
                                dimension.Length = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("length") == true)?["value"]?.ToString();
                                dimension.Width = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("width") == true)?["value"]?.ToString();
                                dimension.Height = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("heigth") == true)?["value"]?.ToString();
                                dimension.Diameter = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("diameter") == true)?["value"]?.ToString();

                                //Console.WriteLine($"Production system: {productionSystemName}; Status: {productionSystemStatus}\nLength: {dimension.Length};\nWidth:{dimension.Width}\nHeight:{dimension.Height}\nDiameter:{dimension.Diameter}");
                                prodSystem.ProductionSystemDimenions.Add(dimension);
                            }
                            aquaData.ProductionSystems.Add(prodSystem);
                        }
                    }
                }

                var tanksContent = aquacultureSection["Tanks"] as JArray;

                if (tanksContent != null && tanksContent.Any())
                {
                    foreach (var tank in tanksContent)
                    {
                        var systemContent = tank["content"] as JArray;

                        if (systemContent != null && systemContent.Any())
                        {

                            var productionSystemName = "Tanks";
                            var productionSystemStatus = string.Empty;
                            var statusContent
    = systemContent.FirstOrDefault(p => p["name"]?.ToString().Contains("status_of_") == true)?["value"] as JArray;

                            var dimensionsContent = systemContent.Where(p => p["name"]?.ToString().Contains("_active_units_") == true
                            && p["keyboardType"]?.ToString() == "numeric")?.ToArray();

                            if (statusContent != null && statusContent.Any())
                            {
                                productionSystemStatus = statusContent[0]?["value"].ToString();
                            }

                            var prodSystem = new AquaProductionSystem();
                            prodSystem.ProductionSystem = productionSystemName;
                            prodSystem.ProductionSystemStatus = productionSystemStatus;

                            if (dimensionsContent != null && dimensionsContent.Any())
                            {
                                var dimension = new AquaProductionSystemDimensions();
                                dimension.Length = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("length") == true)?["value"]?.ToString();
                                dimension.Width = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("width") == true)?["value"]?.ToString();
                                dimension.Height = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("heigth") == true)?["value"]?.ToString();
                                dimension.Diameter = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("diameter") == true)?["value"]?.ToString();

                                //Console.WriteLine($"Production system: {productionSystemName}; Status: {productionSystemStatus}\nLength: {dimension.Length};\nWidth:{dimension.Width}\nHeight:{dimension.Height}\nDiameter:{dimension.Diameter}");
                                prodSystem.ProductionSystemDimenions.Add(dimension);
                            }
                            aquaData.ProductionSystems.Add(prodSystem);
                        }
                    }
                }

                var rasContent = aquacultureSection["RAS"] as JArray;

                if (rasContent != null && rasContent.Any())
                {
                    foreach (var ras in rasContent)
                    {
                        var systemContent = ras["content"] as JArray;

                        if (systemContent != null && systemContent.Any())
                        {

                            var productionSystemName = "RAS";
                            var productionSystemStatus = string.Empty;
                            var statusContent
    = systemContent.FirstOrDefault(p => p["name"]?.ToString().Contains("status_of_") == true)?["value"] as JArray;

                            var dimensionsContent = systemContent.Where(p => p["name"]?.ToString().Contains("_active_units_") == true
                            && p["keyboardType"]?.ToString() == "numeric")?.ToArray();

                            if (statusContent != null && statusContent.Any())
                            {
                                productionSystemStatus = statusContent[0]?["value"].ToString();
                            }

                            var prodSystem = new AquaProductionSystem();
                            prodSystem.ProductionSystem = productionSystemName;
                            prodSystem.ProductionSystemStatus = productionSystemStatus;

                            if (dimensionsContent != null && dimensionsContent.Any())
                            {
                                var dimension = new AquaProductionSystemDimensions();
                                dimension.Length = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("length") == true)?["value"]?.ToString();
                                dimension.Width = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("width") == true)?["value"]?.ToString();
                                dimension.Height = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("heigth") == true)?["value"]?.ToString();
                                dimension.Diameter = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("diameter") == true)?["value"]?.ToString();

                                //Console.WriteLine($"Production system: {productionSystemName}; Status: {productionSystemStatus}\nLength: {dimension.Length};\nWidth:{dimension.Width}\nHeight:{dimension.Height}\nDiameter:{dimension.Diameter}");
                                prodSystem.ProductionSystemDimenions.Add(dimension);
                            }
                            aquaData.ProductionSystems.Add(prodSystem);
                        }
                    }
                }

                var raceWaysContent = aquacultureSection["Race ways"] as JArray;

                if (raceWaysContent != null && raceWaysContent.Any())
                {
                    foreach (var raceWay in raceWaysContent)
                    {
                        var systemContent = raceWay["content"] as JArray;

                        if (systemContent != null && systemContent.Any())
                        {

                            var productionSystemName = "Race ways";
                            var productionSystemStatus = string.Empty;
                            var statusContent
    = systemContent.FirstOrDefault(p => p["name"]?.ToString().Contains("status_of_") == true)?["value"] as JArray;

                            var dimensionsContent = systemContent.Where(p => p["name"]?.ToString().Contains("_active_units_") == true
                            && p["keyboardType"]?.ToString() == "numeric")?.ToArray();

                            if (statusContent != null && statusContent.Any())
                            {
                                productionSystemStatus = statusContent[0]?["value"].ToString();
                            }

                            var prodSystem = new AquaProductionSystem();
                            prodSystem.ProductionSystem = productionSystemName;
                            prodSystem.ProductionSystemStatus = productionSystemStatus;

                            if (dimensionsContent != null && dimensionsContent.Any())
                            {
                                var dimension = new AquaProductionSystemDimensions();
                                dimension.Length = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("length") == true)?["value"]?.ToString();
                                dimension.Width = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("width") == true)?["value"]?.ToString();
                                dimension.Height = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("heigth") == true)?["value"]?.ToString();
                                dimension.Diameter = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("diameter") == true)?["value"]?.ToString();

                                //Console.WriteLine($"Production system: {productionSystemName}; Status: {productionSystemStatus}\nLength: {dimension.Length};\nWidth:{dimension.Width}\nHeight:{dimension.Height}\nDiameter:{dimension.Diameter}");
                                prodSystem.ProductionSystemDimenions.Add(dimension);
                            }
                            aquaData.ProductionSystems.Add(prodSystem);
                        }
                    }
                }

                var aquaponicsContent = aquacultureSection["Aquaponic"] as JArray;

                if (aquaponicsContent != null && aquaponicsContent.Any())
                {
                    foreach (var aquaponic in aquaponicsContent)
                    {
                        var systemContent = aquaponic["content"] as JArray;

                        if (systemContent != null && systemContent.Any())
                        {

                            var productionSystemName = "Aquaponic";
                            var productionSystemStatus = string.Empty;
                            var statusContent
    = systemContent.FirstOrDefault(p => p["name"]?.ToString().Contains("status_of_") == true)?["value"] as JArray;

                            var dimensionsContent = systemContent.Where(p => p["name"]?.ToString().Contains("_active_units_") == true
                            && p["keyboardType"]?.ToString() == "numeric")?.ToArray();

                            if (statusContent != null && statusContent.Any())
                            {
                                productionSystemStatus = statusContent[0]?["value"].ToString();
                            }

                            var prodSystem = new AquaProductionSystem();
                            prodSystem.ProductionSystem = productionSystemName;
                            prodSystem.ProductionSystemStatus = productionSystemStatus;

                            if (dimensionsContent != null && dimensionsContent.Any())
                            {
                                var dimension = new AquaProductionSystemDimensions();
                                dimension.Length = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("length") == true)?["value"]?.ToString();
                                dimension.Width = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("width") == true)?["value"]?.ToString();
                                dimension.Height = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("heigth") == true)?["value"]?.ToString();
                                dimension.Diameter = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("diameter") == true)?["value"]?.ToString();

                                //Console.WriteLine($"Production system: {productionSystemName}; Status: {productionSystemStatus}\nLength: {dimension.Length};\nWidth:{dimension.Width}\nHeight:{dimension.Height}\nDiameter:{dimension.Diameter}");
                                prodSystem.ProductionSystemDimenions.Add(dimension);
                            }
                            aquaData.ProductionSystems.Add(prodSystem);
                        }
                    }
                }

                var damsContent = aquacultureSection["Dams"] as JArray;

                if (damsContent != null && damsContent.Any())
                {
                    foreach (var dam in damsContent)
                    {
                        var systemContent = dam["content"] as JArray;

                        if (systemContent != null && systemContent.Any())
                        {

                            var productionSystemName = "Dams";
                            var productionSystemStatus = string.Empty;
                            var statusContent
    = systemContent.FirstOrDefault(p => p["name"]?.ToString().Contains("status_of_") == true)?["value"] as JArray;

                            var dimensionsContent = systemContent.Where(p => p["name"]?.ToString().Contains("_active_units_") == true
                            && p["keyboardType"]?.ToString() == "numeric")?.ToArray();

                            if (statusContent != null && statusContent.Any())
                            {
                                productionSystemStatus = statusContent[0]?["value"].ToString();
                            }

                            var prodSystem = new AquaProductionSystem();
                            prodSystem.ProductionSystem = productionSystemName;
                            prodSystem.ProductionSystemStatus = productionSystemStatus;

                            if (dimensionsContent != null && dimensionsContent.Any())
                            {
                                var dimension = new AquaProductionSystemDimensions();
                                dimension.Length = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("length") == true)?["value"]?.ToString();
                                dimension.Width = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("width") == true)?["value"]?.ToString();
                                dimension.Height = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("heigth") == true)?["value"]?.ToString();
                                dimension.Diameter = dimensionsContent.FirstOrDefault(d => d["name"]?.ToString().Contains("diameter") == true)?["value"]?.ToString();

                                //Console.WriteLine($"Production system: {productionSystemName}; Status: {productionSystemStatus}\nLength: {dimension.Length};\nWidth:{dimension.Width}\nHeight:{dimension.Height}\nDiameter:{dimension.Diameter}");
                                prodSystem.ProductionSystemDimenions.Add(dimension);
                            }
                            aquaData.ProductionSystems.Add(prodSystem);
                        }
                    }
                }

                resultData.Add(aquaData);
            }
            catch (Exception)
            {
            }

            return resultData;
        }

        private static KYFFarmerDetailModel ProcessCropsData(JArray parcels)
        {
            var rawResult = new KYFFarmerDetailModel();
            if (parcels != null && parcels.Any())
            {
                foreach (JObject parcel in parcels)
                {
                    JArray parcelContent = parcel["content"] as JArray;

                    try
                    {
                        try
                        {
                            //JArray parcelContent = parcel["content"] as JArray;
                            string acreage =
                                parcelContent
                                    ?.FirstOrDefault(c =>
                                        c["name"]
                                            ?.ToString()
                                            .Equals(
                                                "parcel_total_acreage",
                                                StringComparison.OrdinalIgnoreCase
                                            ) == true
                                    )
                                    ?["value"]?.ToString() ?? "";
                            string parcelOwnershipStatus =
                                parcelContent
                                    ?.FirstOrDefault(c =>
                                        c["name"]
                                            ?.ToString()
                                            .Equals(
                                                "ownership_status",
                                                StringComparison.OrdinalIgnoreCase
                                            ) == true
                                    )
                                    ?["value"]?.FirstOrDefault()["value"]
                            ?.ToString() ?? "";

                            rawResult.Parcels.Add(
                                new ParcelModel
                                {
                                    Acreage = acreage,
                                    OwnershipStatus = parcelOwnershipStatus,
                                }
                            );
                        }
                        catch (Exception) { }

                        // Extract crop details from the key "crops" under the parcel.
                        JArray cropsArray = parcel["crops"] as JArray;
                        if (cropsArray != null && cropsArray.Any())
                        {
                            var crops = new List<KYFCropDetail>();
                            foreach (JObject cropObject in cropsArray)
                            {
                                JArray content =
                                    cropObject["content"] as JArray;

                                if (content != null && content.Any())
                                {
                                    // Extract the crop name.
                                    string cropName = content
                                        .FirstOrDefault(item =>
                                            item["name"] != null
                                            && item["name"].ToString()
                                                == "crop_name"
                                        )
                                        ?["value"]?.ToString();

                                    // Extract the acreage for this crop.
                                    string cropAcreageStr = content
                                        .FirstOrDefault(item =>
                                            item["name"] != null
                                            && (item["name"].ToString()
                                                == "acreage_annual_crop")
                                        )
                                        ?["value"]?.ToString();

                                    // Extract the for all crops.
                                    string totalCropAcreageStr = content
                                        .FirstOrDefault(item =>
                                            item["name"] != null
                                            && (item["name"].ToString()
                                                == "area_under_crops")
                                        )
                                        ?["value"]?.ToString();

                                    string numberOfTrees = content
                                        .FirstOrDefault(item =>
                                            item["name"] != null
                                            && (item["name"].ToString()
                                                == "number_perenial_crop")
                                        )
                                        ?["value"]?.ToString();
                                    var totalCropAcreage = 0f;
                                    var numberOfCropTrees = 0f;

                                    if (
                                        float.TryParse(
                                            cropAcreageStr,
                                            out float cropAcreage
                                        )
                                        || float.TryParse(totalCropAcreageStr,
                                        out totalCropAcreage)

                                    )
                                    {
                                        float.TryParse(numberOfTrees,
                                       out numberOfCropTrees);
                                        crops.Add(
                                            new KYFCropDetail(numberOfTrees, cropName)
                                            {
                                                CropName = cropName,
                                                TotalCropAcreage = cropAcreage,
                                                TotalAreaUnderCrops = totalCropAcreage,
                                                NumberOfTrees = numberOfCropTrees,
                                            }
                                        );
                                    }
                                }
                            }
                            if (crops != null && crops.Any())
                            {
                                var summedCropDetails = crops
                                    .GroupBy(c => c.CropName)
                                    .Select(g => new KYFCropDetail(g.Sum(c => c.NumberOfTrees).ToString(), g.Key)
                                    {
                                        CropName = g.Key,
                                        TotalCropAcreage = g.Sum(c =>
                                            c.TotalCropAcreage
                                        ),
                                        TotalAreaUnderCrops = g.Sum(c => c.TotalAreaUnderCrops),
                                        NumberOfTrees = g.Sum(c => c.NumberOfTrees),
                                    })
                                .ToList();

                                rawResult.Crops.AddRange(summedCropDetails);
                            }
                        }

                        //Extract coffee details
                        JArray coffeeArray = parcel["coffee"] as JArray;
                        if (coffeeArray != null && coffeeArray.Any())
                        {
                            var crops = new List<KYFCropDetail>();
                            foreach (JObject coffeeObject in coffeeArray)
                            {
                                JArray content =
                                    coffeeObject["content"] as JArray;

                                if (content != null && content.Any())
                                {
                                    // Extract the crop name.
                                    string cropName = content
                                        .FirstOrDefault(item =>
                                            item["name"] != null
                                            && item["name"].ToString()
                                                == "crop_name"
                                        )
                                        ?["value"]?.ToString();

                                    // Extract the acreage for this crop.
                                    string numberOfTreesStr = content
                                        .FirstOrDefault(item =>
                                            item["name"] != null
                                            && item["name"].ToString()
                                                == "number_coffee_crop"
                                        )
                                        ?["value"]?.ToString();


                                    // Extract the acreage.
                                    string totalCropAcreageStr = content
                                        .FirstOrDefault(item =>
                                            item["name"] != null
                                            && item["name"].ToString()
                                                == "area_under_crops"
                                        )
                                        ?["value"]?.ToString();

                                    float.TryParse(
                                            totalCropAcreageStr,
                                            out float totalCropAcreage
                                            );

                                    if (
                                        float.TryParse(
                                            numberOfTreesStr,
                                            out float numberOfTrees
                                        )
                                    )
                                    {
                                        crops.Add(
                                            new KYFCropDetail(numberOfTreesStr, cropName)
                                            {
                                                CropName = cropName,
                                                TotalCropAcreage =
                                                    numberOfTrees,
                                                TotalAreaUnderCrops = totalCropAcreage,
                                                NumberOfTrees = numberOfTrees
                                            }
                                        );
                                    }
                                }


                            }

                            if (crops != null && crops.Any())
                            {
                                var summedCropDetails = crops
                                .GroupBy(c => c.CropName)
                                .Select(g => new KYFCropDetail(g.Sum(c => c.NumberOfTrees).ToString(), g.Key)
                                {
                                    CropName = g.Key,
                                    TotalCropAcreage = g.Sum(c =>
                                        c.TotalCropAcreage
                                    ),
                                    TotalAreaUnderCrops = g.Sum(c => c.TotalAreaUnderCrops),
                                    NumberOfTrees = g.Sum(c => c.NumberOfTrees),
                                })
                                .ToList();

                                rawResult.Crops.AddRange(summedCropDetails);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            $"Error getting crops in parcel: {ex.Message}"
                        );
                    }
                }
            }
            return rawResult;
        }

        private static KYFFarmerExtractModel ProcessCropsExtract(JArray parcels)
        {
            var rawResult = new KYFFarmerExtractModel();
            if (parcels != null && parcels.Any())
            {
                int parcelCounter = 1;
                foreach (JObject parcel in parcels)
                {
                    JArray parcelContent = parcel["content"] as JArray;

                    try
                    {
                        try
                        {
                            //JArray parcelContent = parcel["content"] as JArray;
                            string acreage =
                                parcelContent
                                    ?.FirstOrDefault(c =>
                                        c["name"]
                                            ?.ToString()
                                            .Equals(
                                                "parcel_total_acreage",
                                                StringComparison.OrdinalIgnoreCase
                                            ) == true
                                    )
                                    ?["value"]?.ToString() ?? "";
                            string parcelOwnershipStatus =
                                parcelContent
                                    ?.FirstOrDefault(c =>
                                        c["name"]
                                            ?.ToString()
                                            .Equals(
                                                "ownership_status",
                                                StringComparison.OrdinalIgnoreCase
                                            ) == true
                                    )
                                    ?["value"]?.FirstOrDefault()["value"]
                                    ?.ToString() ?? "";

                            rawResult.Parcels.Add(
                                new ParcelExract
                                {
                                    ParcelId = $"PCL{parcelCounter.ToString().PadLeft(4, '0')}",
                                    Acreage = acreage,
                                    OwnershipStatus = parcelOwnershipStatus,
                                }
                            );
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine($"Error: {ex.Message}"); 
                        }

                        // Extract crop details from the key "crops" under the parcel.
                        JArray cropsArray = parcel["crops"] as JArray;
                        if (cropsArray != null && cropsArray.Any())
                        {
                            var crops = new List<KYFCropExtract>();
                            foreach (JObject cropObject in cropsArray)
                            {
                                JArray content =
                                    cropObject["content"] as JArray;

                                if (content != null && content.Any())
                                {
                                    // Extract the crop name.
                                    string cropName = content
                                        .FirstOrDefault(item =>
                                            item["name"] != null
                                            && item["name"].ToString()
                                                == "crop_name"
                                        )
                                        ?["value"]?.ToString();

                                    // Extract the acreage for this crop.
                                    string cropAcreageStr = content
                                        .FirstOrDefault(item =>
                                            item["name"] != null
                                            && (item["name"].ToString()
                                                == "acreage_annual_crop")
                                        )
                                        ?["value"]?.ToString();

                                    // Extract the for all crops.
                                    string totalCropAcreageStr = content
                                        .FirstOrDefault(item =>
                                            item["name"] != null
                                            && (item["name"].ToString()
                                                == "area_under_crops")
                                        )
                                        ?["value"]?.ToString();

                                    string numberOfTrees = content
                                        .FirstOrDefault(item =>
                                            item["name"] != null
                                            && (item["name"].ToString()
                                                == "number_perenial_crop")
                                        )
                                        ?["value"]?.ToString();
                                    var totalCropAcreage = 0f;
                                    var numberOfCropTrees = 0f;

                                    if (
                                        float.TryParse(
                                            cropAcreageStr,
                                            out float cropAcreage
                                        )
                                        || float.TryParse(totalCropAcreageStr,
                                        out totalCropAcreage)

                                    )
                                    {
                                        float.TryParse(numberOfTrees,
                                       out numberOfCropTrees);
                                        crops.Add(
                                            new KYFCropExtract(numberOfTrees, cropName)
                                            {
                                                ParcelId = $"PCL{parcelCounter.ToString().PadLeft(4, '0')}",
                                                CropName = cropName,
                                                TotalCropAcreage = cropAcreage,
                                                TotalAreaUnderCrops = totalCropAcreage,
                                                NumberOfTrees = numberOfCropTrees,
                                            }
                                        );
                                    }
                                }
                            }
                            if (crops != null && crops.Any())
                            {
                                rawResult.Crops.AddRange(crops.ToArray());
                            }
                        }

                        //Extract coffee details
                        JArray coffeeArray = parcel["coffee"] as JArray;
                        if (coffeeArray != null && coffeeArray.Any())
                        {
                            var crops = new List<KYFCropExtract>();
                            foreach (JObject coffeeObject in coffeeArray)
                            {
                                JArray content =
                                    coffeeObject["content"] as JArray;

                                if (content != null && content.Any())
                                {
                                    // Extract the crop name.
                                    string cropName = content
                                        .FirstOrDefault(item =>
                                            item["name"] != null
                                            && item["name"].ToString()
                                                == "crop_name"
                                        )
                                        ?["value"]?.ToString();

                                    // Extract the acreage for this crop.
                                    string numberOfTreesStr = content
                                        .FirstOrDefault(item =>
                                            item["name"] != null
                                            && item["name"].ToString()
                                                == "number_coffee_crop"
                                        )
                                        ?["value"]?.ToString();


                                    // Extract the acreage.
                                    string totalCropAcreageStr = content
                                        .FirstOrDefault(item =>
                                            item["name"] != null
                                            && item["name"].ToString()
                                                == "area_under_crops"
                                        )
                                        ?["value"]?.ToString();

                                    float.TryParse(
                                            totalCropAcreageStr,
                                            out float totalCropAcreage
                                            );

                                    if (
                                        float.TryParse(
                                            numberOfTreesStr,
                                            out float numberOfTrees
                                        )
                                    )
                                    {
                                        crops.Add(
                                            new KYFCropExtract(numberOfTreesStr, cropName)
                                            {
                                                ParcelId = $"PCL{parcelCounter.ToString().PadLeft(4, '0')}",
                                                CropName = cropName,
                                                TotalCropAcreage =
                                                    numberOfTrees,
                                                TotalAreaUnderCrops = totalCropAcreage,
                                                NumberOfTrees = numberOfTrees
                                            }
                                        );
                                    }
                                }


                            }

                            if (crops != null && crops.Any())
                            {
                                //var summedCropDetails = crops
                                //.GroupBy(c => c.CropName)
                                //.Select(g => new KYFCropExtract(g.Sum(c => c.NumberOfTrees).ToString(), g.Key)
                                //{
                                //    CropName = g.Key,
                                //    TotalCropAcreage = g.Sum(c =>
                                //        c.TotalCropAcreage
                                //    ),
                                //    TotalAreaUnderCrops = g.Sum(c => c.TotalAreaUnderCrops),
                                //    NumberOfTrees = g.Sum(c => c.NumberOfTrees),
                                //})
                                //.ToList();

                                //rawResult.Crops.AddRange(summedCropDetails);

                                rawResult.Crops.AddRange(crops.ToArray());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            $"Error getting crops in parcel: {ex.Message}"
                        );
                    }
                    //Console.WriteLine($"Parcel count: {parcelCounter}");

                    parcelCounter++;
                }
            }
            return rawResult;
        }

        /// <summary>
        /// The GetKYFWardFarmerDetails
        /// </summary>
        /// <param name="county">The county<see cref="string"/></param>
        /// <param name="subcounty">The subcounty<see cref="string"/></param>
        /// <param name="ward">The ward<see cref="string"/></param>
        /// <returns>The <see cref="Task{List{KYFFarmerDetailModel}}"/></returns>
        public static async Task<List<KYFFarmerDetailModel>> GetKYFWardFarmerDetails(
            string county,
            string subcounty,
            string ward
        )
        {
            var rawResultsBag = new ConcurrentBag<KYFFarmerDetailModel>();
            try
            {
                var collection = KYFDataCollection();

                var farmerBios = await GetFarmersBio(county, subcounty, ward);

                if (farmerBios.Data != null && farmerBios.Data.Farmers != null && farmerBios.Data.Farmers.Any())
                {
                    var missingRecords = new List<FarmerBioModel>();
                    var ids = farmerBios.Data.Farmers.Where(x => !string.IsNullOrEmpty(x.MongoId)).Select(x => Helpers.CleanStrict(x.MongoId.Trim())).ToList().Distinct().ToList();

                    #region Process Query for multiple ids
                    Parallel.ForEach(ids.Chunk(50), idChunk =>
                    {
                        var objectIdArray = new BsonArray(idChunk.Select(ObjectId.Parse));


                        var filter = Builders<BsonDocument>.Filter.In("_id", objectIdArray);

                        #region Process Queries for batch
                        var options = new FindOptions<BsonDocument>
                        {
                            NoCursorTimeout = true
                        };

                        #region Processing
                        using (var cursor = collection.FindAsync(filter, options).GetAwaiter().GetResult())
                        {
                            while (cursor.MoveNextAsync().GetAwaiter().GetResult())
                            {
                                if (cursor.Current != null && cursor.Current.Any())
                                {
                                    foreach (var document in cursor.Current)
                                    {
                                        #region Cursor Processing
                                        var rawResult = new KYFFarmerDetailModel()
                                        {
                                            County = county,
                                            Subcounty = subcounty,
                                            Ward = ward,
                                        };
                                        try
                                        {
                                            // Convert the document to a JSON string and parse into a JObject.
                                            string jsonString = document.ToJson();
                                            JObject jObject = JObject.Parse(jsonString);

                                            // The JSON is expected to have a "data" array with multiple sections.
                                            JArray sections = jObject["data"] as JArray;

                                            rawResult.RegistrationStatus = jObject["registration_status"]
                                                ?.ToString();

                                            rawResult.FarmerStatus = jObject["farmer_status"]?.ToString();

                                            if (sections != null)
                                            {
                                                // Extract administrative details from Section A.
                                                JObject sectionA =
                                                    sections.FirstOrDefault(s =>
                                                        s["name"]?.ToString().Contains("Section A") == true
                                                    ) as JObject;

                                                if (sectionA != null)
                                                {
                                                    // Extract administrative details from Section A.
                                                    JArray contentA = sectionA["content"] as JArray;
                                                    if (contentA != null)
                                                    {
                                                        rawResult.RecordDate =
                                                            contentA
                                                                .FirstOrDefault(c =>
                                                                    c["label"]
                                                                        ?.ToString()
                                                                        .ToLower()
                                                                        .Contains("date") == true
                                                                )
                                                                ?["value"]?.ToString() ?? "";

                                                        try
                                                        {
                                                            // Extract household GPS from the field with name "gps" and type "gps"
                                                            JObject gpsField =
                                                                contentA.FirstOrDefault(obj =>
                                                                    (string)obj["name"] == "gps"
                                                                    && (string)obj["type"] == "gps"
                                                                ) as JObject;

                                                            if (gpsField != null)
                                                            {
                                                                JObject coords =
                                                                    gpsField["value"]["coords"] as JObject;
                                                                rawResult.Latitude = coords["latitude"]?.ToString();
                                                                rawResult.Longitude = coords["longitude"]
                                                                    ?.ToString();

                                                                //Console.WriteLine($"Coordinates =>\nLatitude: {latitude}\nLongitude: {longitude}");
                                                            }
                                                        }
                                                        catch (Exception) { }
                                                    }
                                                }

                                                // Extract farmer personal details from Section B.
                                                JObject sectionB =
                                                    sections.FirstOrDefault(s =>
                                                        s["name"]?.ToString().Contains("Section B") == true
                                                    ) as JObject;

                                                string farmerName = "",
                                                    farmerId = "",
                                                    farmerPhone = "",
                                                    farmerGender = "";
                                                if (sectionB != null)
                                                {
                                                    JArray contentB = sectionB["content"] as JArray;
                                                    if (contentB != null)
                                                    {
                                                        rawResult.Name =
                                                            contentB
                                                                .FirstOrDefault(c =>
                                                                    c["q_code"]?.ToString() == "B02"
                                                                )
                                                                ?["value"]?.ToString() ?? "";
                                                        rawResult.NationalID =
                                                            contentB
                                                                .FirstOrDefault(c =>
                                                                    c["q_code"]?.ToString() == "B03"
                                                                )
                                                                ?["value"]?.ToString() ?? "";
                                                        rawResult.MobileNumber =
                                                            contentB
                                                                .FirstOrDefault(c =>
                                                                    c["q_code"]?.ToString() == "B04"
                                                                )
                                                                ?["value"]?.ToString() ?? "";
                                                        rawResult.Sex =
                                                            contentB
                                                                .FirstOrDefault(c =>
                                                                    c["q_code"]?.ToString() == "B05"
                                                                )
                                                                ?["value"]?.ToString() ?? "";

                                                        rawResult.YOB =
                                                            sectionB["content"]
                                                                .FirstOrDefault(obj =>
                                                                    (string)obj["name"] == "farmer_yob"
                                                                )
                                                                ?["value"]?.ToString() ?? "";
                                                    }
                                                }

                                                // Extract parcel details from Section C.
                                                JObject sectionC =
                                                    sections.FirstOrDefault(s =>
                                                        s["name"]?.ToString().Contains("Section C") == true
                                                    ) as JObject;

                                                if (sectionC != null)
                                                {
                                                    JArray parcels = sectionC["parcels"] as JArray;
                                                    if (parcels != null && parcels.Any())
                                                    {
                                                        var cropsResult = ProcessCropsData(parcels);

                                                        if (cropsResult.Crops.Any() && cropsResult.Parcels.Any())
                                                        {
                                                            rawResult.Crops.AddRange(cropsResult.Crops);
                                                            rawResult.Parcels.AddRange(cropsResult.Parcels);
                                                        }
                                                        #region Old
                                                        //foreach (JObject parcel in parcels)
                                                        //{
                                                        //    JArray parcelContent = parcel["content"] as JArray;

                                                        //    try
                                                        //    {
                                                        //        try
                                                        //        {
                                                        //            //JArray parcelContent = parcel["content"] as JArray;
                                                        //            string acreage =
                                                        //                parcelContent
                                                        //                    ?.FirstOrDefault(c =>
                                                        //                        c["name"]
                                                        //                            ?.ToString()
                                                        //                            .Equals(
                                                        //                                "parcel_total_acreage",
                                                        //                                StringComparison.OrdinalIgnoreCase
                                                        //                            ) == true
                                                        //                    )
                                                        //                    ?["value"]?.ToString() ?? "";
                                                        //            string parcelOwnershipStatus =
                                                        //                parcelContent
                                                        //                    ?.FirstOrDefault(c =>
                                                        //                        c["name"]
                                                        //                            ?.ToString()
                                                        //                            .Equals(
                                                        //                                "ownership_status",
                                                        //                                StringComparison.OrdinalIgnoreCase
                                                        //                            ) == true
                                                        //                    )
                                                        //                    ?["value"]?.FirstOrDefault()["value"]
                                                        //                    ?.ToString() ?? "";

                                                        //            rawResult.Parcels.Add(
                                                        //                new ParcelModel
                                                        //                {
                                                        //                    Acreage = acreage,
                                                        //                    OwnershipStatus = parcelOwnershipStatus,
                                                        //                }
                                                        //            );
                                                        //        }
                                                        //        catch (Exception) { }

                                                        //        // Extract crop details from the key "crops" under the parcel.
                                                        //        JArray cropsArray = parcel["crops"] as JArray;
                                                        //        if (cropsArray != null && cropsArray.Any())
                                                        //        {
                                                        //            var crops = new List<KYFCropDetail>();
                                                        //            foreach (JObject cropObject in cropsArray)
                                                        //            {
                                                        //                JArray content =
                                                        //                    cropObject["content"] as JArray;

                                                        //                if (content != null && content.Any())
                                                        //                {
                                                        //                    // Extract the crop name.
                                                        //                    string cropName = content
                                                        //                        .FirstOrDefault(item =>
                                                        //                            item["name"] != null
                                                        //                            && item["name"].ToString()
                                                        //                                == "crop_name"
                                                        //                        )
                                                        //                        ?["value"]?.ToString();

                                                        //                    // Extract the acreage for this crop.
                                                        //                    string cropAcreageStr = content
                                                        //                        .FirstOrDefault(item =>
                                                        //                            item["name"] != null
                                                        //                            && (item["name"].ToString()
                                                        //                                == "acreage_annual_crop")
                                                        //                        )
                                                        //                        ?["value"]?.ToString();

                                                        //                    // Extract the for all crops.
                                                        //                    string totalCropAcreageStr = content
                                                        //                        .FirstOrDefault(item =>
                                                        //                            item["name"] != null
                                                        //                            && (item["name"].ToString()
                                                        //                                == "area_under_crops")
                                                        //                        )
                                                        //                        ?["value"]?.ToString();

                                                        //                    string numberOfTrees = content
                                                        //                        .FirstOrDefault(item =>
                                                        //                            item["name"] != null
                                                        //                            && (item["name"].ToString()
                                                        //                                == "number_perenial_crop")
                                                        //                        )
                                                        //                        ?["value"]?.ToString();
                                                        //                    var totalCropAcreage = 0f;
                                                        //                    var numberOfCropTrees = 0f;

                                                        //                    if (
                                                        //                        float.TryParse(
                                                        //                            cropAcreageStr,
                                                        //                            out float cropAcreage
                                                        //                        )
                                                        //                        || float.TryParse(totalCropAcreageStr,
                                                        //                        out totalCropAcreage)

                                                        //                    )
                                                        //                    {
                                                        //                        float.TryParse(numberOfTrees,
                                                        //                       out numberOfCropTrees);
                                                        //                        crops.Add(
                                                        //                            new KYFCropDetail(numberOfTrees, cropName)
                                                        //                            {
                                                        //                                CropName = cropName,
                                                        //                                TotalCropAcreage = cropAcreage,
                                                        //                                TotalAreaUnderCrops = totalCropAcreage,
                                                        //                                NumberOfTrees = numberOfCropTrees,
                                                        //                            }
                                                        //                        );
                                                        //                    }
                                                        //                }
                                                        //            }
                                                        //            if (crops != null && crops.Any())
                                                        //            {
                                                        //                var summedCropDetails = crops
                                                        //                    .GroupBy(c => c.CropName)
                                                        //                    .Select(g => new KYFCropDetail(g.Sum(c => c.NumberOfTrees).ToString(), g.Key)
                                                        //                    {
                                                        //                        CropName = g.Key,
                                                        //                        TotalCropAcreage = g.Sum(c =>
                                                        //                            c.TotalCropAcreage
                                                        //                        ),
                                                        //                        TotalAreaUnderCrops = g.Sum(c => c.TotalAreaUnderCrops),
                                                        //                        NumberOfTrees = g.Sum(c => c.NumberOfTrees),
                                                        //                    })
                                                        //                    .ToList();

                                                        //                rawResult.Crops.AddRange(summedCropDetails);
                                                        //            }
                                                        //        }

                                                        //        //Extract coffee details
                                                        //        JArray coffeeArray = parcel["coffee"] as JArray;
                                                        //        if (coffeeArray != null && coffeeArray.Any())
                                                        //        {
                                                        //            var crops = new List<KYFCropDetail>();
                                                        //            foreach (JObject coffeeObject in coffeeArray)
                                                        //            {
                                                        //                JArray content =
                                                        //                    coffeeObject["content"] as JArray;

                                                        //                if (content != null && content.Any())
                                                        //                {
                                                        //                    // Extract the crop name.
                                                        //                    string cropName = content
                                                        //                        .FirstOrDefault(item =>
                                                        //                            item["name"] != null
                                                        //                            && item["name"].ToString()
                                                        //                                == "crop_name"
                                                        //                        )
                                                        //                        ?["value"]?.ToString();

                                                        //                    // Extract the acreage for this crop.
                                                        //                    string numberOfTreesStr = content
                                                        //                        .FirstOrDefault(item =>
                                                        //                            item["name"] != null
                                                        //                            && item["name"].ToString()
                                                        //                                == "number_coffee_crop"
                                                        //                        )
                                                        //                        ?["value"]?.ToString();


                                                        //                    // Extract the acreage.
                                                        //                    string totalCropAcreageStr = content
                                                        //                        .FirstOrDefault(item =>
                                                        //                            item["name"] != null
                                                        //                            && item["name"].ToString()
                                                        //                                == "area_under_crops"
                                                        //                        )
                                                        //                        ?["value"]?.ToString();

                                                        //                    float.TryParse(
                                                        //                            totalCropAcreageStr,
                                                        //                            out float totalCropAcreage
                                                        //                            );

                                                        //                    if (
                                                        //                        float.TryParse(
                                                        //                            numberOfTreesStr,
                                                        //                            out float numberOfTrees
                                                        //                        )
                                                        //                    )
                                                        //                    {
                                                        //                        crops.Add(
                                                        //                            new KYFCropDetail(numberOfTreesStr, cropName)
                                                        //                            {
                                                        //                                CropName = cropName,
                                                        //                                TotalCropAcreage =
                                                        //                                    numberOfTrees,
                                                        //                                TotalAreaUnderCrops = totalCropAcreage,
                                                        //                                NumberOfTrees = numberOfTrees
                                                        //                            }
                                                        //                        );
                                                        //                    }
                                                        //                }


                                                        //            }

                                                        //            if (crops != null && crops.Any())
                                                        //            {
                                                        //                var summedCropDetails = crops
                                                        //                .GroupBy(c => c.CropName)
                                                        //                .Select(g => new KYFCropDetail(g.Sum(c => c.NumberOfTrees).ToString(), g.Key)
                                                        //                {
                                                        //                    CropName = g.Key,
                                                        //                    TotalCropAcreage = g.Sum(c =>
                                                        //                        c.TotalCropAcreage
                                                        //                    ),
                                                        //                    TotalAreaUnderCrops = g.Sum(c => c.TotalAreaUnderCrops),
                                                        //                    NumberOfTrees = g.Sum(c => c.NumberOfTrees),
                                                        //                })
                                                        //                .ToList();

                                                        //                rawResult.Crops.AddRange(summedCropDetails);
                                                        //            }
                                                        //        }
                                                        //    }
                                                        //    catch (Exception ex)
                                                        //    {
                                                        //        Console.WriteLine(
                                                        //            $"Error getting crops in parcel: {ex.Message}"
                                                        //        );
                                                        //    }
                                                        //}
                                                        #endregion
                                                    }
                                                }

                                                JObject sectionF =
                                                 sections.FirstOrDefault(s =>
                                                        s["name"]?.ToString().Equals("Section F") == true
                                                    ) as JObject;

                                                if (sectionF != null)
                                                {
                                                    JArray livestockContent = sectionF["content"] as JArray;

                                                    try
                                                    {
                                                        var livestockData = ProcessLivestockData(livestockContent, sections);
                                                        //Console.WriteLine($"Livestock count: {livestockData.Count}");
                                                        if (livestockData != null && livestockData.Any())
                                                        {
                                                            rawResult.Livestock.AddRange(livestockData);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Console.WriteLine($"Error processing livestock data: {ex.Message}");
                                                    }

                                                }

                                                if (rawResult.Crops != null && rawResult.Crops.Any())
                                                {
                                                    rawResultsBag.Add(rawResult);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Error processing document: {ex.Message}");
                                        }
                                        #endregion
                                    }
                                }
                            }
                        }
                        #endregion

                        #endregion
                    });
                    #endregion

                    //Find the missing ids
                    var tempResult = rawResultsBag.ToList();
                    var missing = farmerBios.Data.Farmers.Where(x => !tempResult.Any(r => r.NationalID == x.NationalId && r.MobileNumber == x.FarmerPhone))?.ToList();
                    var missingIds = missing.Select(m => m.MongoId).ToList();

                    #region Process Query for missing single ids
                    Parallel.ForEach(missingIds.Chunk(100), ids =>
                    {
                        for (int i = 0; i < ids.Length; i++)
                        {
                            #region Process query for single Id


                            var filters = new List<FilterDefinition<BsonDocument>>();

                            // try ObjectId branch
                            if (ObjectId.TryParse(ids[i], out var oid))
                                filters.Add(Builders<BsonDocument>.Filter.Eq("_id", oid));

                            // always include string branch
                            filters.Add(Builders<BsonDocument>.Filter.Eq("_id", ids[i]));


                            var filter = Builders<BsonDocument>.Filter.Or(filters);

                            //var orArray = new BsonArray();

                            // 2. Parse the 24-char hex into an ObjectId
                            //var objectId = ObjectId.Parse(Helpers.CleanStrict(ids[i]));



                            //orArray.Add(new BsonDocument("_id", objectId));
                            //orArray.Add(new BsonDocument("_id", Helpers.CleanStrict(ids[i])));

                            // 3. Build an equality filter on "_id"
                            //var filter = new BsonDocument("_id", objectId);

                            //var filter = new BsonDocument("$or", orArray);

                            //var filter = Builders<BsonDocument>.Filter.In("_id", orArray);

                            var args = new RenderArgs<BsonDocument>(
    collection.DocumentSerializer,                        // IBsonSerializer<BsonDocument>
     BsonSerializer.SerializerRegistry     // IBsonSerializerRegistry
);
                            var options = new FindOptions<BsonDocument>
                            {
                                NoCursorTimeout = true
                            };

                            #region Processing
                            using (var cursor = collection.FindAsync(filter, options).GetAwaiter().GetResult())
                            {
                                while (cursor.MoveNextAsync().GetAwaiter().GetResult())
                                {
                                    if (cursor.Current != null && cursor.Current.Any())
                                    {
                                        var document = cursor.Current.FirstOrDefault();
                                        #region Cursor Processing
                                        var rawResult = new KYFFarmerDetailModel()
                                        {
                                            County = county,
                                            Subcounty = subcounty,
                                            Ward = ward,
                                        };
                                        try
                                        {
                                            // Convert the document to a JSON string and parse into a JObject.
                                            string jsonString = document.ToJson();
                                            JObject jObject = JObject.Parse(jsonString);

                                            // The JSON is expected to have a "data" array with multiple sections.
                                            JArray sections = jObject["data"] as JArray;

                                            rawResult.RegistrationStatus = jObject["registration_status"]
                                                ?.ToString();

                                            rawResult.FarmerStatus = jObject["farmer_status"]?.ToString();

                                            if (sections != null)
                                            {
                                                // Extract administrative details from Section A.
                                                JObject sectionA =
                                                    sections.FirstOrDefault(s =>
                                                        s["name"]?.ToString().Contains("Section A") == true
                                                    ) as JObject;

                                                if (sectionA != null)
                                                {
                                                    // Extract administrative details from Section A.
                                                    JArray contentA = sectionA["content"] as JArray;
                                                    if (contentA != null)
                                                    {
                                                        rawResult.RecordDate =
                                                            contentA
                                                                .FirstOrDefault(c =>
                                                                    c["label"]
                                                                        ?.ToString()
                                                                        .ToLower()
                                                                        .Contains("date") == true
                                                                )
                                                                ?["value"]?.ToString() ?? "";

                                                        try
                                                        {
                                                            // Extract household GPS from the field with name "gps" and type "gps"
                                                            JObject gpsField =
                                                                contentA.FirstOrDefault(obj =>
                                                                    (string)obj["name"] == "gps"
                                                                    && (string)obj["type"] == "gps"
                                                                ) as JObject;

                                                            if (gpsField != null)
                                                            {
                                                                JObject coords =
                                                                    gpsField["value"]["coords"] as JObject;
                                                                rawResult.Latitude = coords["latitude"]?.ToString();
                                                                rawResult.Longitude = coords["longitude"]
                                                                    ?.ToString();

                                                                //Console.WriteLine($"Coordinates =>\nLatitude: {latitude}\nLongitude: {longitude}");
                                                            }
                                                        }
                                                        catch (Exception) { }
                                                    }
                                                }

                                                // Extract farmer personal details from Section B.
                                                JObject sectionB =
                                                    sections.FirstOrDefault(s =>
                                                        s["name"]?.ToString().Contains("Section B") == true
                                                    ) as JObject;

                                                string farmerName = "",
                                                    farmerId = "",
                                                    farmerPhone = "",
                                                    farmerGender = "";
                                                if (sectionB != null)
                                                {
                                                    JArray contentB = sectionB["content"] as JArray;
                                                    if (contentB != null)
                                                    {
                                                        rawResult.Name =
                                                            contentB
                                                                .FirstOrDefault(c =>
                                                                    c["q_code"]?.ToString() == "B02"
                                                                )
                                                                ?["value"]?.ToString() ?? "";
                                                        rawResult.NationalID =
                                                            contentB
                                                                .FirstOrDefault(c =>
                                                                    c["q_code"]?.ToString() == "B03"
                                                                )
                                                                ?["value"]?.ToString() ?? "";
                                                        rawResult.MobileNumber =
                                                            contentB
                                                                .FirstOrDefault(c =>
                                                                    c["q_code"]?.ToString() == "B04"
                                                                )
                                                                ?["value"]?.ToString() ?? "";
                                                        rawResult.Sex =
                                                            contentB
                                                                .FirstOrDefault(c =>
                                                                    c["q_code"]?.ToString() == "B05"
                                                                )
                                                                ?["value"]?.ToString() ?? "";

                                                        rawResult.YOB =
                                                            sectionB["content"]
                                                                .FirstOrDefault(obj =>
                                                                    (string)obj["name"] == "farmer_yob"
                                                                )
                                                                ?["value"]?.ToString() ?? "";
                                                    }
                                                }

                                                // Extract parcel details from Section C.
                                                JObject sectionC =
                                                    sections.FirstOrDefault(s =>
                                                        s["name"]?.ToString().Contains("Section C") == true
                                                    ) as JObject;

                                                if (sectionC != null)
                                                {
                                                    JArray parcels = sectionC["parcels"] as JArray;
                                                    if (parcels != null && parcels.Any())
                                                    {
                                                        var cropsData = ProcessCropsData(parcels);

                                                        if (cropsData.Crops.Any() && cropsData.Parcels.Any())
                                                        {
                                                            rawResult.Crops.AddRange(cropsData.Crops);
                                                            rawResult.Parcels.AddRange(cropsData.Parcels);
                                                        }
                                                        #region Old
                                                        //foreach (JObject parcel in parcels)
                                                        //{
                                                        //    JArray parcelContent = parcel["content"] as JArray;

                                                        //    try
                                                        //    {
                                                        //        try
                                                        //        {
                                                        //            //JArray parcelContent = parcel["content"] as JArray;
                                                        //            string acreage =
                                                        //                parcelContent
                                                        //                    ?.FirstOrDefault(c =>
                                                        //                        c["name"]
                                                        //                            ?.ToString()
                                                        //                            .Equals(
                                                        //                                "parcel_total_acreage",
                                                        //                                StringComparison.OrdinalIgnoreCase
                                                        //                            ) == true
                                                        //                    )
                                                        //                    ?["value"]?.ToString() ?? "";
                                                        //            string parcelOwnershipStatus =
                                                        //                parcelContent
                                                        //                    ?.FirstOrDefault(c =>
                                                        //                        c["name"]
                                                        //                            ?.ToString()
                                                        //                            .Equals(
                                                        //                                "ownership_status",
                                                        //                                StringComparison.OrdinalIgnoreCase
                                                        //                            ) == true
                                                        //                    )
                                                        //                    ?["value"]?.FirstOrDefault()["value"]
                                                        //                    ?.ToString() ?? "";

                                                        //            rawResult.Parcels.Add(
                                                        //                new ParcelModel
                                                        //                {
                                                        //                    Acreage = acreage,
                                                        //                    OwnershipStatus = parcelOwnershipStatus,
                                                        //                }
                                                        //            );
                                                        //        }
                                                        //        catch (Exception) { }

                                                        //        // Extract crop details from the key "crops" under the parcel.
                                                        //        JArray cropsArray = parcel["crops"] as JArray;
                                                        //        if (cropsArray != null && cropsArray.Any())
                                                        //        {
                                                        //            var crops = new List<KYFCropDetail>();
                                                        //            foreach (JObject cropObject in cropsArray)
                                                        //            {
                                                        //                JArray content =
                                                        //                    cropObject["content"] as JArray;

                                                        //                if (content != null && content.Any())
                                                        //                {
                                                        //                    // Extract the crop name.
                                                        //                    string cropName = content
                                                        //                        .FirstOrDefault(item =>
                                                        //                            item["name"] != null
                                                        //                            && item["name"].ToString()
                                                        //                                == "crop_name"
                                                        //                        )
                                                        //                        ?["value"]?.ToString();

                                                        //                    // Extract the acreage for this crop.
                                                        //                    string cropAcreageStr = content
                                                        //                        .FirstOrDefault(item =>
                                                        //                            item["name"] != null
                                                        //                            && (item["name"].ToString()
                                                        //                                == "acreage_annual_crop")
                                                        //                        )
                                                        //                        ?["value"]?.ToString();

                                                        //                    // Extract the for all crops.
                                                        //                    string totalCropAcreageStr = content
                                                        //                        .FirstOrDefault(item =>
                                                        //                            item["name"] != null
                                                        //                            && (item["name"].ToString()
                                                        //                                == "area_under_crops")
                                                        //                        )
                                                        //                        ?["value"]?.ToString();

                                                        //                    string numberOfTrees = content
                                                        //                        .FirstOrDefault(item =>
                                                        //                            item["name"] != null
                                                        //                            && (item["name"].ToString()
                                                        //                                == "number_perenial_crop")
                                                        //                        )
                                                        //                        ?["value"]?.ToString();
                                                        //                    var totalCropAcreage = 0f;
                                                        //                    var numberOfCropTrees = 0f;

                                                        //                    if (
                                                        //                        float.TryParse(
                                                        //                            cropAcreageStr,
                                                        //                            out float cropAcreage
                                                        //                        )
                                                        //                        || float.TryParse(totalCropAcreageStr,
                                                        //                        out totalCropAcreage)

                                                        //                    )
                                                        //                    {
                                                        //                        float.TryParse(numberOfTrees,
                                                        //                        out numberOfCropTrees);
                                                        //                        crops.Add(
                                                        //                            new KYFCropDetail(numberOfTrees, cropName)
                                                        //                            {
                                                        //                                CropName = cropName,
                                                        //                                TotalCropAcreage = cropAcreage,
                                                        //                                TotalAreaUnderCrops = totalCropAcreage,
                                                        //                                NumberOfTrees = numberOfCropTrees,
                                                        //                            }
                                                        //                        );
                                                        //                    }
                                                        //                }
                                                        //            }
                                                        //            if (crops != null && crops.Any())
                                                        //            {
                                                        //                var summedCropDetails = crops
                                                        //                    .GroupBy(c => c.CropName)
                                                        //                    .Select(g => new KYFCropDetail(g.Sum(c => c.NumberOfTrees).ToString(), g.Key)
                                                        //                    {
                                                        //                        CropName = g.Key,
                                                        //                        TotalCropAcreage = g.Sum(c =>
                                                        //                            c.TotalCropAcreage
                                                        //                        ),
                                                        //                        TotalAreaUnderCrops = g.Sum(c => c.TotalAreaUnderCrops),
                                                        //                        NumberOfTrees = g.Sum(c => c.NumberOfTrees),
                                                        //                    })
                                                        //                    .ToList();

                                                        //                rawResult.Crops.AddRange(summedCropDetails);
                                                        //            }
                                                        //        }

                                                        //        //Extract coffee details
                                                        //        JArray coffeeArray = parcel["coffee"] as JArray;
                                                        //        if (coffeeArray != null && coffeeArray.Any())
                                                        //        {
                                                        //            var crops = new List<KYFCropDetail>();
                                                        //            foreach (JObject coffeeObject in coffeeArray)
                                                        //            {
                                                        //                JArray content =
                                                        //                    coffeeObject["content"] as JArray;

                                                        //                if (content != null && content.Any())
                                                        //                {
                                                        //                    // Extract the crop name.
                                                        //                    string cropName = content
                                                        //                        .FirstOrDefault(item =>
                                                        //                            item["name"] != null
                                                        //                            && item["name"].ToString()
                                                        //                                == "crop_name"
                                                        //                        )
                                                        //                        ?["value"]?.ToString();

                                                        //                    // Extract the acreage for this crop.
                                                        //                    string numberOfTreesStr = content
                                                        //                        .FirstOrDefault(item =>
                                                        //                            item["name"] != null
                                                        //                            && item["name"].ToString()
                                                        //                                == "number_coffee_crop"
                                                        //                        )
                                                        //                        ?["value"]?.ToString();


                                                        //                    // Extract the acreage.
                                                        //                    string totalCropAcreageStr = content
                                                        //                        .FirstOrDefault(item =>
                                                        //                            item["name"] != null
                                                        //                            && item["name"].ToString()
                                                        //                                == "area_under_crops"
                                                        //                        )
                                                        //                        ?["value"]?.ToString();

                                                        //                    float.TryParse(
                                                        //                            totalCropAcreageStr,
                                                        //                            out float totalCropAcreage
                                                        //                            );

                                                        //                    if (
                                                        //                        float.TryParse(
                                                        //                            numberOfTreesStr,
                                                        //                            out float numberOfTrees
                                                        //                        )
                                                        //                    )
                                                        //                    {
                                                        //                        crops.Add(
                                                        //                            new KYFCropDetail(numberOfTreesStr, cropName)
                                                        //                            {
                                                        //                                CropName = cropName,
                                                        //                                TotalCropAcreage =
                                                        //                                    numberOfTrees,
                                                        //                                TotalAreaUnderCrops = totalCropAcreage,
                                                        //                                NumberOfTrees = numberOfTrees
                                                        //                            }
                                                        //                        );
                                                        //                    }
                                                        //                }


                                                        //            }

                                                        //            if (crops != null && crops.Any())
                                                        //            {
                                                        //                var summedCropDetails = crops
                                                        //                .GroupBy(c => c.CropName)
                                                        //                .Select(g => new KYFCropDetail(g.Sum(c => c.NumberOfTrees).ToString(), g.Key)
                                                        //                {
                                                        //                    CropName = g.Key,
                                                        //                    TotalCropAcreage = g.Sum(c =>
                                                        //                        c.TotalCropAcreage
                                                        //                    ),
                                                        //                    TotalAreaUnderCrops = g.Sum(c => c.TotalAreaUnderCrops),
                                                        //                    NumberOfTrees = g.Sum(c => c.NumberOfTrees),
                                                        //                })
                                                        //                .ToList();

                                                        //                rawResult.Crops.AddRange(summedCropDetails);
                                                        //            }
                                                        //        }
                                                        //    }
                                                        //    catch (Exception ex)
                                                        //    {
                                                        //        Console.WriteLine(
                                                        //            $"Error getting crops in parcel: {ex.Message}"
                                                        //        );
                                                        //    }
                                                        //}
                                                        #endregion
                                                    }
                                                }
                                            }

                                            if (rawResult.Crops != null && rawResult.Crops.Any())
                                            {
                                                //Console.WriteLine("Here!!!55555");
                                                rawResultsBag.Add(rawResult);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Error processing document: {ex.Message}");
                                        }
                                        break;
                                        #endregion
                                    }
                                    else
                                    {

                                        var rendered = filter.Render(
       args
    )
    .ToJson(new MongoDB.Bson.IO.JsonWriterSettings { Indent = true });
                                        //Console.WriteLine("Query filter:\n" + rendered);

                                    }
                                }
                            }
                            #endregion

                            #endregion
                        }
                    });
                    #endregion
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error fetching and processing farmer data: {ex.Message}\nStack Trace: {ex.StackTrace}"
                );
            }

            return rawResultsBag.ToList();
        }


        #region Old County Extract
        //        public static async Task<List<KYFAPExtractModel>> GetKYFCountyFarmerDetails(
        //    string county, IMemoryCache dataCache
        //)
        //        {
        //            var rawResultsBag = new ConcurrentBag<KYFAPExtractModel>();
        //            try
        //            {
        //                var collection = KYFDataExtraction();

        //                var admins = APZoneCache.APZones.Where(ap => ap.County.ToLower().Trim() == county.ToLower().Trim())?
        //                    .Select(a => new KYFQueryAdminModel
        //                    {
        //                        County = a.County,
        //                        Subcounty = a.Subcounty,
        //                        Ward = a.Ward
        //                    })?.ToList();

        //                if (admins != null && admins.Any())
        //                {
        //                    Parallel.ForEach(admins, admin =>
        //                    {
        //                        var dataKey = admin.GetHashCode();
        //                        var farmerBios = GetFarmersBio(admin.County, admin.Subcounty, admin.Ward).GetAwaiter().GetResult();


        //                        if (farmerBios.Data != null && farmerBios.Data.Farmers != null && farmerBios.Data.Farmers.Any())
        //                        {
        //                            var missingRecords = new List<FarmerBioModel>();
        //                            var ids = farmerBios.Data.Farmers.Where(x => !string.IsNullOrEmpty(x.MongoId)).Select(x => Helpers.CleanStrict(x.MongoId.Trim())).ToList().Distinct().ToList();

        //                            #region Process Query for multiple ids
        //                            Parallel.ForEach(ids.Chunk(50), idChunk =>
        //                            {
        //                                var objectIdArray = new BsonArray(idChunk.Select(ObjectId.Parse));


        //                                var filter = Builders<BsonDocument>.Filter.In("_id", objectIdArray);

        //                                #region Process Queries for batch
        //                                var options = new FindOptions<BsonDocument>
        //                                {
        //                                    NoCursorTimeout = true
        //                                };

        //                                #region Processing
        //                                using (var cursor = collection.FindAsync(filter, options).GetAwaiter().GetResult())
        //                                {
        //                                    while (cursor.MoveNextAsync().GetAwaiter().GetResult())
        //                                    {
        //                                        if (cursor.Current != null && cursor.Current.Any())
        //                                        {
        //                                            foreach (var document in cursor.Current)
        //                                            {
        //                                                #region Cursor Processing
        //                                                var rawResult = new KYFAPExtractModel()
        //                                                {
        //                                                    County = admin.County,
        //                                                    Subcounty = admin.Subcounty,
        //                                                    Ward = admin.Ward,
        //                                                };
        //                                                try
        //                                                {
        //                                                    // Convert the document to a JSON string and parse into a JObject.
        //                                                    string jsonString = document.ToJson();
        //                                                    JObject jObject = JObject.Parse(jsonString);

        //                                                    // The JSON is expected to have a "data" array with multiple sections.
        //                                                    JArray sections = jObject["data"] as JArray;

        //                                                    rawResult.RegistrationStatus = jObject["registration_status"]
        //                                                        ?.ToString();

        //                                                    rawResult.FarmerStatus = jObject["farmer_status"]?.ToString();

        //                                                    if (sections != null)
        //                                                    {
        //                                                        // Extract administrative details from Section A.
        //                                                        JObject sectionA =
        //                                                            sections.FirstOrDefault(s =>
        //                                                                s["name"]?.ToString().Contains("Section A") == true
        //                                                            ) as JObject;

        //                                                        if (sectionA != null)
        //                                                        {
        //                                                            // Extract administrative details from Section A.
        //                                                            JArray contentA = sectionA["content"] as JArray;
        //                                                            if (contentA != null)
        //                                                            {
        //                                                                rawResult.RecordDate =
        //                                                                    contentA
        //                                                                        .FirstOrDefault(c =>
        //                                                                            c["label"]
        //                                                                                ?.ToString()
        //                                                                                .ToLower()
        //                                                                                .Contains("date") == true
        //                                                                        )
        //                                                                        ?["value"]?.ToString() ?? "";

        //                                                                try
        //                                                                {
        //                                                                    // Extract household GPS from the field with name "gps" and type "gps"
        //                                                                    JObject gpsField =
        //                                                                        contentA.FirstOrDefault(obj =>
        //                                                                            (string)obj["name"] == "gps"
        //                                                                            && (string)obj["type"] == "gps"
        //                                                                        ) as JObject;

        //                                                                    if (gpsField != null)
        //                                                                    {
        //                                                                        JObject coords =
        //                                                                            gpsField["value"]["coords"] as JObject;
        //                                                                        rawResult.Latitude = coords["latitude"]?.ToString();
        //                                                                        rawResult.Longitude = coords["longitude"]
        //                                                                            ?.ToString();

        //                                                                        //Console.WriteLine($"Coordinates =>\nLatitude: {latitude}\nLongitude: {longitude}");
        //                                                                    }
        //                                                                }
        //                                                                catch (Exception) { }
        //                                                            }
        //                                                        }

        //                                                        // Extract farmer personal details from Section B.
        //                                                        JObject sectionB =
        //                                                            sections.FirstOrDefault(s =>
        //                                                                s["name"]?.ToString().Contains("Section B") == true
        //                                                            ) as JObject;

        //                                                        string farmerName = "",
        //                                                            farmerId = "",
        //                                                            farmerPhone = "",
        //                                                            farmerGender = "";
        //                                                        if (sectionB != null)
        //                                                        {
        //                                                            JArray contentB = sectionB["content"] as JArray;
        //                                                            if (contentB != null)
        //                                                            {
        //                                                                rawResult.Name =
        //                                                                    contentB
        //                                                                        .FirstOrDefault(c =>
        //                                                                            c["q_code"]?.ToString() == "B02"
        //                                                                        )
        //                                                                        ?["value"]?.ToString() ?? "";
        //                                                                rawResult.NationalID =
        //                                                                    contentB
        //                                                                        .FirstOrDefault(c =>
        //                                                                            c["q_code"]?.ToString() == "B03"
        //                                                                        )
        //                                                                        ?["value"]?.ToString() ?? "";
        //                                                                rawResult.MobileNumber =
        //                                                                    contentB
        //                                                                        .FirstOrDefault(c =>
        //                                                                            c["q_code"]?.ToString() == "B04"
        //                                                                        )
        //                                                                        ?["value"]?.ToString() ?? "";
        //                                                                rawResult.Sex =
        //                                                                    contentB
        //                                                                        .FirstOrDefault(c =>
        //                                                                            c["q_code"]?.ToString() == "B05"
        //                                                                        )
        //                                                                        ?["value"]?.ToString() ?? "";

        //                                                                rawResult.YOB =
        //                                                                    sectionB["content"]
        //                                                                        .FirstOrDefault(obj =>
        //                                                                            (string)obj["name"] == "farmer_yob"
        //                                                                        )
        //                                                                        ?["value"]?.ToString() ?? "";
        //                                                            }
        //                                                        }

        //                                                        // Extract parcel details from Section C.
        //                                                        JObject sectionC =
        //                                                            sections.FirstOrDefault(s =>
        //                                                                s["name"]?.ToString().Contains("Section C") == true
        //                                                            ) as JObject;

        //                                                        if (sectionC != null)
        //                                                        {
        //                                                            JArray parcels = sectionC["parcels"] as JArray;
        //                                                            int parcelCounter = 0;
        //                                                            if (parcels != null && parcels.Any())
        //                                                            {
        //                                                                foreach (JObject parcel in parcels)
        //                                                                {
        //                                                                    JArray parcelContent = parcel["content"] as JArray;

        //                                                                    try
        //                                                                    {
        //                                                                        try
        //                                                                        {
        //                                                                            //JArray parcelContent = parcel["content"] as JArray;
        //                                                                            string acreage =
        //                                                                                parcelContent
        //                                                                                    ?.FirstOrDefault(c =>
        //                                                                                        c["name"]
        //                                                                                            ?.ToString()
        //                                                                                            .Equals(
        //                                                                                                "parcel_total_acreage",
        //                                                                                                StringComparison.OrdinalIgnoreCase
        //                                                                                            ) == true
        //                                                                                    )
        //                                                                                    ?["value"]?.ToString() ?? "";
        //                                                                            string parcelOwnershipStatus =
        //                                                                                parcelContent
        //                                                                                    ?.FirstOrDefault(c =>
        //                                                                                        c["name"]
        //                                                                                            ?.ToString()
        //                                                                                            .Equals(
        //                                                                                                "ownership_status",
        //                                                                                                StringComparison.OrdinalIgnoreCase
        //                                                                                            ) == true
        //                                                                                    )
        //                                                                                    ?["value"]?.FirstOrDefault()["value"]
        //                                                                                    ?.ToString() ?? "";

        //                                                                            rawResult.Parcels.Add(
        //                                                                                new ParcelExract
        //                                                                                {
        //                                                                                    Acreage = acreage,
        //                                                                                    OwnershipStatus = parcelOwnershipStatus,
        //                                                                                }
        //                                                                            );
        //                                                                        }
        //                                                                        catch (Exception) { }

        //                                                                        // Extract crop details from the key "crops" under the parcel.
        //                                                                        JArray cropsArray = parcel["crops"] as JArray;
        //                                                                        if (cropsArray != null && cropsArray.Any())
        //                                                                        {
        //                                                                            var crops = new List<KYFCropDetail>();
        //                                                                            foreach (JObject cropObject in cropsArray)
        //                                                                            {
        //                                                                                JArray content =
        //                                                                                    cropObject["content"] as JArray;

        //                                                                                if (content != null && content.Any())
        //                                                                                {
        //                                                                                    // Extract the crop name.
        //                                                                                    string cropName = content
        //                                                                                        .FirstOrDefault(item =>
        //                                                                                            item["name"] != null
        //                                                                                            && item["name"].ToString()
        //                                                                                                == "crop_name"
        //                                                                                        )
        //                                                                                        ?["value"]?.ToString();

        //                                                                                    // Extract the acreage for this crop.
        //                                                                                    string cropAcreageStr = content
        //                                                                                        .FirstOrDefault(item =>
        //                                                                                            item["name"] != null
        //                                                                                            && (item["name"].ToString()
        //                                                                                                == "acreage_annual_crop")
        //                                                                                        )
        //                                                                                        ?["value"]?.ToString();

        //                                                                                    // Extract the for all crops.
        //                                                                                    string totalCropAcreageStr = content
        //                                                                                        .FirstOrDefault(item =>
        //                                                                                            item["name"] != null
        //                                                                                            && (item["name"].ToString()
        //                                                                                                == "area_under_crops")
        //                                                                                        )
        //                                                                                        ?["value"]?.ToString();

        //                                                                                    string numberOfTrees = content
        //                                                                                        .FirstOrDefault(item =>
        //                                                                                            item["name"] != null
        //                                                                                            && (item["name"].ToString()
        //                                                                                                == "number_perenial_crop")
        //                                                                                        )
        //                                                                                        ?["value"]?.ToString();
        //                                                                                    var totalCropAcreage = 0f;
        //                                                                                    var numberOfCropTrees = 0f;

        //                                                                                    if (
        //                                                                                        float.TryParse(
        //                                                                                            cropAcreageStr,
        //                                                                                            out float cropAcreage
        //                                                                                        )
        //                                                                                        || float.TryParse(totalCropAcreageStr,
        //                                                                                        out totalCropAcreage)

        //                                                                                    )
        //                                                                                    {
        //                                                                                        float.TryParse(numberOfTrees,
        //                                                                                       out numberOfCropTrees);
        //                                                                                        crops.Add(
        //                                                                                            new KYFCropDetail(numberOfTrees, cropName)
        //                                                                                            {
        //                                                                                                CropName = cropName,
        //                                                                                                TotalCropAcreage = cropAcreage,
        //                                                                                                TotalAreaUnderCrops = totalCropAcreage,
        //                                                                                                NumberOfTrees = numberOfCropTrees,
        //                                                                                            }
        //                                                                                        );
        //                                                                                    }
        //                                                                                }
        //                                                                            }
        //                                                                            if (crops != null && crops.Any())
        //                                                                            {
        //                                                                                var summedCropDetails = crops
        //                                                                                    .GroupBy(c => c.CropName)
        //                                                                                    .Select(g => new KYFCropDetail(g.Sum(c => c.NumberOfTrees).ToString(), g.Key)
        //                                                                                    {
        //                                                                                        CropName = g.Key,
        //                                                                                        TotalCropAcreage = g.Sum(c =>
        //                                                                                            c.TotalCropAcreage
        //                                                                                        ),
        //                                                                                        TotalAreaUnderCrops = g.Sum(c => c.TotalAreaUnderCrops),
        //                                                                                        NumberOfTrees = g.Sum(c => c.NumberOfTrees),
        //                                                                                    })
        //                                                                                    .ToList();

        //                                                                                rawResult.Crops.AddRange(summedCropDetails);
        //                                                                            }
        //                                                                        }

        //                                                                        //Extract coffee details
        //                                                                        JArray coffeeArray = parcel["coffee"] as JArray;
        //                                                                        if (coffeeArray != null && coffeeArray.Any())
        //                                                                        {
        //                                                                            var crops = new List<KYFCropDetail>();
        //                                                                            foreach (JObject coffeeObject in coffeeArray)
        //                                                                            {
        //                                                                                JArray content =
        //                                                                                    coffeeObject["content"] as JArray;

        //                                                                                if (content != null && content.Any())
        //                                                                                {
        //                                                                                    // Extract the crop name.
        //                                                                                    string cropName = content
        //                                                                                        .FirstOrDefault(item =>
        //                                                                                            item["name"] != null
        //                                                                                            && item["name"].ToString()
        //                                                                                                == "crop_name"
        //                                                                                        )
        //                                                                                        ?["value"]?.ToString();

        //                                                                                    // Extract the acreage for this crop.
        //                                                                                    string numberOfTreesStr = content
        //                                                                                        .FirstOrDefault(item =>
        //                                                                                            item["name"] != null
        //                                                                                            && item["name"].ToString()
        //                                                                                                == "number_coffee_crop"
        //                                                                                        )
        //                                                                                        ?["value"]?.ToString();


        //                                                                                    // Extract the acreage.
        //                                                                                    string totalCropAcreageStr = content
        //                                                                                        .FirstOrDefault(item =>
        //                                                                                            item["name"] != null
        //                                                                                            && item["name"].ToString()
        //                                                                                                == "area_under_crops"
        //                                                                                        )
        //                                                                                        ?["value"]?.ToString();

        //                                                                                    float.TryParse(
        //                                                                                            totalCropAcreageStr,
        //                                                                                            out float totalCropAcreage
        //                                                                                            );

        //                                                                                    if (
        //                                                                                        float.TryParse(
        //                                                                                            numberOfTreesStr,
        //                                                                                            out float numberOfTrees
        //                                                                                        )
        //                                                                                    )
        //                                                                                    {
        //                                                                                        crops.Add(
        //                                                                                            new KYFCropDetail(numberOfTreesStr, cropName)
        //                                                                                            {
        //                                                                                                CropName = cropName,
        //                                                                                                TotalCropAcreage =
        //                                                                                                    numberOfTrees,
        //                                                                                                TotalAreaUnderCrops = totalCropAcreage,
        //                                                                                                NumberOfTrees = numberOfTrees
        //                                                                                            }
        //                                                                                        );
        //                                                                                    }
        //                                                                                }


        //                                                                            }

        //                                                                            if (crops != null && crops.Any())
        //                                                                            {
        //                                                                                var summedCropDetails = crops
        //                                                                                .GroupBy(c => c.CropName)
        //                                                                                .Select(g => new KYFCropDetail(g.Sum(c => c.NumberOfTrees).ToString(), g.Key)
        //                                                                                {
        //                                                                                    CropName = g.Key,
        //                                                                                    TotalCropAcreage = g.Sum(c =>
        //                                                                                        c.TotalCropAcreage
        //                                                                                    ),
        //                                                                                    TotalAreaUnderCrops = g.Sum(c => c.TotalAreaUnderCrops),
        //                                                                                    NumberOfTrees = g.Sum(c => c.NumberOfTrees),
        //                                                                                })
        //                                                                                .ToList();

        //                                                                                rawResult.Crops.AddRange(summedCropDetails);
        //                                                                            }
        //                                                                        }
        //                                                                    }
        //                                                                    catch (Exception ex)
        //                                                                    {
        //                                                                        Console.WriteLine(
        //                                                                            $"Error getting crops in parcel: {ex.Message}"
        //                                                                        );
        //                                                                    }
        //                                                                }
        //                                                            }
        //                                                        }

        //                                                        JObject sectionF =
        //                                                         sections.FirstOrDefault(s =>
        //                                                                s["name"]?.ToString().Equals("Section F") == true
        //                                                            ) as JObject;

        //                                                        if (sectionF != null)
        //                                                        {
        //                                                            JArray livestockContent = sectionF["content"] as JArray;

        //                                                            try
        //                                                            {
        //                                                                var livestockData = ProcessLivestockData(livestockContent, sections);

        //                                                                if (livestockData != null && livestockData.Any())
        //                                                                {
        //                                                                    rawResult.Livestock = livestockData;
        //                                                                }
        //                                                                else
        //                                                                {
        //                                                                    //Console.WriteLine("No livestock");
        //                                                                }
        //                                                            }
        //                                                            catch (Exception ex)
        //                                                            {
        //                                                                Console.WriteLine($"Error processing livestock data: {ex.Message}");
        //                                                            }

        //                                                        }

        //                                                        if (rawResult.Crops != null && rawResult.Crops.Any())
        //                                                        {
        //                                                            rawResultsBag.Add(rawResult);
        //                                                        }
        //                                                    }
        //                                                }
        //                                                catch (Exception ex)
        //                                                {
        //                                                    Console.WriteLine($"Error processing document: {ex.Message}");
        //                                                }
        //                                                #endregion
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                                #endregion

        //                                #endregion
        //                            });
        //                            #endregion

        //                            //Find the missing ids
        //                            var tempResult = rawResultsBag.ToList();
        //                            var missing = farmerBios.Data.Farmers.Where(x => !tempResult.Any(r => r.NationalID == x.NationalId && r.MobileNumber == x.FarmerPhone))?.ToList();
        //                            var missingIds = missing.Select(m => m.MongoId).ToList();

        //                            #region Process Query for missing single ids
        //                            Parallel.ForEach(missingIds.Chunk(100), ids =>
        //                            {
        //                                for (int i = 0; i < ids.Length; i++)
        //                                {
        //                                    #region Process query for single Id


        //                                    var filters = new List<FilterDefinition<BsonDocument>>();

        //                                    // try ObjectId branch
        //                                    if (ObjectId.TryParse(ids[i], out var oid))
        //                                        filters.Add(Builders<BsonDocument>.Filter.Eq("_id", oid));

        //                                    // always include string branch
        //                                    filters.Add(Builders<BsonDocument>.Filter.Eq("_id", ids[i]));


        //                                    var filter = Builders<BsonDocument>.Filter.Or(filters);

        //                                    //var orArray = new BsonArray();

        //                                    // 2. Parse the 24-char hex into an ObjectId
        //                                    //var objectId = ObjectId.Parse(Helpers.CleanStrict(ids[i]));



        //                                    //orArray.Add(new BsonDocument("_id", objectId));
        //                                    //orArray.Add(new BsonDocument("_id", Helpers.CleanStrict(ids[i])));

        //                                    // 3. Build an equality filter on "_id"
        //                                    //var filter = new BsonDocument("_id", objectId);

        //                                    //var filter = new BsonDocument("$or", orArray);

        //                                    //var filter = Builders<BsonDocument>.Filter.In("_id", orArray);

        //                                    var args = new RenderArgs<BsonDocument>(
        //            collection.DocumentSerializer,                        // IBsonSerializer<BsonDocument>
        //             BsonSerializer.SerializerRegistry     // IBsonSerializerRegistry
        //        );
        //                                    var options = new FindOptions<BsonDocument>
        //                                    {
        //                                        NoCursorTimeout = true
        //                                    };

        //                                    #region Processing
        //                                    using (var cursor = collection.FindAsync(filter, options).GetAwaiter().GetResult())
        //                                    {
        //                                        while (cursor.MoveNextAsync().GetAwaiter().GetResult())
        //                                        {
        //                                            if (cursor.Current != null && cursor.Current.Any())
        //                                            {
        //                                                var document = cursor.Current.FirstOrDefault();
        //                                                #region Cursor Processing
        //                                                var rawResult = new KYFAPExtractModel()
        //                                                {
        //                                                    County = admin.County,
        //                                                    Subcounty = admin.Subcounty,
        //                                                    Ward = admin.Ward,
        //                                                };
        //                                                try
        //                                                {
        //                                                    // Convert the document to a JSON string and parse into a JObject.
        //                                                    string jsonString = document.ToJson();
        //                                                    JObject jObject = JObject.Parse(jsonString);

        //                                                    // The JSON is expected to have a "data" array with multiple sections.
        //                                                    JArray sections = jObject["data"] as JArray;

        //                                                    rawResult.RegistrationStatus = jObject["registration_status"]
        //                                                        ?.ToString();

        //                                                    rawResult.FarmerStatus = jObject["farmer_status"]?.ToString();

        //                                                    if (sections != null)
        //                                                    {
        //                                                        // Extract administrative details from Section A.
        //                                                        JObject sectionA =
        //                                                            sections.FirstOrDefault(s =>
        //                                                                s["name"]?.ToString().Contains("Section A") == true
        //                                                            ) as JObject;

        //                                                        if (sectionA != null)
        //                                                        {
        //                                                            // Extract administrative details from Section A.
        //                                                            JArray contentA = sectionA["content"] as JArray;
        //                                                            if (contentA != null)
        //                                                            {
        //                                                                rawResult.RecordDate =
        //                                                                    contentA
        //                                                                        .FirstOrDefault(c =>
        //                                                                            c["label"]
        //                                                                                ?.ToString()
        //                                                                                .ToLower()
        //                                                                                .Contains("date") == true
        //                                                                        )
        //                                                                        ?["value"]?.ToString() ?? "";

        //                                                                try
        //                                                                {
        //                                                                    // Extract household GPS from the field with name "gps" and type "gps"
        //                                                                    JObject gpsField =
        //                                                                        contentA.FirstOrDefault(obj =>
        //                                                                            (string)obj["name"] == "gps"
        //                                                                            && (string)obj["type"] == "gps"
        //                                                                        ) as JObject;

        //                                                                    if (gpsField != null)
        //                                                                    {
        //                                                                        JObject coords =
        //                                                                            gpsField["value"]["coords"] as JObject;
        //                                                                        rawResult.Latitude = coords["latitude"]?.ToString();
        //                                                                        rawResult.Longitude = coords["longitude"]
        //                                                                            ?.ToString();

        //                                                                        //Console.WriteLine($"Coordinates =>\nLatitude: {latitude}\nLongitude: {longitude}");
        //                                                                    }
        //                                                                }
        //                                                                catch (Exception) { }
        //                                                            }
        //                                                        }

        //                                                        // Extract farmer personal details from Section B.
        //                                                        JObject sectionB =
        //                                                            sections.FirstOrDefault(s =>
        //                                                                s["name"]?.ToString().Contains("Section B") == true
        //                                                            ) as JObject;

        //                                                        string farmerName = "",
        //                                                            farmerId = "",
        //                                                            farmerPhone = "",
        //                                                            farmerGender = "";
        //                                                        if (sectionB != null)
        //                                                        {
        //                                                            JArray contentB = sectionB["content"] as JArray;
        //                                                            if (contentB != null)
        //                                                            {
        //                                                                rawResult.Name =
        //                                                                    contentB
        //                                                                        .FirstOrDefault(c =>
        //                                                                            c["q_code"]?.ToString() == "B02"
        //                                                                        )
        //                                                                        ?["value"]?.ToString() ?? "";
        //                                                                rawResult.NationalID =
        //                                                                    contentB
        //                                                                        .FirstOrDefault(c =>
        //                                                                            c["q_code"]?.ToString() == "B03"
        //                                                                        )
        //                                                                        ?["value"]?.ToString() ?? "";
        //                                                                rawResult.MobileNumber =
        //                                                                    contentB
        //                                                                        .FirstOrDefault(c =>
        //                                                                            c["q_code"]?.ToString() == "B04"
        //                                                                        )
        //                                                                        ?["value"]?.ToString() ?? "";
        //                                                                rawResult.Sex =
        //                                                                    contentB
        //                                                                        .FirstOrDefault(c =>
        //                                                                            c["q_code"]?.ToString() == "B05"
        //                                                                        )
        //                                                                        ?["value"]?.ToString() ?? "";

        //                                                                rawResult.YOB =
        //                                                                    sectionB["content"]
        //                                                                        .FirstOrDefault(obj =>
        //                                                                            (string)obj["name"] == "farmer_yob"
        //                                                                        )
        //                                                                        ?["value"]?.ToString() ?? "";
        //                                                            }
        //                                                        }

        //                                                        // Extract parcel details from Section C.
        //                                                        JObject sectionC =
        //                                                            sections.FirstOrDefault(s =>
        //                                                                s["name"]?.ToString().Contains("Section C") == true
        //                                                            ) as JObject;

        //                                                        if (sectionC != null)
        //                                                        {
        //                                                            JArray parcels = sectionC["parcels"] as JArray;
        //                                                            if (parcels != null && parcels.Any())
        //                                                            {
        //                                                                foreach (JObject parcel in parcels)
        //                                                                {
        //                                                                    JArray parcelContent = parcel["content"] as JArray;

        //                                                                    try
        //                                                                    {
        //                                                                        try
        //                                                                        {
        //                                                                            //JArray parcelContent = parcel["content"] as JArray;
        //                                                                            string acreage =
        //                                                                                parcelContent
        //                                                                                    ?.FirstOrDefault(c =>
        //                                                                                        c["name"]
        //                                                                                            ?.ToString()
        //                                                                                            .Equals(
        //                                                                                                "parcel_total_acreage",
        //                                                                                                StringComparison.OrdinalIgnoreCase
        //                                                                                            ) == true
        //                                                                                    )
        //                                                                                    ?["value"]?.ToString() ?? "";
        //                                                                            string parcelOwnershipStatus =
        //                                                                                parcelContent
        //                                                                                    ?.FirstOrDefault(c =>
        //                                                                                        c["name"]
        //                                                                                            ?.ToString()
        //                                                                                            .Equals(
        //                                                                                                "ownership_status",
        //                                                                                                StringComparison.OrdinalIgnoreCase
        //                                                                                            ) == true
        //                                                                                    )
        //                                                                                    ?["value"]?.FirstOrDefault()["value"]
        //                                                                                    ?.ToString() ?? "";

        //                                                                            rawResult.Parcels.Add(
        //                                                                                new ParcelModel
        //                                                                                {
        //                                                                                    Acreage = acreage,
        //                                                                                    OwnershipStatus = parcelOwnershipStatus,
        //                                                                                }
        //                                                                            );
        //                                                                        }
        //                                                                        catch (Exception) { }

        //                                                                        // Extract crop details from the key "crops" under the parcel.
        //                                                                        JArray cropsArray = parcel["crops"] as JArray;
        //                                                                        if (cropsArray != null && cropsArray.Any())
        //                                                                        {
        //                                                                            var crops = new List<KYFCropDetail>();
        //                                                                            foreach (JObject cropObject in cropsArray)
        //                                                                            {
        //                                                                                JArray content =
        //                                                                                    cropObject["content"] as JArray;

        //                                                                                if (content != null && content.Any())
        //                                                                                {
        //                                                                                    // Extract the crop name.
        //                                                                                    string cropName = content
        //                                                                                        .FirstOrDefault(item =>
        //                                                                                            item["name"] != null
        //                                                                                            && item["name"].ToString()
        //                                                                                                == "crop_name"
        //                                                                                        )
        //                                                                                        ?["value"]?.ToString();

        //                                                                                    // Extract the acreage for this crop.
        //                                                                                    string cropAcreageStr = content
        //                                                                                        .FirstOrDefault(item =>
        //                                                                                            item["name"] != null
        //                                                                                            && (item["name"].ToString()
        //                                                                                                == "acreage_annual_crop")
        //                                                                                        )
        //                                                                                        ?["value"]?.ToString();

        //                                                                                    // Extract the for all crops.
        //                                                                                    string totalCropAcreageStr = content
        //                                                                                        .FirstOrDefault(item =>
        //                                                                                            item["name"] != null
        //                                                                                            && (item["name"].ToString()
        //                                                                                                == "area_under_crops")
        //                                                                                        )
        //                                                                                        ?["value"]?.ToString();

        //                                                                                    string numberOfTrees = content
        //                                                                                        .FirstOrDefault(item =>
        //                                                                                            item["name"] != null
        //                                                                                            && (item["name"].ToString()
        //                                                                                                == "number_perenial_crop")
        //                                                                                        )
        //                                                                                        ?["value"]?.ToString();
        //                                                                                    var totalCropAcreage = 0f;
        //                                                                                    var numberOfCropTrees = 0f;

        //                                                                                    if (
        //                                                                                        float.TryParse(
        //                                                                                            cropAcreageStr,
        //                                                                                            out float cropAcreage
        //                                                                                        )
        //                                                                                        || float.TryParse(totalCropAcreageStr,
        //                                                                                        out totalCropAcreage)

        //                                                                                    )
        //                                                                                    {
        //                                                                                        float.TryParse(numberOfTrees,
        //                                                                                        out numberOfCropTrees);
        //                                                                                        crops.Add(
        //                                                                                            new KYFCropDetail(numberOfTrees, cropName)
        //                                                                                            {
        //                                                                                                CropName = cropName,
        //                                                                                                TotalCropAcreage = cropAcreage,
        //                                                                                                TotalAreaUnderCrops = totalCropAcreage,
        //                                                                                                NumberOfTrees = numberOfCropTrees,
        //                                                                                            }
        //                                                                                        );
        //                                                                                    }
        //                                                                                }
        //                                                                            }
        //                                                                            if (crops != null && crops.Any())
        //                                                                            {
        //                                                                                var summedCropDetails = crops
        //                                                                                    .GroupBy(c => c.CropName)
        //                                                                                    .Select(g => new KYFCropDetail(g.Sum(c => c.NumberOfTrees).ToString(), g.Key)
        //                                                                                    {
        //                                                                                        CropName = g.Key,
        //                                                                                        TotalCropAcreage = g.Sum(c =>
        //                                                                                            c.TotalCropAcreage
        //                                                                                        ),
        //                                                                                        TotalAreaUnderCrops = g.Sum(c => c.TotalAreaUnderCrops),
        //                                                                                        NumberOfTrees = g.Sum(c => c.NumberOfTrees),
        //                                                                                    })
        //                                                                                    .ToList();

        //                                                                                rawResult.Crops.AddRange(summedCropDetails);
        //                                                                            }
        //                                                                        }

        //                                                                        //Extract coffee details
        //                                                                        JArray coffeeArray = parcel["coffee"] as JArray;
        //                                                                        if (coffeeArray != null && coffeeArray.Any())
        //                                                                        {
        //                                                                            var crops = new List<KYFCropDetail>();
        //                                                                            foreach (JObject coffeeObject in coffeeArray)
        //                                                                            {
        //                                                                                JArray content =
        //                                                                                    coffeeObject["content"] as JArray;

        //                                                                                if (content != null && content.Any())
        //                                                                                {
        //                                                                                    // Extract the crop name.
        //                                                                                    string cropName = content
        //                                                                                        .FirstOrDefault(item =>
        //                                                                                            item["name"] != null
        //                                                                                            && item["name"].ToString()
        //                                                                                                == "crop_name"
        //                                                                                        )
        //                                                                                        ?["value"]?.ToString();

        //                                                                                    // Extract the acreage for this crop.
        //                                                                                    string numberOfTreesStr = content
        //                                                                                        .FirstOrDefault(item =>
        //                                                                                            item["name"] != null
        //                                                                                            && item["name"].ToString()
        //                                                                                                == "number_coffee_crop"
        //                                                                                        )
        //                                                                                        ?["value"]?.ToString();


        //                                                                                    // Extract the acreage.
        //                                                                                    string totalCropAcreageStr = content
        //                                                                                        .FirstOrDefault(item =>
        //                                                                                            item["name"] != null
        //                                                                                            && item["name"].ToString()
        //                                                                                                == "area_under_crops"
        //                                                                                        )
        //                                                                                        ?["value"]?.ToString();

        //                                                                                    float.TryParse(
        //                                                                                            totalCropAcreageStr,
        //                                                                                            out float totalCropAcreage
        //                                                                                            );

        //                                                                                    if (
        //                                                                                        float.TryParse(
        //                                                                                            numberOfTreesStr,
        //                                                                                            out float numberOfTrees
        //                                                                                        )
        //                                                                                    )
        //                                                                                    {
        //                                                                                        crops.Add(
        //                                                                                            new KYFCropDetail(numberOfTreesStr, cropName)
        //                                                                                            {
        //                                                                                                CropName = cropName,
        //                                                                                                TotalCropAcreage =
        //                                                                                                    numberOfTrees,
        //                                                                                                TotalAreaUnderCrops = totalCropAcreage,
        //                                                                                                NumberOfTrees = numberOfTrees
        //                                                                                            }
        //                                                                                        );
        //                                                                                    }
        //                                                                                }


        //                                                                            }

        //                                                                            if (crops != null && crops.Any())
        //                                                                            {
        //                                                                                var summedCropDetails = crops
        //                                                                                .GroupBy(c => c.CropName)
        //                                                                                .Select(g => new KYFCropDetail(g.Sum(c => c.NumberOfTrees).ToString(), g.Key)
        //                                                                                {
        //                                                                                    CropName = g.Key,
        //                                                                                    TotalCropAcreage = g.Sum(c =>
        //                                                                                        c.TotalCropAcreage
        //                                                                                    ),
        //                                                                                    TotalAreaUnderCrops = g.Sum(c => c.TotalAreaUnderCrops),
        //                                                                                    NumberOfTrees = g.Sum(c => c.NumberOfTrees),
        //                                                                                })
        //                                                                                .ToList();

        //                                                                                rawResult.Crops.AddRange(summedCropDetails);
        //                                                                            }
        //                                                                        }
        //                                                                    }
        //                                                                    catch (Exception ex)
        //                                                                    {
        //                                                                        Console.WriteLine(
        //                                                                            $"Error getting crops in parcel: {ex.Message}"
        //                                                                        );
        //                                                                    }
        //                                                                }
        //                                                            }
        //                                                        }
        //                                                    }

        //                                                    if (rawResult.Crops != null && rawResult.Crops.Any())
        //                                                    {
        //                                                        //Console.WriteLine("Here!!!55555");
        //                                                        rawResultsBag.Add(rawResult);
        //                                                    }
        //                                                }
        //                                                catch (Exception ex)
        //                                                {
        //                                                    Console.WriteLine($"Error processing document: {ex.Message}");
        //                                                }
        //                                                break;
        //                                                #endregion
        //                                            }
        //                                            else
        //                                            {

        //                                                var rendered = filter.Render(
        //               args
        //            )
        //            .ToJson(new MongoDB.Bson.IO.JsonWriterSettings { Indent = true });
        //                                                //Console.WriteLine("Query filter:\n" + rendered);

        //                                            }
        //                                        }
        //                                    }
        //                                    #endregion

        //                                    #endregion
        //                                }
        //                            });
        //                            #endregion
        //                        }
        //                    });
        //                }




        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine(
        //                    $"Error fetching and processing farmer data: {ex.Message}\nStack Trace: {ex.StackTrace}"
        //                );
        //            }

        //            return rawResultsBag.ToList();
        //        }
        #endregion
        public static async Task<List<KYFAPExtractModel>> GetKYFCountyExtract(
string county, IMemoryCache dataCache
)
        {
            var rawResultsBag = new ConcurrentBag<KYFAPExtractModel>();
            try
            {
                var collection = KYFDataExtraction();

                var admins = APZoneCache.APZones.Where(ap => ap.County.ToLower().Trim() == county.ToLower().Trim())?
                    .Select(a => new KYFQueryAdminModel
                    {
                        County = a.County,
                        Subcounty = a.Subcounty,
                        Ward = a.Ward
                    })?.ToList();

                if (admins != null && admins.Any())
                {
                    Parallel.ForEach(admins,
                        new ParallelOptions { MaxDegreeOfParallelism = 100 },
                        admin =>
                    {
                        var dataKey = admin.GetHashCode();
                        var farmerBios = GetFarmersBio(admin.County, admin.Subcounty, admin.Ward).GetAwaiter().GetResult();

                        //var farmerBios = new FarmersBioModel
                        //{
                        //    Data = new Data
                        //    {
                        //        Farmers = new List<FarmerBioModel> { new FarmerBioModel { MongoId = "682eef83c23e1cb8cfaa442e" } }
                        //    }
                        //};
                        Console.WriteLine("Here");
                        if (farmerBios != null)
                        {
                            Console.WriteLine($"County: {admin.County}; Subcounty: {admin.Subcounty}; Ward: {admin.Ward} ======> {farmerBios?.Count}");
                            try
                            {
                                if (farmerBios.Data != null && farmerBios.Data.Farmers != null && farmerBios.Data.Farmers.Any())
                                {
                                    var missingRecords = new List<FarmerBioModel>();
                                    var ids = farmerBios.Data.Farmers.Where(x => !string.IsNullOrEmpty(x.MongoId)).Select(x => Helpers.CleanStrict(x.MongoId.Trim())).ToList().Distinct().ToList();

                                    #region Process Query for multiple ids
                                    foreach (var idChunk in ids.Chunk(50))
                                    //Parallel.ForEach(ids.Chunk(50),
                                    //    new ParallelOptions { MaxDegreeOfParallelism = 50 },
                                    //    idChunk =>
                                    {
                                        var objectIdArray = new BsonArray(idChunk.Select(ObjectId.Parse));


                                        var filter = Builders<BsonDocument>.Filter.In("_id", objectIdArray);

                                        #region Process Queries for batch
                                        var options = new FindOptions<BsonDocument>
                                        {
                                            NoCursorTimeout = true
                                        };

                                        #region Processing
                                        using (var cursor = collection.FindAsync(filter, options).GetAwaiter().GetResult())
                                        {
                                            while (cursor.MoveNextAsync().GetAwaiter().GetResult())
                                            {
                                                if (cursor.Current != null && cursor.Current.Any())
                                                {
                                                    foreach (var document in cursor.Current)
                                                    {
                                                        #region Cursor Processing
                                                        var rawResult = new KYFAPExtractModel()
                                                        {
                                                            County = admin.County,
                                                            Subcounty = admin.Subcounty,
                                                            Ward = admin.Ward,
                                                        };
                                                        try
                                                        {
                                                            // Convert the document to a JSON string and parse into a JObject.
                                                            string jsonString = document.ToJson();
                                                            JObject jObject = JObject.Parse(jsonString);

                                                            rawResult.RecordId = JsonDocument.Parse(jObject["_id"].ToString()).RootElement.GetProperty("$oid").GetString();

                                                            // The JSON is expected to have a "data" array with multiple sections.
                                                            JArray sections = jObject["data"] as JArray;



                                                            rawResult.RegistrationStatus = jObject["registration_status"]
                                                                ?.ToString();

                                                            rawResult.FarmerStatus = jObject["farmer_status"]?.ToString();

                                                            if (sections != null)
                                                            {
                                                                // Extract administrative details from Section A.
                                                                JObject sectionA =
                                                                    sections.FirstOrDefault(s =>
                                                                        s["name"]?.ToString().Contains("Section A") == true
                                                                    ) as JObject;

                                                                if (sectionA != null)
                                                                {
                                                                    // Extract administrative details from Section A.
                                                                    JArray contentA = sectionA["content"] as JArray;
                                                                    if (contentA != null)
                                                                    {
                                                                        rawResult.RecordDate =
                                                                            contentA
                                                                                .FirstOrDefault(c =>
                                                                                    c["label"]
                                                                                        ?.ToString()
                                                                                        .ToLower()
                                                                                        .Contains("date") == true
                                                                                )
                                                                                ?["value"]?.ToString() ?? "";

                                                                        try
                                                                        {
                                                                            // Extract household GPS from the field with name "gps" and type "gps"
                                                                            JObject gpsField =
                                                                                contentA.FirstOrDefault(obj =>
                                                                                    (string)obj["name"] == "gps"
                                                                                    && (string)obj["type"] == "gps"
                                                                                ) as JObject;

                                                                            if (gpsField != null)
                                                                            {
                                                                                JObject coords =
                                                                                    gpsField["value"]["coords"] as JObject;
                                                                                rawResult.Latitude = coords["latitude"]?.ToString();
                                                                                rawResult.Longitude = coords["longitude"]
                                                                                    ?.ToString();

                                                                                //Console.WriteLine($"Coordinates =>\nLatitude: {latitude}\nLongitude: {longitude}");
                                                                            }

                                                                            rawResult.ZoneNo = contentA.FirstOrDefault(obj =>
                                                                            obj["name"]?.ToString() == "enumeration_area")?["value"]?.ToString();
                                                                        }
                                                                        catch (Exception) { }
                                                                    }
                                                                }

                                                                JObject enumeratorSection =
                                                                sections.FirstOrDefault(s =>
                                                                s["name"]?.ToString().Contains("Enumerator details") == true) as JObject;

                                                                if (enumeratorSection != null)
                                                                {
                                                                    var enumeratorContent = enumeratorSection["content"] as JArray;

                                                                    if (enumeratorContent != null && enumeratorContent.Any())
                                                                    {
                                                                        try
                                                                        {
                                                                            rawResult.APName = enumeratorContent[0]?["value"]?.ToString();
                                                                            rawResult.APId = enumeratorContent[1]?["value"]?.ToString();
                                                                            rawResult.APPhoneNo = enumeratorContent[2]?["value"]?.ToString();
                                                                        }
                                                                        catch (Exception)
                                                                        {
                                                                        }

                                                                    }
                                                                }

                                                                // Extract farmer personal details from Section B.
                                                                JObject sectionB =
                                                                    sections.FirstOrDefault(s =>
                                                                        s["name"]?.ToString().Contains("Section B") == true
                                                                    ) as JObject;

                                                                string farmerName = "",
                                                                    farmerId = "",
                                                                    farmerPhone = "",
                                                                    farmerGender = "";
                                                                if (sectionB != null)
                                                                {
                                                                    JArray contentB = sectionB["content"] as JArray;
                                                                    if (contentB != null)
                                                                    {
                                                                        try
                                                                        {
                                                                            rawResult.Name =
                                                                            contentB
                                                                                .FirstOrDefault(c =>
                                                                                    c["q_code"]?.ToString() == "B02"
                                                                                )
                                                                                ?["value"]?.ToString() ?? "";
                                                                        }
                                                                        catch (Exception)
                                                                        {
                                                                        }

                                                                        try
                                                                        {
                                                                            rawResult.NationalID =
                                                                                contentB
                                                                                    .FirstOrDefault(c =>
                                                                                        c["q_code"]?.ToString() == "B03"
                                                                                    )
                                                                                    ?["value"]?.ToString() ?? "";
                                                                            rawResult.MobileNumber =
                                                                                contentB
                                                                                    .FirstOrDefault(c =>
                                                                                        c["q_code"]?.ToString() == "B04"
                                                                                    )
                                                                                    ?["value"]?.ToString() ?? "";
                                                                        }
                                                                        catch (Exception)
                                                                        {
                                                                        }

                                                                        try
                                                                        {
                                                                            var genderTest = contentB
                                                                                    .FirstOrDefault(c =>
                                                                                        c["q_code"]?.ToString() == "B05"
                                                                                    )
                                                                                    ?["value"]?.ToString() ?? "";
                                                                            var validGenders = new string[] { "Male", "Female", "Other" };

                                                                            if (validGenders.Contains(genderTest))
                                                                                rawResult.Sex = genderTest;
                                                                        }
                                                                        catch (Exception)
                                                                        {
                                                                        }

                                                                        //rawResult.Sex =
                                                                        //    contentB
                                                                        //        .FirstOrDefault(c =>
                                                                        //            c["q_code"]?.ToString() == "B05"
                                                                        //        )
                                                                        //        ?["value"]?.ToString() ?? "";

                                                                        rawResult.YOB =
                                                                            sectionB["content"]
                                                                                .FirstOrDefault(obj =>
                                                                                    (string)obj["name"] == "farmer_yob"
                                                                                )
                                                                                ?["value"]?.ToString() ?? "";
                                                                    }
                                                                }

                                                                // Extract parcel details from Section C.
                                                                JObject sectionC =
                                                                    sections.FirstOrDefault(s =>
                                                                        s["name"]?.ToString().Contains("Section C") == true
                                                                    ) as JObject;

                                                                if (sectionC != null)
                                                                {
                                                                    JArray parcels = sectionC["parcels"] as JArray;
                                                                    if (parcels != null && parcels.Any())
                                                                    {
                                                                        var cropsExtract = ProcessCropsExtract(parcels);

                                                                        if (cropsExtract.Crops.Any() && cropsExtract.Parcels.Any())
                                                                        {
                                                                            rawResult.Crops.AddRange(cropsExtract.Crops);
                                                                            rawResult.Parcels.AddRange(cropsExtract.Parcels);
                                                                        }

                                                                        #region Old
                                                                        //int parcelCounter = 1;
                                                                        //foreach (JObject parcel in parcels)
                                                                        //{
                                                                        //    JArray parcelContent = parcel["content"] as JArray;

                                                                        //    try
                                                                        //    {
                                                                        //        try
                                                                        //        {
                                                                        //            //JArray parcelContent = parcel["content"] as JArray;
                                                                        //            string acreage =
                                                                        //                parcelContent
                                                                        //                    ?.FirstOrDefault(c =>
                                                                        //                        c["name"]
                                                                        //                            ?.ToString()
                                                                        //                            .Equals(
                                                                        //                                "parcel_total_acreage",
                                                                        //                                StringComparison.OrdinalIgnoreCase
                                                                        //                            ) == true
                                                                        //                    )
                                                                        //                    ?["value"]?.ToString() ?? "";
                                                                        //            string parcelOwnershipStatus =
                                                                        //                parcelContent
                                                                        //                    ?.FirstOrDefault(c =>
                                                                        //                        c["name"]
                                                                        //                            ?.ToString()
                                                                        //                            .Equals(
                                                                        //                                "ownership_status",
                                                                        //                                StringComparison.OrdinalIgnoreCase
                                                                        //                            ) == true
                                                                        //                    )
                                                                        //                    ?["value"]?.FirstOrDefault()["value"]
                                                                        //                    ?.ToString() ?? "";

                                                                        //            rawResult.Parcels.Add(
                                                                        //                new ParcelExract
                                                                        //                {
                                                                        //                    ParcelId = $"PCL{parcelCounter.ToString().PadLeft(4, '0')}",
                                                                        //                    Acreage = acreage,
                                                                        //                    OwnershipStatus = parcelOwnershipStatus,
                                                                        //                }
                                                                        //            );
                                                                        //        }
                                                                        //        catch (Exception ex)
                                                                        //        {
                                                                        //            //Console.WriteLine($"Error: {ex.Message}"); 
                                                                        //        }

                                                                        //        // Extract crop details from the key "crops" under the parcel.
                                                                        //        JArray cropsArray = parcel["crops"] as JArray;
                                                                        //        if (cropsArray != null && cropsArray.Any())
                                                                        //        {
                                                                        //            var crops = new List<KYFCropExtract>();
                                                                        //            foreach (JObject cropObject in cropsArray)
                                                                        //            {
                                                                        //                JArray content =
                                                                        //                    cropObject["content"] as JArray;

                                                                        //                if (content != null && content.Any())
                                                                        //                {
                                                                        //                    // Extract the crop name.
                                                                        //                    string cropName = content
                                                                        //                        .FirstOrDefault(item =>
                                                                        //                            item["name"] != null
                                                                        //                            && item["name"].ToString()
                                                                        //                                == "crop_name"
                                                                        //                        )
                                                                        //                        ?["value"]?.ToString();

                                                                        //                    // Extract the acreage for this crop.
                                                                        //                    string cropAcreageStr = content
                                                                        //                        .FirstOrDefault(item =>
                                                                        //                            item["name"] != null
                                                                        //                            && (item["name"].ToString()
                                                                        //                                == "acreage_annual_crop")
                                                                        //                        )
                                                                        //                        ?["value"]?.ToString();

                                                                        //                    // Extract the for all crops.
                                                                        //                    string totalCropAcreageStr = content
                                                                        //                        .FirstOrDefault(item =>
                                                                        //                            item["name"] != null
                                                                        //                            && (item["name"].ToString()
                                                                        //                                == "area_under_crops")
                                                                        //                        )
                                                                        //                        ?["value"]?.ToString();

                                                                        //                    string numberOfTrees = content
                                                                        //                        .FirstOrDefault(item =>
                                                                        //                            item["name"] != null
                                                                        //                            && (item["name"].ToString()
                                                                        //                                == "number_perenial_crop")
                                                                        //                        )
                                                                        //                        ?["value"]?.ToString();
                                                                        //                    var totalCropAcreage = 0f;
                                                                        //                    var numberOfCropTrees = 0f;

                                                                        //                    if (
                                                                        //                        float.TryParse(
                                                                        //                            cropAcreageStr,
                                                                        //                            out float cropAcreage
                                                                        //                        )
                                                                        //                        || float.TryParse(totalCropAcreageStr,
                                                                        //                        out totalCropAcreage)

                                                                        //                    )
                                                                        //                    {
                                                                        //                        float.TryParse(numberOfTrees,
                                                                        //                       out numberOfCropTrees);
                                                                        //                        crops.Add(
                                                                        //                            new KYFCropExtract(numberOfTrees, cropName)
                                                                        //                            {
                                                                        //                                ParcelId = $"PCL{parcelCounter.ToString().PadLeft(4, '0')}",
                                                                        //                                CropName = cropName,
                                                                        //                                TotalCropAcreage = cropAcreage,
                                                                        //                                TotalAreaUnderCrops = totalCropAcreage,
                                                                        //                                NumberOfTrees = numberOfCropTrees,
                                                                        //                            }
                                                                        //                        );
                                                                        //                    }
                                                                        //                }
                                                                        //            }
                                                                        //            if (crops != null && crops.Any())
                                                                        //            {
                                                                        //                rawResult.Crops.AddRange(crops.ToArray());
                                                                        //            }
                                                                        //        }

                                                                        //        //Extract coffee details
                                                                        //        JArray coffeeArray = parcel["coffee"] as JArray;
                                                                        //        if (coffeeArray != null && coffeeArray.Any())
                                                                        //        {
                                                                        //            var crops = new List<KYFCropExtract>();
                                                                        //            foreach (JObject coffeeObject in coffeeArray)
                                                                        //            {
                                                                        //                JArray content =
                                                                        //                    coffeeObject["content"] as JArray;

                                                                        //                if (content != null && content.Any())
                                                                        //                {
                                                                        //                    // Extract the crop name.
                                                                        //                    string cropName = content
                                                                        //                        .FirstOrDefault(item =>
                                                                        //                            item["name"] != null
                                                                        //                            && item["name"].ToString()
                                                                        //                                == "crop_name"
                                                                        //                        )
                                                                        //                        ?["value"]?.ToString();

                                                                        //                    // Extract the acreage for this crop.
                                                                        //                    string numberOfTreesStr = content
                                                                        //                        .FirstOrDefault(item =>
                                                                        //                            item["name"] != null
                                                                        //                            && item["name"].ToString()
                                                                        //                                == "number_coffee_crop"
                                                                        //                        )
                                                                        //                        ?["value"]?.ToString();


                                                                        //                    // Extract the acreage.
                                                                        //                    string totalCropAcreageStr = content
                                                                        //                        .FirstOrDefault(item =>
                                                                        //                            item["name"] != null
                                                                        //                            && item["name"].ToString()
                                                                        //                                == "area_under_crops"
                                                                        //                        )
                                                                        //                        ?["value"]?.ToString();

                                                                        //                    float.TryParse(
                                                                        //                            totalCropAcreageStr,
                                                                        //                            out float totalCropAcreage
                                                                        //                            );

                                                                        //                    if (
                                                                        //                        float.TryParse(
                                                                        //                            numberOfTreesStr,
                                                                        //                            out float numberOfTrees
                                                                        //                        )
                                                                        //                    )
                                                                        //                    {
                                                                        //                        crops.Add(
                                                                        //                            new KYFCropExtract(numberOfTreesStr, cropName)
                                                                        //                            {
                                                                        //                                ParcelId = $"PCL{parcelCounter.ToString().PadLeft(4, '0')}",
                                                                        //                                CropName = cropName,
                                                                        //                                TotalCropAcreage =
                                                                        //                                    numberOfTrees,
                                                                        //                                TotalAreaUnderCrops = totalCropAcreage,
                                                                        //                                NumberOfTrees = numberOfTrees
                                                                        //                            }
                                                                        //                        );
                                                                        //                    }
                                                                        //                }


                                                                        //            }

                                                                        //            if (crops != null && crops.Any())
                                                                        //            {
                                                                        //                //var summedCropDetails = crops
                                                                        //                //.GroupBy(c => c.CropName)
                                                                        //                //.Select(g => new KYFCropExtract(g.Sum(c => c.NumberOfTrees).ToString(), g.Key)
                                                                        //                //{
                                                                        //                //    CropName = g.Key,
                                                                        //                //    TotalCropAcreage = g.Sum(c =>
                                                                        //                //        c.TotalCropAcreage
                                                                        //                //    ),
                                                                        //                //    TotalAreaUnderCrops = g.Sum(c => c.TotalAreaUnderCrops),
                                                                        //                //    NumberOfTrees = g.Sum(c => c.NumberOfTrees),
                                                                        //                //})
                                                                        //                //.ToList();

                                                                        //                //rawResult.Crops.AddRange(summedCropDetails);

                                                                        //                rawResult.Crops.AddRange(crops.ToArray());
                                                                        //            }
                                                                        //        }
                                                                        //    }
                                                                        //    catch (Exception ex)
                                                                        //    {
                                                                        //        Console.WriteLine(
                                                                        //            $"Error getting crops in parcel: {ex.Message}"
                                                                        //        );
                                                                        //    }
                                                                        //    //Console.WriteLine($"Parcel count: {parcelCounter}");

                                                                        //    parcelCounter++;
                                                                        //}
                                                                        #endregion
                                                                    }
                                                                }

                                                                JObject sectionF =
                                                                 sections.FirstOrDefault(s =>
                                                                        s["name"]?.ToString().Equals("Section F") == true
                                                                    ) as JObject;

                                                                if (sectionF != null)
                                                                {
                                                                    JArray livestockContent = sectionF["content"] as JArray;

                                                                    try
                                                                    {
                                                                        var livestockData = ProcessLivestockData(livestockContent, sections);
                                                                        //Console.WriteLine($"Livestock count: {livestockData.Count}");
                                                                        if (livestockData != null && livestockData.Any())
                                                                        {
                                                                            rawResult.Livestock.AddRange(livestockData);
                                                                        }
                                                                        else
                                                                        {
                                                                            //Console.WriteLine("No livestock");
                                                                        }
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        //Console.WriteLine($"Error processing livestock data: {ex.Message}");
                                                                    }

                                                                }

                                                                #region Process Aquaculture
                                                                JObject sectionG =
                                                                sections.FirstOrDefault(s =>
                                                                        s["name"]?.ToString().Equals("Section G") == true
                                                                    ) as JObject;

                                                                if (sectionG != null)
                                                                {
                                                                    JArray aquaContent = sectionG["content"] as JArray;
                                                                    if (aquaContent != null && aquaContent.Any())
                                                                    {
                                                                        try
                                                                        {
                                                                            var aquaData = ProcessAquacultureData(aquaContent, sectionG);

                                                                            if (aquaData != null && aquaData.Any())
                                                                                rawResult.Aquaculture.AddRange(aquaData);
                                                                        }
                                                                        catch (Exception)
                                                                        {
                                                                        }
                                                                    }

                                                                }
                                                                #endregion
                                                                //Console.WriteLine($"Raw and : {rawResultsBag.Count}");
                                                                rawResultsBag.Add(rawResult);
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine($"Error processing document: {ex.Message}\n{ex.StackTrace}");
                                                        }
                                                        #endregion
                                                    }
                                                }
                                            }
                                        }
                                        #endregion

                                        #endregion
                                    }
                                    //);
                                    #endregion

                                    //Find the missing ids
                                    var tempResult = rawResultsBag.ToList();
                                    var missing = farmerBios.Data.Farmers.Where(x => !tempResult.Any(r => r.NationalID == x.NationalId && r.MobileNumber == x.FarmerPhone))?.ToList();
                                    var missingIds = missing.Select(m => m.MongoId).ToList();

                                    #region Process Query for missing single ids
                                    Parallel.ForEach(missingIds.Chunk(50),
                                        new ParallelOptions { MaxDegreeOfParallelism = 100 },
                                        idChunk =>
                                    {
                                        try
                                        {
                                            for (int i = 0; i < idChunk.Length; i++)
                                            {
                                                try
                                                {
                                                    #region Process query for single Id


                                                    var filters = new List<FilterDefinition<BsonDocument>>();

                                                    // try ObjectId branch
                                                    if (ObjectId.TryParse(idChunk[i], out var oid))
                                                        filters.Add(Builders<BsonDocument>.Filter.Eq("_id", oid));

                                                    // always include string branch
                                                    filters.Add(Builders<BsonDocument>.Filter.Eq("_id", idChunk[i]));


                                                    var filter = Builders<BsonDocument>.Filter.Or(filters);


                                                    var args = new RenderArgs<BsonDocument>(
                            collection.DocumentSerializer,                        // IBsonSerializer<BsonDocument>
                             BsonSerializer.SerializerRegistry     // IBsonSerializerRegistry
                        );
                                                    var options = new FindOptions<BsonDocument>
                                                    {
                                                        NoCursorTimeout = true
                                                    };

                                                    #region Processing
                                                    using (var cursor = collection.FindAsync(filter, options).GetAwaiter().GetResult())
                                                    {
                                                        while (cursor.MoveNextAsync().GetAwaiter().GetResult())
                                                        {
                                                            if (cursor.Current != null && cursor.Current.Any())
                                                            {
                                                                var document = cursor.Current.FirstOrDefault();
                                                                #region Cursor Processing
                                                                var rawResult = new KYFAPExtractModel()
                                                                {
                                                                    County = admin.County,
                                                                    Subcounty = admin.Subcounty,
                                                                    Ward = admin.Ward,
                                                                };
                                                                try
                                                                {
                                                                    // Convert the document to a JSON string and parse into a JObject.
                                                                    string jsonString = document.ToJson();
                                                                    JObject jObject = JObject.Parse(jsonString);

                                                                    // The JSON is expected to have a "data" array with multiple sections.
                                                                    JArray sections = jObject["data"] as JArray;

                                                                    rawResult.RecordId = JsonDocument.Parse(jObject["_id"].ToString()).RootElement.GetProperty("$oid").GetString();

                                                                    rawResult.RegistrationStatus = jObject["registration_status"]
                                                                        ?.ToString();

                                                                    rawResult.FarmerStatus = jObject["farmer_status"]?.ToString();

                                                                    if (sections != null)
                                                                    {
                                                                        // Extract administrative details from Section A.
                                                                        JObject sectionA =
                                                                            sections.FirstOrDefault(s =>
                                                                                s["name"]?.ToString().Contains("Section A") == true
                                                                            ) as JObject;

                                                                        // Extract AP details
                                                                        if (sectionA != null)
                                                                        {
                                                                            // Extract administrative details from Section A.
                                                                            JArray contentA = sectionA["content"] as JArray;
                                                                            if (contentA != null)
                                                                            {
                                                                                rawResult.RecordDate =
                                                                                    contentA
                                                                                        .FirstOrDefault(c =>
                                                                                            c["label"]
                                                                                                ?.ToString()
                                                                                                .ToLower()
                                                                                                .Contains("date") == true
                                                                                        )
                                                                                        ?["value"]?.ToString() ?? "";



                                                                                try
                                                                                {
                                                                                    // Extract household GPS from the field with name "gps" and type "gps"
                                                                                    JObject gpsField =
                                                                                        contentA.FirstOrDefault(obj =>
                                                                                            (string)obj["name"] == "gps"
                                                                                            && (string)obj["type"] == "gps"
                                                                                        ) as JObject;

                                                                                    if (gpsField != null)
                                                                                    {
                                                                                        JObject coords =
                                                                                            gpsField["value"]["coords"] as JObject;
                                                                                        rawResult.Latitude = coords["latitude"]?.ToString();
                                                                                        rawResult.Longitude = coords["longitude"]
                                                                                            ?.ToString();

                                                                                        //Console.WriteLine($"Coordinates =>\nLatitude: {latitude}\nLongitude: {longitude}");
                                                                                    }

                                                                                    rawResult.ZoneNo = contentA.FirstOrDefault(obj =>
                                                                                   obj["name"]?.ToString() == "enumeration_area")?["value"]?.ToString();
                                                                                }
                                                                                catch (Exception) { }
                                                                            }
                                                                        }


                                                                        JObject enumeratorSection =
                sections.FirstOrDefault(s =>
                s["name"]?.ToString().Contains("Enumerator details") == true) as JObject;

                                                                        if (enumeratorSection != null)
                                                                        {
                                                                            try
                                                                            {
                                                                                var enumeratorContent = enumeratorSection["content"] as JArray;

                                                                                if (enumeratorContent != null && enumeratorContent.Any())
                                                                                {
                                                                                    rawResult.APName = enumeratorContent[0]?["value"]?.ToString();
                                                                                    rawResult.APId = enumeratorContent[1]?["value"]?.ToString();
                                                                                    rawResult.APPhoneNo = enumeratorContent[2]?["value"]?.ToString();
                                                                                }
                                                                            }
                                                                            catch (Exception)
                                                                            {
                                                                            }
                                                                        }

                                                                        // Extract farmer personal details from Section B.
                                                                        JObject sectionB =
                                                                            sections.FirstOrDefault(s =>
                                                                                s["name"]?.ToString().Contains("Section B") == true
                                                                            ) as JObject;


                                                                        string farmerName = "",
                                                                            farmerId = "",
                                                                            farmerPhone = "",
                                                                            farmerGender = "";
                                                                        if (sectionB != null)
                                                                        {
                                                                            JArray contentB = sectionB["content"] as JArray;
                                                                            if (contentB != null)
                                                                            {
                                                                                rawResult.Name =
                                                                                    contentB
                                                                                        .FirstOrDefault(c =>
                                                                                            c["q_code"]?.ToString() == "B02"
                                                                                        )
                                                                                        ?["value"]?.ToString() ?? "";
                                                                                rawResult.NationalID =
                                                                                    contentB
                                                                                        .FirstOrDefault(c =>
                                                                                            c["q_code"]?.ToString() == "B03"
                                                                                        )
                                                                                        ?["value"]?.ToString() ?? "";
                                                                                rawResult.MobileNumber =
                                                                                    contentB
                                                                                        .FirstOrDefault(c =>
                                                                                            c["q_code"]?.ToString() == "B04"
                                                                                        )
                                                                                        ?["value"]?.ToString() ?? "";

                                                                                try
                                                                                {
                                                                                    var genderTest = contentB
                                                                                            .FirstOrDefault(c =>
                                                                                                c["q_code"]?.ToString() == "B05"
                                                                                            )
                                                                                            ?["value"]?.ToString() ?? "";
                                                                                    var validGenders = new string[] { "Male", "Female", "Other" };

                                                                                    if (validGenders.Contains(genderTest))
                                                                                        rawResult.Sex = genderTest;

                                                                                    rawResult.YOB =
                                                                                                    sectionB["content"]
                                                                                                        .FirstOrDefault(obj =>
                                                                                                            (string)obj["name"] == "farmer_yob"
                                                                                                        )
                                                                                                        ?["value"]?.ToString() ?? "";
                                                                                }
                                                                                catch (Exception)
                                                                                {
                                                                                }
                                                                            }
                                                                        }

                                                                        // Extract parcel details from Section C.
                                                                        JObject sectionC =
                                                                            sections.FirstOrDefault(s =>
                                                                                s["name"]?.ToString().Contains("Section C") == true
                                                                            ) as JObject;

                                                                        if (sectionC != null)
                                                                        {
                                                                            JArray parcels = sectionC["parcels"] as JArray;
                                                                            if (parcels != null && parcels.Any())
                                                                            {
                                                                                var cropsExtract = ProcessCropsExtract(parcels);

                                                                                if (cropsExtract.Crops.Any() && cropsExtract.Parcels.Any())
                                                                                {
                                                                                    rawResult.Crops.AddRange(cropsExtract.Crops);
                                                                                    rawResult.Parcels.AddRange(cropsExtract.Parcels);
                                                                                }

                                                                                #region Old
                                                                                //int parcelCounter = 1;
                                                                                //foreach (JObject parcel in parcels)
                                                                                //{
                                                                                //    JArray parcelContent = parcel["content"] as JArray;

                                                                                //    try
                                                                                //    {
                                                                                //        try
                                                                                //        {
                                                                                //            //JArray parcelContent = parcel["content"] as JArray;
                                                                                //            string acreage =
                                                                                //                parcelContent
                                                                                //                    ?.FirstOrDefault(c =>
                                                                                //                        c["name"]
                                                                                //                            ?.ToString()
                                                                                //                            .Equals(
                                                                                //                                "parcel_total_acreage",
                                                                                //                                StringComparison.OrdinalIgnoreCase
                                                                                //                            ) == true
                                                                                //                    )
                                                                                //                    ?["value"]?.ToString() ?? "";
                                                                                //            string parcelOwnershipStatus =
                                                                                //                parcelContent
                                                                                //                    ?.FirstOrDefault(c =>
                                                                                //                        c["name"]
                                                                                //                            ?.ToString()
                                                                                //                            .Equals(
                                                                                //                                "ownership_status",
                                                                                //                                StringComparison.OrdinalIgnoreCase
                                                                                //                            ) == true
                                                                                //                    )
                                                                                //                    ?["value"]?.FirstOrDefault()["value"]
                                                                                //                    ?.ToString() ?? "";

                                                                                //            rawResult.Parcels.Add(
                                                                                //                new ParcelExract
                                                                                //                {
                                                                                //                    ParcelId = $"PCL{parcelCounter.ToString().PadLeft(4, '0')}",
                                                                                //                    Acreage = acreage,
                                                                                //                    OwnershipStatus = parcelOwnershipStatus,
                                                                                //                }
                                                                                //            );
                                                                                //        }
                                                                                //        catch (Exception) { }

                                                                                //        // Extract crop details from the key "crops" under the parcel.
                                                                                //        JArray cropsArray = parcel["crops"] as JArray;
                                                                                //        if (cropsArray != null && cropsArray.Any())
                                                                                //        {
                                                                                //            var crops = new List<KYFCropExtract>();
                                                                                //            foreach (JObject cropObject in cropsArray)
                                                                                //            {
                                                                                //                JArray content =
                                                                                //                    cropObject["content"] as JArray;

                                                                                //                if (content != null && content.Any())
                                                                                //                {
                                                                                //                    // Extract the crop name.
                                                                                //                    string cropName = content
                                                                                //                        .FirstOrDefault(item =>
                                                                                //                            item["name"] != null
                                                                                //                            && item["name"].ToString()
                                                                                //                                == "crop_name"
                                                                                //                        )
                                                                                //                        ?["value"]?.ToString();

                                                                                //                    // Extract the acreage for this crop.
                                                                                //                    string cropAcreageStr = content
                                                                                //                        .FirstOrDefault(item =>
                                                                                //                            item["name"] != null
                                                                                //                            && (item["name"].ToString()
                                                                                //                                == "acreage_annual_crop")
                                                                                //                        )
                                                                                //                        ?["value"]?.ToString();

                                                                                //                    // Extract the for all crops.
                                                                                //                    string totalCropAcreageStr = content
                                                                                //                        .FirstOrDefault(item =>
                                                                                //                            item["name"] != null
                                                                                //                            && (item["name"].ToString()
                                                                                //                                == "area_under_crops")
                                                                                //                        )
                                                                                //                        ?["value"]?.ToString();

                                                                                //                    string numberOfTrees = content
                                                                                //                        .FirstOrDefault(item =>
                                                                                //                            item["name"] != null
                                                                                //                            && (item["name"].ToString()
                                                                                //                                == "number_perenial_crop")
                                                                                //                        )
                                                                                //                        ?["value"]?.ToString();
                                                                                //                    var totalCropAcreage = 0f;
                                                                                //                    var numberOfCropTrees = 0f;

                                                                                //                    if (
                                                                                //                        float.TryParse(
                                                                                //                            cropAcreageStr,
                                                                                //                            out float cropAcreage
                                                                                //                        )
                                                                                //                        || float.TryParse(totalCropAcreageStr,
                                                                                //                        out totalCropAcreage)

                                                                                //                    )
                                                                                //                    {
                                                                                //                        float.TryParse(numberOfTrees,
                                                                                //                        out numberOfCropTrees);
                                                                                //                        crops.Add(
                                                                                //                            new KYFCropExtract(numberOfTrees, cropName)
                                                                                //                            {
                                                                                //                                ParcelId = $"PCL{parcelCounter.ToString().PadLeft(4, '0')}",
                                                                                //                                CropName = cropName,
                                                                                //                                TotalCropAcreage = cropAcreage,
                                                                                //                                TotalAreaUnderCrops = totalCropAcreage,
                                                                                //                                NumberOfTrees = numberOfCropTrees,
                                                                                //                            }
                                                                                //                        );
                                                                                //                    }
                                                                                //                }
                                                                                //            }
                                                                                //            if (crops != null && crops.Any())
                                                                                //            {
                                                                                //                //var summedCropDetails = crops
                                                                                //                //    .GroupBy(c => c.CropName)
                                                                                //                //    .Select(g => new KYFCropExtract(g.Sum(c => c.NumberOfTrees).ToString(), g.Key)
                                                                                //                //    {
                                                                                //                //        CropName = g.Key,
                                                                                //                //        TotalCropAcreage = g.Sum(c =>
                                                                                //                //            c.TotalCropAcreage
                                                                                //                //        ),
                                                                                //                //        TotalAreaUnderCrops = g.Sum(c => c.TotalAreaUnderCrops),
                                                                                //                //        NumberOfTrees = g.Sum(c => c.NumberOfTrees),
                                                                                //                //    })
                                                                                //                //    .ToList();

                                                                                //                //rawResult.Crops.AddRange(summedCropDetails);

                                                                                //                rawResult.Crops.AddRange(crops.ToArray());
                                                                                //            }
                                                                                //        }

                                                                                //        //Extract coffee details
                                                                                //        JArray coffeeArray = parcel["coffee"] as JArray;
                                                                                //        if (coffeeArray != null && coffeeArray.Any())
                                                                                //        {
                                                                                //            var crops = new List<KYFCropExtract>();
                                                                                //            foreach (JObject coffeeObject in coffeeArray)
                                                                                //            {
                                                                                //                JArray content =
                                                                                //                    coffeeObject["content"] as JArray;

                                                                                //                if (content != null && content.Any())
                                                                                //                {
                                                                                //                    // Extract the crop name.
                                                                                //                    string cropName = content
                                                                                //                        .FirstOrDefault(item =>
                                                                                //                            item["name"] != null
                                                                                //                            && item["name"].ToString()
                                                                                //                                == "crop_name"
                                                                                //                        )
                                                                                //                        ?["value"]?.ToString();

                                                                                //                    // Extract the acreage for this crop.
                                                                                //                    string numberOfTreesStr = content
                                                                                //                        .FirstOrDefault(item =>
                                                                                //                            item["name"] != null
                                                                                //                            && item["name"].ToString()
                                                                                //                                == "number_coffee_crop"
                                                                                //                        )
                                                                                //                        ?["value"]?.ToString();


                                                                                //                    // Extract the acreage.
                                                                                //                    string totalCropAcreageStr = content
                                                                                //                        .FirstOrDefault(item =>
                                                                                //                            item["name"] != null
                                                                                //                            && item["name"].ToString()
                                                                                //                                == "area_under_crops"
                                                                                //                        )
                                                                                //                        ?["value"]?.ToString();

                                                                                //                    float.TryParse(
                                                                                //                            totalCropAcreageStr,
                                                                                //                            out float totalCropAcreage
                                                                                //                            );

                                                                                //                    if (
                                                                                //                        float.TryParse(
                                                                                //                            numberOfTreesStr,
                                                                                //                            out float numberOfTrees
                                                                                //                        )
                                                                                //                    )
                                                                                //                    {
                                                                                //                        crops.Add(
                                                                                //                            new KYFCropExtract(numberOfTreesStr, cropName)
                                                                                //                            {
                                                                                //                                ParcelId = $"PCL{parcelCounter.ToString().PadLeft(4, '0')}",
                                                                                //                                CropName = cropName,
                                                                                //                                TotalCropAcreage =
                                                                                //                                    numberOfTrees,
                                                                                //                                TotalAreaUnderCrops = totalCropAcreage,
                                                                                //                                NumberOfTrees = numberOfTrees
                                                                                //                            }
                                                                                //                        );
                                                                                //                    }
                                                                                //                }


                                                                                //            }

                                                                                //            if (crops != null && crops.Any())
                                                                                //            {
                                                                                //                //var summedCropDetails = crops
                                                                                //                //.GroupBy(c => c.CropName)
                                                                                //                //.Select(g => new KYFCropExtract(g.Sum(c => c.NumberOfTrees).ToString(), g.Key)
                                                                                //                //{
                                                                                //                //    CropName = g.Key,
                                                                                //                //    TotalCropAcreage = g.Sum(c =>
                                                                                //                //        c.TotalCropAcreage
                                                                                //                //    ),
                                                                                //                //    TotalAreaUnderCrops = g.Sum(c => c.TotalAreaUnderCrops),
                                                                                //                //    NumberOfTrees = g.Sum(c => c.NumberOfTrees),
                                                                                //                //})
                                                                                //                //.ToList();

                                                                                //                //rawResult.Crops.AddRange(summedCropDetails);

                                                                                //                rawResult.Crops.AddRange(crops.ToArray());
                                                                                //            }
                                                                                //        }
                                                                                //    }
                                                                                //    catch (Exception ex)
                                                                                //    {
                                                                                //        Console.WriteLine(
                                                                                //            $"Error getting crops in parcel: {ex.Message}"
                                                                                //        );
                                                                                //    }


                                                                                //    parcelCounter++;
                                                                                //}
                                                                                #endregion
                                                                            }
                                                                        }
                                                                    }

                                                                    JObject sectionF =
 sections.FirstOrDefault(s =>
        s["name"]?.ToString().Equals("Section F") == true
    ) as JObject;

                                                                    if (sectionF != null)
                                                                    {
                                                                        JArray livestockContent = sectionF["content"] as JArray;

                                                                        try
                                                                        {
                                                                            var livestockData = ProcessLivestockData(livestockContent, sections);
                                                                            //Console.WriteLine($"Livestock count: {livestockData.Count}");
                                                                            if (livestockData != null && livestockData.Any())
                                                                            {
                                                                                rawResult.Livestock.AddRange(livestockData);
                                                                            }
                                                                            else
                                                                            {
                                                                                //Console.WriteLine("No livestock");
                                                                            }
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            //Console.WriteLine($"Error processing livestock data: {ex.Message}");
                                                                        }

                                                                    }
                                                                    #region Process Aquaculture
                                                                    JObject sectionG =
                                                                    sections.FirstOrDefault(s =>
                                                                            s["name"]?.ToString().Equals("Section G") == true
                                                                        ) as JObject;

                                                                    if (sectionG != null)
                                                                    {
                                                                        JArray aquaContent = sectionG["content"] as JArray;
                                                                        if (aquaContent != null && aquaContent.Any())
                                                                        {
                                                                            try
                                                                            {
                                                                                var aquaData = ProcessAquacultureData(aquaContent, sectionG);

                                                                                if (aquaData != null && aquaData.Any())
                                                                                    rawResult.Aquaculture.AddRange(aquaData);
                                                                            }
                                                                            catch (Exception)
                                                                            {
                                                                            }
                                                                        }

                                                                    }
                                                                    #endregion

                                                                    rawResultsBag.Add(rawResult);
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    Console.WriteLine($"Error processing document: {ex.Message}\n{ex.StackTrace}");
                                                                }
                                                                break;
                                                                #endregion
                                                            }
                                                            else
                                                            {

                                                                var rendered = filter.Render(
                               args
                            )
                            .ToJson(new MongoDB.Bson.IO.JsonWriterSettings { Indent = true });
                                                                //Console.WriteLine("Query filter:\n" + rendered);

                                                            }
                                                        }
                                                    }
                                                    #endregion

                                                    #endregion
                                                }
                                                catch (Exception)
                                                {
                                                }

                                            }
                                        }
                                        catch (Exception)
                                        {

                                        }

                                    });
                                    #endregion
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error in main parallel query: {ex.Message}");
                            }
                        }

                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error fetching and processing farmer data: {ex.Message}\nStack Trace: {ex.StackTrace}"
                );
            }

            return rawResultsBag.DistinctBy(d => d.RecordId).ToList();
        }

        public static async Task<List<KYFAPExtractModel>> GetKYFCountyExtractAquaculture(string county)
        {
            var resultExtractBag = new ConcurrentBag<KYFAPExtractModel>();

            var collection = KYFDataExtraction();

            var admins = APZoneCache.APZones.Where(ap => ap.County.ToLower().Trim() == county.ToLower().Trim())?
                .Select(a => new KYFQueryAdminModel
                {
                    County = a.County,
                    Subcounty = a.Subcounty,
                    Ward = a.Ward
                })?.ToList();

            if (admins != null && admins.Any())
            {
                Parallel.ForEach(admins,
                    new ParallelOptions { MaxDegreeOfParallelism = 100 },
                    admin =>
                {
                    var dataKey = admin.GetHashCode();
                    var farmerBios = GetFarmersBio(admin.County, admin.Subcounty, admin.Ward).GetAwaiter().GetResult();

                    //var farmerBios = new FarmersBioModel
                    //{
                    //    Data = new Data
                    //    {
                    //        Farmers = new List<FarmerBioModel> { new FarmerBioModel { MongoId = "678637372a21a99a47b403c1" } }
                    //    }
                    //};

                    if (farmerBios != null)
                    {
                        Console.WriteLine($"County: {admin.County}; Subcounty: {admin.Subcounty}; Ward: {admin.Ward} ======> {farmerBios?.Count}");
                        try
                        {
                            if (farmerBios.Data != null && farmerBios.Data.Farmers != null && farmerBios.Data.Farmers.Any())
                            {
                                var missingRecords = new List<FarmerBioModel>();
                                var ids = farmerBios.Data.Farmers.Where(x => !string.IsNullOrEmpty(x.MongoId)).Select(x => Helpers.CleanStrict(x.MongoId.Trim())).ToList().Distinct().ToList();


                                foreach (var idChunk in ids.Chunk(50))
                                {
                                    var objectIdArray = new BsonArray(idChunk.Select(ObjectId.Parse));


                                    var filter = Builders<BsonDocument>.Filter.In("_id", objectIdArray);

                                    var options = new FindOptions<BsonDocument>
                                    {
                                        NoCursorTimeout = true
                                    };

                                    #region Processing
                                    using (var cursor = collection.FindAsync(filter, options).GetAwaiter().GetResult())
                                    {
                                        while (cursor.MoveNextAsync().GetAwaiter().GetResult())
                                        {
                                            if (cursor.Current != null && cursor.Current.Any())
                                            {
                                                foreach (var document in cursor.Current)
                                                {
                                                    #region Cursor Processing
                                                    var rawResult = new KYFAPExtractModel()
                                                    {
                                                        County = admin.County,
                                                        Subcounty = admin.Subcounty,
                                                        Ward = admin.Ward,
                                                    };
                                                    try
                                                    {
                                                        // Convert the document to a JSON string and parse into a JObject.
                                                        string jsonString = document.ToJson();
                                                        JObject jObject = JObject.Parse(jsonString);

                                                        rawResult.RecordId = JsonDocument.Parse(jObject["_id"].ToString()).RootElement.GetProperty("$oid").GetString();

                                                        // The JSON is expected to have a "data" array with multiple sections.
                                                        JArray sections = jObject["data"] as JArray;



                                                        rawResult.RegistrationStatus = jObject["registration_status"]
                                                            ?.ToString();

                                                        rawResult.FarmerStatus = jObject["farmer_status"]?.ToString();

                                                        if (sections != null)
                                                        {
                                                            // Extract administrative details from Section A.
                                                            JObject sectionA =
                                                                sections.FirstOrDefault(s =>
                                                                    s["name"]?.ToString().Contains("Section A") == true
                                                                ) as JObject;

                                                            if (sectionA != null)
                                                            {
                                                                // Extract administrative details from Section A.
                                                                JArray contentA = sectionA["content"] as JArray;
                                                                if (contentA != null)
                                                                {
                                                                    rawResult.RecordDate =
                                                                        contentA
                                                                            .FirstOrDefault(c =>
                                                                                c["label"]
                                                                                    ?.ToString()
                                                                                    .ToLower()
                                                                                    .Contains("date") == true
                                                                            )
                                                                            ?["value"]?.ToString() ?? "";

                                                                    try
                                                                    {
                                                                        // Extract household GPS from the field with name "gps" and type "gps"
                                                                        JObject gpsField =
                                                                            contentA.FirstOrDefault(obj =>
                                                                                (string)obj["name"] == "gps"
                                                                                && (string)obj["type"] == "gps"
                                                                            ) as JObject;

                                                                        if (gpsField != null)
                                                                        {
                                                                            JObject coords =
                                                                                gpsField["value"]["coords"] as JObject;
                                                                            rawResult.Latitude = coords["latitude"]?.ToString();
                                                                            rawResult.Longitude = coords["longitude"]
                                                                                ?.ToString();

                                                                            //Console.WriteLine($"Coordinates =>\nLatitude: {latitude}\nLongitude: {longitude}");
                                                                        }

                                                                        rawResult.ZoneNo = contentA.FirstOrDefault(obj =>
                                                                        obj["name"]?.ToString() == "enumeration_area")?["value"]?.ToString();
                                                                    }
                                                                    catch (Exception) { }
                                                                }
                                                            }

                                                            JObject enumeratorSection =
                                                            sections.FirstOrDefault(s =>
                                                            s["name"]?.ToString().Contains("Enumerator details") == true) as JObject;

                                                            if (enumeratorSection != null)
                                                            {
                                                                var enumeratorContent = enumeratorSection["content"] as JArray;

                                                                if (enumeratorContent != null && enumeratorContent.Any())
                                                                {
                                                                    rawResult.APName = enumeratorContent[0]?["value"]?.ToString();
                                                                    rawResult.APId = enumeratorContent[1]?["value"]?.ToString();
                                                                    rawResult.APPhoneNo = enumeratorContent[2]?["value"]?.ToString();
                                                                }
                                                            }

                                                            // Extract farmer personal details from Section B.
                                                            JObject sectionB =
                                                                sections.FirstOrDefault(s =>
                                                                    s["name"]?.ToString().Contains("Section B") == true
                                                                ) as JObject;

                                                            string farmerName = "",
                                                                farmerId = "",
                                                                farmerPhone = "",
                                                                farmerGender = "";
                                                            if (sectionB != null)
                                                            {
                                                                JArray contentB = sectionB["content"] as JArray;
                                                                if (contentB != null)
                                                                {
                                                                    rawResult.Name =
                                                                        contentB
                                                                            .FirstOrDefault(c =>
                                                                                c["q_code"]?.ToString() == "B02"
                                                                            )
                                                                            ?["value"]?.ToString() ?? "";
                                                                    rawResult.NationalID =
                                                                        contentB
                                                                            .FirstOrDefault(c =>
                                                                                c["q_code"]?.ToString() == "B03"
                                                                            )
                                                                            ?["value"]?.ToString() ?? "";
                                                                    rawResult.MobileNumber =
                                                                        contentB
                                                                            .FirstOrDefault(c =>
                                                                                c["q_code"]?.ToString() == "B04"
                                                                            )
                                                                            ?["value"]?.ToString() ?? "";

                                                                    try
                                                                    {
                                                                        var genderTest = contentB
                                                                                .FirstOrDefault(c =>
                                                                                    c["q_code"]?.ToString() == "B05"
                                                                                )
                                                                                ?["value"]?.ToString() ?? "";
                                                                        var validGenders = new string[] { "Male", "Female", "Other" };

                                                                        if (validGenders.Contains(genderTest))
                                                                            rawResult.Sex = genderTest;
                                                                    }
                                                                    catch (Exception)
                                                                    {
                                                                    }
                                                                    //rawResult.Sex =
                                                                    //    contentB
                                                                    //        .FirstOrDefault(c =>
                                                                    //            c["q_code"]?.ToString() == "B05"
                                                                    //        )
                                                                    //        ?["value"]?.ToString() ?? "";

                                                                    rawResult.YOB =
                                                                        sectionB["content"]
                                                                            .FirstOrDefault(obj =>
                                                                                (string)obj["name"] == "farmer_yob"
                                                                            )
                                                                            ?["value"]?.ToString() ?? "";
                                                                }
                                                            }

                                                            #region Process Aquaculture
                                                            JObject sectionG =
                                                            sections.FirstOrDefault(s =>
                                                                    s["name"]?.ToString().Equals("Section G") == true
                                                                ) as JObject;

                                                            if (sectionG != null)
                                                            {
                                                                JArray aquaContent = sectionG["content"] as JArray;
                                                                if (aquaContent != null && aquaContent.Any())
                                                                {
                                                                    try
                                                                    {
                                                                        var aquaData = ProcessAquacultureData(aquaContent, sectionG);

                                                                        if (aquaData != null && aquaData.Any())
                                                                        {
                                                                            rawResult.Aquaculture.AddRange(aquaData);
                                                                        }
                                                                    }
                                                                    catch (Exception)
                                                                    {
                                                                    }
                                                                }

                                                            }
                                                            #endregion

                                                            if (rawResult.Aquaculture != null && rawResult.Aquaculture.Any())
                                                            {
                                                                resultExtractBag.Add(rawResult);
                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Console.WriteLine($"Error processing document: {ex.Message}\n{ex.StackTrace}");
                                                    }
                                                    #endregion
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                });
            }

            return resultExtractBag.DistinctBy(d => d.RecordId).ToList();
        }

        public static async Task<List<KYFAPExtractModel>> GetKYFCountyExtractBio(string county)
        {
            var rawResultsBag = new ConcurrentBag<KYFAPExtractModel>();

            try
            {
                var collection = KYFDataExtraction();

                var admins = APZoneCache.APZones.Where(ap => ap.County.ToLower().Trim() == county.ToLower().Trim())?
                    .Select(a => new KYFQueryAdminModel
                    {
                        County = a.County,
                        Subcounty = a.Subcounty,
                        Ward = a.Ward
                    })?.ToList();

                if (admins != null && admins.Any())
                {
                    Parallel.ForEach(admins,
                        new ParallelOptions { MaxDegreeOfParallelism = 100 },
                        admin =>
                    {
                        var dataKey = admin.GetHashCode();
                        var farmerBios = GetFarmersBio(admin.County, admin.Subcounty, admin.Ward).GetAwaiter().GetResult();

                        if (farmerBios != null)
                        {
                            try
                            {
                                if (farmerBios.Data != null && farmerBios.Data.Farmers != null && farmerBios.Data.Farmers.Any())
                                {
                                    var missingRecords = new List<FarmerBioModel>();
                                    var ids = farmerBios.Data.Farmers.Where(x => !string.IsNullOrEmpty(x.MongoId)).Select(x => Helpers.CleanStrict(x.MongoId.Trim())).ToList().Distinct().ToList();


                                    foreach (var idChunk in ids.Chunk(50))
                                    {
                                        var objectIdArray = new BsonArray(idChunk.Select(ObjectId.Parse));


                                        var filter = Builders<BsonDocument>.Filter.In("_id", objectIdArray);

                                        var options = new FindOptions<BsonDocument>
                                        {
                                            NoCursorTimeout = true
                                        };

                                        #region Processing
                                        using (var cursor = collection.FindAsync(filter, options).GetAwaiter().GetResult())
                                        {
                                            while (cursor.MoveNextAsync().GetAwaiter().GetResult())
                                            {
                                                if (cursor.Current != null && cursor.Current.Any())
                                                {
                                                    foreach (var document in cursor.Current)
                                                    {
                                                        #region Cursor Processing
                                                        var rawResult = new KYFAPExtractModel()
                                                        {
                                                            County = admin.County,
                                                            Subcounty = admin.Subcounty,
                                                            Ward = admin.Ward,
                                                        };
                                                        try
                                                        {
                                                            // Convert the document to a JSON string and parse into a JObject.
                                                            string jsonString = document.ToJson();
                                                            JObject jObject = JObject.Parse(jsonString);

                                                            rawResult.RecordId = JsonDocument.Parse(jObject["_id"].ToString()).RootElement.GetProperty("$oid").GetString();

                                                            // The JSON is expected to have a "data" array with multiple sections.
                                                            JArray sections = jObject["data"] as JArray;



                                                            rawResult.RegistrationStatus = jObject["registration_status"]
                                                                ?.ToString();

                                                            rawResult.FarmerStatus = jObject["farmer_status"]?.ToString();

                                                            if (sections != null)
                                                            {
                                                                // Extract administrative details from Section A.
                                                                JObject sectionA =
                                                                    sections.FirstOrDefault(s =>
                                                                        s["name"]?.ToString().Contains("Section A") == true
                                                                    ) as JObject;

                                                                if (sectionA != null)
                                                                {
                                                                    // Extract administrative details from Section A.
                                                                    JArray contentA = sectionA["content"] as JArray;
                                                                    if (contentA != null)
                                                                    {
                                                                        rawResult.RecordDate =
                                                                            contentA
                                                                                .FirstOrDefault(c =>
                                                                                    c["label"]
                                                                                        ?.ToString()
                                                                                        .ToLower()
                                                                                        .Contains("date") == true
                                                                                )
                                                                                ?["value"]?.ToString() ?? "";

                                                                        try
                                                                        {
                                                                            // Extract household GPS from the field with name "gps" and type "gps"
                                                                            JObject gpsField =
                                                                                contentA.FirstOrDefault(obj =>
                                                                                    (string)obj["name"] == "gps"
                                                                                    && (string)obj["type"] == "gps"
                                                                                ) as JObject;

                                                                            if (gpsField != null)
                                                                            {
                                                                                JObject coords =
                                                                                    gpsField["value"]["coords"] as JObject;
                                                                                rawResult.Latitude = coords["latitude"]?.ToString();
                                                                                rawResult.Longitude = coords["longitude"]
                                                                                    ?.ToString();

                                                                                //Console.WriteLine($"Coordinates =>\nLatitude: {latitude}\nLongitude: {longitude}");
                                                                            }

                                                                            rawResult.ZoneNo = contentA.FirstOrDefault(obj =>
                                                                            obj["name"]?.ToString() == "enumeration_area")?["value"]?.ToString();
                                                                        }
                                                                        catch (Exception) { }
                                                                    }
                                                                }

                                                                JObject enumeratorSection =
                                                                sections.FirstOrDefault(s =>
                                                                s["name"]?.ToString().Contains("Enumerator details") == true) as JObject;

                                                                if (enumeratorSection != null)
                                                                {
                                                                    var enumeratorContent = enumeratorSection["content"] as JArray;

                                                                    if (enumeratorContent != null && enumeratorContent.Any())
                                                                    {
                                                                        rawResult.APId = enumeratorContent[1]?["value"]?.ToString();
                                                                        rawResult.APPhoneNo = enumeratorContent[2]?["value"]?.ToString();
                                                                    }
                                                                }

                                                                // Extract farmer personal details from Section B.
                                                                JObject sectionB =
                                                                    sections.FirstOrDefault(s =>
                                                                        s["name"]?.ToString().Contains("Section B") == true
                                                                    ) as JObject;

                                                                string farmerName = "",
                                                                    farmerId = "",
                                                                    farmerPhone = "",
                                                                    farmerGender = "";
                                                                if (sectionB != null)
                                                                {
                                                                    JArray contentB = sectionB["content"] as JArray;
                                                                    if (contentB != null)
                                                                    {
                                                                        rawResult.Name =
                                                                            contentB
                                                                                .FirstOrDefault(c =>
                                                                                    c["q_code"]?.ToString() == "B02"
                                                                                )
                                                                                ?["value"]?.ToString() ?? "";
                                                                        rawResult.NationalID =
                                                                            contentB
                                                                                .FirstOrDefault(c =>
                                                                                    c["q_code"]?.ToString() == "B03"
                                                                                )
                                                                                ?["value"]?.ToString() ?? "";
                                                                        rawResult.MobileNumber =
                                                                            contentB
                                                                                .FirstOrDefault(c =>
                                                                                    c["q_code"]?.ToString() == "B04"
                                                                                )
                                                                                ?["value"]?.ToString() ?? "";

                                                                        try
                                                                        {
                                                                            var genderTest = contentB
                                                                                    .FirstOrDefault(c =>
                                                                                        c["q_code"]?.ToString() == "B05"
                                                                                    )
                                                                                    ?["value"]?.ToString() ?? "";
                                                                            var validGenders = new string[] { "Male", "Female", "Other" };

                                                                            if (validGenders.Contains(genderTest))
                                                                                rawResult.Sex = genderTest;
                                                                        }
                                                                        catch (Exception)
                                                                        {
                                                                        }
                                                                        //rawResult.Sex =
                                                                        //    contentB
                                                                        //        .FirstOrDefault(c =>
                                                                        //            c["q_code"]?.ToString() == "B05"
                                                                        //        )
                                                                        //        ?["value"]?.ToString() ?? "";

                                                                        rawResult.YOB =
                                                                            sectionB["content"]
                                                                                .FirstOrDefault(obj =>
                                                                                    (string)obj["name"] == "farmer_yob"
                                                                                )
                                                                                ?["value"]?.ToString() ?? "";

                                                                        rawResult.HouseholdSize =
                                                                            contentB
                                                                                .FirstOrDefault(c =>
                                                                                    (c["q_code"]?.ToString() == "B08" || c["name"]?.Contains("hh_size") == true)
                                                                                )
                                                                                ?["value"]?.ToString() ?? "";
                                                                    }
                                                                }


                                                                // Extract parcel details from Section C.
                                                                JObject sectionC =
                                                                    sections.FirstOrDefault(s =>
                                                                        s["name"]?.ToString().Contains("Section C") == true
                                                                    ) as JObject;

                                                                if (sectionC != null)
                                                                {
                                                                    JArray parcels = sectionC["parcels"] as JArray;
                                                                    if (parcels != null && parcels.Any())
                                                                    {
                                                                        var totalParcelsAcreage = 0f;
                                                                        foreach (var parcel in parcels)
                                                                        {
                                                                            var parcelContent = parcel["content"] as JArray;

                                                                            string acreage = parcelContent
                                                                            ?.FirstOrDefault(c => c["name"]?.ToString().Equals("parcel_total_acreage",
                                                                            StringComparison.OrdinalIgnoreCase) == true)?["value"]?.ToString() ?? "";

                                                                            if (float.TryParse(acreage, out float acre))
                                                                                totalParcelsAcreage += acre;
                                                                        }

                                                                        rawResult.Parcels.Add(new ParcelExract
                                                                        {
                                                                            Acreage = totalParcelsAcreage.ToString()
                                                                        });
                                                                    }
                                                                }

                                                                rawResultsBag.Add(rawResult);
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine($"Error processing document: {ex.Message}\n{ex.StackTrace}");
                                                        }
                                                        #endregion
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    });
                }
            }
            catch (Exception)
            {
            }

            return rawResultsBag.ToList();
        }

        public static async Task<DataTable> FarmerBiosFormatter(List<KYFAPExtractModel> kyfFarmerData)
        {
            var resultTable = new DataTable();

            resultTable.Columns.Add(new DataColumn("RecordId", typeof(string)));
            resultTable.Columns.Add(new DataColumn("FarmerId", typeof(string)));
            resultTable.Columns.Add(new DataColumn("FarmerName", typeof(string)));
            resultTable.Columns.Add(new DataColumn("FarmerPhone", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Gender", typeof(string)));
            resultTable.Columns.Add(new DataColumn("HouseholdSize", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Latitude", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Longitude", typeof(string)));
            resultTable.Columns.Add(new DataColumn("County", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Subcounty", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Ward", typeof(string)));
            resultTable.Columns.Add(new DataColumn("ApName", typeof(string)));
            resultTable.Columns.Add(new DataColumn("ApZone", typeof(string)));
            resultTable.Columns.Add(new DataColumn("ApId", typeof(string)));
            resultTable.Columns.Add(new DataColumn("ApPhone", typeof(string)));
            resultTable.Columns.Add(new DataColumn("TotalParcelAcreage", typeof(string)));

            if (kyfFarmerData != null && kyfFarmerData.Any())
            {
                foreach (var data in kyfFarmerData)
                {
                    try
                    {
                        var resultRow = resultTable.NewRow();

                        resultRow[0] = data.RecordId;
                        resultRow[1] = data.NationalID;
                        resultRow[2] = data.Name;
                        resultRow[3] = data.MobileNumber;
                        resultRow[4] = data.Sex;
                        resultRow[5] = data.HouseholdSize;
                        resultRow[6] = data.Latitude;
                        resultRow[7] = data.Longitude;
                        resultRow[8] = data.County;
                        resultRow[9] = data.Subcounty;
                        resultRow[10] = data.Ward;
                        resultRow[11] = data.APName;
                        resultRow[12] = data.ZoneNo;
                        resultRow[13] = data.APId;
                        resultRow[14] = data.APPhoneNo;
                        resultRow[15] = data.Parcels?.FirstOrDefault()?.Acreage;

                        resultTable.Rows.Add(resultRow);
                    }
                    catch (Exception)
                    {
                    }

                }
            }

            resultTable.AcceptChanges();

            return resultTable;
        }
        private static async Task<FarmersBioModel> GetFarmersBio(string county, string subcounty, string ward)
        {
            var farmers = new FarmersBioModel();
            try
            {
                var credentials = new CredentialsManager.Supplier("Credentials.json");
                var client = new HttpClient()
                {
                    Timeout = TimeSpan.FromMinutes(60)
                };
                var request = new HttpRequestMessage(HttpMethod.Get, $"{credentials.GetSecret("APIKeys:KYFFarmerBioUrl")}county={county}&subcounty={subcounty}&ward={ward}&page=1&page_size={99999999}");
                request.Headers.Add("Authorization", $"Token {credentials.GetSecret("APIKeys:KYFFarmerBio")}");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var rawFarmers = await response.Content.ReadAsStringAsync();

                farmers = Newtonsoft.Json.JsonConvert.DeserializeObject<FarmersBioModel>(rawFarmers);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching biodata and deserializing: {ex.Message}\n{ex.StackTrace}");
            }
            return farmers;
        }
        public static async Task<DataTable> CropsDataFormatter(List<KYFAPExtractModel> kyfFarmerData)
        {
            using var resultTable = new DataTable();

            resultTable.Columns.Add(new DataColumn("RecordId", typeof(string)));
            resultTable.Columns.Add(new DataColumn("FarmerId", typeof(string)));
            resultTable.Columns.Add(new DataColumn("FarmerName", typeof(string)));
            resultTable.Columns.Add(new DataColumn("FarmerPhone", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Gender", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Latitude", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Longitude", typeof(string)));
            resultTable.Columns.Add(new DataColumn("County", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Subcounty", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Ward", typeof(string)));
            resultTable.Columns.Add(new DataColumn("ApName", typeof(string)));
            resultTable.Columns.Add(new DataColumn("ApZone", typeof(string)));
            resultTable.Columns.Add(new DataColumn("ApId", typeof(string)));
            resultTable.Columns.Add(new DataColumn("ApPhone", typeof(string)));
            resultTable.Columns.Add(new DataColumn("ParcelId", typeof(string)));
            resultTable.Columns.Add(new DataColumn("ParcelAcreage", typeof(string)));
            resultTable.Columns.Add(new DataColumn("ParcelOwnership", typeof(string)));


            var crops = kyfFarmerData.SelectMany(k => k.Crops.Select(c => c.CropName)).Distinct().Order().ToList();
            var recordIds = kyfFarmerData.Select(k => k.RecordId).Distinct().ToList();

            foreach (var crop in crops)
            {
                resultTable.Columns.Add(new DataColumn(crop.Replace(" ", "_"), typeof(string)));
                //resultTable.Columns.Add(new DataColumn($"{crop.Replace(" ", "_")}_Acreage", typeof(string)));
            }

            var recordParcels = recordIds.AsParallel().Select(r =>
            {
                var parcels = kyfFarmerData.Where(k => k.RecordId == r).SelectMany(r => r.Parcels).ToList();

                return new { RecordId = r, Parcels = parcels };
            }).ToList();

            var resultRowBag = new ConcurrentBag<DataRow>();

            Parallel.ForEach(recordParcels, recordParcel =>
            {
                var farmerBio = kyfFarmerData.FirstOrDefault(k => k.RecordId == recordParcel.RecordId);
                var farmerCrops = kyfFarmerData.Where(k => k.RecordId == recordParcel.RecordId).SelectMany(k => k.Crops).ToList();

                foreach (var parcel in recordParcel.Parcels)
                {
                    var parcelCrops = farmerCrops.Where(f => f.ParcelId == parcel.ParcelId).ToList();


                    try
                    {
                        var resultRow = resultTable.NewRow();
                        if (IsValidRecordId(recordParcel.RecordId))
                        {
                            resultRow["RecordId"] = recordParcel.RecordId;
                            resultRow["FarmerId"] = farmerBio.NationalID;
                            resultRow["FarmerName"] = farmerBio.Name;
                            resultRow["FarmerPhone"] = farmerBio.MobileNumber;
                            resultRow["Gender"] = farmerBio.Sex;
                            resultRow["Latitude"] = farmerBio.Latitude;
                            resultRow["Longitude"] = farmerBio.Longitude;
                            resultRow["County"] = farmerBio.County;
                            resultRow["Subcounty"] = farmerBio.Subcounty;
                            resultRow["Ward"] = farmerBio.Ward;
                            resultRow["ApName"] = farmerBio.APName;
                            resultRow["ApZone"] = farmerBio.ZoneNo;
                            resultRow["ApId"] = farmerBio.APId;
                            resultRow["ApPhone"] = farmerBio.APPhoneNo;
                            resultRow["ParcelId"] = parcel.ParcelId;
                            resultRow["ParcelAcreage"] = parcel.Acreage;
                            resultRow["ParcelOwnership"] = parcel.OwnershipStatus;

                            foreach (var crop in parcelCrops)
                            {
                                resultRow[$"{crop.CropName.Replace(" ", "_")}"] = crop.TotalCropAcreage;
                            }

                            resultRowBag.Add(resultRow);
                        }
                    }
                    catch (Exception)
                    {
                    }

                }
            });

            foreach (var row in resultRowBag)
            {
                resultTable.Rows.Add(row);
            }

            #region Old
            //foreach (var recordParcel in recordParcels)
            //{
            //    var farmerBio = kyfFarmerData.FirstOrDefault(k => k.RecordId == recordParcel.RecordId);
            //    var farmerCrops = kyfFarmerData.Where(k => k.RecordId == recordParcel.RecordId).SelectMany(k => k.Crops).ToList();

            //    foreach (var parcel in recordParcel.Parcels)
            //    {
            //        var parcelCrops = farmerCrops.Where(f => f.ParcelId == parcel.ParcelId).ToList();
            //        var resultRow = resultTable.NewRow();

            //        resultRow[0] = recordParcel.RecordId;
            //        resultRow[1] = farmerBio.NationalID;
            //        resultRow[2] = farmerBio.Name;
            //        resultRow[3] = farmerBio.MobileNumber;
            //        resultRow[4] = farmerBio.Sex;
            //        resultRow[5] = farmerBio.Latitude;
            //        resultRow[6] = farmerBio.Longitude;
            //        resultRow[7] = farmerBio.County;
            //        resultRow[8] = farmerBio.Subcounty;
            //        resultRow[9] = farmerBio.Ward;
            //        resultRow[10] = farmerBio.ZoneNo;
            //        resultRow[11] = farmerBio.APId;
            //        resultRow[12] = farmerBio.APPhoneNo;
            //        resultRow[13] = parcel.ParcelId;
            //        resultRow[14] = parcel.Acreage;
            //        resultRow[15] = parcel.OwnershipStatus;

            //        foreach (var crop in parcelCrops)
            //        {
            //            resultRow[$"{crop.CropName.Replace(" ", "_")}"] = crop.TotalCropAcreage;
            //        }

            //        resultTable.Rows.Add(resultRow);

            //    }
            //}
            #endregion
            resultTable.AcceptChanges();
            return resultTable;
        }

        public static async Task<DataTable> LivestockDataFormatter(List<KYFAPExtractModel> kyfFarmerData)
        {
            using var resultTable = new DataTable();

            resultTable.Columns.Add(new DataColumn("RecordId", typeof(string)));
            resultTable.Columns.Add(new DataColumn("FarmerId", typeof(string)));
            resultTable.Columns.Add(new DataColumn("FarmerName", typeof(string)));
            resultTable.Columns.Add(new DataColumn("FarmerPhone", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Gender", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Latitude", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Longitude", typeof(string)));
            resultTable.Columns.Add(new DataColumn("County", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Subcounty", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Ward", typeof(string)));
            resultTable.Columns.Add(new DataColumn("ApZone", typeof(string)));
            resultTable.Columns.Add(new DataColumn("ApName", typeof(string)));
            resultTable.Columns.Add(new DataColumn("ApId", typeof(string)));
            resultTable.Columns.Add(new DataColumn("ApPhone", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Category", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Subcategory", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Species", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Detail", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Count", typeof(string)));
            resultTable.Columns.Add(new DataColumn("ProductionSystem", typeof(string)));

            var recordIds = kyfFarmerData.Select(k => k.RecordId).Distinct().ToList();

            var records = recordIds.AsParallel().Select(r =>

            new
            {
                RecordId = r,
                Livestock = kyfFarmerData.Where(k => k.RecordId == r && k.Livestock.Any())?.SelectMany(k => k.Livestock)?.ToList(),
                Bio = kyfFarmerData.Where(k => k.RecordId == r && !string.IsNullOrEmpty(k.County) && !string.IsNullOrEmpty(k.Subcounty) && !string.IsNullOrEmpty(k.Ward))?.FirstOrDefault()
            }).ToArray();

            var resultRowBag = new ConcurrentBag<DataRow>();

            Parallel.For(0, records.Length, i =>
            {
                try
                {
                    if (records[i].Livestock.Any())
                    {
                        for (int j = 0; j < records[i].Livestock.Count(); j++)
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(records[i].RecordId) &&
                                IsValidRecordId(records[i].RecordId) &&
                                !string.IsNullOrEmpty(records[i].Bio.County) &&
                                !string.IsNullOrEmpty(records[i].Bio.Subcounty) &&
                                !string.IsNullOrEmpty(records[i].Bio.Ward) &&
                                !string.IsNullOrEmpty(records[i].Livestock[j].Category) &&
                                !string.IsNullOrEmpty(records[i].Livestock[j]?.Count.ToString()))
                                {
                                    var resultRow = resultTable.NewRow();

                                    resultRow["RecordId"] = records[i].RecordId;
                                    resultRow["FarmerId"] = records[i].Bio.NationalID;
                                    resultRow["FarmerName"] = records[i].Bio.Name;
                                    resultRow["FarmerPhone"] = records[i].Bio.MobileNumber;
                                    resultRow["Gender"] = records[i].Bio.Sex;
                                    resultRow["Latitude"] = records[i].Bio.Latitude;
                                    resultRow["Longitude"] = records[i].Bio.Longitude;
                                    resultRow["County"] = records[i].Bio.County;
                                    resultRow["Subcounty"] = records[i].Bio.Subcounty;
                                    resultRow["Ward"] = records[i].Bio.Ward;
                                    resultRow["ApZone"] = records[i].Bio.ZoneNo;
                                    resultRow["ApName"] = records[i].Bio.APName;
                                    resultRow["ApId"] = records[i].Bio.APId;
                                    resultRow["ApPhone"] = records[i].Bio.APPhoneNo;
                                    resultRow["Category"] = records[i].Livestock[j].Category;
                                    resultRow["Subcategory"] = records[i].Livestock[j].SubCategory;
                                    resultRow["Species"] = records[i].Livestock[j].Breed;
                                    resultRow["Detail"] = records[i].Livestock[j].LivestockDetail;
                                    resultRow["Count"] = records[i].Livestock[j].Count;
                                    resultRow["ProductionSystem"] = records[i].Livestock[j].ProductionSystem;

                                    resultRowBag.Add(resultRow);
                                }

                            }
                            catch (Exception)
                            {
                            }
                        }

                    }
                }
                catch (Exception)
                {
                }
            });

            foreach (var row in resultRowBag)
            {
                resultTable.Rows.Add(row);
            }

            #region Old
            //for (int i = 0; i < records.Length; i++)
            //{
            //    if (records[i].Livestock.Any())
            //    {
            //        for (int j = 0; j < records[i].Livestock.Count(); j++)
            //        {
            //            var resultRow = resultTable.NewRow();

            //            resultRow[0] = records[i].RecordId;
            //            resultRow[1] = records[i].Bio.NationalID;
            //            resultRow[2] = records[i].Bio.Name;
            //            resultRow[3] = records[i].Bio.MobileNumber;
            //            resultRow[4] = records[i].Bio.Sex;
            //            resultRow[5] = records[i].Bio.Latitude;
            //            resultRow[6] = records[i].Bio.Longitude;
            //            resultRow[7] = records[i].Bio.County;
            //            resultRow[8] = records[i].Bio.Subcounty;
            //            resultRow[9] = records[i].Bio.Ward;
            //            resultRow[10] = records[i].Bio.ZoneNo;
            //            resultRow[11] = records[i].Bio.APId;
            //            resultRow[12] = records[i].Bio.APPhoneNo;
            //            resultRow[13] = records[i].Livestock[j].Category;
            //            resultRow[14] = records[i].Livestock[j].SubCategory;
            //            resultRow[15] = records[i].Livestock[j].Breed;
            //            resultRow[16] = records[i].Livestock[j].LivestockDetail;
            //            resultRow[17] = records[i].Livestock[j].Count;
            //            resultRow[18] = records[i].Livestock[j].ProductionSystem;

            //            resultTable.Rows.Add(resultRow);
            //        }

            //    }


            //}
            #endregion
            resultTable.AcceptChanges();

            return resultTable;
        }

        private static bool IsValidRecordId(string recordId)
        {
            string pattern = @"\b[a-fA-F0-9]{24}\b";

            Match match = Regex.Match(recordId, pattern);

            return match.Success;
        }

        public static async Task SendAtlert(string alert, string county)
        {
            try
            {
                string from = "allan.odwar@kalro.org";
                string username = "allan.odwar@kalro.org";
                string password = "";
                string[] tos = new string[] {
                    "allan.odwar@kalro.org",
                    "Kevin.Okombo@kalro.org",
                    "fndegwamsg@gmail.com",
                    "simon.mulwa@kalro.org",
                    "salim.kinyimu@kalro.org"
                };

                foreach (var to in tos)
                {
                    using var mailMessage = new MailMessage();
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Subject = $"{county.ToUpper()} COMPLETED";
                    mailMessage.Body = alert;
                    mailMessage.From = new MailAddress(from);
                    mailMessage.To.Add(new MailAddress(to));

                    var host = "mail.kalro.org";
                    var port = 25;
                    var counter = 0;

                    while (counter <= 10)
                    {
                        var emailClient = new SmtpClient(host, port)
                        {
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(username, password),
                            //Timeout = Timeout.Infinite,
                            //EnableSsl = true,
                        };
                        try
                        {

                            emailClient.Send(mailMessage);
                            //Console.WriteLine("Email Sent");
                            counter = 10;
                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine(e);
                            //Console.WriteLine("Trying again to send email to: {0}: ", mailMessage.To.First().Address);
                        }
                        counter++;
                        Thread.Sleep(3000);
                    }
                }
            }
            catch (Exception)
            {
            }


        }

        private static async Task<FarmersBioModel> GetFarmersCountyBio(string county)
        {
            var farmers = new FarmersBioModel();
            try
            {
                var client = new HttpClient();

                var credentials = new CredentialsManager.Supplier("Credentials.json");

                var request = new HttpRequestMessage(HttpMethod.Get, $"{credentials.GetSecret("APIKeys:KYFFarmerBioUrl")}county={county}&page=1&page_size={99999999}");
                request.Headers.Add("Authorization", $"Token {credentials.GetSecret("APIKeys:KYFFarmerBio")}");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var rawFarmers = await response.Content.ReadAsStringAsync();

                farmers = Newtonsoft.Json.JsonConvert.DeserializeObject<FarmersBioModel>(rawFarmers);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching county biodata and deserializing: {ex.Message}");
            }
            return farmers;
        }
    }
}

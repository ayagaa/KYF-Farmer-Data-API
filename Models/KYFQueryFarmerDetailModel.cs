using Newtonsoft.Json;

namespace Farmer.Data.API.Models
{
    public class Data
    {
        [JsonProperty("farmers")]
        public List<FarmerBioModel> Farmers;
    }

    public class FarmerBioModel
    {
        [JsonProperty("id")]
        public int Id;

        [JsonIgnore]
        [JsonProperty("_id")]
        public string IdO;

        [JsonIgnore]
        [JsonProperty("crops")]
        public List<object> Crops;

        [JsonProperty("mongo_id")]
        public string MongoId;

        [JsonProperty("farmer_phone")]
        public string FarmerPhone;

        [JsonProperty("national_id")]
        public string NationalId;

        [JsonProperty("farmer_name")]
        public string FarmerName;

        [JsonProperty("gender")]
        public string Gender;

        [JsonProperty("farmer_status")]
        public string FarmerStatus;

        [JsonProperty("registration_status")]
        public string RegistrationStatus;

        [JsonProperty("edit_status")]
        public string EditStatus;

        [JsonProperty("remarks")]
        public string Remarks;

        [JsonIgnore]
        [JsonProperty("latitude")]
        public double Latitude;

        [JsonIgnore]
        [JsonProperty("longitude")]
        public double Longitude;

        [JsonProperty("county")]
        public string County;

        [JsonProperty("sub_county")]
        public string SubCounty;

        [JsonProperty("ward")]
        public string Ward;

        [JsonProperty("division")]
        public string Division;

        [JsonProperty("location")]
        public string Location;

        [JsonProperty("sublocation")]
        public string Sublocation;

        [JsonProperty("enumeration_area")]
        public string EnumerationArea;

        [JsonProperty("ap_national_id_number")]
        public string ApNationalIdNumber;

        [JsonProperty("ap_mobile_number")]
        public string ApMobileNumber;

        [JsonIgnore]
        [JsonProperty("deleted")]
        public bool Deleted;

        [JsonProperty("enumerator_name")]
        public string EnumeratorName;

        [JsonProperty("nearest_shopping_center")]
        public string NearestShoppingCenter;

        [JsonIgnore]
        [JsonProperty("created_at")]
        public DateTime CreatedAt;

        [JsonIgnore]
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt;

        [JsonIgnore]
        [JsonProperty("version_timestamp")]
        public DateTime VersionTimestamp;

        [JsonIgnore]
        [JsonProperty("is_latest")]
        public bool IsLatest;
    }

    public class FarmersBioModel
    {
        [JsonProperty("count")]
        public int Count;

        [JsonProperty("total_pages")]
        public int TotalPages;

        [JsonProperty("current_page")]
        public int CurrentPage;

        [JsonProperty("page_size")]
        public int PageSize;

        [JsonProperty("data")]
        public Data Data;

        [JsonProperty("next")]
        public string Next;

        [JsonProperty("previous")]
        public object Previous;
    }
}

using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Farmer.Data.API.Models
{
    public class FarmerBiodataDetailsModel
    {
        [JsonProperty("_id")]
        [JsonPropertyName("_id")]
        public string Id;

        [JsonProperty("farmer_id")]
        [JsonPropertyName("farmer_id")]
        public int FarmerId;

        [JsonProperty("id_no")]
        [JsonPropertyName("id_no")]
        public int IdNo;

        [JsonProperty("registration_status_id")]
        [JsonPropertyName("registration_status_id")]
        public int RegistrationStatusId;

        [JsonProperty("farmer_name")]
        [JsonPropertyName("farmer_name")]
        public string FarmerName;

        [JsonProperty("ward_id")]
        [JsonPropertyName("ward_id")]
        public int WardId;

        [JsonProperty("ward")]
        [JsonPropertyName("ward")]
        public string Ward;

        [JsonProperty("subcounty_id")]
        [JsonPropertyName("subcounty_id")]
        public int SubcountyId;

        [JsonProperty("subcounty")]
        [JsonPropertyName("subcounty")]
        public string Subcounty;

        [JsonProperty("county_id")]
        [JsonPropertyName("county_id")]
        public int CountyId;

        [JsonProperty("county")]
        [JsonPropertyName("county")]
        public string County;

        [JsonProperty("constituency_id")]
        [JsonPropertyName("constituency_id")]
        public int ConstituencyId;

        [JsonProperty("constituency")]
        [JsonPropertyName("constituency")]
        public string Constituency;

        [JsonProperty("division_id")]
        [JsonPropertyName("division_id")]
        public int DivisionId;

        [JsonProperty("division")]
        [JsonPropertyName("division")]
        public string Division;

        [JsonProperty("sublocation_id")]
        [JsonPropertyName("sublocation_id")]
        public int SublocationId;

        [JsonProperty("sublocation")]
        [JsonPropertyName("sublocation")]
        public string Sublocation;

        [JsonProperty("location_id")]
        [JsonPropertyName("location_id")]
        public int LocationId;

        [JsonProperty("village_name")]
        [JsonPropertyName("village_name")]
        public string VillageName;

        [JsonProperty("enumeration_area_number")]
        [JsonPropertyName("enumeration_area_number")]
        public string EnumerationAreaNumber;

        [JsonProperty("shopping_center")]
        [JsonPropertyName("shopping_center")]
        public string ShoppingCenter;

        [JsonProperty("year_of_birth")]
        [JsonPropertyName("year_of_birth")]
        public int YearOfBirth;

        [JsonProperty("gender")]
        [JsonPropertyName("gender")]
        public string Gender;

        [JsonProperty("postal_address")]
        [JsonPropertyName("postal_address")]
        public string PostalAddress;

        [JsonProperty("postal_code")]
        [JsonPropertyName("postal_code")]
        public string PostalCode;

        [JsonProperty("mobile")]
        [JsonPropertyName("mobile")]
        public int Mobile;

        [JsonProperty("marital_status_id")]
        [JsonPropertyName("marital_status_id")]
        public int MaritalStatusId;

        [JsonProperty("marital_status")]
        [JsonPropertyName("marital_status")]
        public string MaritalStatus;

        [JsonProperty("house_holdsize")]
        [JsonPropertyName("house_holdsize")]
        public int HouseHoldsize;

        [JsonProperty("education_level_id")]
        [JsonPropertyName("education_level_id")]
        public string EducationLevelId;

        [JsonProperty("education_level")]
        [JsonPropertyName("education_level")]
        public string EducationLevel;

        [JsonProperty("formal_agri_training")]
        [JsonPropertyName("formal_agri_training")]
        public string FormalAgriTraining;

        [JsonProperty("crop_prod")]
        [JsonPropertyName("crop_prod")]
        public int CropProd;

        [JsonProperty("livestock_prod")]
        [JsonPropertyName("livestock_prod")]
        public int LivestockProd;

        [JsonProperty("fish_farming")]
        [JsonPropertyName("fish_farming")]
        public int FishFarming;

        [JsonProperty("respondent_name")]
        [JsonPropertyName("respondent_name")]
        public string RespondentName;

        [JsonProperty("resp_national_id")]
        [JsonPropertyName("resp_national_id")]
        public string RespNationalId;

        [JsonProperty("respondent_mobile")]
        [JsonPropertyName("respondent_mobile")]
        public string RespondentMobile;

        [JsonProperty("farm_name")]
        [JsonPropertyName("farm_name")]
        public string FarmName;

        [JsonProperty("gps_latitude")]
        [JsonPropertyName("gps_latitude")]
        public double GpsLatitude;

        [JsonProperty("gps_longitude")]
        [JsonPropertyName("gps_longitude")]
        public double GpsLongitude;

        [JsonProperty("accuracy_level")]
        [JsonPropertyName("accuracy_level")]
        public string AccuracyLevel;

        [JsonProperty("data_source")]
        [JsonPropertyName("data_source")]
        public string DataSource;

        [JsonProperty("registration_status")]
        [JsonPropertyName("registration_status")]
        public string RegistrationStatus;

        [JsonProperty("date_of_registration")]
        [JsonPropertyName("date_of_registration")]
        public DateTime DateOfRegistration;

        [JsonProperty("start_of_registration")]
        [JsonPropertyName("start_of_registration")]
        public DateTime StartOfRegistration;

        [JsonProperty("end_of_registration")]
        [JsonPropertyName("end_of_registration")]
        public DateTime EndOfRegistration;

        [JsonProperty("location")]
        [JsonPropertyName("location")]
        public string Location;
    }
}

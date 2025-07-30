using CsvHelper.Configuration.Attributes;
using Farmer.Data.API.Utils;
using Newtonsoft.Json;

namespace Farmer.Data.API.Models
{
    public class KYFFarmerDetailModel
    {
        public KYFFarmerDetailModel()
        {

        }

        [JsonProperty("nationalId")]
        public string NationalID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("sex")]
        public string Sex { get; set; }

        [JsonProperty("yearOfBirth")]
        public string YOB { get; set; }

        [JsonProperty("mobileNumber")]
        public string MobileNumber { get; set; }

        [JsonProperty("county")]
        public string County { get; set; }

        [JsonProperty("subcounty")]
        public string Subcounty { get; set; }

        [JsonProperty("ward")]
        public string Ward { get; set; }

        [JsonProperty("registrationStatus")]
        public string RegistrationStatus { get; set; }

        [JsonProperty("farmerStatus")]
        public string FarmerStatus { get; set; }

        [JsonProperty("recordDate")]
        public string RecordDate { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("crops")]
        public List<KYFCropDetail> Crops { get; set; } = new List<KYFCropDetail>();

        [JsonProperty("livestock")]
        public List<KYFLivestockCountModel> Livestock { get; set; } = new List<KYFLivestockCountModel>();

        [JsonProperty("parcels")]
        public List<ParcelModel> Parcels { get; set; } = new List<ParcelModel>();

    }

    public class KYFFarmerExtractModel : KYFFarmerDetailModel
    {

        [Name("parcelId")]
        [JsonProperty("parcel_id")]
        public string ParcelId { get; set; }

        [JsonProperty("crops")]
        public List<KYFCropExtract> Crops { get; set; } = new List<KYFCropExtract>();

        [JsonProperty("parcels")]
        public List<ParcelExract> Parcels { get; set; } = new List<ParcelExract>();
    }

    public class KYFCropDetail
    {
        public KYFCropDetail()
        {

        }

        private float _numberOfTrees = 0f;
        private float _calculatedAcreage = 0f;


        public KYFCropDetail(string numberOfTrees, string cropName)
        {
            if (!string.IsNullOrEmpty(cropName) && float.TryParse(numberOfTrees, out _numberOfTrees) && _numberOfTrees > 0f && APZoneCache.CropTreeConversionFactors.Any(c => c.CropName == cropName))
            {
                var factor = APZoneCache.CropTreeConversionFactors.First(c => c.CropName == cropName).ConverstionFactor;

                _calculatedAcreage = _numberOfTrees / factor;
            }
        }

        [JsonProperty("cropName")]
        public string CropName { get; set; }

        private float _cropAcreage = 0f;
        [JsonProperty("totalCropAcreage")]
        public float TotalCropAcreage
        {
            get
            {
                if (_calculatedAcreage > 0f) return _calculatedAcreage;
                else return _cropAcreage;
            }
            set
            {
                if (_calculatedAcreage > 0f)
                    _cropAcreage = _calculatedAcreage;
                else
                    _cropAcreage = value;

            }
        }

        [JsonProperty("totalAreaUnderCrops")]
        public float TotalAreaUnderCrops { get; set; }

        [JsonProperty("numberOfTrees")]
        public float NumberOfTrees { get; set; }
    }

    public class KYFCropExtract : KYFCropDetail
    {
        private float _numberOfTrees = 0f;
        private float _calculatedAcreage = 0f;

        public KYFCropExtract()
        {

        }
        public KYFCropExtract(string numberOfTrees, string cropName)
        {
            if (!string.IsNullOrEmpty(cropName) && float.TryParse(numberOfTrees, out _numberOfTrees) && _numberOfTrees > 0f && APZoneCache.CropTreeConversionFactors.Any(c => c.CropName == cropName))
            {
                var factor = APZoneCache.CropTreeConversionFactors.First(c => c.CropName == cropName).ConverstionFactor;

                _calculatedAcreage = _numberOfTrees / factor;
            }
        }

        private float _cropAcreage = 0f;
        [JsonProperty("totalCropAcreage")]
        public float TotalCropAcreage
        {
            get
            {
                if (_calculatedAcreage > 0f) return _calculatedAcreage;
                else return _cropAcreage;
            }
            set
            {
                if (_calculatedAcreage > 0f)
                    _cropAcreage = _calculatedAcreage;
                else
                    _cropAcreage = value;

            }
        }

        [JsonProperty("parcel_id")]
        public string ParcelId { get; set; }
    }
    public class KYFAPExtractModel
    {
        [JsonProperty("householdSize")]
        public string HouseholdSize { get; set; }

        [JsonProperty("recordId")]
        public string RecordId { get; set; }

        [JsonProperty("apId")]
        public string APId { get; set; }

        [JsonProperty("apName")]
        public string APName { get; set; }

        [JsonProperty("apPhoneNo")]
        public string APPhoneNo { get; set; }

        [JsonProperty("zoneNo")]
        public string ZoneNo { get; set; }

        [JsonProperty("nationalId")]
        public string NationalID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("sex")]
        public string Sex { get; set; }

        [JsonProperty("yearOfBirth")]
        public string YOB { get; set; }

        [JsonProperty("mobileNumber")]
        public string MobileNumber { get; set; }

        [JsonProperty("county")]
        public string County { get; set; }

        [JsonProperty("subcounty")]
        public string Subcounty { get; set; }

        [JsonProperty("ward")]
        public string Ward { get; set; }

        [JsonProperty("registrationStatus")]
        public string RegistrationStatus { get; set; }

        [JsonProperty("farmerStatus")]
        public string FarmerStatus { get; set; }

        [JsonProperty("recordDate")]
        public string RecordDate { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("crops")]
        public List<KYFCropExtract> Crops { get; set; } = new List<KYFCropExtract>();

        [JsonProperty("livestock")]
        public List<KYFLivestockCountModel> Livestock { get; set; } = new List<KYFLivestockCountModel>();

        [JsonProperty("parcels")]
        public List<ParcelExract> Parcels { get; set; } = new List<ParcelExract>();

        [JsonProperty("aquaculture")]
        public List<KYFAquacultureModel> Aquaculture { get; set; } = new List<KYFAquacultureModel>();
    }
}

using MongoDB.Bson;
using Newtonsoft.Json;

namespace Farmer.Data.API.Utils
{
    public class ObjectIdConverter : Newtonsoft.Json.JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //base.ReadJson(reader, objectType, existingValue, serializer);
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(ObjectId).IsAssignableFrom(objectType);
            //return true;
        }
    }
}

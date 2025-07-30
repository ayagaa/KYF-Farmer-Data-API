using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Farmer.Data.API.Utils
{
    public class JsonStringAsObjectSerializer<TObject> : SerializerBase<string> where TObject : struct
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
        {
            if (value == null)
            {
                var bsonWriter = context.Writer;
                bsonWriter.WriteNull();
            }
            else
            {
                var obj = MyBsonExtensionMethods.FromJson<TObject>(value);
                var serializer = BsonSerializer.LookupSerializer(typeof(TObject));
                serializer.Serialize(context, obj);
            }
        }

        public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;
            var serializer = BsonSerializer.LookupSerializer(typeof(TObject));
            var obj = (TObject)serializer.Deserialize(context);
            return BsonExtensionMethods.ToJson(obj);
        }
    }
}

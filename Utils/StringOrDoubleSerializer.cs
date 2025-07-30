using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Farmer.Data.API.Utils
{
    public class StringOrDoubleSerializer : SerializerBase<string>
    {
        public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonType = context.Reader.GetCurrentBsonType();

            return bsonType switch
            {
                BsonType.String => context.Reader.ReadString(),
                BsonType.Double => context.Reader.ReadDouble().ToString(),
                BsonType.Int32 => context.Reader.ReadInt32().ToString(),
                _ => string.Empty,
            };
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
        {
            context.Writer.WriteString(value);
        }
    }
}

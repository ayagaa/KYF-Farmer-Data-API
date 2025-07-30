using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Farmer.Data.API.Utils
{
    public static class Json
    {
        public static JObject ParseJsonWithErrorHandling(string jsonString)
        {
            using (var stringReader = new StringReader(jsonString))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                jsonReader.SupportMultipleContent = true;

                var jsonSerializer = new JsonSerializer();
                jsonReader.DateParseHandling = DateParseHandling.None; // Customize if needed

                try
                {
                    return jsonSerializer.Deserialize<JObject>(jsonReader);
                }
                catch (JsonReaderException ex)
                {
                    Console.WriteLine($"Skipped invalid content: {ex.Message}");
                    return null;  // If you want to return a partial result, handle that here
                }
            }
        }

        public static bool HasData(JToken token)
        {
            // Check for null or JTokenType.Null
            if (token == null || token.Type == JTokenType.Null)
            {
                return false;
            }

            // For JObject or JArray, check if it has any children
            if (token is JObject || token is JArray)
            {
                return token.HasValues;
            }

            // For JValue, check if the value is not null or empty
            if (token is JValue jValue)
            {
                return jValue.Value != null && !string.IsNullOrEmpty(jValue.ToString());
            }

            // Otherwise, return true if it's not null
            return true;
        }
    }
}

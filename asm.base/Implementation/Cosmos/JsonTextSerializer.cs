using Microsoft.Azure.Cosmos;
using System.IO;
using System.Text.Json;

namespace Asm.Implementation.Cosmos
{
    public class JsonTextSerializer : CosmosSerializer
    {
        protected static JsonSerializerOptions Options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true,
            IgnoreReadOnlyProperties = true,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public override T FromStream<T>(Stream stream)
        {
            using var streamreader = new StreamReader(stream);
            return System.Text.Json.JsonSerializer.Deserialize<T>(streamreader.ReadToEnd(), Options);
        }

        public override Stream ToStream<T>(T input)
        {
            var bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(input, typeof(T), Options);
            return new MemoryStream(bytes);
        }
    }
}

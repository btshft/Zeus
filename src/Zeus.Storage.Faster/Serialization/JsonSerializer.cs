using System.Text.Json;

namespace Zeus.Storage.Faster.Serialization
{
    internal abstract class JsonSerializer<TEnvelope, TContent> : Serializer<TEnvelope, TContent>
    {
        protected JsonSerializerOptions JsonOption { get; }

        protected JsonSerializer(JsonSerializerOptions jsonOption)
        {
            JsonOption = jsonOption;
        }

        /// <inheritdoc />
        protected override byte[] GetBytes(TContent content)
        {
            return JsonSerializer.SerializeToUtf8Bytes(content, JsonOption);
        }

        /// <inheritdoc />
        protected override TContent GetContent(byte[] bytes)
        {
            return JsonSerializer.Deserialize<TContent>(bytes, JsonOption);
        }
    }
}
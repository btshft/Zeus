using System.Text;
using System.Text.Json;
using Confluent.Kafka;

namespace Zeus.Transport.Kafka.Internal
{
    internal class JsonSerializer<T> : ISerializer<T>
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public JsonSerializer(JsonSerializerOptions serializerOptions)
        {
            _serializerOptions = serializerOptions;
        }

        /// <inheritdoc />
        public byte[] Serialize(T data, SerializationContext context)
        {
            var text = JsonSerializer.Serialize(data, _serializerOptions);
            return Encoding.UTF8.GetBytes(text);
        }
    }
}
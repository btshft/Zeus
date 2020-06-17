using System;
using System.Text;
using System.Text.Json;
using Confluent.Kafka;

namespace Zeus.Transport.Kafka.Internal
{
    internal class JsonDeserializer<T> : IDeserializer<T>
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public JsonDeserializer(JsonSerializerOptions serializerOptions)
        {
            _serializerOptions = serializerOptions;
        }

        /// <inheritdoc />
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (data == null || data.Length == 0)
                return default;

            var text = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<T>(text, _serializerOptions);
        }
    }
}
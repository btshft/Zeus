using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using FASTER.core;
using Zeus.Storage.Faster.Utils;

namespace Zeus.Storage.Faster.Store.Subscriptions
{
    [JsonConverter(typeof(Converter))]
    internal readonly struct AlertsSubscriptionKey
    {
        public readonly string Channel;

        public readonly long ChatId;

        public AlertsSubscriptionKey(long chatId, string channel)
        {
            ChatId = chatId;
            Channel = channel;
        }

        public class Converter : JsonConverter<AlertsSubscriptionKey>
        {
            /// <inheritdoc />
            public override AlertsSubscriptionKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.String)
                    throw new JsonException("Malformed input, expected string token");

                var stringValues = reader.GetString().Split(':', StringSplitOptions.RemoveEmptyEntries);
                if (stringValues.Length != 2)
                    throw new JsonException("Malformed input, expected token in format v1:v2");

                if (!long.TryParse(stringValues[1], out var chatId))
                    throw new JsonException("Malformed input, expected int64 value of chat_id");

                return new AlertsSubscriptionKey(chatId, stringValues[0]);
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, AlertsSubscriptionKey value, JsonSerializerOptions options)
            {
                writer.WriteStringValue($"{value.Channel}:{value.ChatId}");
            }
        }

        public class Comparer : IFasterEqualityComparer<AlertsSubscriptionKey>
        {
            /// <inheritdoc />
            public long GetHashCode64(ref AlertsSubscriptionKey k)
            {
                var value = $"{k.ChatId}-{k.Channel}";
                var hash = MurMur3.Hash(value);
                return BitConverter.ToInt64(hash);
            }

            /// <inheritdoc />
            public bool Equals(ref AlertsSubscriptionKey k1, ref AlertsSubscriptionKey k2)
            {
                return k2.ChatId == k1.ChatId && string.Equals(k1.Channel, k2.Channel, StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}
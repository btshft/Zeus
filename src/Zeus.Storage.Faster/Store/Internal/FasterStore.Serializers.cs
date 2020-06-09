using System.Text.Json;
using Zeus.Storage.Faster.Serialization;

namespace Zeus.Storage.Faster.Store.Internal
{
    internal partial class FasterStore<TKey, TValue>
    {
        internal class KeySerializer : JsonSerializer<KeyHolder, TKey>
        {
            public KeySerializer(JsonSerializerOptions jsonOption) 
                : base(jsonOption)
            {
            }

            /// <inheritdoc />
            protected override ref TKey GetContentReference(ref KeyHolder holder)
            {
                return ref holder.Key;
            }
        }

        internal class ValueSerializer : JsonSerializer<ValueHolder, TValue>
        {
            public ValueSerializer(JsonSerializerOptions jsonOption) 
                : base(jsonOption)
            {
            }

            /// <inheritdoc />
            protected override ref TValue GetContentReference(ref ValueHolder holder)
            {
                return ref holder.Value;
            }
        }
    }
}
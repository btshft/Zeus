using Consul;

namespace Zeus.Storage.Consul.Internal
{
    internal class ConsulEntry<TValue>
    {
        public ConsulEntry(string key, TValue value, KVPair original)
        {
            Key = key;
            Value = value;
            Original = original;
        }

        public string Key { get; }

        public TValue Value { get; }

        public KVPair Original { get; }

        public static implicit operator TValue(ConsulEntry<TValue> entry) => entry.Value;
    }
}
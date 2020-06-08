using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Newtonsoft.Json;

namespace Zeus.Storage.Consul.Internal
{
    internal static class ConsulExtensions
    {
        public static Task<WriteResult<bool>> PutAsync<T>(this IKVEndpoint endpoint, string key, T value,
            JsonSerializerSettings settings = default, CancellationToken cancellation = default)
        {
            var json = settings == null
                ? JsonConvert.SerializeObject(value)
                : JsonConvert.SerializeObject(value, settings);

            var utf8Bytes = Encoding.UTF8.GetBytes(json);

            return endpoint.Put(new KVPair(key)
            {
                Value = utf8Bytes
            }, cancellation);
        }

        public static async Task<QueryResult<ConsulEntry<T>[]>> ListAsync<T>(this IKVEndpoint endpoint, string prefix,
            JsonSerializerSettings settings = default, CancellationToken cancellation = default)
        {
            ConsulEntry<T> Deserialize(KVPair content)
            {
                if (content.Value == null || content.Value.Length == 0)
                    return default;

                var utf8 = Encoding.UTF8.GetString(content.Value);
                var obj = settings != null
                    ? JsonConvert.DeserializeObject<T>(utf8, settings)
                    : JsonConvert.DeserializeObject<T>(utf8);

                return new ConsulEntry<T>(content.Key, obj, content);
            }

            var results = await endpoint.List(prefix, cancellation);
            if (results.Response == null || results.Response.Length == 0)
                return new QueryResult<ConsulEntry<T>[]>(results);

            var entries = results.Response.Select(Deserialize).ToArray();
            return new QueryResult<ConsulEntry<T>[]>(results, entries);
        }
    }
}
using System;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Zeus.Storage.Consul
{
    public class ConsulOptions
    {
        public Uri Address { get; set; }

        public string Datacenter { get; set; }

        public string Token { get; set; }

        public TimeSpan? WaitTime { get; set; }

        public Action<HttpClient> ConfigureClient { get; set; }

        public Action<HttpClientHandler> ConfigureHandler { get; set; }

        public JsonSerializerSettings SerializerSettings { get; set; } = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters =
            {
                new StringEnumConverter()
            },
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        };

        public string Prefix { get; set; } = "zeus/";
    }
}
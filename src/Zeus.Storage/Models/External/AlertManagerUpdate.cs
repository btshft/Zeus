using System;
using System.Dynamic;
using System.Text.Json.Serialization;

namespace Zeus.Storage.Models.External
{
    public class AlertManagerUpdate
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("groupKey")]
        public string GroupKey { get; set; }

        [JsonPropertyName("status")]
        public AlertStatus Status { get; set; }

        [JsonPropertyName("receiver")]
        public string Receiver { get; set; }

        [JsonPropertyName("groupLabels")]
        public ExpandoObject GroupLabels { get; set; }

        [JsonPropertyName("commonLabels")]
        public ExpandoObject CommonLabels { get; set; }

        [JsonPropertyName("commonAnnotations")]
        public ExpandoObject CommonAnnotations { get; set; }

        [JsonPropertyName("externalURL")]
        public string ExternalUrl { get; set; }

        [JsonPropertyName("alerts")]
        public Alert[] Alerts { get; set; }

        public class Alert
        {
            [JsonPropertyName("status")]
            public AlertStatus Status { get; set; }

            [JsonPropertyName("labels")]
            public ExpandoObject Labels { get; set; }

            [JsonPropertyName("annotations")]
            public ExpandoObject Annotations { get; set; }

            [JsonPropertyName("startsAt")]
            public DateTime? StartsAt { get; set; }

            [JsonPropertyName("endsAt")]
            public DateTime? EndsAt { get; set; }

            [JsonPropertyName("generatorUrl")]
            public string GeneratorUrl { get; set; }
        }

        public enum AlertStatus
        {
            Firing = 0,
            Resolved = 1
        }
    }
}

using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using Newtonsoft.Json;

namespace Zeus.Alerting.Models
{ 
    public class AlertManagerUpdate
    {
        [JsonProperty("version"), Required]
        public string Version { get; set; }

        [JsonProperty("groupKey")]
        public string GroupKey { get; set; }

        [JsonProperty("status")]
        public AlertStatus Status { get; set; }

        [JsonProperty("receiver")]
        public string Receiver { get; set; }

        [JsonProperty("groupLabels")]
        public ExpandoObject GroupLabels { get; set; }

        [JsonProperty("commonLabels")]
        public ExpandoObject CommonLabels { get; set; }

        [JsonProperty("commonAnnotations")]
        public ExpandoObject CommonAnnotations { get; set; }

        [JsonProperty("externalURL")]
        public string ExternalUrl { get; set; }

        [JsonProperty("alerts"), Required]
        public AlertManagerAlert[] Alerts { get; set; }

        public enum AlertStatus
        {
            Firing = 0,
            Resolved = 1
        }
    }
}

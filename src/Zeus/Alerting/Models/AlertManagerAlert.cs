using System;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using Newtonsoft.Json;

namespace Zeus.Alerting.Models
{
    public class AlertManagerAlert
    {
        [JsonProperty("status")]
        public AlertManagerUpdate.AlertStatus Status { get; set; }

        [JsonProperty("labels")]
        public ExpandoObject Labels { get; set; }

        [JsonProperty("annotations")]
        public ExpandoObject Annotations { get; set; }

        [JsonProperty("startsAt"), Required]
        public DateTime? StartsAt { get; set; }

        [JsonProperty("endsAt")]
        public DateTime? EndsAt { get; set; }

        [JsonProperty("generatorUrl")]
        public string GeneratorUrl { get; set; }
    }
}
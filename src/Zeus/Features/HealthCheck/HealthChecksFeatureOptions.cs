using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Zeus.Shared.Features.Optional;

namespace Zeus.Features.HealthCheck
{
    public class HealthChecksFeatureOptions : OptionalFeatureOptions
    {
        public HealthChecksFeatureOptions()
        {
            Enabled = true;
        }

        public bool AllowCachingResponses { get; set; }

        [Required]
        public HealthCheckOptions Telegram { get; set; } = new HealthCheckOptions
        {
            FailureStatus = HealthStatus.Degraded,
            Timeout = TimeSpan.FromSeconds(3)
        };

        public HealthCheckOptions Consul { get; set; } = new HealthCheckOptions
        {
            FailureStatus = HealthStatus.Degraded,
            Timeout = TimeSpan.FromSeconds(3)
        };

        public class HealthCheckOptions
        {
            public TimeSpan? Timeout { get; set; }

            public HealthStatus FailureStatus { get; set; } 
                = HealthStatus.Unhealthy;
        }
    }
}
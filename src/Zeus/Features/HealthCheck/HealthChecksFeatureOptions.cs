using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Zeus.Features.HealthCheck
{
    public class HealthChecksFeatureOptions
    {
        public bool AllowCachingResponses { get; set; }

        public bool Enabled { get; set; } = true;

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

        public HealthCheckOptions Callback { get; set; } = new HealthCheckOptions
        {
            FailureStatus = HealthStatus.Unhealthy,
            Timeout = TimeSpan.FromSeconds(2)
        };

        public class HealthCheckOptions
        {
            public TimeSpan? Timeout { get; set; }

            public HealthStatus FailureStatus { get; set; } 
                = HealthStatus.Unhealthy;
        }
    }
}
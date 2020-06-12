using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Gauge;

namespace Zeus.Features.Metrics.Core
{
    public class MetricsRegistry
    {
        public static class Units
        {
            public static readonly Unit Alerts = Unit.Custom("alert");
            public static readonly Unit Subscription = Unit.Custom("subscription");
        }

        public static class Gauges
        {
            public static readonly GaugeOptions ActiveSubscriptions = new GaugeOptions
            {
                Name = "Active Subscriptions",
                MeasurementUnit = Units.Subscription
            };
        }

        public static class Counters
        {
            public static readonly CounterOptions AlertsSentFailed = new CounterOptions
            {
                MeasurementUnit = Units.Alerts,
                Name = "Alerts Sent Failed",
                ResetOnReporting = true
            };

            public static readonly CounterOptions AlertsSentFailedTotal = new CounterOptions
            {
                MeasurementUnit = Units.Alerts,
                Name = "Alerts Sent Failed Total"
            };

            public static readonly CounterOptions AlertsSent = new CounterOptions
            {
                MeasurementUnit = Units.Alerts,
                Name = "Alerts Sent",
                ResetOnReporting = true
            };

            public static readonly CounterOptions AlertsSentTotal = new CounterOptions
            {
                MeasurementUnit = Units.Alerts,
                Name = "Alerts Sent Total",
            };
        }
    }
}
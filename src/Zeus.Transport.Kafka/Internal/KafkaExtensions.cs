using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Zeus.Transport.Kafka.Internal
{
    internal static class KafkaExtensions
    {
        public static string GetCategory(this Error error)
        {
            var category = error switch
            {
                _ when error.IsError => "Error", // lgtm[cs/constant-condition]
                _ when error.IsBrokerError => "Broker error",
                _ when error.IsLocalError => "Local error",
                _ when error.IsFatal => "Fatal",
                _ => "Unknown"
            };

            return category;
        }

        public static LogLevel GetLogLevel(this Error error)
        {
            return error.IsFatal ? LogLevel.Critical : LogLevel.Error;
        }

        public static LogLevel GetLogLevel(this SyslogLevel level)
        {
            return level switch
            {
                SyslogLevel.Emergency => LogLevel.Critical,
                SyslogLevel.Alert => LogLevel.Critical,
                SyslogLevel.Critical => LogLevel.Critical,
                SyslogLevel.Error => LogLevel.Error,
                SyslogLevel.Warning => LogLevel.Warning,
                SyslogLevel.Notice => LogLevel.Information,
                SyslogLevel.Info => LogLevel.Information,
                SyslogLevel.Debug => LogLevel.Debug,
                _ => LogLevel.Information
            };
        }
    }
}
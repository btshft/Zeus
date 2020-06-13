using System;
using Zeus.Storage.Models.External;

namespace Zeus.Models.Extensions
{
    public static class AlertExtensions
    {
        public static AlertManagerWebhookUpdate.Alert[] Normalize(this AlertManagerWebhookUpdate.Alert[] alerts)
        {
            if (alerts == null)
                return Array.Empty<AlertManagerWebhookUpdate.Alert>();

            Array.ForEach(alerts, a =>
            {
                // System.Json not supports Ignore default values yet
                // https://github.com/dotnet/runtime/pull/36322
                a.StartsAt = a.StartsAt == DateTime.MinValue ? (DateTime?) null : a.StartsAt;
                a.EndsAt = a.EndsAt == DateTime.MinValue ? (DateTime?) null : a.EndsAt;
            });

            return alerts;
        }
    }
}
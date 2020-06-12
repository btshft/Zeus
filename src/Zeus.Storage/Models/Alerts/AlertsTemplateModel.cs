using System;
using System.Dynamic;
using Zeus.Storage.Models.External;

namespace Zeus.Storage.Models.Alerts
{
    /// <summary>
    /// Template model.
    /// </summary>
    public class AlertsTemplateModel
    {
        /// <summary>
        /// Alerts.
        /// </summary>
        public AlertManagerWebhookUpdate.Alert[] Alerts { get; set; } 
            = Array.Empty<AlertManagerWebhookUpdate.Alert>();

        /// <summary>
        /// Alert status.
        /// </summary>
        public AlertManagerWebhookUpdate.AlertStatus Status { get; set; }

        /// <summary>
        /// Annotations.
        /// </summary>
        public ExpandoObject CommonAnnotations { get; set; }

        /// <summary>
        /// Common labels.
        /// </summary>
        public ExpandoObject CommonLabels { get; set; }

        /// <summary>
        /// Group labels.
        /// </summary>
        public ExpandoObject GroupLabels { get; set; }

        /// <summary>
        /// Group key.
        /// </summary>
        public string GroupKey { get; set; }
    }
}
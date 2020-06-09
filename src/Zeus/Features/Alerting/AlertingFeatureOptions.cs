using System.ComponentModel.DataAnnotations;
using Zeus.Features.Alerting.Channels;
using Zeus.Features.Alerting.Subscriptions;
using Zeus.Features.Alerting.Templates;
using Zeus.Shared.Validation;

namespace Zeus.Features.Alerting
{
    public class AlertingFeatureOptions
    {
        [Required, ValidateObject]
        public TemplatesOptions Templates { get; set; }

        [Required, ValidateObject]
        public ChannelsOptions Channels { get; set; }

        [Required, ValidateObject]
        public SubscriptionsOptions Subscriptions { get; set; }
    }
}
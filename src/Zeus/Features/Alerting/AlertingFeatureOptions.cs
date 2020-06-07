using System.ComponentModel.DataAnnotations;
using Zeus.Features.Alerting.Channels;
using Zeus.Features.Alerting.Subscriptions;
using Zeus.Features.Alerting.Templates;

namespace Zeus.Features.Alerting
{
    public class AlertingFeatureOptions
    {
        [Required]
        public TemplatesOptions Templates { get; set; }

        [Required]
        public ChannelsOptions Channels { get; set; }

        [Required]
        public SubscriptionsOptions Subscriptions { get; set; }
    }
}
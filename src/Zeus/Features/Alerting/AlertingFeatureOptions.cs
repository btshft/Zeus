using System.ComponentModel.DataAnnotations;
using Zeus.v2.Features.Alerting.Channels;
using Zeus.v2.Features.Alerting.Subscriptions;
using Zeus.v2.Features.Alerting.Templates;

namespace Zeus.v2.Features.Alerting
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
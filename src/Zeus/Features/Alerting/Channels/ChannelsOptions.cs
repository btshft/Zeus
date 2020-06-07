using System.ComponentModel.DataAnnotations;

namespace Zeus.v2.Features.Alerting.Channels
{
    public class ChannelsOptions
    {
        [Required]
        public ChannelsStoreOptions Store { get; set; }
    }
}

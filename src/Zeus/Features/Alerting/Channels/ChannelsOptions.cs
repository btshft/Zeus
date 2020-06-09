using System.ComponentModel.DataAnnotations;
using Zeus.Shared.Validation;

namespace Zeus.Features.Alerting.Channels
{
    public class ChannelsOptions
    {
        [Required, ValidateObject]
        public ChannelsStoreOptions Store { get; set; }
    }
}

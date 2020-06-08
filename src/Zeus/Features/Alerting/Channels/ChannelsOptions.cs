﻿using System.ComponentModel.DataAnnotations;

namespace Zeus.Features.Alerting.Channels
{
    public class ChannelsOptions
    {
        [Required]
        public ChannelsStoreOptions Store { get; set; }
    }
}

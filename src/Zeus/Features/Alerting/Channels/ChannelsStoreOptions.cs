using System.Collections.Generic;
using Zeus.Storage.Models.Alerts;

namespace Zeus.Features.Alerting.Channels
{
    public class ChannelsStoreOptions
    {
        public List<AlertsChannel> Predefined { get; set; }
            = new List<AlertsChannel>();
    }
}
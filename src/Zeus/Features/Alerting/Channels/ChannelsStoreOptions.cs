using System.Collections.Generic;
using Zeus.Storage.Models.Alerts;

namespace Zeus.v2.Features.Alerting.Channels
{
    public class ChannelsStoreOptions
    {
        public List<AlertsChannel> Predefined { get; set; }
            = new List<AlertsChannel>();
    }
}
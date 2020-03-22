using System.Collections.Generic;
using Telegram.Bot.Types;
using Zeus.Alerting.Models;

namespace Zeus.Alerting.Routing
{
    public class AlertRoute
    {
        public Chat Destination { get; }
        public IReadOnlyCollection<AlertManagerAlert> Alerts { get; }
        public string Name { get; }

        public AlertRoute(Chat destination, IReadOnlyCollection<AlertManagerAlert> alerts, string name)
        {
            Destination = destination;
            Alerts = alerts;
            Name = name;
        }
    }
}
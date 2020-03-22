using System.Collections.Generic;

namespace Zeus.Alerting.Options
{
    public partial class AlertingOptions
    {
        public class RoutingOptions
        {
            public DefaultRouteOptions Default { get; set; }

            public ExtendedRouteOptions[] Routes { get; set; }
        }

        public abstract class RouteOptions
        {
            public string Name { get; set; }

            public long ChatId { get; set; }
        }

        public class DefaultRouteOptions : RouteOptions
        {
            public DefaultRouteOptions()
            {
                Name = "__default";
            }
        }

        public class ExtendedRouteOptions : RouteOptions
        {
            public Dictionary<string, string> Match { get; set; }

            public Dictionary<string, string> MatchRegex { get; set; }
        }
    }
}
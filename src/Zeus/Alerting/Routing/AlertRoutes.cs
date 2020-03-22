using System;
using System.Collections.Generic;
using Zeus.Alerting.Models;

namespace Zeus.Alerting.Routing
{
    public class AlertRoutes
    {
        public AlertManagerUpdate Update { get; }

        public IReadOnlyCollection<AlertRoute> Routes { get; }

        public AlertRoutes(AlertManagerUpdate update, IReadOnlyCollection<AlertRoute> routes)
        {
            Update = update;
            Routes = routes;
        }

        public static AlertRoutes Empty(AlertManagerUpdate update) 
            => new AlertRoutes(update, Array.Empty<AlertRoute>());
    }
}
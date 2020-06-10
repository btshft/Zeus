using System;
using System.Collections.Generic;
using System.Linq;

namespace Zeus.Shared.AppFeature.Internal
{
    internal class AppFeatureServiceProvider : IServiceProvider
    {
        private readonly HashSet<object> _services;

        public AppFeatureServiceProvider(params object[] services)
        {
            _services = services.ToHashSet();
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            var service = _services
                .FirstOrDefault(serviceType.IsInstanceOfType);

            return service;
        }

        internal IEnumerable<object> GetServicesOf(Type serviceType)
        {
            return _services.Where(serviceType.IsInstanceOfType);
        }

        internal void AddService(object service)
        {
            if (service == null) 
                throw new ArgumentNullException(nameof(service));

            _services.Add(service);
        }
    }
}
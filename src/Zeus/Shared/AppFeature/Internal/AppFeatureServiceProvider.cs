using System;
using System.Collections.Generic;
using System.Linq;

namespace Zeus.Shared.AppFeature.Internal
{
    internal class AppFeatureServiceProvider : IServiceProvider
    {
        private readonly IReadOnlyCollection<object> _availableServices;

        public AppFeatureServiceProvider(params object[] services)
        {
            _availableServices = services;
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            var service = _availableServices
                .FirstOrDefault(serviceType.IsInstanceOfType);

            return service;
        }
    }
}
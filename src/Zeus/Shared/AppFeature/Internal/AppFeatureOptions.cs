using System;
using Microsoft.Extensions.Options;
using Zeus.Shared.Validation;

namespace Zeus.Shared.AppFeature.Internal
{
    internal class AppFeatureOptions<TOptions> : IOptions<TOptions> 
        where TOptions : class, new()
    {
        private readonly Lazy<TOptions> _valueFactory;

        public AppFeatureOptions(Action<TOptions> configureOptions)
        {
            if (configureOptions == null)
                throw new ArgumentNullException(nameof(configureOptions));

            _valueFactory = new Lazy<TOptions>(() =>
            {
                var options = new TOptions();
                configureOptions(options);

                DataAnnotationsValidator.EnsureValid(options);

                return options;
            });
        }

        /// <inheritdoc />
        public TOptions Value => _valueFactory.Value;
    }
}
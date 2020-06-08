using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Zeus.Localization
{
    public class MessageLocalizer<TResource> : IMessageLocalizer<TResource>
    {
        private readonly IStringLocalizer<TResource> _localizer;
        private readonly ILogger<MessageLocalizer<TResource>> _logger;

        public MessageLocalizer(
            IStringLocalizer<TResource> localizer, 
            ILogger<MessageLocalizer<TResource>> logger)
        {
            _localizer = localizer;
            _logger = logger;
        }

        /// <inheritdoc />
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var strings = _localizer.GetAllStrings(includeParentCultures).ToArray();
            foreach (var str in strings)
            {
                if (str.ResourceNotFound)
                    _logger.LogWarning($"Localized string for message '{str.Value}' not found. " +
                                       $"Searched location: {str.SearchedLocation}" +
                                       $" Current culture: {CultureInfo.CurrentCulture.Name}");
            }

            return strings;
        }

        /// <inheritdoc />
        [Obsolete("This method is obsolete. Use `CurrentCulture` and `CurrentUICulture` instead.")]
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return _localizer.WithCulture(culture);
        }

        /// <inheritdoc />
        public LocalizedString this[string name]
        {
            get
            {
                var localizedString = _localizer[name];
                if (localizedString.ResourceNotFound)
                    _logger.LogWarning($"Localized string for message '{localizedString.Value}' not found. " +
                                       $"Searched location: {localizedString.SearchedLocation}" +
                                       $" Current culture: {CultureInfo.CurrentCulture.Name}");

                return localizedString;
            }
        }

        /// <inheritdoc />
        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var localizedString = _localizer[name, arguments];
                if (localizedString.ResourceNotFound)
                    _logger.LogWarning($"Localized string for message '{localizedString.Value}' not found. " +
                                       $"Searched location: {localizedString.SearchedLocation}" +
                                       $" Current culture: {CultureInfo.CurrentCulture.Name}");

                return localizedString;
            }
        }
    }
}
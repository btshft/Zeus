using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;
using StackExchange.Profiling;

namespace Zeus.v2.Features.Profiling.Decorators
{
    public class ProfilingStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IStringLocalizerFactory _original;

        public ProfilingStringLocalizerFactory(IStringLocalizerFactory original)
        {
            _original = original;
        }

        /// <inheritdoc />
        public IStringLocalizer Create(string baseName, string location)
        {
            var localizer = _original.Create(baseName, location);
            return new ProfilingStringLocalizer(localizer, $"{location}/{baseName}");
        }

        /// <inheritdoc />
        public IStringLocalizer Create(Type resourceSource)
        {
            var localizer = _original.Create(resourceSource);
            return new ProfilingStringLocalizer(localizer, $"{resourceSource.Name}");
        }

        private class ProfilingStringLocalizer : IStringLocalizer
        {
            private readonly IStringLocalizer _original;
            private readonly string _resourceName;

            public ProfilingStringLocalizer(IStringLocalizer original, string resourceName)
            {
                _original = original;
                _resourceName = resourceName;
            }

            /// <inheritdoc />
            public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
            {
                var profiler = MiniProfiler.Current;
                if (profiler == null)
                    return _original.GetAllStrings(includeParentCultures);

                using (profiler.Step($"Localizer<{_resourceName}>: all strings"))
                {
                    return _original.GetAllStrings(includeParentCultures);
                }
            }

            /// <inheritdoc />
            [Obsolete]
            public IStringLocalizer WithCulture(CultureInfo culture)
            {
                return _original.WithCulture(culture);
            }

            /// <inheritdoc />
            public LocalizedString this[string name]
            {
                get
                {
                    var profiler = MiniProfiler.Current;
                    if (profiler == null)
                        return _original[name];

                    using (profiler.Step($"Localizer<{_resourceName}>: '{name}'"))
                        return _original[name];
                }
            }

            /// <inheritdoc />
            public LocalizedString this[string name, params object[] arguments]
            {
                get
                {
                    var profiler = MiniProfiler.Current;
                    if (profiler == null)
                        return _original[name, arguments];

                    using (profiler.Step($"Localizer<{_resourceName}>: '{name}'"))
                        return _original[name, arguments];
                }
            }
        }
    }
}
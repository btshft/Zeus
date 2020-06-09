using System;
using Microsoft.Extensions.Configuration;
using Zeus.Shared.Exceptions;

namespace Zeus.Shared.Extensions
{
    public static class ConfigurationExtensions
    {
        public static bool SectionExists(this IConfiguration configuration, string sectionName)
        {
            var section = configuration.GetSection(sectionName);
            return section.Exists();
        }

        public static IConfigurationSection GetRequiredSection(this IConfiguration configuration, string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var section = configuration.GetSection(name);
            if (!section.Exists())
                throw new ConfigurationException($"Section '{section}' not exists in configuration");

            return section;
        }

        public static Action<TOptions> CreateBinder<TOptions>(this IConfiguration configuration, string name,
            bool required)
        {
            if (name == null) 
                throw new ArgumentNullException(nameof(name));

            return (o) =>
            {
                var section = required ? configuration.GetRequiredSection(name) : configuration.GetSection(name);
                if (section.Exists())
                {
                    section.Bind(o);
                }
            };
        }
    }
}
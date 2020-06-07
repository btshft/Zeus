using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Zeus.v2.Shared.Extensions
{
    public static class OptionsBuilderExtensions
    {
        public static OptionsBuilder<TOptions> Bind<TOptions>(this OptionsBuilder<TOptions> optionsBuilder,
            IConfigurationSection section) 
            where TOptions : class
        {
            return optionsBuilder
                .Configure(section.Bind);
        }
    }
}
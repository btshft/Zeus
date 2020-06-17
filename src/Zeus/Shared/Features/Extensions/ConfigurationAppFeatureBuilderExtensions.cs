using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zeus.Shared.AppFeature;
using Zeus.Shared.AppFeature.Extensions;
using Zeus.Shared.Extensions;

namespace Zeus.Shared.Features.Extensions
{
    public static class ConfigurationAppFeatureBuilderExtensions
    {
        public static IAppFeatureCollection AddFromConfiguration<TFeature, TOptions>(this IAppFeatureCollection builder, string sectionName,
            bool required = true, bool addAlways = false)
            where TFeature : class, IAppFeature<TOptions>
            where TOptions : class, new()
        {
            var configuration = builder.ServiceProvider.GetRequiredService<IConfiguration>();

            return required
                ? builder.Add<TFeature, TOptions>(
                    configuration.CreateBinder<TOptions>(sectionName, required: true))
                : builder.AddWhen<TFeature, TOptions>(
                    () => configuration.SectionExists(sectionName) || addAlways,
                    configuration.CreateBinder<TOptions>(sectionName, required: false));
        }
    }
}
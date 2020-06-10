using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Zeus.Localization;
using Zeus.Shared.AppFeature;

namespace Zeus.Features.Localization
{
    public class LocalizationFeature : AppFeature<LocalizationFeatureOptions>
    {
        public LocalizationFeature(
            IConfiguration configuration, 
            IHostEnvironment environment, 
            IOptions<LocalizationFeatureOptions> options) : base(configuration, environment, options)
        {
        }

        /// <inheritdoc />
        public override void Configure(IServiceCollection services, IAppFeatureCollection features)
        {
            var options = Options.Value;

            services.AddLocalization(o =>
            {
                o.ResourcesPath = Path.Combine("Localization", "Resources");
            });

            services.TryAddTransient(typeof(IMessageLocalizer<>), typeof(MessageLocalizer<>));
            services.Configure<RequestLocalizationOptions>(o =>
            {
                var culture = new CultureInfo(options.Culture);

                o.SupportedCultures = new List<CultureInfo>
                {
                    culture
                };

                o.SupportedUICultures = new List<CultureInfo>
                {
                    culture
                };

                o.DefaultRequestCulture = new RequestCulture(culture);
            });
        }

        /// <inheritdoc />
        public override void Use(IApplicationBuilder builder)
        {
            builder.UseRequestLocalization();
        }
    }
}
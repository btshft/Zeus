using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Zeus.v2.Features.Api.Authorization;
using Zeus.v2.Features.Api.Localization;
using Zeus.v2.Features.Api.Swagger;
using Zeus.v2.Shared.AppFeature;
using Zeus.v2.Shared.AppFeature.Extensions;
using Zeus.v2.Shared.Features.Extensions;

namespace Zeus.v2.Features.Api
{
    public class ApiFeature : AppFeature<ApiFeatureOptions>
    {
        public ApiFeature(IConfiguration configuration, IHostEnvironment environment, IOptions<ApiFeatureOptions> options) 
            : base(configuration, environment, options)
        {
        }

        /// <inheritdoc />
        public override void Configure(IServiceCollection services, IAppFeatureCollection features)
        {
            features.AddFromConfiguration<LocalizationFeature, LocalizationFeatureOptions>("Api:Localization")
                .AddFromConfiguration<AuthorizationFeature, AuthorizationFeatureOptions>("Api:Authorization")
                .AddFromConfiguration<SwaggerFeature, SwaggerFeatureOptions>("Api:Swagger", required: false);

            services
                .AddApiVersioning(o =>
                {
                    o.ReportApiVersions = true;
                })
                .AddVersionedApiExplorer();

            services.AddHttpContextAccessor();
            services.AddControllers()
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
        }

        /// <inheritdoc />
        public override void Use(IApplicationBuilder builder)
        {
            builder.UseExceptionHandler("/error");

            builder
                .UseFeature<LocalizationFeature>();

            builder.UseRouting();
            builder.UseFeature<AuthorizationFeature>();
            builder.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            builder.UseFeature<SwaggerFeature>(required: false);
        }
    }
}

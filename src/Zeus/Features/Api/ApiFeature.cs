using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Zeus.Features.Api.Swagger;
using Zeus.Shared.AppFeature;
using Zeus.Shared.AppFeature.Extensions;
using Zeus.Shared.Features.Extensions;

namespace Zeus.Features.Api
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
            features
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
                    o.JsonSerializerOptions.IgnoreNullValues = true;
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
        }

        /// <inheritdoc />
        public override void Use(IApplicationBuilder builder)
        {
            builder.UseFeature<SwaggerFeature>(required: false);
        }

        /// <inheritdoc />
        public override void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllers();
        }
    }
}

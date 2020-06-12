using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Zeus.Shared.AppFeature;

namespace Zeus.Features.Api
{
    public class ApiFeature : AppFeature
    {
        public ApiFeature(IConfiguration configuration, IHostEnvironment environment) 
            : base(configuration, environment)
        {
        }

        /// <inheritdoc />
        protected override void ConfigureFeature(IServiceCollection services, IAppFeatureCollection features)
        {
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
        protected override void MapFeature(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllers();
        }
    }
}

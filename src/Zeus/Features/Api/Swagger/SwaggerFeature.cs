using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using Telegram.Bot.Types;
using Zeus.Storage.Models.External;
using Zeus.v2.Controllers;
using Zeus.v2.Shared.AppFeature;
using File = System.IO.File;

namespace Zeus.v2.Features.Api.Swagger
{
    public class SwaggerFeature : AppFeature<SwaggerFeatureOptions>
    {
        public SwaggerFeature(IConfiguration configuration, IHostEnvironment environment, IOptions<SwaggerFeatureOptions> options) 
            : base(configuration, environment, options)
        {
        }

        /// <inheritdoc />
        public override void Configure(IServiceCollection services, IAppFeatureCollection features)
        {
            var options = Options.Value;
            if (!options.Enabled)
                return;

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();
            services.AddSwaggerGen(s =>
            {
                var includeDocsTypesMarkers = new[]
                {
                    typeof(CallbackController),
                    typeof(AlertManagerUpdate),
                    typeof(Update)
                };

                foreach (var marker in includeDocsTypesMarkers)
                {
                    var xmlName = $"{marker.Assembly.GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlName);

                    if (File.Exists(xmlPath))
                        s.IncludeXmlComments(xmlPath);
                }
            });
        }

        /// <inheritdoc />
        public override void Use(IApplicationBuilder builder)
        {
            var options = Options.Value;
            if (!options.Enabled)
                return;

            var provider = builder.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

            builder.UseSwagger();
            builder.UseSwaggerUI(s =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    s.SwaggerEndpoint($"../swagger/{description.GroupName}/swagger.json", description.GroupName);
                    s.DocExpansion(DocExpansion.List);
                }
            });
        }
    }
}

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
using Zeus.Controllers;
using Zeus.Shared.AppFeature;
using Zeus.Shared.Features.Optional;
using Zeus.Storage.Models.External;
using File = System.IO.File;

namespace Zeus.Features.Api.Swagger
{
    public class SwaggerFeature : OptionalFeature<SwaggerFeatureOptions>
    {
        public SwaggerFeature(IConfiguration configuration, IHostEnvironment environment, IOptions<SwaggerFeatureOptions> options) 
            : base(configuration, environment, options)
        {
        }

        /// <inheritdoc />
        protected override void ConfigureFeature(IServiceCollection services, IAppFeatureCollection features)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();
            services.AddSwaggerGen(s =>
            {
                s.OperationFilter<SwaggerDefaultValues>();

                var includeDocsTypesMarkers = new[]
                {
                    typeof(CallbackController),
                    typeof(AlertManagerWebhookUpdate),
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
        protected override void UseFeature(IApplicationBuilder builder)
        {
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

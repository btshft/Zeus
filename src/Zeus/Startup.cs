using System;
using System.IO;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Zeus.Alerting.Registration;
using Zeus.Bot.Registration;
using Zeus.Bot.Requests;
using Zeus.Bot.State;
using Zeus.Extensions;
using Zeus.Storage.LiteDb;
using Zeus.Swagger.Options;
using Zeus.Templating.Scriban;

namespace Zeus
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddNewtonsoftJson();
            
            services.AddMediatR(typeof(SubscribeOnNotifications).Assembly);
            services.AddRouting();

            services.AddAlerting(o =>
            {
                var section = Configuration.GetSection("Alerting");
                // TODO Validation
                section.Bind(o);
            });

            services.AddBot(o =>
            {
                var section = Configuration.GetSection("Bot");
                // TODO Validation
                section.Bind(o);
            });

            AddStorage(services);
            AddTemplating(services);
            AddSwagger(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            UseSwagger(app);

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void AddSwagger(IServiceCollection services)
        {
            var section = Configuration.GetSection("Swagger");
            if (section.Exists())
            {
                var options = new SwaggerOptions();
                section.Bind(options);

                services.AddOptions<SwaggerOptions>()
                    .Configure<IConfiguration>((o, c) =>
                    {
                        c.GetSection("Swagger")
                            .Bind(o);
                    });

                services.AddSwaggerGen(c =>
                {
                    if (options.Docs != null)
                    {
                        foreach (var doc in options.Docs)
                            c.SwaggerGeneratorOptions.SwaggerDocs.Add(doc);
                    }

                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                    if (File.Exists(xmlPath))
                        c.IncludeXmlComments(xmlPath);
                });
            }
        }

        private void UseSwagger(IApplicationBuilder builder)
        {
            var options = builder.ApplicationServices.GetOptions<SwaggerOptions>();
            if (options != null)
            {
                builder.UseSwagger();
                builder.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zeus API V1");
                    c.RoutePrefix = options.RoutePrefix ?? string.Empty;
                });
            }
        }

        private void AddTemplating(IServiceCollection services)
        {
            var section = Configuration.GetSection("Templating:Scriban");
            if (section.Exists())
            {
                services.AddScriban(o =>
                {
                    // TODO: Validation
                    section.Bind(o);
                });
            }
        }

        private void AddStorage(IServiceCollection services)
        {
            var section = Configuration.GetSection("Storage:LiteDb");
            if (section.Exists())
            {
                services.AddLiteDb(o =>
                {
                    // TODO: Validation
                    section.Bind(o);

                    o.ConfigureMapper = mapper =>
                    {
                        mapper.RegisterType(
                            reference => JsonConvert.SerializeObject(reference),
                            value => JsonConvert.DeserializeObject<BotState>(value.AsString));

                        mapper.Entity<BotState>()
                            .Id(s => s.Id);
                    };
                });
            }
        }
    }
}
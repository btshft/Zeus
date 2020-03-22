using System;
using System.IO;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Zeus.Alerting.Options;
using Zeus.Alerting.Routing;
using Zeus.Alerting.Templating;
using Zeus.Bot.Registration;
using Zeus.Bot.Requests;
using Zeus.Bot.State;
using Zeus.Storage.LiteDb;
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

            services.AddOptions<AlertingOptions>()
                .Configure(o =>
                {
                    var section = Configuration.GetSection("Alerting");
                    // TODO Validation
                    section.Bind(o);
                });

            services.AddSingleton<IAlertRoutesResolver, AlertRoutesResolver>();
            services.AddSingleton<ITemplateResolver, TemplateResolver>();

            services.AddMediatR(typeof(SubscribeOnNotifications).Assembly);
            services.AddRouting();
            services.AddBot(o =>
            {
                var section = Configuration.GetSection("Bot");
                // TODO Validation
                section.Bind(o);
            });

            services.AddScriban(o =>
            {
                o.MemberRenamer = (m) => m.Name;
            });
            services.AddLiteDb(o =>
            {
                var section = Configuration.GetSection("Storage:LiteDb");
                // TODO Validation
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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Description = "Sends prometheus alerts to Telegram", 
                    Title = "Zeus API", 
                    Version = "1.0"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zeus API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
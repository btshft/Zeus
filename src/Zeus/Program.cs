using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Zeus.Shared.Serilog.Enrichers;

namespace Zeus
{
    public class Program
    {
        private const string LogTemplate = "{Timestamp:HH:mm:ss} [{Level:u3}] <{SourceContext:l}> {NewLine}{Message:lj}{NewLine}{Exception}{NewLine}";

        public static int Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: LogTemplate, theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            try
            {
                logger.Information("Starting host...");
                CreateHostBuilder(args).Build().Run();
                logger.Information("Stopping host...");
                return 0;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Host terminated unexpectedly.");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>();
                })
                .UseSerilog((host, config) =>
                {
                    config.ReadFrom.Configuration(host.Configuration)
                        .Enrich.FromLogContext()
                        .Enrich.With<DemistifyExceptionEnricher>();
                });
    }
}
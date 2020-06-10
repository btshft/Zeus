using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Zeus.Features.HealthCheck.Services
{
    public class SystemJsonHealthCheckResponseWriter : IHealthCheckResponseWriter
    {
        /// <inheritdoc />
        public Task WriteAsync(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var options = new JsonWriterOptions
            {
                Indented = true
            };

            using var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream, options))
            {
                writer.WriteStartObject();
                writer.WriteString("status", result.Status.ToString());
                writer.WriteStartObject("results");

                foreach (var (s, healthReportEntry) in result.Entries)
                {
                    writer.WriteStartObject(s);
                    writer.WriteString("status", healthReportEntry.Status.ToString());
                    writer.WriteString("description", healthReportEntry.Description);
                    writer.WriteStartObject("data");

                    foreach (var (key, value) in healthReportEntry.Data)
                    {
                        writer.WritePropertyName(key);
                        JsonSerializer.Serialize(
                            writer, value, value?.GetType() ?? typeof(object));
                    }
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }
                writer.WriteEndObject();
                writer.WriteEndObject();
            }

            var json = Encoding.UTF8.GetString(stream.ToArray());

            return context.Response.WriteAsync(json);
        }
    }
}
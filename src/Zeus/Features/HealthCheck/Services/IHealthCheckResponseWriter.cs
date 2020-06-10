using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Zeus.Features.HealthCheck.Services
{
    public interface IHealthCheckResponseWriter
    {
        Task WriteAsync(HttpContext context, HealthReport result);
    }
}
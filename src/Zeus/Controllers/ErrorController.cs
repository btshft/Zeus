using System;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Zeus.Shared.Exceptions;

namespace Zeus.Controllers
{
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error([FromServices] IWebHostEnvironment environment)
        {
            var details = CreateProblemInformation(environment);
            return Problem(details.Detail, details.Instance, details.StatusCode, details.Title, details.Type);
        }

        private ProblemInformation CreateProblemInformation(IHostEnvironment environment)
        {
            static HttpStatusCode GetStatusCode(Exception exception)
            {
                return exception switch
                {
                    ConflictException _ => HttpStatusCode.Conflict,
                    NotFoundException _ => HttpStatusCode.NotFound,
                    UnauthorizedAccessException _ => HttpStatusCode.Unauthorized,
                    AlertManagerUpdateException _ => HttpStatusCode.BadRequest,
                    _ => HttpStatusCode.InternalServerError
                };
            }

            var context = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var statusCode = GetStatusCode(context.Error);

            if (environment.IsDevelopment())
            {
                return new ProblemInformation
                {
                    Type = context.Error.GetType().Name,
                    Detail = $"{context.Error.Message}{context.Error.StackTrace}",
                    Instance = context.Path,
                    StatusCode = (int) statusCode,
                    Title = "Exception occured"
                };
            }

            return new ProblemInformation
            {
                Instance = context.Path,
                StatusCode = (int) statusCode,
            };
        }

        internal class ProblemInformation
        {
            public string Detail { get; set; }
            public string Instance { get; set; }
            public int? StatusCode { get; set; }
            public string Title { get; set; }
            public string Type { get; set; }
        }
    }
}
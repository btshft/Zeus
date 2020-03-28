using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Zeus.Swagger.Options
{
    /// <summary>
    /// Swagger feature options.
    /// </summary>
    public class SwaggerOptions
    {
        /// <summary>
        /// Route prefix.
        /// </summary>
        public string RoutePrefix { get; set; }

        /// <summary>
        /// API descriptions.
        /// </summary>
        public Dictionary<string, OpenApiInfo> Docs { get; set; }
    }
}
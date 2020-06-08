using System;
using System.ComponentModel.DataAnnotations;

namespace Zeus.Features.Clients
{
    public class ClientOptions
    {
        /// <summary>
        /// Clients base URL.
        /// </summary>
        [Required]
        public string BaseUrl { get; set; }

        /// <summary>
        /// Request timeout.
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
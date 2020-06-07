using System.ComponentModel.DataAnnotations;

namespace Zeus.v2.Features.Clients
{
    public class ClientsFeatureOptions
    {
        /// <summary>
        /// Callback client options.
        /// </summary>
        [Required]
        public ClientOptions Callback { get; set; }
    }
}
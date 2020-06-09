using System.ComponentModel.DataAnnotations;
using Zeus.Shared.Validation;

namespace Zeus.Features.Clients
{
    public class ClientsFeatureOptions
    {
        /// <summary>
        /// Callback client options.
        /// </summary>
        [Required, ValidateObject]
        public ClientOptions Callback { get; set; }
    }
}
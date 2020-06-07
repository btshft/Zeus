using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Zeus.v2.Shared.Security.TrustedNetwork
{
    /// <summary>
    /// Trusted network options.
    /// </summary>
    public class TrustedNetworkOptions
    {
        /// <summary>
        /// List of trusted networks.
        /// </summary>
        [CustomValidation(typeof(TrustedNetworkOptions), nameof(Validate))]
        public List<string> Networks { get; set; } 
            = new List<string>();

        public static ValidationResult Validate(List<string> networks, ValidationContext context)
        {
            if (networks == null)
                return new ValidationResult("Neworks not set");

            foreach (var network in networks)
            {
                if (!System.Net.IPNetwork.TryParse(network, out _))
                    return new ValidationResult($"'{network}' is now valid ip network");
            }

            return ValidationResult.Success;
        }
    }
}
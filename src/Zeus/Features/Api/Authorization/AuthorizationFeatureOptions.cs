using System.Collections.Generic;
using Zeus.Shared.Security.TrustedNetwork;

namespace Zeus.Features.Api.Authorization
{
    public class AuthorizationFeatureOptions
    {
        public Dictionary<string, TrustedNetworkOptions> TrustedNetwork { get; set; }
    }
}
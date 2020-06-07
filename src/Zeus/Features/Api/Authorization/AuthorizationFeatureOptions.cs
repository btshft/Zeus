using System.Collections.Generic;
using Zeus.v2.Shared.Security.TrustedNetwork;

namespace Zeus.v2.Features.Api.Authorization
{
    public class AuthorizationFeatureOptions
    {
        public Dictionary<string, TrustedNetworkOptions> TrustedNetwork { get; set; }
    }
}
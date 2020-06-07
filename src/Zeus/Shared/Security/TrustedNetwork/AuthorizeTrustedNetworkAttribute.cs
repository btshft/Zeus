using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IPNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;

namespace Zeus.Shared.Security.TrustedNetwork
{
    /// <summary>
    /// Attribute to manage trusted networks.
    /// </summary>
    public class AuthorizeTrustedNetworkAttribute : ActionFilterAttribute, IFilterFactory
    {
        private readonly IOptionsMonitor<TrustedNetworkOptions> _optionsMonitor;
        private readonly ILogger<AuthorizeTrustedNetworkAttribute> _logger;

        private string Policy { get; set; }

        public AuthorizeTrustedNetworkAttribute(string policy)
        {
            Policy = policy;
        }

        private AuthorizeTrustedNetworkAttribute(IOptionsMonitor<TrustedNetworkOptions> optionsMonitor, ILogger<AuthorizeTrustedNetworkAttribute> logger)
        {
            _optionsMonitor = optionsMonitor;
            _logger = logger;
        }

        /// <inheritdoc />
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<TrustedNetworkOptions>>();
            var logger = serviceProvider.GetRequiredService<ILogger<AuthorizeTrustedNetworkAttribute>>();

            return new AuthorizeTrustedNetworkAttribute(optionsMonitor, logger)
            {
                Policy = Policy
            };
        }

        /// <inheritdoc />
        public bool IsReusable => false;

        /// <inheritdoc />
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var options = _optionsMonitor.Get(Policy);
            if (!options.Networks.Any())
                return;

            var networks = options.Networks.Select(s =>
            {
                var typedNetwork = System.Net.IPNetwork.Parse(s);
                return new IPNetwork(typedNetwork.Network, typedNetwork.Cidr);
            });

            var remoteIp = context.HttpContext.Connection.RemoteIpAddress.IsIPv4MappedToIPv6
                ? context.HttpContext.Connection.RemoteIpAddress.MapToIPv4()
                : context.HttpContext.Connection.RemoteIpAddress;

            var anyNetworkAllowsRemote = networks.Any(n => n.Contains(remoteIp));
            if (!anyNetworkAllowsRemote)
            {
                _logger.LogWarning($"Access to IP '{remoteIp}' denied by trusted network policy '{Policy}'");
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
            }
            else
            {
                _logger.LogInformation($"Access to IP '{remoteIp}' allowed by trusted network policy '{Policy}'");
                base.OnActionExecuting(context);
            }
        }
    }
}
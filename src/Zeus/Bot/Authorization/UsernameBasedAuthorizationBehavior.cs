using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Zeus.Bot.Logging;
using Zeus.Bot.Options;
using Zeus.Bot.Requests.Abstractions;

namespace Zeus.Bot.Authorization
{
    public class UsernameBasedAuthorizationBehavior<TRequest> : IPipelineBehavior<TRequest, Unit>
        where TRequest : class, IBotCommandRequest
    {
        private readonly IOptions<BotOptions> _optionsProvider;
        private readonly ILogger<UsernameBasedAuthorizationBehavior<TRequest>> _logger;

        public UsernameBasedAuthorizationBehavior(
            IOptions<BotOptions> optionsProvider, 
            ILogger<UsernameBasedAuthorizationBehavior<TRequest>> logger)
        {
            _optionsProvider = optionsProvider;
            _logger = logger;
        }

        /// <inheritdoc />
        public Task<Unit> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Unit> next)
        {
            var security = _optionsProvider.Value.Authorization;
            if (security?.Users != null)
            {
                var isAllowed = security.Users.Contains(request.Update.Message.From.Username);
                if (!isAllowed)
                {
                    var json = JsonConvert.SerializeObject(request.Update, Formatting.Indented);
                    _logger.LogError(BotLogEvents.UserActionForbidden,
                        $"Request '{typeof(TRequest).Name}' execution forbidden - current user is not bot administrator. Update:{Environment.NewLine}{json}");

                    return Task.FromResult(Unit.Value);
                }
            }

            return next();
        }
    }
}

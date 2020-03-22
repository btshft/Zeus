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

namespace Zeus.Bot.Security
{
    public class AuthorizeBotCommandRequestBehavior<TRequest> : IPipelineBehavior<TRequest, Unit>
        where TRequest : class, IBotCommandRequest
    {
        private readonly IOptions<BotOptions> _optionsProvider;
        private readonly ILogger<AuthorizeBotCommandRequestBehavior<TRequest>> _logger;

        public AuthorizeBotCommandRequestBehavior(IOptions<BotOptions> optionsProvider, ILogger<AuthorizeBotCommandRequestBehavior<TRequest>> logger)
        {
            _optionsProvider = optionsProvider;
            _logger = logger;
        }

        /// <inheritdoc />
        public Task<Unit> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Unit> next)
        {
            var security = _optionsProvider.Value.Security;
            if (security?.Administrators != null)
            {
                var isAllowed = security.Administrators.Contains(request.Update.Message.From.Username);
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

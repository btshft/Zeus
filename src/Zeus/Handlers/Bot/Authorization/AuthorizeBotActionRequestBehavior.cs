using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Zeus.Features.Bot.Authorize;
using Zeus.Handlers.Bot.Abstractions;
using Zeus.Handlers.Bot.Context;
using Zeus.Shared.Mediatr;

namespace Zeus.Handlers.Bot.Authorization
{
    public class AuthorizeBotActionRequestBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    {
        private readonly IOptions<BotAuthorizationFeatureOptions> _optionsProvider;
        private readonly ILogger<AuthorizeBotActionRequestBehavior<TRequest, TResult>> _logger;
        private readonly IBotActionContextAccessor _contextAccessor;
        private readonly IRequestHandlerFinder _handlerFinder;

        public AuthorizeBotActionRequestBehavior(
            IOptions<BotAuthorizationFeatureOptions> optionsProvider,
            ILogger<AuthorizeBotActionRequestBehavior<TRequest, TResult>> logger, 
            IBotActionContextAccessor contextAccessor, 
            IRequestHandlerFinder handlerFinder)
        {
            _optionsProvider = optionsProvider;
            _logger = logger;
            _contextAccessor = contextAccessor;
            _handlerFinder = handlerFinder;
        }

        /// <inheritdoc />
        public Task<TResult> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResult> next)
        {
            if (request is IBotActionRequest botRequest)
            {
                if (_contextAccessor.Context.IsAuthorized)
                    return next();

                var handlerType = _handlerFinder.FindHandlerType(typeof(TRequest));
                if (handlerType != null)
                {
                    var isAnonymous = handlerType.GetCustomAttribute<AllowAnonymousAttribute>(inherit: false) != null;
                    if (isAnonymous)
                    {
                        _contextAccessor.Context.IsAnonymous = true;
                        return next();
                    }
                }

                var options = _optionsProvider.Value;
                var sender = botRequest.Update.Message?.From;
                if (sender != null)
                {
                    var isTrusted = options.TrustedUsers.AdministratorIds.Contains(sender.Id);
                    if (!isTrusted)
                        return Task.FromException<TResult>(
                            new UnauthorizedAccessException($"User '{sender}' is not trusted, command declined."));

                    _contextAccessor.Context.IsAuthorized = true;

                    _logger.LogInformation($"Action from user '{sender}' authorized successfully");
                }
            }

            return next();
        }
    }
}
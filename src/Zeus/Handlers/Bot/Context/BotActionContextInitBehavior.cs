using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Zeus.Handlers.Bot.Abstractions;

namespace Zeus.Handlers.Bot.Context
{
    public class BotActionContextInitBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IBotActionContextAccessor _botContextAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBotUserProvider _botUserProvider;

        public BotActionContextInitBehavior(
            IBotActionContextAccessor botContextAccessor, 
            IHttpContextAccessor httpContextAccessor, 
            IBotUserProvider botUserProvider)
        {
            _botContextAccessor = botContextAccessor;
            _httpContextAccessor = httpContextAccessor;
            _botUserProvider = botUserProvider;
        }

        /// <inheritdoc />
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is IBotActionRequest botRequest)
            {
                var update = botRequest.Update;
                var bot = await _botUserProvider.GetAsync(cancellationToken);
                
                _botContextAccessor.Context = new BotActionContext
                {
                    Update = update,
                    TraceId = _httpContextAccessor.HttpContext.TraceIdentifier,
                    Bot = bot
                };
            }

            return await next();
        }
    }
}
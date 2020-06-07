using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Zeus.v2.Handlers.Bot.Abstractions;

namespace Zeus.v2.Handlers.Bot.Context
{
    public class BotActionContextInitBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IBotActionContextAccessor _botContextAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BotActionContextInitBehavior(IBotActionContextAccessor botContextAccessor, IHttpContextAccessor httpContextAccessor)
        {
            _botContextAccessor = botContextAccessor;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc />
        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is IBotActionRequest botRequest)
            {
                var update = botRequest.Update;
                
                _botContextAccessor.Context = new BotActionContext
                {
                    Update = update,
                    TraceId = _httpContextAccessor.HttpContext.TraceIdentifier
                };
            }

            return next();
        }
    }
}
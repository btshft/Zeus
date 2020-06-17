using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Serilog.Context;
using Zeus.Handlers.Bot.Abstractions;

namespace Zeus.Handlers.Bot.Context
{
    public class BotActionContextInitBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IBotActionContextAccessor _botContextAccessor;
        private readonly IBotUserProvider _botUserProvider;

        public BotActionContextInitBehavior(
            IBotActionContextAccessor botContextAccessor,
            IBotUserProvider botUserProvider)
        {
            _botContextAccessor = botContextAccessor;
            _botUserProvider = botUserProvider;
        }

        /// <inheritdoc />
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is IBotActionRequest botRequest)
            {
                var update = botRequest.Update;
                var bot = await _botUserProvider.GetAsync(cancellationToken);
                var traceId = Guid.NewGuid().ToString();

                _botContextAccessor.Context = new BotActionContext
                {
                    Update = update,
                    TraceId = traceId,
                    Bot = bot
                };

                using (LogContext.PushProperty("TraceId", traceId))
                {
                    return await next();
                }
            }

            return await next();
        }
    }
}
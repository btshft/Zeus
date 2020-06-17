using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Localization;
using Zeus.Handlers.Bot.Abstractions;
using Zeus.Handlers.Bot.Consumers;
using Zeus.Handlers.Bot.Context;
using Zeus.Localization;
using Zeus.Transport;

namespace Zeus.Handlers.Bot.Behaviors
{
    public class HandleActionExceptionsBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    {
        private readonly IBotActionContextAccessor _contextAccessor;
        private readonly ITransport<SendTelegramReply> _reply;
        private readonly IMessageLocalizer<BotResources> _localizer;

        public HandleActionExceptionsBehavior(
            IBotActionContextAccessor contextAccessor,
            IMessageLocalizer<BotResources> localizer, 
            ITransport<SendTelegramReply> reply)
        {
            _contextAccessor = contextAccessor;
            _localizer = localizer;
            _reply = reply;
        }

        /// <inheritdoc />
        public async Task<TResult> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResult> next)
        {
            static bool CanReply(Exception exception)
            {
                var exceptionBlacklisted = exception is UnauthorizedAccessException;
                return !exceptionBlacklisted;
            }

            if (!(request is IBotActionRequest botRequest)) 
                return await next();

            try
            {
                return await next();
            }
            catch (Exception e) when(CanReply(e))
            {
                try
                {
                    var context = _contextAccessor.Context;
                    if ((context.IsAuthorized || context.IsAnonymous))
                    {
                        var message = _localizer.GetString(BotResources.CommandFailedUnexpectedly, context.TraceId);
                        var messageId = botRequest.Message.MessageId;
                        var chatId = botRequest.Chat.Id;

                        await _reply.SendAsync(new SendTelegramReply(chatId, message) { ReplyToMessageId = messageId }, cancellationToken);
                    }
                }
                catch (Exception inner)
                {
                    throw new AggregateException(inner, e);
                }

                throw;
            }
        }
    }
}
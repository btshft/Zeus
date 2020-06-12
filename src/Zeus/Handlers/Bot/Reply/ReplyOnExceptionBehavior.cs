using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Zeus.Handlers.Bot.Abstractions;
using Zeus.Handlers.Bot.Context;
using Zeus.Localization;
using Zeus.Shared.Mediatr;

namespace Zeus.Handlers.Bot.Reply
{
    public class ReplyOnExceptionBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    {
        private readonly IBotActionContextAccessor _contextAccessor;
        private readonly ITelegramBotClient _bot;
        private readonly IMessageLocalizer<BotResources> _localizer;
        private readonly IRequestHandlerFinder _handlerFinder;

        public ReplyOnExceptionBehavior(
            IBotActionContextAccessor contextAccessor, 
            ITelegramBotClient bot,
            IMessageLocalizer<BotResources> localizer,
            IRequestHandlerFinder handlerFinder)
        {
            _contextAccessor = contextAccessor;
            _bot = bot;
            _localizer = localizer;
            _handlerFinder = handlerFinder;
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
                    var handlerType = _handlerFinder.FindHandlerTypeByRequest(request.GetType());
                    var replyAllowed = handlerType.GetCustomAttribute<ReplyOnExceptionAttribute>(inherit: false) != null;

                    if (replyAllowed && (context.IsAuthorized || context.IsAnonymous))
                    {
                        var message = _localizer.GetString(BotResources.CommandFailedUnexpectedly, context.TraceId);
                        var messageId = botRequest.Message.MessageId;
                        var chatId = botRequest.Chat.Id;

                        await _bot.SendTextMessageAsync(new ChatId(chatId), message, replyToMessageId: messageId,
                            cancellationToken: cancellationToken);
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
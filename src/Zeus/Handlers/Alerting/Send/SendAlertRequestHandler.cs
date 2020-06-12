using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using Zeus.Shared.Exceptions;
using Zeus.Shared.Extensions;
using Zeus.Shared.Mediatr;

namespace Zeus.Handlers.Alerting.Send
{
    [WrapExceptions]
    public class SendAlertRequestHandler : AsyncRequestHandler<SendAlertRequest>
    {
        private readonly ITelegramBotClient _botClient;

        public SendAlertRequestHandler(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        /// <inheritdoc />
        protected override async Task Handle(SendAlertRequest request, CancellationToken cancellationToken)
        {
            await _botClient
                .SendTextMessageSplitAsync(new ChatId(request.Subscription.ChatId),
                    request.Text, request.ParseMode,
                    disableWebPagePreview: true,
                    disableNotification: request.Subscription.DisableNotifications, cancellationToken: cancellationToken);
        }

        public class WrapExceptionsAttribute : WrapExceptionsBaseAttribute
        {
            /// <inheritdoc />
            public override Exception Wrap(Exception source, object request)
            {
                var channel = request is SendAlertRequest typedRequest 
                    ? typedRequest.Subscription?.Channel 
                    : null;

                return new AlertSendException($"Exception occured while sending update to channel '{channel ?? "unknown"}'", source);
            }
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using Zeus.Handlers.Alerting.Consumers;
using Zeus.Shared.Exceptions;
using Zeus.Shared.Mediatr;
using Zeus.Transport;

namespace Zeus.Handlers.Alerting.Send
{
    [WrapExceptions]
    public class SendAlertRequestHandler : AsyncRequestHandler<SendAlertRequest>
    {
        private readonly ITransport<SendTelegramAlert> _transport;

        public SendAlertRequestHandler(ITransport<SendTelegramAlert> transport)
        {
            _transport = transport;
        }

        /// <inheritdoc />
        protected override async Task Handle(SendAlertRequest request, CancellationToken cancellationToken)
        {
            var chatId = new ChatId(request.Subscription.ChatId);
            var text = request.Text;

            await _transport.SendAsync(new SendTelegramAlert(chatId.Identifier, text)
            {
                ParseMode = request.ParseMode,
                DisableNotification = request.Subscription.DisableNotifications
            }, cancellationToken);
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
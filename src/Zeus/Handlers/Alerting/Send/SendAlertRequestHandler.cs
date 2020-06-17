using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using Zeus.Handlers.Bot.Consumers;
using Zeus.Shared.Exceptions;
using Zeus.Shared.Mediatr;
using Zeus.Transport;

namespace Zeus.Handlers.Alerting.Send
{
    [WrapExceptions]
    public class SendAlertRequestHandler : AsyncRequestHandler<SendAlertRequest>
    {
        private readonly ITransport<SendTelegramReply> _sendMessageTransport;

        public SendAlertRequestHandler(ITransport<SendTelegramReply> sendMessageTransport)
        {
            _sendMessageTransport = sendMessageTransport;
        }

        /// <inheritdoc />
        protected override async Task Handle(SendAlertRequest request, CancellationToken cancellationToken)
        {
            var chatId = new ChatId(request.Subscription.ChatId);
            var text = request.Text;

            await _sendMessageTransport.SendAsync(new SendTelegramReply(chatId, text)
            {
                DisableWebPagePreview = true,
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
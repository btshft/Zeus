using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot;
using Zeus.Handlers.Alerting.Notifications;
using Zeus.Shared.Extensions;
using Zeus.Transport;

namespace Zeus.Handlers.Alerting.Consumers
{
    public class SendTelegramAlertConsumer : ITransportConsumer<SendTelegramAlert>
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IMediator _mediator;

        public SendTelegramAlertConsumer(ITelegramBotClient botClient, IMediator mediator)
        {
            _botClient = botClient;
            _mediator = mediator;
        }

        /// <inheritdoc />
        public async Task ConsumeAsync(SendTelegramAlert value, CancellationToken cancellation = default)
        {
            var result = await _botClient.SendTextMessageSplitAsync(value.ChatId, value.Text, value.ParseMode,
                disableWebPagePreview: true, value.DisableNotification, replyToMessageId: 0,
                cancellationToken: cancellation);

            await _mediator.Publish(new BotSentAlertToChat(result), cancellation);
        }
    }
}
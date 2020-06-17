using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot;
using Zeus.Handlers.Bot.Notifications;
using Zeus.Transport;

namespace Zeus.Handlers.Bot.Consumers
{
    public class SendTelegramReplyConsumer : ITransportConsumer<SendTelegramReply>
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IMediator _mediator;

        public SendTelegramReplyConsumer(ITelegramBotClient botClient, IMediator mediator)
        {
            _botClient = botClient;
            _mediator = mediator;
        }

        /// <inheritdoc />
        public async Task ConsumeAsync(SendTelegramReply value, CancellationToken cancellation = default)
        {
            var message = await _botClient.SendTextMessageAsync(value.ChatId, value.Text, value.ParseMode,
                value.DisableWebPagePreview, value.DisableNotification, value.ReplyToMessageId, 
                cancellationToken: cancellation);

            await _mediator.Publish(new BotRepliedToChat(message), cancellation);
        }
    }
}
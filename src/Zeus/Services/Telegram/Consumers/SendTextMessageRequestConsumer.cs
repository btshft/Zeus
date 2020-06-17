using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Zeus.Shared.Extensions;
using Zeus.Shared.Transport;

namespace Zeus.Services.Telegram.Consumers
{
    public class SendTextMessageRequestConsumer : ITransportConsumer<SendTextMessageRequest>
    {
        private readonly ITelegramBotClient _botClient;

        public SendTextMessageRequestConsumer(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        /// <inheritdoc />
        public async Task ConsumeAsync(SendTextMessageRequest value, CancellationToken cancellation = default)
        {
            await _botClient.SendTextMessageSplitAsync(value.ChatId, value.Text, value.ParseMode,
                value.DisableWebPagePreview, value.DisableNotification, value.ReplyToMessageId, 
                cancellationToken: cancellation);
        }
    }
}
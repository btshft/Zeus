using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Zeus.Bot.Requests.Handlers
{
    public class CurrentChatInfoRequestHandler : AsyncRequestHandler<CurrentChatInfoRequest>
    {
        private readonly ITelegramBotClient _bot;

        public CurrentChatInfoRequestHandler(ITelegramBotClient bot)
        {
            _bot = bot;
        }

        /// <inheritdoc />
        protected override async Task Handle(CurrentChatInfoRequest request, CancellationToken cancellationToken)
        {
            var update = request.Update;
            var chat = await _bot.GetChatAsync(update.Message.Chat.Id, cancellationToken);
            var message = $"```json {JsonConvert.SerializeObject(chat, Formatting.Indented)}```";

            await _bot.SendTextMessageAsync(update.Message.Chat.Id, message, ParseMode.MarkdownV2,
                disableNotification: true, disableWebPagePreview: true, cancellationToken: cancellationToken);
        }
    }
}

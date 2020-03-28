using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Zeus.Bot.Requests.Handlers
{
    public class CurrentUserInfoRequestHandler : AsyncRequestHandler<CurrentUserInfoRequest>
    {
        private readonly ITelegramBotClient _bot;

        public CurrentUserInfoRequestHandler(ITelegramBotClient bot)
        {
            _bot = bot;
        }

        /// <inheritdoc />
        protected override async Task Handle(CurrentUserInfoRequest request, CancellationToken cancellationToken)
        {
            var update = request.Update;
            var user = await _bot.GetChatMemberAsync(update.Message.Chat.Id, update.Message.From.Id, cancellationToken);
            var message = $"```json {JsonConvert.SerializeObject(user, Formatting.Indented)}```";

            await _bot.SendTextMessageAsync(update.Message.Chat.Id, message, ParseMode.MarkdownV2,
                disableNotification: true, disableWebPagePreview: true, cancellationToken: cancellationToken);
        }
    }
}
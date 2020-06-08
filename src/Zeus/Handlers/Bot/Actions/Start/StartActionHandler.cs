using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Zeus.Handlers.Bot.Abstractions;
using Zeus.Handlers.Bot.Reply;
using Zeus.Localization;
using Zeus.Services.Telegram;

namespace Zeus.Handlers.Bot.Actions.Start
{
    [ReplyOnException]
    public class StartActionHandler : BotActionHandler<StartAction>
    {
        public StartActionHandler(
            ITelegramBotClient bot, 
            IMessageLocalizer<BotResources> localizer, 
            ILoggerFactory loggerFactory) : base(bot, localizer, loggerFactory)
        {
        }

        /// <inheritdoc />
        protected override async Task Handle(BotActionRequest<StartAction> request, CancellationToken cancellationToken)
        {
            var text = Localizer.GetString(BotResources.StartText);
            await Bot.SendTextMessageAsync(new ChatId(request.Chat.Id), text.EscapeMarkdown(), ParseMode.MarkdownV2,
                disableWebPagePreview: true, cancellationToken: cancellationToken);
        }
    }
}
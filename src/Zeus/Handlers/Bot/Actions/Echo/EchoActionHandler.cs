using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Zeus.Handlers.Bot.Abstractions;
using Zeus.Localization;
using Zeus.Shared.Extensions;

namespace Zeus.Handlers.Bot.Actions.Echo
{
    [AllowAnonymous]
    public class EchoActionHandler : BotActionHandler<EchoAction>
    {
        public EchoActionHandler(ITelegramBotClient bot, IMessageLocalizer<BotResources> localizer, ILoggerFactory loggerFactory) 
            : base(bot, localizer, loggerFactory)
        {
        }

        /// <inheritdoc />
        protected override async Task Handle(BotActionRequest<EchoAction> request, CancellationToken cancellationToken)
        {
            var updateJson = request.Update.ToJson();
            var message = $"```json{Environment.NewLine}{updateJson}{Environment.NewLine}```";

            await Bot.SendTextMessageAsync(new ChatId(request.Update.Message.Chat.Id), message, ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);
        }
    }
}
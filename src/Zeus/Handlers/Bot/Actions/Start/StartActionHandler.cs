using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Zeus.Handlers.Bot.Abstractions;
using Zeus.Handlers.Bot.Consumers;
using Zeus.Localization;
using Zeus.Services.Telegram;
using Zeus.Transport;

namespace Zeus.Handlers.Bot.Actions.Start
{
    public class StartActionHandler : BotActionHandler<StartAction>
    {
        /// <inheritdoc />
        public StartActionHandler(IMessageLocalizer<BotResources> localizer, ILoggerFactory loggerFactory, ITransport<SendTelegramReply> reply) 
            : base(localizer, loggerFactory, reply)
        {
        }

        /// <inheritdoc />
        protected override async Task Handle(BotActionRequest<StartAction> request, CancellationToken cancellationToken)
        {
            var text = Localizer.GetString(BotResources.StartText);
            var messageRequest = new SendTelegramReply(request.Chat.Id, text.EscapeMarkdown())
            {
                ParseMode = ParseMode.MarkdownV2,
                DisableWebPagePreview = true
            };

            await Reply.SendAsync(messageRequest, cancellationToken);
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Zeus.Bot.Requests.Handlers
{
    public class EchoHandler : AsyncRequestHandler<EchoRequest>
    {
        private readonly ITelegramBotClient _bot;

        public EchoHandler(ITelegramBotClient bot)
        {
            _bot = bot;
        }

        /// <inheritdoc />
        protected override async Task Handle(EchoRequest request, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(request.Update, Formatting.Indented);
            var message = $"```{Environment.NewLine}{json}{Environment.NewLine}```";

            await _bot.SendTextMessageAsync(request.Update.Message.Chat.Id, message, ParseMode.MarkdownV2,
                disableWebPagePreview: true, cancellationToken: cancellationToken);
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Zeus.Bot.Requests.Handlers
{
    public class UnrecognizedUpdateHandler : AsyncRequestHandler<ProcessUnrecognizedUpdate>
    {
        private readonly ITelegramBotClient _client;

        public UnrecognizedUpdateHandler(ITelegramBotClient client)
        {
            _client = client;
        }

        protected override async Task Handle(ProcessUnrecognizedUpdate request, CancellationToken cancellationToken)
        {
            if (request.Update.Type == UpdateType.Message)
            {
                await _client.SendTextMessageAsync(request.Update.Message.Chat.Id, "Message unrecognized",
                    cancellationToken: cancellationToken);
            }
        }
    }
}
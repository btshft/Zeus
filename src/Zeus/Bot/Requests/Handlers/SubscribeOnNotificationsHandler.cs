using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot;
using Zeus.Bot.State;

namespace Zeus.Bot.Requests.Handlers
{
    public class SubscribeOnNotificationsHandler : AsyncRequestHandler<SubscribeOnNotifications>
    {
        private readonly ITelegramBotClient _client;
        private readonly IBotStateStorage _stateStorage;

        public SubscribeOnNotificationsHandler(ITelegramBotClient client, IBotStateStorage stateStorage)
        {
            _client = client;
            _stateStorage = stateStorage;
        }

        protected override async Task Handle(SubscribeOnNotifications request, CancellationToken cancellationToken)
        {
            var state = await _stateStorage.GetStateAsync(cancellationToken);
            if (state.Conversations.Any(c => c.Id == request.Update.Message.Chat.Id))
            {
                await _client.SendTextMessageAsync(request.Update.Message.Chat.Id,
                    $"Already subscribed", cancellationToken: cancellationToken);
            }
            else
            {
                await _stateStorage.ModifyStateAsync(s =>
                {
                    s.Conversations.Add(request.Update.Message.Chat);
                }, cancellationToken);

                await _client.SendTextMessageAsync(request.Update.Message.Chat.Id,
                    $"Subscribed with id '{request.Update.Message.Chat.Id}'", cancellationToken: cancellationToken);
            }
        }
    }
}
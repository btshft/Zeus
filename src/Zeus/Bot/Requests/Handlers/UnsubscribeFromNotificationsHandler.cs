using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot;
using Zeus.Bot.State;

namespace Zeus.Bot.Requests.Handlers
{
    public class UnsubscribeFromNotificationsHandler : AsyncRequestHandler<UnsubscribeFromNotifications>
    {
        private readonly ITelegramBotClient _client;
        private readonly IBotStateStorage _storage;

        public UnsubscribeFromNotificationsHandler(ITelegramBotClient client, IBotStateStorage storage)
        {
            _client = client;
            _storage = storage;
        }

        protected override async Task Handle(UnsubscribeFromNotifications request, CancellationToken cancellationToken)
        {
            await _storage.ModifyStateAsync(s =>
            {
                var matchedChat = s.Conversations.FirstOrDefault(c => c.Id == request.Update.Message.Chat.Id);
                if (matchedChat != null)
                    s.Conversations.Remove(matchedChat);

            }, cancellationToken);

            await _client.SendTextMessageAsync(request.Update.Message.Chat.Id, "Unsubscribed", cancellationToken: cancellationToken);
        }
    }
}
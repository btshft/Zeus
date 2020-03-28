using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Zeus.Bot.Requests;

namespace Zeus.Bot.Notifications.Handlers
{
    public class BotUpdateRouter : INotificationHandler<BotUpdateReceived>
    {
        private readonly IMediator _mediator;
        private readonly ITelegramBotClient _bot;

        public BotUpdateRouter(IMediator mediator, ITelegramBotClient bot)
        {
            _mediator = mediator;
            _bot = bot;
        }

        /// <inheritdoc />
        public async Task Handle(BotUpdateReceived notification, CancellationToken cancellationToken)
        {
            IRequest request = notification.Update.Type switch
            {
                UpdateType.Message => MatchMessage(notification.Update),
                _ => new ProcessUnrecognizedUpdate(notification.Update)
            };

            await _mediator.Send(request, cancellationToken);
        }

        private static IRequest MatchMessage(Update update)
        {
            IRequest request = update.Message.Text switch
            {
                var s when string.Equals(s, "/subscribe", StringComparison.InvariantCultureIgnoreCase) => new SubscribeOnNotifications(update),
                var s when string.Equals(s, "/unsubscribe", StringComparison.InvariantCultureIgnoreCase) => new UnsubscribeFromNotifications(update),
                var s when string.Equals(s, "/chat", StringComparison.InvariantCultureIgnoreCase) => new CurrentChatInfoRequest(update),
                var s when string.Equals(s, "/me", StringComparison.InvariantCultureIgnoreCase) => new CurrentUserInfoRequest(update),

                _ => new ProcessUnrecognizedUpdate(update)
            };

            return request;
        }
    }
}
using MediatR;
using Telegram.Bot.Types.Enums;
using Zeus.Storage.Models.Alerts;
using Zeus.Storage.Models.External;

namespace Zeus.Handlers.Alerting.Send
{
    public class SendAlertRequest : IRequest
    {
        public AlertsSubscription Subscription { get; }

        public AlertManagerWebhookUpdate AlertManagerUpdate { get; }

        public string Text { get; }

        public ParseMode ParseMode { get; }

        public SendAlertRequest(AlertsSubscription subscription, string text, ParseMode parseMode, AlertManagerWebhookUpdate alertManagerUpdate)
        {
            Subscription = subscription;
            Text = text;
            ParseMode = parseMode;
            AlertManagerUpdate = alertManagerUpdate;
        }
    }
}
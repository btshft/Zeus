using MediatR;
using Zeus.Storage.Models.External;

namespace Zeus.Handlers.Alerting.Webhook
{
    public class AlertManagerUpdateRequest : IRequest
    {
        public AlertManagerUpdateRequest(string channel, AlertManagerWebhookUpdate update)
        {
            Update = update;
            Channel = channel;
        }

        public string Channel { get; }

        public AlertManagerWebhookUpdate Update { get; }
    }
}
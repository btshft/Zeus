using MediatR;
using Zeus.Storage.Models.External;

namespace Zeus.Handlers.Webhook
{
    public class AlertManagerUpdateRequest : IRequest
    {
        public AlertManagerUpdateRequest(string channel, AlertManagerUpdate update)
        {
            Update = update;
            Channel = channel;
        }

        public string Channel { get; }

        public AlertManagerUpdate Update { get; }
    }
}
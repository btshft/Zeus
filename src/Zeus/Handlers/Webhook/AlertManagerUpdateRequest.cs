using MediatR;
using Zeus.Storage.Models.External;

namespace Zeus.v2.Handlers.Webhook
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
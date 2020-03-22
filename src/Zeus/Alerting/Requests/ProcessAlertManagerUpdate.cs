using MediatR;
using Zeus.Alerting.Models;

namespace Zeus.Alerting.Requests
{
    public class ProcessAlertManagerUpdate : IRequest
    {
        public AlertManagerUpdate Update { get; }

        public ProcessAlertManagerUpdate(AlertManagerUpdate update)
        {
            Update = update;
        }
    }
}
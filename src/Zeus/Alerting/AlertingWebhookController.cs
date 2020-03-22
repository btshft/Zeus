using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zeus.Alerting.Models;
using Zeus.Alerting.Requests;

namespace Zeus.Alerting
{
    [ApiController]
    [Route("api/alerting/webhook")]
    public class AlertingWebhookController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AlertingWebhookController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Receives AlertManager update.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/alert
        ///     {
        ///       "receiver": "telegram",
        ///       "status": "resolved",
        ///       "alerts": [
        ///         {
        ///           "status": "resolved",
        ///           "labels": {
        ///             "alertname": "Fire",
        ///             "severity": "critical"
        ///           },
        ///           "annotations": {
        ///             "message": "Something is on fire"
        ///           },
        ///           "startsAt": "2018-11-04T22:43:58.283995108+01:00",
        ///           "endsAt": "2018-11-04T22:48:13.283995108+01:00"
        ///         }
        ///       ],
        ///       "groupLabels": {
        ///         "alertname": "Fire"
        ///       },
        ///       "commonLabels": {
        ///         "alertname": "Fire",
        ///         "severity": "critical"
        ///       },
        ///       "commonAnnotations": {
        ///         "message": "Something is on fire"
        ///       },
        ///       "externalURL": "http://localhost:9093",
        ///       "version": "4",
        ///     }
        /// 
        /// </remarks>
        /// <param name="update">AlertManager update message.</param>
        /// <param name="cancellation">Cancellation token.</param>
        [HttpPost]
        public async Task<IActionResult> ReceiveUpdate(AlertManagerUpdate update, CancellationToken cancellation = default)
        {
            await _mediator.Send(new ProcessAlertManagerUpdate(update), cancellation);
            return Ok();
        }
    }
}
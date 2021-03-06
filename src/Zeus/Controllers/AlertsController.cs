﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zeus.Handlers.Alerting.Webhook;
using Zeus.Storage.Models.External;

namespace Zeus.Controllers
{
    [ApiController, ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/webhook/alerts")]
    public class AlertsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AlertsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{channel}")]
        public async Task<IActionResult> Handle([FromRoute] string channel, [FromBody] AlertManagerWebhookUpdate update, CancellationToken cancellation = default)
        {
            var request = new AlertManagerUpdateRequest(channel, update);
            await _mediator.Send(request, cancellation);
            return Ok();
        }
    }
}
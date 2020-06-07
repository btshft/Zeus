using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Zeus.v2.Features.Api.Authorization;
using Zeus.v2.Handlers.Bot.Updates;
using Zeus.v2.Shared.Json;
using Zeus.v2.Shared.Security.TrustedNetwork;

namespace Zeus.v2.Controllers
{
    [ApiController, ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/callback")]
    public class CallbackController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CallbackController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("bot/update")]
        [AuthorizeTrustedNetwork(AuthPolicies.TrustedNetwork.Callback)]
        public async Task<IActionResult> HandleBotUpdate([FromNewtonsoftJsonBody] Update update, CancellationToken cancellation = default)
        {
            var request = new BotUpdateRequest(update);
            await _mediator.Send(request, cancellation);

            return Ok();
        }
    }
}
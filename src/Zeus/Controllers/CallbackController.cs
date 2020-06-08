using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Zeus.Features.Api.Authorization;
using Zeus.Handlers.Bot.Updates;
using Zeus.Shared.Json;
using Zeus.Shared.Security.TrustedNetwork;

namespace Zeus.Controllers
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
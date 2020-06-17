using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Zeus.Handlers.Bot.Updates;
using Zeus.Shared.Json;

namespace Zeus.Controllers
{
    [ApiController, ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/bot")]
    public class BotController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BotController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("update")]
        public async Task<IActionResult> HandleBotUpdate([FromNewtonsoftJsonBody] Update update, CancellationToken cancellation = default)
        {
            var request = new BotUpdateRequest(update);
            await _mediator.Send(request, cancellation);

            return Ok();
        }
    }
}
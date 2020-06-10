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
    [Route("api/v{version:apiVersion}/callback")]
    public class CallbackController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CallbackController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("bot/update")]
        public async Task<IActionResult> HandleBotUpdate([FromNewtonsoftJsonBody] Update update, CancellationToken cancellation = default)
        {
            var request = new BotUpdateRequest(update);
            await _mediator.Send(request, cancellation);

            return Ok();
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new
            {
                Text = "pong"
            });
        }
    }
}
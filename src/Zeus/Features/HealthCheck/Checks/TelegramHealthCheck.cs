using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Telegram.Bot;

namespace Zeus.Features.HealthCheck.Checks
{
    public class TelegramHealthCheck : IHealthCheck
    {
        private readonly ITelegramBotClient _client;

        public TelegramHealthCheck(ITelegramBotClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                var bot = await _client.GetMeAsync(cancellationToken);
                var data = new Dictionary<string, object>
                {
                    ["bot.username"] = $"@{bot.Username}",
                    ["bot.id"] = bot.Id,
                };

                return HealthCheckResult.Healthy(data: data);
            }
            catch (Exception e)
            {
                var text = e is TaskCanceledException || e is HttpRequestException
                    ? "Telegram connection could not be established"
                    : "Telegram connection could not be established or bot authorization data is invalid.";

                var data = new Dictionary<string, object>
                {
                    ["exception.type"] = e.GetType().Name,
                    ["exception.message"] = e.Message
                };

                return HealthCheckResult.Unhealthy(text, e, data);
            }
        }
    }
}

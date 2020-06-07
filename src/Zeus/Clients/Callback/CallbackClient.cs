using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Zeus.Clients.Exceptions;
using Zeus.Shared.Extensions;
using Zeus.Shared.Json;

namespace Zeus.Clients.Callback
{
    public class CallbackClient : ICallbackClient
    {
        private readonly HttpClient _client;

        public CallbackClient(HttpClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public async Task HandleBotUpdateAsync(Update update, CancellationToken cancellation = default)
        {
            var content = new NewtonsoftJsonContent(update);

            try
            {
                using var response = await _client.PostAsync(Urls.BotUpdate, content, cancellation);
                if (!response.IsSuccessStatusCode)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();
                    throw new ClientException(
                        $"Request to URL '{Urls.BotUpdate}' failed with status code '{(int) response.StatusCode}'. Message:{Environment.NewLine}{stringContent}");
                }
            }
            catch (ClientException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ClientException($"Request to URL '{Urls.BotUpdate}' failed with exception with update:{Environment.NewLine}{update.ToJson()}", e);
            }
        }

        private class Urls
        {
            public const string BotUpdate = "/api/v1/callback/bot/update";
        }
    }
}

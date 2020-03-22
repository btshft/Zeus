using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Zeus.Alerting.Models;
using Zeus.Alerting.Options;
using Zeus.Bot.State;

namespace Zeus.Alerting.Routing
{
    public class AlertRoutesResolver : IAlertRoutesResolver
    {
        private readonly IOptions<AlertingOptions> _optionProvider;
        private readonly IBotStateStorage _botStateStorage;

        public AlertRoutesResolver(IOptions<AlertingOptions> optionProvider, IBotStateStorage botStateStorage)
        {
            _optionProvider = optionProvider;
            _botStateStorage = botStateStorage;
        }


        /// <inheritdoc />
        public async Task<AlertRoutes> ResolveAsync(AlertManagerUpdate update, CancellationToken cancellation = default)
        {
            var options = _optionProvider.Value.Routing;
            if (update.Alerts == null)
                return AlertRoutes.Empty(update);

            var state = await _botStateStorage.GetStateAsync(cancellation);
            var chatsToRoutes = new Dictionary<long, (string routeName, List<AlertManagerAlert> alerts)>();

            foreach (var alert in update.Alerts)
            {
                var (chatId, routeName) = ResolveRoute(alert);
                if (chatId.HasValue)
                {
                    if (chatsToRoutes.TryGetValue(chatId.Value, out var route) && !route.alerts.Contains(alert))
                    {
                        route.alerts.Add(alert);
                    }
                    else
                    {
                        chatsToRoutes[chatId.Value] = (routeName, new List<AlertManagerAlert> { alert });
                    }
                }
            }

            var converted = chatsToRoutes.Select(cr =>
            {
                var (chatId, (routeName, alerts)) = cr;
                var conversation = state.Conversations.First(s => s.Id == chatId);
                return new AlertRoute(conversation, alerts, routeName);
            });

            return new AlertRoutes(update, converted.ToArray());

            (long? chatId, string routeName) ResolveRoute(AlertManagerAlert alert)
            {
                var isDefaultChatSubscribed = options.Default != null && state.Conversations.Any(c => c.Id == options.Default.ChatId);

                if (options.Routes == null)
                {
                    return isDefaultChatSubscribed
                        ? ((long?) options.Default.ChatId, options.Default.Name)
                        : ((long?) null, null);
                }

                var availableDefinitions = options.Routes.Where(r => state.Conversations.Any(s => s.Id == r.ChatId));
                var alertLabels = (IDictionary<string, object>)alert.Labels;

                var resolvedChatId = (long?) null;
                var resolvedRouteName = (string) null;

                foreach (var definition in availableDefinitions)
                {
                    if (definition.Match != null)
                    {
                        var definitionMatched = true;
                        foreach (var (label, value) in definition.Match)
                        {
                            if (alertLabels.TryGetValue(label, out var alertLabelValue) &&
                                alertLabelValue != null &&
                                string.Equals(alertLabelValue.ToString(), value))
                            {
                                continue;
                            }

                            definitionMatched = false;
                            break;
                        }

                        if (!definitionMatched)
                            continue;
                    }

                    if (definition.MatchRegex != null)
                    {
                        var definitionMatched = true;
                        foreach (var (label, regex) in definition.MatchRegex)
                        {
                            if (alertLabels.TryGetValue(label, out var alertLabelValue) &&
                                alertLabelValue != null &&
                                Regex.IsMatch(alertLabelValue.ToString(), regex, RegexOptions.Compiled))
                            {
                                continue;
                            }

                            definitionMatched = false;
                            break;
                        }

                        if (!definitionMatched)
                            continue;
                    }

                    resolvedChatId = definition.ChatId;
                    resolvedRouteName = definition.Name;
                    break;
                }

                if (!resolvedChatId.HasValue && isDefaultChatSubscribed)
                {
                    resolvedChatId = options.Default.ChatId;
                    resolvedRouteName = options.Default.Name;
                }

                return (resolvedChatId, resolvedRouteName);
            }
        }
    }
}
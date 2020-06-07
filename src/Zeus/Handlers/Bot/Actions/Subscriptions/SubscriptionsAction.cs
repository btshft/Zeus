using System;
using Zeus.v2.Handlers.Bot.Abstractions;

namespace Zeus.v2.Handlers.Bot.Actions.Subscriptions
{
    public class SubscriptionsAction : IBotAction
    {
        public class Format : IBotActionFormat<SubscriptionsAction>
        {
            /// <inheritdoc />
            public bool TryParse(string message, out SubscriptionsAction action)
            {
                action = null;

                var isMatched = message.Equals("/subscriptions", StringComparison.InvariantCultureIgnoreCase);
                if (isMatched)
                    action = new SubscriptionsAction();

                return isMatched;
            }
        }
    }
}
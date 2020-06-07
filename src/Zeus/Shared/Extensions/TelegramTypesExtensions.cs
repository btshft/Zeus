using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Zeus.Shared.Extensions
{
    public static class TelegramTypesExtensions
    {
        public static string ToJson(this Update update)
        {
            if (update == null)
                return "null";

            return JsonConvert.SerializeObject(update, Formatting.Indented);
        }
    }
}
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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

        public static bool IsCommand(this Update update)
        {
            return update.Type == UpdateType.Message &&
                   update.Message.Type == MessageType.Text &&
                   update.Message.Text.StartsWith('/');
        }
    }
}
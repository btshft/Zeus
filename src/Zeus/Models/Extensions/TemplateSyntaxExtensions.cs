using Telegram.Bot.Types.Enums;
using Zeus.Storage.Models.Alerts;

namespace Zeus.v2.Models.Extensions
{
    public static class TemplateSyntaxExtensions
    {
        public static ParseMode ToParseMode(this AlertsTemplate.TemplateSyntax syntax)
        {
            return syntax switch
            {
                AlertsTemplate.TemplateSyntax.Default => ParseMode.Default,
                AlertsTemplate.TemplateSyntax.Markdown => ParseMode.MarkdownV2,
                AlertsTemplate.TemplateSyntax.Html => ParseMode.Html,
                _ => ParseMode.Default
            };
        }
    }
}
using Telegram.Bot.Types.Enums;
using Zeus.Alerting.Options;

namespace Zeus.Alerting.Templating
{
    public static class TemplateRenderModeExtensions
    {
        public static ParseMode ToParseMode(this AlertingOptions.TemplateRenderMode mode) =>
            mode switch
            {
                AlertingOptions.TemplateRenderMode.Text => ParseMode.Default,
                AlertingOptions.TemplateRenderMode.Html => ParseMode.Html,
                AlertingOptions.TemplateRenderMode.Markdown => ParseMode.MarkdownV2,
                _ => ParseMode.Default
            };
    }
}
namespace Zeus.Alerting.Options
{
    public partial class AlertingOptions
    {
        public class TemplatingOptions
        {
            public DefaultTemplateOptions Default { get; set; }

            public OverrideTemplateOptions[] Overrides { get; set; }
        }

        public class DefaultTemplateOptions
        {
            public string TemplatePath { get; set; }

            public TemplateRenderMode RenderMode { get; set; }
        }

        public class OverrideTemplateOptions
        {
            public TemplateSelector Selector { get; set; }
            public string TemplatePath { get; set; }

            public TemplateRenderMode RenderMode { get; set; }
        }

        public class TemplateSelector
        {
            public long? ChatId { get; set; }
            public string RouteName { get; set; }
        }

        public enum TemplateRenderMode
        {
            Text = 0,
            Html = 1,
            Markdown = 2
        }
    }
}
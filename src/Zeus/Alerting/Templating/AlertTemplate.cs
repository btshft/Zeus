using Zeus.Alerting.Options;

namespace Zeus.Alerting.Templating
{
    public class AlertTemplate
    {
        public AlertingOptions.TemplateRenderMode RenderMode { get; }

        public string Path { get; }

        public AlertTemplate(AlertingOptions.TemplateRenderMode renderMode, string path)
        {
            RenderMode = renderMode;
            Path = path;
        }
    }
 }

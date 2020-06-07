namespace Zeus.Storage.Models.Alerts
{
    /// <summary>
    /// Alert template name.
    /// </summary>
    public class AlertsTemplate
    {
        /// <summary>
        /// Template content.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Template syntax
        /// </summary>
        public TemplateSyntax Syntax { get; set; }

        /// <summary>
        /// Template syntax.
        /// </summary>
        public enum TemplateSyntax
        {
            Default = 0,
            Markdown = 1,
            Html = 2
        }
    }
}
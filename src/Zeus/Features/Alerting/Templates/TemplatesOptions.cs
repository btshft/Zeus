using System.ComponentModel.DataAnnotations;

namespace Zeus.Features.Alerting.Templates
{
    public class TemplatesOptions
    {
        [Required]
        public TemplatesStoreOptions Store { get; set; }
    }
}
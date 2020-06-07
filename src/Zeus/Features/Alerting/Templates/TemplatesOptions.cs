using System.ComponentModel.DataAnnotations;

namespace Zeus.v2.Features.Alerting.Templates
{
    public class TemplatesOptions
    {
        [Required]
        public TemplatesStoreOptions Store { get; set; }
    }
}
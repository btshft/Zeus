using System.ComponentModel.DataAnnotations;
using Zeus.Shared.Validation;

namespace Zeus.Features.Alerting.Templates
{
    public class TemplatesOptions
    {
        [Required, ValidateObject]
        public TemplatesStoreOptions Store { get; set; }
    }
}
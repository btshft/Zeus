using System.ComponentModel.DataAnnotations;
using Zeus.Shared.Validation;

namespace Zeus.Features.Alerting.Templates
{
    public class TemplatesStoreOptions
    {
        [Required, ValidateObject]
        public TemplatesFileSystemStoreOptions FileSystem { get; set; }
    }
}
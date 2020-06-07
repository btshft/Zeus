using System.ComponentModel.DataAnnotations;

namespace Zeus.Features.Alerting.Templates
{
    public class TemplatesStoreOptions
    {
        [Required]
        public TemplatesFileSystemStoreOptions FileSystem { get; set; }
    }
}
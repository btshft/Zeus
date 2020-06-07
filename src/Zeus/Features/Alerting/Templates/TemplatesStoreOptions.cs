using System.ComponentModel.DataAnnotations;

namespace Zeus.v2.Features.Alerting.Templates
{
    public class TemplatesStoreOptions
    {
        [Required]
        public TemplatesFileSystemStoreOptions FileSystem { get; set; }
    }
}
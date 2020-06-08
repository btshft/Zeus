using System.ComponentModel.DataAnnotations;

namespace Zeus.Features.Profiling
{
    public class ProfilingFeatureOptions
    {
        [Required]
        public string Path { get; set; } = "/profiler";
    }
}

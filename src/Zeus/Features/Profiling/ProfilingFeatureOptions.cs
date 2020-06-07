using System.ComponentModel.DataAnnotations;

namespace Zeus.v2.Features.Profiling
{
    public class ProfilingFeatureOptions
    {
        [Required]
        public string Path { get; set; } = "/profiler";
    }
}

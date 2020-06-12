using System.ComponentModel.DataAnnotations;

namespace Zeus.Features.Metrics
{
    public class MetricsFeatureOptions
    {
        public bool Enabled { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Prefix { get; set; } = "zeus";
    }
}
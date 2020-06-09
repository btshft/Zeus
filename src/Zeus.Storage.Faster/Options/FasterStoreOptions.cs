using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Zeus.Storage.Faster.Options
{
    public class FasterStoreOptions
    {
        [Required]
        public string Directory { get; set; } = "./storage";

        [Required]
        public string Name { get; set; } = "zeus";

        public MemorySize MemorySize { get; set; } = MemorySize.Mb32;

        public MemorySize PageSize { get; set; } = MemorySize.Mb8;

        public MemorySize SegmentSize { get; set; } = MemorySize.Mb16;

        public JsonSerializerOptions JsonOptions { get; set; } = new JsonSerializerOptions
        {
            IgnoreNullValues = true
        };
    }
}
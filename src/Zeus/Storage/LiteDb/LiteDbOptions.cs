using System;
using System.ComponentModel.DataAnnotations;
using LiteDB;

namespace Zeus.Storage.LiteDb
{
    public class LiteDbOptions
    {
        [Required]
        public string ConnectionString { get; set; }

        public Action<BsonMapper> ConfigureMapper { get; set; }
    }
}
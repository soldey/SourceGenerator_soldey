using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeGenService.entity
{
    public class SwaggerEntity
    {
        [JsonPropertyName("swagger")]
        public object Swagger { get; set; }
        [JsonPropertyName("info")]
        public object Info { get; set; }
        [JsonPropertyName("host")]
        public object Host { get; set; }
        [JsonPropertyName("tags")]
        public object Tags { get; set; }
        [JsonPropertyName("paths")]
        public Dictionary<string, Dictionary<string, Path>> Paths { get; set; }
        [JsonPropertyName("definitions")]
        public Dictionary<string, Definition> Definitions { get; set; }
    }
}

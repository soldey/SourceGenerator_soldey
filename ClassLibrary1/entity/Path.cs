using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeGenService.entity
{
    public class Path
    {
        [JsonPropertyName("parameters")]
        public List<Parameter> Parameters { get; set; }
        [JsonPropertyName("responses")]
        public Dictionary<string, Response> Responses { get; set; }
        [JsonPropertyName("summary")]
        public string Summary { get; set; }
    }
}

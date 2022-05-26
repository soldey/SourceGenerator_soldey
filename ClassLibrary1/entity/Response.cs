using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeGenService.entity
{
    public class Response
    {
        [JsonPropertyName("schema")]
        public Schema Schema { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}

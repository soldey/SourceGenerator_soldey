using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeGenService.entity
{
    public class Schema
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("format")]
        public string Format { get; set; }
        [JsonPropertyName("$ref")]
        public string Ref { get; set; }

        public string GetType()
        {
            if (Type != null)
            {
                return Type;
            }

            return Ref.Split('/').Last();
        }
    }
}

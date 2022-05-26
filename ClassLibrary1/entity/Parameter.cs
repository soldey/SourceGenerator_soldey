using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeGenService.entity
{
    public class Parameter
    {
        [JsonPropertyName("schema")]
        public Schema Schema { get; set; }
        [JsonPropertyName("in")]
        public string In { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }

        public string FormatType()
        {
            switch (Type)
            {
                case "integer":
                    return "int";
                case "boolean":
                    return "bool";
                case "number":
                    return "double";
                default:
                    return Type;
            }
        }

        public string GetType()
        {
            if (Type != null)
            {
                return FormatType();
            }
            return Schema.GetType();
        }
    }
}

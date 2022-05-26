using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeGenService.entity
{
    public class Property
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

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
    }
}

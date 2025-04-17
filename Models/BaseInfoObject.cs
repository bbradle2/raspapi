using System.Text.Json.Serialization;

namespace raspapi.Models
{
    public abstract class BaseInfoObject
    {
        //[JsonPropertyName("ProductName")]
        public required string ProductName { get; set; }
        public required string Description { get; set; }
    }

}

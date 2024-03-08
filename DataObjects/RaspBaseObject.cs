using System.Text.Json.Serialization;

namespace first_test.DataObjects
{
    public abstract class RaspBaseObject
    {
        [JsonPropertyName("Call")]
        [JsonPropertyOrder(0)]
        public string? Call { get; set; }

        [JsonPropertyName("Description")]
        [JsonPropertyOrder(1)]
        public string? Description { get; set; }

      
    }

}

using System.Text.Json.Serialization;

namespace first_test.DataObjects
{
    public abstract class RaspBaseObject
    {
        [JsonPropertyName("Call")]
        public required string Call { get; set; }

        [JsonPropertyName("Content")]
        public required string Content { get; set; }
    }

}

using System.Text.Json.Serialization;

namespace first_test.DataObjects
{
    public class MemoryObject : RaspBaseObject
    {
        [JsonPropertyName("MemTotal")]
        public string? MemTotal { get; set; }
        [JsonPropertyName("MemFree")] 
        public string? MemFree { get; set; }
        [JsonPropertyName("MemAvailable")] 
        public string? MemAvailable { get; set; }
    }
}
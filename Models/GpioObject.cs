using System.Text.Json.Serialization;

namespace raspapi.Models
{
    public class GpioObject
    {
        [JsonPropertyName("GpioNumber")]
        public int GpioNumber { get; set; }
        [JsonPropertyName("GpioValue")]
        public bool? GpioValue { get; set; }
    }
}
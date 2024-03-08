using System.Text.Json.Serialization;

namespace first_test.DataObjects
{
    public class CPUInfoObject : RaspBaseObject
    {
        [JsonPropertyOrder(2)]
        [JsonPropertyName("Revision")]
        public string? Revision { get; set; }

        [JsonPropertyName("Serial")]
        [JsonPropertyOrder(3)]
        public string? Serial { get; set; }
        [JsonPropertyOrder(4)]
        [JsonPropertyName("Model")]
        public string? Model { get; set; }
        [JsonPropertyOrder(5)]
        [JsonPropertyName("ProcessorInfoObjects")]
        public ProcessorObject[]? ProcessorInfoObjects { get; set; }
    }

    public class ProcessorObject
    {
        [JsonPropertyName("Processor")]
        public string? Processor { get; set; }

        [JsonPropertyName("BogoMIPS")]
        public string? BogoMIPS { get; set; }

        [JsonPropertyName("Features")]
        public string? Features { get; set; }

        [JsonPropertyName("CPUImplementer")]
        public string? CPUImplementer { get; set; }

        [JsonPropertyName("CPUArchitecture")]
        public string? CPUArchitecture { get; set; }
        [JsonPropertyName("CPUVariant")]
        public string? CPUVariant { get; set; }
        [JsonPropertyName("CPUPart")]
        public string? CPUPart { get; set; }
        [JsonPropertyName("CPURevision")]
        public string? CPURevision { get; set; }
    }
}
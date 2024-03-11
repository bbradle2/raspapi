using System.Text.Json.Serialization;

namespace first_test.DataObjects
{
    public class MemoryInfoObject : BaseInfoObject
    {
        public MemoryInfoObject()
        {
            Name = "memoryinfo";
            Description = "Raspberry PI 5 memoryinfo";
        }

        public string? MemoryTotal { get; set; }
        public string? MemoryFree { get; set; }
        public string? MemoryAvailable { get; set; }
    }
}
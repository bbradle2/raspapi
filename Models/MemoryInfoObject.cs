namespace raspapi.Models
{
    public class MemoryInfoObject : BaseInfoObject
    {

        public int? MemoryTotal { get; set; }
        public int? MemoryFree { get; set; }
        public int? MemoryAvailable { get; set; }

        public int? Cached { get; set; }
        public int? SwapCached { get; set; }
        public int? SwapFree { get; set; }

    }
}
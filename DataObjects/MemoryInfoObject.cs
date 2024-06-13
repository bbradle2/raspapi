namespace raspapi.DataObjects
{
    public class MemoryInfoObject : BaseInfoObject
    {

        public string? MemoryTotal { get; set; }
        public string? MemoryFree { get; set; }
        public string? MemoryAvailable { get; set; }

        public string? Cached { get; set; }
        public string? SwapCached { get; set; }
        public string? SwapFree { get; set; }

    }


}
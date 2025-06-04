namespace raspapi.Models
{
    public class MemoryInfoObject : BaseInfoObject
    {

        public decimal? MemoryTotal { get; set; }
        public decimal? MemoryFree { get; set; }
        public decimal? MemoryAvailable { get; set; }

        public decimal? Cached { get; set; }
        public decimal? SwapCached { get; set; }
        public decimal? SwapFree { get; set; }

    }
}
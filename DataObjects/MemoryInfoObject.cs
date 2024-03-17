using System.Text.Json.Serialization;

namespace raspapi.DataObjects
{
    public class MemoryInfoObject : BaseInfoObject
    {
        public MemoryInfoObject()
        {
            Name = "memoryinfo";
            Description = "Raspberry PI 5 memoryinfo.";
        }

        public string? MemoryTotal { get; set; }
        public string? MemoryFree { get; set; }
        public string? MemoryAvailable { get; set; }

        public string? Cached { get; set; }
        public string? SwapCached { get; set; }
        public string? SwapFree { get; set; }

        public static MemoryInfoObject ParseMemoryInfo(string[] lines, string delimeter = ":")
        {
            var memTotalValue = lines.SingleOrDefault(item => item.Contains($"memtotal{delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{ delimeter}")[1].Trim();
            var memFreeValue = lines.SingleOrDefault(item => item.Contains($"memfree{delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[1].Trim();
            var memAvailableValue = lines.SingleOrDefault(item => item.Contains($"memavailable{delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[1].Trim();

            var cachedValue = lines.SingleOrDefault(item => item.Contains($"cached{delimeter}", StringComparison.CurrentCultureIgnoreCase)
                                                              &&
                                                              !item.Contains($"swapcached{delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[1].Trim();
            var swapCachedValue = lines.SingleOrDefault(item => item.Contains($"swapcached{delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[1].Trim();

            var swapFreeValue = lines.SingleOrDefault(item => item.Contains($"swapfree{delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[1].Trim();

            var activeAnonValue = lines.SingleOrDefault(item => item.Contains($"active(anon){delimeter}", StringComparison.CurrentCultureIgnoreCase)
                                                                  &&
                                                                  !item.Contains($"inactive(anon){delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[1].Trim();
            var inActiveAnonValue = lines.SingleOrDefault(item => item.Contains($"inactive(anon){delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[1].Trim();

            MemoryInfoObject memInfoObject = new()
            {
                MemoryTotal = memTotalValue,
                MemoryFree = memFreeValue,
                MemoryAvailable = memAvailableValue,
                Cached = cachedValue,
                SwapCached = swapCachedValue,
                SwapFree = swapFreeValue,
            };

            return memInfoObject;
        }

    }

    
}
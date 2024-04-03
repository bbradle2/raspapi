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

        public static async Task<MemoryInfoObject> ParseMemoryInfoAsync(string delimeter = ":")
        {
            var meminfoLines = await File.ReadAllLinesAsync("/proc/meminfo");
            if (meminfoLines.Length == 0)
            {
                throw new Exception("Meminfo has no entries");
            }


            var memTotalValue = meminfoLines.SingleOrDefault(item => item.Contains($"memtotal{delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[1].Trim();

            var memFreeValue = meminfoLines.SingleOrDefault(item => item.Contains($"memfree{delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[1].Trim();

            var memAvailableValue = meminfoLines.SingleOrDefault(item => item.Contains($"memavailable{delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[1].Trim();

            var cachedValue = meminfoLines.SingleOrDefault(item => item.Contains($"cached{delimeter}", StringComparison.CurrentCultureIgnoreCase)
                                                              &&
                                                              !item.Contains($"swapcached{delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[1].Trim();

            var swapCachedValue = meminfoLines.SingleOrDefault(item => item.Contains($"swapcached{delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[1].Trim();

            var swapFreeValue = meminfoLines.SingleOrDefault(item => item.Contains($"swapfree{delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[1].Trim();

            var activeAnonValue = meminfoLines.SingleOrDefault(item => item.Contains($"active(anon){delimeter}", StringComparison.CurrentCultureIgnoreCase)
                                                                  &&
                                                                  !item.Contains($"inactive(anon){delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[1].Trim();

            var inActiveAnonValue = meminfoLines.SingleOrDefault(item => item.Contains($"inactive(anon){delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[1].Trim();

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
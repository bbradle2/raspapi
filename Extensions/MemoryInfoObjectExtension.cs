using raspapi.DataObjects;

namespace raspapi.MemoryInfoObjectExtension
{
    public static class MemoryInfoObjectExtension
    {
        public static async Task<MemoryInfoObject> PopulateMemoryInfoAsync(this MemoryInfoObject memObj, string delimeter = ":")
        {
            var meminfoLines = await File.ReadAllLinesAsync("/proc/meminfo");
            if (meminfoLines.Length == 0)
            {
                throw new Exception("Meminfo has no entries");
            }

            int valuePosition = 1;
            var memTotalValue = meminfoLines.SingleOrDefault(item => item.Contains($"memtotal{delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[valuePosition].Trim();

            var memFreeValue = meminfoLines.SingleOrDefault(item => item.Contains($"memfree{delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[valuePosition].Trim();

            var memAvailableValue = meminfoLines.SingleOrDefault(item => item.Contains($"memavailable{delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[valuePosition].Trim();

            var cachedValue = meminfoLines.SingleOrDefault(item => item.Contains($"cached{delimeter}", StringComparison.CurrentCultureIgnoreCase)
                                                              &&
                                                              !item.Contains($"swapcached{delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[valuePosition].Trim();

            var swapCachedValue = meminfoLines.SingleOrDefault(item => item.Contains($"swapcached{delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[valuePosition].Trim();

            var swapFreeValue = meminfoLines.SingleOrDefault(item => item.Contains($"swapfree{delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[valuePosition].Trim();

            var activeAnonValue = meminfoLines.SingleOrDefault(item => item.Contains($"active(anon){delimeter}", StringComparison.CurrentCultureIgnoreCase)
                                                                  &&
                                                                  !item.Contains($"inactive(anon){delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[valuePosition].Trim();

            var inActiveAnonValue = meminfoLines.SingleOrDefault(item => item.Contains($"inactive(anon){delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{delimeter}")[valuePosition].Trim();

            MemoryInfoObject memInfoObject = new()
            {
                Name = memObj.Name,
                Description = memObj.Description,
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
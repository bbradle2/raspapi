namespace raspapi.Utils
{
    using System.Text;
    using System.Text.Json;
    using raspapi.DataObjects;
    using raspapi.Extensions;

    public static class DataUtils
    {
        private readonly static JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);
        public static async Task<MemoryInfoObject> PopulateMemoryInfoAsync(string ProductName, 
                                                                           string Description,
                                                                           string Delimeter = ":"
                                                                          )
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(ProductName);
            ArgumentException.ThrowIfNullOrWhiteSpace(Description);
            ArgumentException.ThrowIfNullOrWhiteSpace(Delimeter);

            var meminfoLines = await File.ReadAllLinesAsync("/proc/meminfo");
            if (meminfoLines.Length == 0)
            {
                throw new Exception("Meminfo has no entries");
            }

            int valuePosition = 1;
            var memTotalValue = meminfoLines.SingleOrDefault(item => item.Contains($"memtotal{Delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{Delimeter}")[valuePosition].Trim();

            var memFreeValue = meminfoLines.SingleOrDefault(item => item.Contains($"memfree{Delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{Delimeter}")[valuePosition].Trim();

            var memAvailableValue = meminfoLines.SingleOrDefault(item => item.Contains($"memavailable{Delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{Delimeter}")[valuePosition].Trim();

            var cachedValue = meminfoLines.SingleOrDefault(item => item.Contains($"cached{Delimeter}", StringComparison.CurrentCultureIgnoreCase)
                                                              &&
                                                              !item.Contains($"swapcached{Delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{Delimeter}")[valuePosition].Trim();

            var swapCachedValue = meminfoLines.SingleOrDefault(item => item.Contains($"swapcached{Delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{Delimeter}")[valuePosition].Trim();

            var swapFreeValue = meminfoLines.SingleOrDefault(item => item.Contains($"swapfree{Delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{Delimeter}")[valuePosition].Trim();

            var activeAnonValue = meminfoLines.SingleOrDefault(item => item.Contains($"active(anon){Delimeter}", StringComparison.CurrentCultureIgnoreCase)
                                                                  &&
                                                                  !item.Contains($"inactive(anon){Delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{Delimeter}")[valuePosition].Trim();

            var inActiveAnonValue = meminfoLines.SingleOrDefault(item => item.Contains($"inactive(anon){Delimeter}", StringComparison.CurrentCultureIgnoreCase))!.Split($"{Delimeter}")[valuePosition].Trim();

            MemoryInfoObject memInfoObject = new()
            {
                ProductName = ProductName,
                Description = Description,
                MemoryTotal = memTotalValue,
                MemoryFree = memFreeValue,
                MemoryAvailable = memAvailableValue,
                Cached = cachedValue,
                SwapCached = swapCachedValue,
                SwapFree = swapFreeValue,
            };

            return memInfoObject;
        }


        public static async Task<SystemInfoObject> PopulateSystemInfoAsync(string ProductName,
                                                                           string Description = "No Description"
                                                                          )
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(ProductName);
            ArgumentException.ThrowIfNullOrWhiteSpace(Description);
             
            static async Task<MemoryStream> memoryStreamSystemInfo()
            {
                string systemInfoResult = await "sudo lshw -class system -json".ExecuteBashScriptAsync();
                ArgumentNullException.ThrowIfNullOrWhiteSpace(systemInfoResult);
                byte[] systemInfoByteArray = Encoding.UTF8.GetBytes(systemInfoResult);
                return new MemoryStream(systemInfoByteArray);
            }

            SystemInfoObject systemInfoObject = new()
            {
                ProductName = ProductName,
                Description = Description,
                SystemObjects = await JsonSerializer.DeserializeAsync<SystemInfoObject.SystemObject[]>(await memoryStreamSystemInfo(), _options)
            };

            return systemInfoObject;
        }

        public static async Task<CPUInfoObject> PopulateCpuInfoAsync(string ProductName,
                                                                     string Description
                                                                    )
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(ProductName);
            ArgumentException.ThrowIfNullOrWhiteSpace(Description);

            static async Task<MemoryStream> memoryStreamcCPUInfo()
            {
                string cpuInfoResult = await "sudo lshw -class cpu -json".ExecuteBashScriptAsync();
                ArgumentNullException.ThrowIfNullOrWhiteSpace(cpuInfoResult);
                byte[] cpuInfoByteArray = Encoding.UTF8.GetBytes(cpuInfoResult);
                return new MemoryStream(cpuInfoByteArray);
            }

            CPUInfoObject cpuInfoObject = new()
            {
                ProductName = ProductName,
                Description = Description,
                CPUObjects = await JsonSerializer.DeserializeAsync<CPUInfoObject.CPUObject[]>(await memoryStreamcCPUInfo(), _options)
            };

            return cpuInfoObject;
        }
    }
}
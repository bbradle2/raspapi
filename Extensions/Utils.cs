using System.Text;
using System.Text.Json;
using raspapi.DataObjects;
using raspapi.LinuxExtensions;

namespace raspapi.Utils
{
    public static class Utils
    {
        private readonly static JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);
        public static async Task<MemoryInfoObject> PopulateMemoryInfoAsync(string Name, 
                                                                           string Description,
                                                                           string delimeter = ":"
                                                                          )
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(Name);
            ArgumentException.ThrowIfNullOrWhiteSpace(Description);
            ArgumentException.ThrowIfNullOrWhiteSpace(delimeter);

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
                Name = Name,
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


        public static async Task<SystemInfoObject> PopulateSystemInfoAsync(string Name,
                                                                           string Description
                                                                          )
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(Name);
            ArgumentException.ThrowIfNullOrWhiteSpace(Description);
             
            static MemoryStream memoryStreamSystemInfo()
            {
                string systemInfoResult = "sudo lshw -class system -json".ExecuteBashScript();
                ArgumentNullException.ThrowIfNullOrWhiteSpace(systemInfoResult);
                byte[] systemInfoByteArray = Encoding.UTF8.GetBytes(systemInfoResult);
                return new MemoryStream(systemInfoByteArray);
            }

            SystemInfoObject systemInfoObject = new()
            {
                Name = Name,
                Description = Description,
                SystemObjects = await JsonSerializer.DeserializeAsync<SystemInfoObject.SystemObject[]>(memoryStreamSystemInfo(), _options)
            };

            return systemInfoObject;
        }

        public static async Task<CPUInfoObject> PopulateCpuInfoAsync(string Name,
                                                                     string Description
                                                                    )
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(Name);
            ArgumentException.ThrowIfNullOrWhiteSpace(Description);

            static MemoryStream memoryStreamcCPUInfo()
            {
                string cpuInfoResult = "sudo lshw -class cpu -json".ExecuteBashScript();
                ArgumentNullException.ThrowIfNullOrWhiteSpace(cpuInfoResult);
                byte[] cpuInfoByteArray = Encoding.UTF8.GetBytes(cpuInfoResult);
                return new MemoryStream(cpuInfoByteArray);
            }

            CPUInfoObject cpuInfoObject = new()
            {
                Name = Name,
                Description = Description,
                CPUObjects = await JsonSerializer.DeserializeAsync<CPUInfoObject.CPUObject[]>(memoryStreamcCPUInfo(), _options)
            };

            return cpuInfoObject;
        }
    }
}
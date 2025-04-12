namespace raspapi.Utils
{
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using Microsoft.AspNetCore.Http;
    using raspapi.Models;
    using raspapi.StringExtensions;

    public static class DataUtils
    {
        private readonly static JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);
        private static readonly SemaphoreSlim _semMemInfo = new(1, 1);
        private static readonly SemaphoreSlim _semSysInfo = new(1, 1);
        private static readonly SemaphoreSlim _semTemperatureInfo = new(1, 1);
        private static readonly SemaphoreSlim _semCpuInfo = new(1, 1);

        public static List<GpioObject> JsonArrayToGpioObjectArray(JsonArray gpioObjects)
        {
            List<GpioObject> gpioList = [];

            foreach (var gpioObject in gpioObjects)
            {
                var gpioNumber = gpioObject![0]!.GetValue<int>();
                var gpioValue = gpioObject![1]!.GetValue<bool>();
                gpioList.Add(new GpioObject { GpioNumber = gpioNumber, GpioValue = gpioValue });

            }

            return gpioList;
        }
        public static int[] JsonArrayToIntArray(JsonArray pinNumbers)
        {
            List<int> gpios = [];

            foreach (var gpio in pinNumbers)
            {
                var pinValue = gpio!.GetValue<int>();
                gpios.Add(pinValue);
            }
            return [.. gpios.DistinctBy(s => s)];
        }
        
        public static async Task<MemoryInfoObject> PopulateMemoryInfoAsync(string ProductName, string Description, string Delimeter = ":")
        {
            try
            {
                await _semMemInfo.WaitAsync();

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
            catch
            {
                throw;
            }
            finally
            {
                _semMemInfo.Release();
            }
        }
        
        public static async Task<SystemInfoObject> PopulateSystemInfoAsync(string ProductName,string Description)
        {
            try
            {
                await _semSysInfo.WaitAsync();

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
            catch 
            {
                throw;
            } 
            finally 
            {
                _semSysInfo.Release();
            }
        }

        public static async Task<TemperatureInfoObject> PopulateGetTemperatureInfoAsync(string ProductName,string Description,string Delimeter = "=")
        {
            try
            {
                await _semTemperatureInfo.WaitAsync();

                ArgumentException.ThrowIfNullOrWhiteSpace(ProductName);
                ArgumentException.ThrowIfNullOrWhiteSpace(Description);

                string temperatureInfoResult = await "vcgencmd measure_temp".ExecuteBashScriptAsync();

                int valuePosition = 1;
                var temperatureValueCelcius = double.Parse(temperatureInfoResult.Split($"{Delimeter}")[valuePosition].Trim().Split("'")[0]);
                TemperatureInfoObject temperatureInfoObject = new()
                {
                    ProductName = ProductName,
                    Description = Description,
                    TemperatureFahrenheit = (temperatureValueCelcius * 1.8) + 32,
                    TemperatureCelcius = temperatureValueCelcius
                };

                return temperatureInfoObject;
            } 
            catch 
            {
                throw;
            }
            finally 
            {
                _semTemperatureInfo.Release();
            }
        }

        public static async Task<CPUInfoObject> PopulateCpuInfoAsync(string ProductName,string Description)
        {
            try
            {
                await _semCpuInfo.WaitAsync();

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
            catch 
            {
                throw;
            }
            finally 
            {
                _semCpuInfo.Release();
            }
        }
    }
}
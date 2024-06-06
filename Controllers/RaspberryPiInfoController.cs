namespace raspapi.Controllers
{
    using System.Text;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System.Text.Json;
    using Microsoft.AspNetCore.Routing;
    using DataObjects;
    using LinuxExtensions;
    using raspapi.MemoryInfoObjectExtension;

    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiInfoController : ControllerBase
    {
        private readonly ILogger<RaspberryPiInfoController> _logger;
        private readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);

        public RaspberryPiInfoController(ILogger<RaspberryPiInfoController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetCpuInfo")]
        public async Task<IActionResult?> GetCpuInfo()
        {
            try
            {
                static MemoryStream memoryStreamCPUInfo()
                {
                    string cpuInfoResult = "sudo lshw -class cpu -json".ExecuteBashScript();
                    ArgumentNullException.ThrowIfNullOrWhiteSpace(cpuInfoResult);

                    byte[] cpuInfoByteArray = Encoding.UTF8.GetBytes(cpuInfoResult);
                    return new MemoryStream(cpuInfoByteArray);
                };

                CPUInfoObject cpuInfoObject = new()
                {
                    Name = "cpuinfo",
                    Description = "Raspberry PI 5 cpuinfo.",
                    CPUObjects = await JsonSerializer.DeserializeAsync<CPUInfoObject.CPUObject[]>(memoryStreamCPUInfo(), _options)
                };

                _logger.LogInformation("Returning cpuInfoObject");
                return Ok(cpuInfoObject);
            }
            catch (Exception e)
            {
                _logger.LogError("{Message}", e.Message);
                return BadRequest(new BadRequestResult());
            }
        }

        [HttpGet("GetSystemInfo")]
        public async Task<IActionResult?> GetSystemInfo()
        {
            try
            {
                static MemoryStream memoryStreamSystemInfo()
                {
                    string systemInfoResult = "sudo lshw -class system -json".ExecuteBashScript();
                    ArgumentNullException.ThrowIfNullOrWhiteSpace(systemInfoResult);
                    byte[] systemInfoByteArray = Encoding.UTF8.GetBytes(systemInfoResult);
                    return new(systemInfoByteArray);
                }

                SystemInfoObject systemInfoObject = new()
                {
                    Name = "systeminfo",
                    Description = "Raspberry PI 5 systeminfo.",
                    SystemObjects = await JsonSerializer.DeserializeAsync<SystemInfoObject.SystemObject[]>(memoryStreamSystemInfo(), _options)
                };

                _logger.LogInformation("Returning systemInfoObject");
                return Ok(systemInfoObject);

            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(new BadRequestResult());
            }
        }

        [HttpGet("GetMemoryInfo")]
        public async Task<IActionResult?> GetMemoryInfo()
        {
            try
            {
                var memObject = new MemoryInfoObject
                {
                    Name = "memoryinfo",
                    Description = "Raspberry PI 5 memoryinfo.",
                };

                var memInfo = await memObject.PopulateMemoryInfoAsync();
                _logger.LogInformation("Returning memInfoObject");
                return Ok(memInfo);
            }
            catch (Exception e)
            {
                _logger.LogError("{Message}", e.Message);
                return BadRequest(new BadRequestResult());
            }
        }
    }
}
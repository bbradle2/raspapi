using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace raspapi.Controllers
{
    using System.Text.Json;
    using DataObjects;
    using LinuxExtensions;
    using Microsoft.AspNetCore.Routing;

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
                    CPUObjects = await JsonSerializer.DeserializeAsync<CPUInfoObject.CPUObject[]>(memoryStreamCPUInfo(), _options)
                };

                _logger.LogInformation("Returning cpuInfoObject");
                return Ok(cpuInfoObject);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest();
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
                    SystemObjects = await JsonSerializer.DeserializeAsync<SystemInfoObject.SystemObject[]>(memoryStreamSystemInfo(), _options)
                };

                _logger.LogInformation("Returning systemInfoObject");
                return Ok(systemInfoObject);

            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return BadRequest();
            }
        }

        [HttpGet("GetMemoryInfo")]
        public async Task<IActionResult?> GetMemoryInfo()
        {
            try
            {
                var meminfoLines = await System.IO.File.ReadAllLinesAsync("/proc/meminfo");
                if(meminfoLines.Length == 0) 
                {
                    throw new Exception("Meminfo has no entries");
                }
                
                var memInfoObject = MemoryInfoObject.ParseMemoryInfo(meminfoLines);
                _logger.LogInformation("Returning memInfoObject");
                return Ok(memInfoObject);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest();
            }
        }
    }   
}
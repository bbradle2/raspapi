using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace raspapi.Controllers
{
    using System.Data;
    using System.Text.Json;
    using DataObjects;
    using Interfaces;
    using LinuxExtensions;
    using Microsoft.AspNetCore.Components.Routing;
    using Microsoft.AspNetCore.Routing;

    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiInfoController : ControllerBase
    {
        private readonly ILogger<RaspberryPiInfoController> _logger;
        private readonly JsonSerializerOptions options = new(JsonSerializerDefaults.Web);

        public RaspberryPiInfoController(ILogger<RaspberryPiInfoController> logger) 
        {
            _logger = logger;
        }

        [HttpGet("GetCpuInfo")]
        public async Task<CPUInfoObject?> GetCpuInfo()
        {
            try
            {
                static MemoryStream memoryStreamCPUInfo()
                {
                    string cpuInfoResult = "sudo lshw -class cpu -json".ExecuteBashScript();
                    byte[] cpuInfoByteArray = Encoding.UTF8.GetBytes(cpuInfoResult);
                    return new MemoryStream(cpuInfoByteArray);
                };

                CPUInfoObject cpuInfoObject = new()
                {
                    CPUObjects = await JsonSerializer.DeserializeAsync<CPUInfoObject.CPUObject[]>(memoryStreamCPUInfo(), options)
                };
                _logger.LogInformation("Returning cpuInfoObject");
                return cpuInfoObject;
            } 
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        [HttpGet("GetSystemInfo")]
        public async Task<SystemInfoObject?> GetSystemInfo()
        {
            try
            {
                static MemoryStream memoryStreamSystemInfo()
                {
                    string systemInfoResult = "sudo lshw -class system -json".ExecuteBashScript();
                    byte[] systemInfoByteArray = Encoding.UTF8.GetBytes(systemInfoResult);
                    return new(systemInfoByteArray);
                }

                SystemInfoObject systemInfoObject = new()
                {
                    SystemObjects = await JsonSerializer.DeserializeAsync<SystemInfoObject.SystemObject[]>(memoryStreamSystemInfo(), options)
                };
                _logger.LogInformation("Returning systemInfoObject");
                return systemInfoObject;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        [HttpGet("GetMemoryInfo")]
        public async Task<MemoryInfoObject?> GetMemoryInfo()
        {
           
            try
            {
                var meminfoLines = await System.IO.File.ReadAllLinesAsync("/proc/meminfo");
                var memInfoObject = MemoryInfoObject.ParseMemoryInfo(meminfoLines);
                _logger.LogInformation("Returning memInfoObject");
                return memInfoObject;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }
    }   
}
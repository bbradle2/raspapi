namespace raspapi.Controllers
{

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Routing;
    using raspapi.Utils;

    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiInfoController : ControllerBase
    {
        private readonly ILogger<RaspberryPiInfoController> _logger;
       
        public RaspberryPiInfoController(ILogger<RaspberryPiInfoController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetCpuInfo")]
        public async Task<IActionResult?> GetCpuInfo()
        {
            try
            {
                var cpuInfoObject = await Utils.PopulateCpuInfoAsync("cpuinfo", "Raspberry PI 5 cpuinfo.");
                //_logger.LogInformation("Returning cpuInfoObject");
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
                var systemInfoObject = await Utils.PopulateSystemInfoAsync("systeminfo", "Raspberry PI 5 systeminfo.");
                //_logger.LogInformation("Returning systemInfoObject");
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
                var memInfo = await Utils.PopulateMemoryInfoAsync("memoryinfo", "Raspberry PI 5 memoryinfo.");
                //_logger.LogInformation("Returning memInfoObject");
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
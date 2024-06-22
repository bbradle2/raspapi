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
        private readonly string ?_product;
        public RaspberryPiInfoController(ILogger<RaspberryPiInfoController> logger)
        {
            _logger = logger;
            var systemInfo = DataUtils.PopulateSystemInfoAsync("systeminfo").GetAwaiter().GetResult();
            _product = systemInfo.SystemObjects?[0]?.Product;

        }

        [HttpGet("GetCpuInfo")]
        public async Task<IActionResult?> GetCpuInfo()
        {
            try
            {
                var cpuInfoObject = await DataUtils.PopulateCpuInfoAsync("cpuinfo", $"{_product}.");
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
                var systemInfoObject = await DataUtils.PopulateSystemInfoAsync("systeminfo", $"{_product}.");
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
                var memInfo = await DataUtils.PopulateMemoryInfoAsync("memoryinfo", $"{_product}.");
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
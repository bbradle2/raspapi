namespace raspapi.Controllers
{

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Routing;
    using raspapi.Utils;
    using raspapi.DataObjects;

    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiInfoController : ControllerBase
    {
        private readonly ILogger<RaspberryPiInfoController> _logger;
        private readonly string ?_product;
        private readonly SystemInfoObject? _systemInfoObject;
        private readonly CPUInfoObject? _cpuInfoObject;
        public RaspberryPiInfoController(ILogger<RaspberryPiInfoController> logger)
        {
            _logger = logger;
            _systemInfoObject = DataUtils.PopulateSystemInfoAsync("systeminfo").GetAwaiter().GetResult();
            _product = _systemInfoObject.SystemObjects?[0]?.Product;

            _cpuInfoObject = DataUtils.PopulateCpuInfoAsync("cpuinfo", $"{_product}.").GetAwaiter().GetResult();

        }

        [HttpGet("GetCpuInfo")]
        public async Task<IActionResult?> GetCpuInfo()
        {
            try
            {
                return Ok(await Task.FromResult(_cpuInfoObject));
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
                return Ok(await Task.FromResult(_systemInfoObject));
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
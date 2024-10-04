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
        private readonly ILogger<RaspberryPiInfoController>? _logger;
        private readonly string? _product;
        private readonly SystemInfoObject? _systemInfoObject;

        public RaspberryPiInfoController(ILogger<RaspberryPiInfoController> logger)
        {
            try
            {
                _logger = logger;
                _systemInfoObject = DataUtils.PopulateSystemInfoAsync("No Description", "systeminfo").GetAwaiter().GetResult();
                _product = _systemInfoObject.SystemObjects?[0]?.Product;
                _systemInfoObject.ProductName = _product!;
            } 
            catch 
            {
                throw;
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
                _logger!.LogCritical("{Message}", e.Message);
                return BadRequest(new BadRequestResult());
            }
        }

        [HttpGet("GetCpuInfo")]
        public async Task<IActionResult?> GetCpuInfo()
        {
            try
            {
                CPUInfoObject _cpuInfoObject = await DataUtils.PopulateCpuInfoAsync($"{_product}.", "cpuinfo");
                return Ok(_cpuInfoObject);
            }
            catch (Exception e)
            {
                _logger!.LogError("{Message}", e.Message);
                return BadRequest(new BadRequestResult());
            }
        }

        [HttpGet("GetTemperatureInfo")]
        public async Task<IActionResult?> GetTemperatureInfo()
        {
            try
            {
                var temperatureInfo = await DataUtils.PopulateGetTemperatureInfoAsync($"{_product}.", "temperatureInfo");
                return Ok(temperatureInfo);
            }
            catch (Exception e)
            {
                _logger!.LogCritical("{Message}", e.Message);
                return BadRequest(new BadRequestResult());
            }
        }

        [HttpGet("GetMemoryInfo")]
        public async Task<IActionResult?> GetMemoryInfo()
        {
            try
            {
                var memInfo = await DataUtils.PopulateMemoryInfoAsync($"{_product}.", "memoryinfo");
                return Ok(memInfo);
            }
            catch (Exception e)
            {
                _logger!.LogError("{Message}", e.Message);
                return BadRequest(new BadRequestResult());
            }
        }
    }
}
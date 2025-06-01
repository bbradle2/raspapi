using Microsoft.AspNetCore.Mvc;
using raspapi.Utils;
using raspapi.Models;
using System.Device.Gpio;
using raspapi.Constants;


namespace raspapi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiInfoController : ControllerBase
    {
        private readonly ILogger<RaspberryPiInfoController>? _logger;
        //private readonly GpioController? _gpioController = gpioController;
        private string? _product;
        private SystemInfoObject? _systemInfoObject;

        public RaspberryPiInfoController(ILogger<RaspberryPiInfoController>? logger)
        {
            _logger = logger; 
            _systemInfoObject = DataUtils.PopulateSystemInfoAsync("No Description", "systeminfo").GetAwaiter().GetResult();
            _product = _systemInfoObject.SystemObjects?[0]?.Product;
            _systemInfoObject.ProductName = _product!;
        }
       

        [HttpGet("GetEndPoints")]
        public async Task<IActionResult?> GetEndPoints()
        {
            try
            {
                List<object> endpoints = [];

                foreach (var endpoint in HttpContext.RequestServices.GetServices<EndpointDataSource>().SelectMany(x => x!.Endpoints))
                {
                    
                    if (endpoint is RouteEndpoint routeEndpoint)
                    {
                        var routepatternrawtext = routeEndpoint.RoutePattern.RawText;

                        foreach (dynamic metadata in routeEndpoint.Metadata)
                        {
                            if (metadata.ToString()!.Contains("HttpMethods:"))
                            {
                                var end = new HttpEndPoint
                                {
                                    HttpCallEndPoint = routepatternrawtext!,
                                    HttpMethod = metadata.HttpMethods[0]
                                };

                                if (!end.HttpCallEndPoint.Contains("GetEndPoints") && !end.HttpCallEndPoint.Contains("GetGpioStatus"))
                                    endpoints.Add(end);
                            }
                        }
                    }
                }

                return Ok(await Task.FromResult(endpoints));

            }
            catch (Exception e)
            {
                _logger!.LogCritical("{Message}", e.Message);
                return BadRequest(e.Message);
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
                return BadRequest(e.Message);
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
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetTemperatureInfo")]
        public async Task<IActionResult?> GetTemperatureInfo()
        {
            try
            {
                
                var temperatureInfo = await DataUtils.PopulateTemperatureInfoAsync($"{_product}.", "temperatureInfo");
                return Ok(temperatureInfo);
            }
            catch (Exception e)
            {
                _logger!.LogCritical("{Message}", e.Message);
                return BadRequest(e.Message);
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
                return BadRequest(e.Message);
            }
        }
    }
}
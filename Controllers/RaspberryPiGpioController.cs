namespace raspapi.Controllers
{
    using System.Device.Gpio;
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using raspapi.Constants.RaspberryPIConstants;
    using System.ComponentModel.DataAnnotations;
    using raspapi.Utils;
    using raspapi.Extensions;
    using System.Linq;
    using System.Text.Json.Nodes;

    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiGpioController : ControllerBase
    {
        private readonly ILogger<RaspberryPiGpioController> _logger;
        private readonly GpioController _gpioController;
        private readonly SemaphoreSlim _semaphoreGpioController;

        public RaspberryPiGpioController(ILogger<RaspberryPiGpioController> logger,
                                         GpioController gpioController,
                                         [FromKeyedServices(MiscConstants.gpioSemaphoreName)] SemaphoreSlim semaphoreGpioController)
        {
            _logger = logger;
            _gpioController = gpioController;
            _semaphoreGpioController = semaphoreGpioController;

        }


        [HttpPut("SetLedOn")]
        public async Task<IActionResult?> SetLedOn(JsonArray pinNumbers)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_semaphoreGpioController);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_logger);


                await _semaphoreGpioController.WaitAsync();
                JsonNode[] nodePins = [.. pinNumbers.ToArray()!];
                List<int> pins = [];

                foreach (var n in pinNumbers)
                {
                    var pinObject = n!.AsObject();
                    var pinNumber = pinObject["pinNumber"];
                    var s = pinNumber!.GetValue<int>();
                    pins.Add(s);
                }

                _gpioController.GpioPinWriteHighValue([.. pins]);

                return Ok(pinNumbers);


            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(e.Message);
            }
            finally
            {
                _semaphoreGpioController.Release();
            }
        }

        [HttpPut("BlinkLeds")]
        public async Task<IActionResult?> BlinkLeds(JsonArray pinNumbers)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_semaphoreGpioController);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_logger);

                await _semaphoreGpioController.WaitAsync();

                JsonNode[] nodePins = [.. pinNumbers.ToArray()!];
                List<int> pins = [];

                foreach (var n in pinNumbers)
                {
                    var pinObject = n!.AsObject();
                    var pinNumber = pinObject["pinNumber"];
                    var s = pinNumber!.GetValue<int>();
                    pins.Add(s);
                }

                
                _gpioController.GpioPinWriteHighValue([.. pins]);
                await Task.Delay(500);

                _gpioController.GpioPinWriteLowValue([.. pins]);
                await Task.Delay(500);

                _gpioController.GpioPinWriteHighValue([.. pins]);
                await Task.Delay(500);

                _gpioController.GpioPinWriteLowValue([.. pins]);
                await Task.Delay(500);

                return Ok(pinNumbers);

            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(e.Message);
            }
            finally
            {
                _semaphoreGpioController.Release();
            }
        }

        [HttpPut("SetLedOff")]
        public async Task<IActionResult?> SetLedOff(JsonArray pinNumbers)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_semaphoreGpioController);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_logger);

                await _semaphoreGpioController.WaitAsync();
                JsonNode[] nodePins = [.. pinNumbers.ToArray()!];
                List<int> pins = [];

                foreach (var n in pinNumbers)
                {
                    var pinObject = n!.AsObject();
                    var pinNumber = pinObject["pinNumber"];
                    var s = pinNumber!.GetValue<int>();
                    pins.Add(s);
                }

                _gpioController.GpioPinWriteLowValue([.. pins]);

                return Ok(pinNumbers);

               
            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(e.Message);
            }
            finally
            {
                _semaphoreGpioController.Release();
            }
        }
    }  
}
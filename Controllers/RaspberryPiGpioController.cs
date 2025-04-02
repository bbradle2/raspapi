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
        private readonly BinarySemaphoreSlim _semaphoreGpioController;

        public RaspberryPiGpioController(ILogger<RaspberryPiGpioController> logger,
                                         GpioController gpioController,
                                         [FromKeyedServices(MiscConstants.gpioSemaphoreName)] BinarySemaphoreSlim semaphoreGpioController)
        {
            _logger = logger;
            _gpioController = gpioController;
            _semaphoreGpioController = semaphoreGpioController;

        }

        
        [HttpPut("SetLedOn")]
        public async Task<IActionResult?> SetLedOn(int[] pinNumbers)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_semaphoreGpioController);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_logger);

                await _semaphoreGpioController.WaitAsync();

                var pins = pinNumbers.DistinctBy(s => s);
                
                if (!pins.Any())
                {
                    return UnprocessableEntity(MiscConstants.Status422PinArrayIsEmpty);
                }
                
                _gpioController.GpioPinWriteHighValue([.. pins]);

                return Ok(pins);


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
        public async Task<IActionResult?> BlinkLeds(int[] pinNumbers)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_semaphoreGpioController);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_logger);

                await _semaphoreGpioController.WaitAsync();

                var pins = pinNumbers.DistinctBy(s => s);

                if (!pins.Any())
                {
                    return UnprocessableEntity(MiscConstants.Status422PinArrayIsEmpty);
                }
                
                _gpioController.GpioPinWriteHighValue([.. pins]);
                await Task.Delay(500);

                _gpioController.GpioPinWriteLowValue([.. pins]);
                await Task.Delay(500);

                _gpioController.GpioPinWriteHighValue([.. pins]);
                await Task.Delay(500);

                _gpioController.GpioPinWriteLowValue([.. pins]);
                await Task.Delay(500);

                return Ok(pins);

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
        public async Task<IActionResult?> SetLedOff(int[] pinNumbers)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_semaphoreGpioController);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_logger);

                await _semaphoreGpioController.WaitAsync();

                var pins = pinNumbers.DistinctBy(s => s);

                if (!pins.Any())
                {
                    return UnprocessableEntity(MiscConstants.Status422PinArrayIsEmpty);
                }               

                _gpioController.GpioPinWriteLowValue([.. pins]);

                return Ok(pins);

               
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
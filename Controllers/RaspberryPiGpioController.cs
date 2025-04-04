namespace raspapi.Controllers
{
    using System.Device.Gpio;
     using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using raspapi.Constants.RaspberryPIConstants;
    using raspapi.Utils;
    using raspapi.Extensions;
    using System.Linq;
    using raspapi.DataObjects;

    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiGpioController : ControllerBase
    {
        private readonly ILogger<RaspberryPiGpioController> _logger;
        private readonly GpioController _gpioController;
        private readonly BinarySemaphoreSlim _semaphoreGpio;

        public RaspberryPiGpioController(ILogger<RaspberryPiGpioController> logger,
                                         GpioController gpioController,
                                         [FromKeyedServices(MiscConstants.gpioSemaphoreName)] BinarySemaphoreSlim semaphoreGpio)
        {
            _logger = logger;
            _gpioController = gpioController;
            _semaphoreGpio = semaphoreGpio;
        }


        [HttpPut("SetPinsHigh")]
        public async Task<IActionResult?> SetPinsHigh(PinObject[] pinObjs)
        {          
    
            try
            {
                ArgumentNullException.ThrowIfNull(_semaphoreGpio);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_logger);
                
                await _semaphoreGpio.WaitAsync();

                var pins = pinObjs.DistinctBy(s => s.PinNumber);
                var pinsStatus = _gpioController.GpioPinWriteHighValue([.. pins]);
                var pinsStatusArray = _gpioController.GetPinsStatusArray(pinsStatus);

                return Ok(pinsStatusArray);


            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(e.Message);
            }
            finally
            {
                _semaphoreGpio.Release();
            }
        }

        [HttpPut("TogglePins")]
        public async Task<IActionResult?> TogglePins(PinObject[] pinNumbers)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_semaphoreGpio);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_logger);

                await _semaphoreGpio.WaitAsync();

                var pins = pinNumbers.DistinctBy(s => s);
                var pinsStatusArray = new List<PinObject>();

                for (int i = 0; i < 2; i++)
                {
                    var pinsStatus = _gpioController.GpioPinWriteHighValue([.. pins]);
                    pinsStatusArray = _gpioController.GetPinsStatusArray(pinsStatus);
                    await Task.Delay(500);

                    pinsStatus = _gpioController.GpioPinWriteLowValue([.. pins]);
                    pinsStatusArray = _gpioController.GetPinsStatusArray(pinsStatus);
                    await Task.Delay(500);

                    

                    
                }

                return Ok(pinsStatusArray);

            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(e.Message);
            }
            finally
            {
                _semaphoreGpio.Release();
            }
        }

        [HttpPut("SetPinsLow")]
        public async Task<IActionResult?> SetPinsLow(PinObject[] pinObjs)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_semaphoreGpio);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_logger);

                await _semaphoreGpio.WaitAsync();

                var pins = pinObjs.DistinctBy(s => s.PinNumber);
                var pinsStatus = _gpioController.GpioPinWriteLowValue([.. pins]);
                var pinsStatusArray = _gpioController.GetPinsStatusArray(pinsStatus);

                return Ok(pinsStatusArray);


            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(e.Message);
            }
            finally
            {
                _semaphoreGpio.Release();
            }
        }

        
        [HttpGet("GetPinsStatus")]
        public async Task<IActionResult?> GetPinsStatus(PinObject[] pinObjs)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_semaphoreGpio);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_logger);
               
                await _semaphoreGpio.WaitAsync();

                var pins = pinObjs.DistinctBy(s => s.PinNumber);               
                var pinsStatus = _gpioController.GpioGetPinValue([.. pins]);
                var pinsStatusArray = _gpioController.GetPinsStatusArray(pinsStatus);

                return Ok(pinsStatusArray);

            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(e.Message);
            }
            finally
            {
                _semaphoreGpio.Release();
            }
        }
    }  
}
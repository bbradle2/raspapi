namespace raspapi.Controllers
{
    using System.Device.Gpio;
     using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using raspapi.Constants;
    using raspapi.Utils;
    using System.Linq;
    using raspapi.Models;
    using System.Net.WebSockets;

    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiGpioController : ControllerBase
    {
        private readonly ILogger<RaspberryPiGpioController> _logger;
        private readonly GpioController _gpioController;
        private readonly BinarySemaphoreSlim _semaphoreGpio;
        private List<GpioObject> _gpioObjects;

        private GpioObjectsWaitEventHandler _gpioObjectsWaitEventHandler;
        private readonly IConfiguration _configuration;

        public RaspberryPiGpioController(ILogger<RaspberryPiGpioController> logger,
                                         [FromKeyedServices(MiscConstants.gpioControllerName)] GpioController gpioController,
                                         [FromKeyedServices(MiscConstants.gpioSemaphoreName)] BinarySemaphoreSlim semaphoreGpio,
                                         [FromKeyedServices(MiscConstants.gpioObjectsName)] List<GpioObject> gpioObjects,
                                         [FromKeyedServices(MiscConstants.gpioObjectsWaitEventName)] GpioObjectsWaitEventHandler gpioObjectsWaitEventHandler,
                                         IConfiguration configuration)
        {
            _logger = logger;
            _gpioController = gpioController;
            _semaphoreGpio = semaphoreGpio;
            _gpioObjects = gpioObjects;
            _gpioObjectsWaitEventHandler = gpioObjectsWaitEventHandler;
            _configuration = configuration;
        }


        [HttpPut("SetGpiosHigh")]
        public async Task<IActionResult?> SetGpiosHigh(List<GpioObject> gpioObjs)
        {

            try
            {
                ArgumentNullException.ThrowIfNull(_semaphoreGpio);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_logger);

                await _semaphoreGpio.WaitAsync();

                List<GpioObject> gpioObjects = [];

                foreach (var gpioObject in _gpioObjects)
                {
                    bool gpioValue = false;
                    if (gpioObjs.Where(s => (s.GpioValue == null || s.GpioValue == false) && s.GpioNumber == gpioObject.GpioNumber).Count() == 1)
                    {
                        if (!_gpioController.IsPinOpen(gpioObject.GpioNumber))
                        {
                            var gpioPin = _gpioController!.OpenPin(gpioObject.GpioNumber, PinMode.Output).PinNumber;
                            _gpioController.Write(gpioPin, PinValue.High);
                            gpioValue = (bool)_gpioController.Read(gpioObject.GpioNumber);
                        }
                        else
                        {
                            _gpioController.Write(gpioObject.GpioNumber, PinValue.High);
                            gpioValue = (bool)_gpioController.Read(gpioObject.GpioNumber);
                        }
                    }

                    gpioObjects.Add(new GpioObject { GpioNumber = gpioObject.GpioNumber, GpioValue = gpioValue });
                }

                _gpioObjects.Clear();

                foreach (var i in gpioObjects)
                {
                    _gpioObjects.Add(i);
                }
                
                return Ok(_gpioObjects);
            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(e.Message);
            }
            finally
            {
                _semaphoreGpio.Release();
                _gpioObjectsWaitEventHandler.Set();
            }
        }

        [HttpPut("SetGpiosLow")]
        public async Task<IActionResult?> SetGpiosLow(List<GpioObject> gpioObjs)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_semaphoreGpio);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_logger);

                await _semaphoreGpio.WaitAsync();

                List<GpioObject> gpioObjects = [];

                foreach (var gpioObject in _gpioObjects)
                {
                    bool gpioValue = false;
                    if (gpioObjs.Where(s => (s.GpioValue == null || s.GpioValue == true) && s.GpioNumber == gpioObject.GpioNumber).Count() == 1)
                    {
                        if (!_gpioController.IsPinOpen(gpioObject.GpioNumber))
                        {
                            var gpioPin = _gpioController!.OpenPin(gpioObject.GpioNumber, PinMode.Output).PinNumber;
                            _gpioController.Write(gpioPin, PinValue.Low);
                            gpioValue = (bool)_gpioController.Read(gpioObject.GpioNumber);

                        }
                        else
                        {
                            _gpioController.Write(gpioObject.GpioNumber, PinValue.Low);
                            gpioValue = (bool)_gpioController.Read(gpioObject.GpioNumber);

                        }
                    }

                    gpioObjects.Add(new GpioObject { GpioNumber = gpioObject.GpioNumber, GpioValue = gpioValue });

                }

                _gpioObjects.Clear();

                foreach (var i in gpioObjects)
                {
                    _gpioObjects.Add(i);
                }

                return Ok(_gpioObjects);
            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(e.Message);
            }
            finally
            {
                _semaphoreGpio.Release();
                _gpioObjectsWaitEventHandler.Set();
            }
        }
    }  
}
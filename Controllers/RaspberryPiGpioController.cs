using System.Device.Gpio;
using Microsoft.AspNetCore.Mvc;
using raspapi.Constants;
using raspapi.Models;
using System.Collections.Concurrent;
using raspapi.Interfaces;

namespace raspapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiGpioController(ILogger<RaspberryPiGpioController> logger,
                                           [FromKeyedServices(MiscConstants.gpioControllerName)] GpioController gpioController,
                                           [FromKeyedServices(MiscConstants.gpioObjectsName)] ConcurrentQueue<GpioObject> gpioObjects,
                                           IConfiguration configuration,
                                           [FromKeyedServices(MiscConstants.binarySemaphoreSlimHandler)] IBinarySemaphoreSlimHandler binarySemaphoreSlimHandler) : ControllerBase
    {
        private readonly ILogger<RaspberryPiGpioController> _logger = logger;
        private readonly GpioController _gpioController = gpioController;
        private readonly ConcurrentQueue<GpioObject> _gpioObjects = gpioObjects;
        private readonly IConfiguration _configuration = configuration;
        private readonly IBinarySemaphoreSlimHandler _binarySemaphoreSlimHandler = binarySemaphoreSlimHandler;



        [HttpGet("GetGpioStatus")]
        public async Task<IActionResult?> GetGpioStatus()
        {
            try
            {
                return await Task.FromResult(Ok(_gpioObjects)); ;
            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest("Something went wrong.");
            }
        }

        [HttpPut("SetGpiosHigh")]
        public async Task<IActionResult?> SetGpiosHigh(ConcurrentQueue<GpioObject> gpioObjs)
        {
            try
            {
                await _binarySemaphoreSlimHandler.WaitAsync();
                ArgumentNullException.ThrowIfNull(gpioObjs);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_logger);

                ConcurrentQueue<GpioObject> gpioObjects = [];

                foreach (var gpioObj in gpioObjs)
                {
                    bool gpioValue = true;
                    if (gpioObjs.Where(s => (s.GpioValue == null || s.GpioValue == false) && s.GpioNumber == gpioObj.GpioNumber).Count() == 1)
                    {
                        if (!_gpioController.IsPinOpen(gpioObj.GpioNumber))
                        {
                            var gpioPin = _gpioController!.OpenPin(gpioObj.GpioNumber, PinMode.Output).PinNumber;
                            _gpioController.Write(gpioPin, PinValue.High);
                            gpioValue = (bool)_gpioController.Read(gpioObj.GpioNumber);
                        }
                        else
                        {
                            _gpioController.Write(gpioObj.GpioNumber, PinValue.High);
                            gpioValue = (bool)_gpioController.Read(gpioObj.GpioNumber);
                        }
                    }

                    gpioObjects.Enqueue(new GpioObject { GpioNumber = gpioObj.GpioNumber, GpioValue = gpioValue });
                }


                _gpioObjects.Clear();
                foreach (var gpioObject in gpioObjects)
                {
                    _gpioObjects.Enqueue(gpioObject);
                }

                return await Task.FromResult(Ok(_gpioObjects));
            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest("Something went wrong.");
            }
            finally
            {
                _binarySemaphoreSlimHandler.Release();
            }
        }

        [HttpPut("SetGpiosLow")]
        public async Task<IActionResult?> SetGpiosLow(ConcurrentQueue<GpioObject> gpioObjs)
        {
            try
            {
                await _binarySemaphoreSlimHandler.WaitAsync();

                ArgumentNullException.ThrowIfNull(gpioObjs);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_logger);

                ConcurrentQueue<GpioObject> gpioObjects = [];

                foreach (var gpioObj in gpioObjs)
                {
                    bool gpioValue = false;
                    if (gpioObjs.Where(s => (s.GpioValue == null || s.GpioValue == true) && s.GpioNumber == gpioObj.GpioNumber).Count() == 1)
                    {
                        if (!_gpioController.IsPinOpen(gpioObj.GpioNumber))
                        {
                            var gpioPin = _gpioController!.OpenPin(gpioObj.GpioNumber, PinMode.Output).PinNumber;
                            _gpioController.Write(gpioPin, PinValue.Low);
                            gpioValue = (bool)_gpioController.Read(gpioObj.GpioNumber);

                        }
                        else
                        {
                            _gpioController.Write(gpioObj.GpioNumber, PinValue.Low);
                            gpioValue = (bool)_gpioController.Read(gpioObj.GpioNumber);

                        }
                    }

                    gpioObjects.Enqueue(new GpioObject { GpioNumber = gpioObj.GpioNumber, GpioValue = gpioValue });

                }

                _gpioObjects.Clear();

                foreach (var gpioObject in gpioObjects)
                {
                    _gpioObjects.Enqueue(gpioObject);
                }

                return await Task.FromResult(Ok(_gpioObjects));

            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest("Something went wrong.");
            }
            finally
            {
                _binarySemaphoreSlimHandler.Release();
            }
        }
    }
}
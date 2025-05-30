using System.Device.Gpio;
using Microsoft.AspNetCore.Mvc;
using raspapi.Constants;
using raspapi.Models;
using raspapi.Interfaces;
using System.Collections.Concurrent;

namespace raspapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiGpioController(ILogger<RaspberryPiGpioController> logger,
                                           [FromKeyedServices(MiscConstants.gpioControllerName)] GpioController gpioController,
                                           [FromKeyedServices(MiscConstants.gpioObjectsName)] ConcurrentQueue<GpioObject> gpioObjects,
                                           [FromKeyedServices(MiscConstants.webSocketHandlerName)] IWebSocketHandler webSocketHandler,
                                           IConfiguration configuration) : ControllerBase
    {
        private readonly ILogger<RaspberryPiGpioController> _logger = logger;
        private readonly GpioController _gpioController = gpioController;
        private readonly ConcurrentQueue<GpioObject> _gpioObjects = gpioObjects;
        private readonly IConfiguration _configuration = configuration;
        private readonly IWebSocketHandler _webSocketHandler = webSocketHandler;
        

        [Route("/ws")]
        [HttpGet("GetGpioStatus")]
        public async Task GetGpioStatus()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _webSocketHandler.GetGpioStatus(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400; // Bad request
            }
        }

        [HttpPut("SetGpiosHigh")]
        public async Task<IActionResult?> SetGpiosHigh(ConcurrentQueue<GpioObject> gpioObjs)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(gpioObjs);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_logger);

                ConcurrentQueue<GpioObject> gpioObjects = [];

                foreach (var gpioObject in gpioObjs)
                {
                    bool gpioValue = true;
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

                    gpioObjects.Enqueue(new GpioObject { GpioNumber = gpioObject.GpioNumber, GpioValue = gpioValue });
                }


                _gpioObjects.Clear();
                foreach (var i in gpioObjects)
                {
                    _gpioObjects.Enqueue(i);
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

            }
        }

        [HttpPut("SetGpiosLow")]
        public async Task<IActionResult?> SetGpiosLow(ConcurrentQueue<GpioObject> gpioObjs)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(gpioObjs);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_logger);

                ConcurrentQueue<GpioObject> gpioObjects = [];

                foreach (var gpioObject in gpioObjs)
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

                    gpioObjects.Enqueue(new GpioObject { GpioNumber = gpioObject.GpioNumber, GpioValue = gpioValue });

                }

                _gpioObjects.Clear();

                foreach (var i in gpioObjects)
                {
                    _gpioObjects.Enqueue(i);
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

            }
        }
    }
}
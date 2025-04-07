namespace raspapi.Controllers
{
    using System.Device.Gpio;
     using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using raspapi.Constants.RaspberryPIConstants;
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
        private HashSet<PinObject> _pinObjects;

        public RaspberryPiGpioController(ILogger<RaspberryPiGpioController> logger,
                                         [FromKeyedServices(MiscConstants.gpioControllerName)] GpioController gpioController,
                                         [FromKeyedServices(MiscConstants.gpioSemaphoreName)] BinarySemaphoreSlim semaphoreGpio,
                                         HashSet<PinObject> pinObjects)
        {
            _logger = logger;
            _gpioController = gpioController;
            _semaphoreGpio = semaphoreGpio;
            _pinObjects = pinObjects;
        }

        // [Route("/ws")]
        // public async Task Get()
        // {
        //     if (HttpContext.WebSockets.IsWebSocketRequest)
        //     {
        //         using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        //         await WebSocketUtils.Echo(webSocket);
        //     }
        //     else
        //     {
        //         HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        //     }
        // }


        [HttpPut("SetPinsHigh")]
        public async Task<IActionResult?> SetPinsHigh(PinObject[] pinObjs)
        {          
    
            try
            {
                ArgumentNullException.ThrowIfNull(_semaphoreGpio);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_logger);
                
                await _semaphoreGpio.WaitAsync();

                var pinObjects = pinObjs.DistinctBy(s => s.PinNumber);
                _pinObjects.Clear();
                //var pinsStatus = _gpioController.GpioPinWriteHighValue([.. pins]);
                HashSet<PinObject> pinsStatus = [];
                
                foreach (var pinObject in pinObjects)
                {
                    if (!_gpioController.IsPinOpen(pinObject.PinNumber))
                    {
                        var gpioPin = _gpioController!.OpenPin(pinObject.PinNumber, PinMode.Output).PinNumber;
                        _gpioController.Write(gpioPin, PinValue.High);
                        var value = _gpioController.Read(pinObject.PinNumber);
                        pinsStatus.Add(new PinObject { PinNumber = pinObject.PinNumber, PinValue = (bool)value });
                    }
                    else
                    {
                        _gpioController.Write(pinObject.PinNumber, PinValue.High);
                        var value =_gpioController.Read(pinObject.PinNumber);
                        pinsStatus.Add(new PinObject { PinNumber = pinObject.PinNumber, PinValue = (bool)value });

                    }                   
                }
                
                foreach (var pinStatus in pinsStatus)
                {
                    _pinObjects.Add(pinStatus);
                }
                
                return Ok(_pinObjects);


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

                var pinObjects = pinObjs.DistinctBy(s => s.PinNumber);
                //var pinsStatus = _gpioController.GpioPinWriteHighValue([.. pins]);
                HashSet<PinObject> pinsStatus = [];
                _pinObjects.Clear();
                foreach (var pinObject in pinObjects)
                {
                    if (!_gpioController.IsPinOpen(pinObject.PinNumber))
                    {
                        var gpioPin = _gpioController!.OpenPin(pinObject.PinNumber, PinMode.Output).PinNumber;
                        _gpioController.Write(gpioPin, PinValue.Low);
                        var value = _gpioController.Read(pinObject.PinNumber);
                        pinsStatus.Add(new PinObject { PinNumber = pinObject.PinNumber, PinValue = (bool)value });
                    }
                    else
                    {
                        _gpioController.Write(pinObject.PinNumber, PinValue.Low);
                        var value = _gpioController.Read(pinObject.PinNumber);
                        pinsStatus.Add(new PinObject { PinNumber = pinObject.PinNumber, PinValue = (bool)value });

                    }
                }
                foreach (var pinStatus in pinsStatus)
                {
                    _pinObjects.Add(pinStatus);
                }

                return Ok(_pinObjects);


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
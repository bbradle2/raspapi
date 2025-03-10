namespace raspapi.Controllers
{
    using System.Device.Gpio;
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using raspapi.Interfaces;
    using raspapi.Constants.RaspberryPIConstants;
    using System.ComponentModel.DataAnnotations;

    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiGpioController: ControllerBase
    {
        private readonly ILogger<RaspberryPiGpioController> _logger;
        private readonly GpioController _gpioController;
        private readonly IDictionary<int, IGpioPin> _pins;
        private readonly SemaphoreSlim _semaphoreGpioController;

        public RaspberryPiGpioController(ILogger<RaspberryPiGpioController> logger, GpioController gpioController, Dictionary<int, IGpioPin> pins, [FromKeyedServices(MiscConstants.gpioSemaphoreName)] SemaphoreSlim semaphoreGpioController)
        {
            _logger = logger;
            _gpioController = gpioController;
            _pins = pins;
            _semaphoreGpioController = semaphoreGpioController;

        }

        [HttpGet("GetLedStatus")]
        public async Task<IActionResult?> GetLedStatus()
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_semaphoreGpioController);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_pins);
                ArgumentNullException.ThrowIfNull(_logger);

                await _semaphoreGpioController.WaitAsync();              

                var openPin23 = _gpioController.OpenPin(_pins[GpioPinConstants.PIN23].Pin, PinMode.Output);
                var openPin24 = _gpioController.OpenPin(_pins[GpioPinConstants.PIN24].Pin, PinMode.Output);
                var openPin25 = _gpioController.OpenPin(_pins[GpioPinConstants.PIN25].Pin, PinMode.Output);

                _pins[GpioPinConstants.PIN23].Status = openPin23.Read() == 1 ? "On" : "Off";
                _pins[GpioPinConstants.PIN24].Status = openPin24.Read() == 1 ? "On" : "Off";
                _pins[GpioPinConstants.PIN25].Status = openPin25.Read() == 1 ? "On" : "Off";

                return Ok(_pins);
            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(e.Message);
            } 
            finally 
            {
                _gpioController.ClosePin(_pins[GpioPinConstants.PIN23].Pin);
                _gpioController.ClosePin(_pins[GpioPinConstants.PIN24].Pin);
                _gpioController.ClosePin(_pins[GpioPinConstants.PIN25].Pin);

                _semaphoreGpioController.Release();
            }
        }

        [HttpPut("SetLedOn")]
        public async Task<IActionResult?> SetLedOn()
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_semaphoreGpioController);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_pins);
                ArgumentNullException.ThrowIfNull(_logger);

                await _semaphoreGpioController.WaitAsync();

                var openPin23 = _gpioController.OpenPin(_pins[GpioPinConstants.PIN23].Pin, PinMode.Output);
                var openPin24 = _gpioController.OpenPin(_pins[GpioPinConstants.PIN24].Pin, PinMode.Output);
                var openPin25 = _gpioController.OpenPin(_pins[GpioPinConstants.PIN25].Pin, PinMode.Output);

                _gpioController.Write(_pins[GpioPinConstants.PIN23].Pin, PinValue.High);
                _gpioController.Write(_pins[GpioPinConstants.PIN24].Pin, PinValue.High);
                _gpioController.Write(_pins[GpioPinConstants.PIN25].Pin, PinValue.High);

                _pins[GpioPinConstants.PIN23].Status = openPin23.Read() == 1 ? "On" : "Off";
                _pins[GpioPinConstants.PIN24].Status = openPin24.Read() == 1 ? "On" : "Off";
                _pins[GpioPinConstants.PIN25].Status = openPin25.Read() == 1 ? "On" : "Off";

                return Ok(_pins);

            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(e.Message);
            }
            finally
            {
                _semaphoreGpioController.Release();
                _gpioController.ClosePin(_pins[GpioPinConstants.PIN23].Pin);
                _gpioController.ClosePin(_pins[GpioPinConstants.PIN24].Pin);
                _gpioController.ClosePin(_pins[GpioPinConstants.PIN25].Pin);
            }
        }

        [HttpPut("BlinkLed")]
        public async Task<IActionResult?> BlinkLed()
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_semaphoreGpioController);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_pins);
                ArgumentNullException.ThrowIfNull(_logger);


                await _semaphoreGpioController.WaitAsync();

                var openPin23 = _gpioController.OpenPin(_pins[GpioPinConstants.PIN23].Pin, PinMode.Output);
                var openPin24 = _gpioController.OpenPin(_pins[GpioPinConstants.PIN24].Pin, PinMode.Output);
                var openPin25 = _gpioController.OpenPin(_pins[GpioPinConstants.PIN25].Pin, PinMode.Output);


                _gpioController.Write(_pins[GpioPinConstants.PIN23].Pin, PinValue.High);
                _gpioController.Write(_pins[GpioPinConstants.PIN24].Pin, PinValue.High);
                _gpioController.Write(_pins[GpioPinConstants.PIN25].Pin, PinValue.High);
                await Task.Delay(2000);

                _gpioController.Write(_pins[GpioPinConstants.PIN23].Pin, PinValue.Low);
                _gpioController.Write(_pins[GpioPinConstants.PIN24].Pin, PinValue.Low);
                _gpioController.Write(_pins[GpioPinConstants.PIN25].Pin, PinValue.Low);
                await Task.Delay(2000);


                _gpioController.Write(_pins[GpioPinConstants.PIN23].Pin, PinValue.High);
                _gpioController.Write(_pins[GpioPinConstants.PIN24].Pin, PinValue.High);
                _gpioController.Write(_pins[GpioPinConstants.PIN25].Pin, PinValue.High);
                await Task.Delay(2000);

                _gpioController.Write(_pins[GpioPinConstants.PIN23].Pin, PinValue.Low);
                _gpioController.Write(_pins[GpioPinConstants.PIN24].Pin, PinValue.Low);
                _gpioController.Write(_pins[GpioPinConstants.PIN25].Pin, PinValue.Low);
                await Task.Delay(2000);


                _pins[GpioPinConstants.PIN23].Status = openPin23.Read() == 1 ? "On" : "Off";
                _pins[GpioPinConstants.PIN24].Status = openPin24.Read() == 1 ? "On" : "Off";
                _pins[GpioPinConstants.PIN25].Status = openPin25.Read() == 1 ? "On" : "Off";

                return Ok(_pins);

            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(e.Message);
            }
            finally
            {
                _semaphoreGpioController.Release();
                _gpioController.ClosePin(_pins[GpioPinConstants.PIN23].Pin);
                _gpioController.ClosePin(_pins[GpioPinConstants.PIN24].Pin);
                _gpioController.ClosePin(_pins[GpioPinConstants.PIN25].Pin);
            }
        }

        [HttpPut("SetLedOff")]
        public async Task<IActionResult?> SetLedOff()
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_semaphoreGpioController);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_pins);
                ArgumentNullException.ThrowIfNull(_logger);

                await _semaphoreGpioController.WaitAsync();

                var openPin23 = _gpioController.OpenPin(_pins[GpioPinConstants.PIN23].Pin, PinMode.Output);
                var openPin24 = _gpioController.OpenPin(_pins[GpioPinConstants.PIN24].Pin, PinMode.Output);
                var openPin25 = _gpioController.OpenPin(_pins[GpioPinConstants.PIN25].Pin, PinMode.Output);

                _gpioController.Write(_pins[GpioPinConstants.PIN23].Pin, PinValue.Low);
                _gpioController.Write(_pins[GpioPinConstants.PIN24].Pin, PinValue.Low);
                _gpioController.Write(_pins[GpioPinConstants.PIN25].Pin, PinValue.Low);

                _pins[GpioPinConstants.PIN23].Status = openPin23.Read() == 1 ? "On" : "Off";
                _pins[GpioPinConstants.PIN24].Status = openPin24.Read() == 1 ? "On" : "Off";
                _pins[GpioPinConstants.PIN25].Status = openPin25.Read() == 1 ? "On" : "Off";

                return Ok(_pins);
            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(e.Message);
            } 
            finally 
            {
                _gpioController.ClosePin(_pins[GpioPinConstants.PIN23].Pin);
                _gpioController.ClosePin(_pins[GpioPinConstants.PIN24].Pin);
                _gpioController.ClosePin(_pins[GpioPinConstants.PIN25].Pin);
                
                _semaphoreGpioController.Release();
            }
        }
    }  
}
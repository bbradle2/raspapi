namespace raspapi.Controllers
{
    using System.Device.Gpio;
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using raspapi.DataObjects;

    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiGpioController: ControllerBase
    {
        private static readonly int pin = 23;
        private readonly ILogger<RaspberryPiGpioController> _logger;
        private readonly GpioController _gpioController;
        private readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);

        public RaspberryPiGpioController(ILogger<RaspberryPiGpioController> logger, GpioController gpioController)
        {
            _logger = logger;
            _gpioController = gpioController;
        }

        [HttpGet("GetLedStatus")]
        public async Task<IActionResult?> GetLedStatus()
        {
            ArgumentNullException.ThrowIfNull(_gpioController);

            var openPin = _gpioController.OpenPin(pin, PinMode.Output);

            try
            {
                Led led = new();
                led.LedPin = pin;

                await Task.Run(() =>
                {
                    led.LedValue = openPin.Read() == 1 ? "On" : "Off";
                });

                return Ok(led);
            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(new BadRequestResult());
            }
        }

        [HttpPut("SetLedOn")]
        public async Task<IActionResult?> SetLedOn()
        {
            var openPin = _gpioController.OpenPin(pin, PinMode.Output);

            try
            {
                _gpioController.Write(pin, PinValue.High);
                Led led = new();
                led.LedPin = pin;
                await Task.Run(() =>
                {
                   led.LedValue = openPin.Read() == 1 ? "On" : "Off";
                });

                return Ok(led);

            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(new BadRequestResult());
            }

        }

        [HttpPut("SetLedOff")]
        public async Task<IActionResult?> SetLedOff()
        {
            var openPin = _gpioController.OpenPin(pin, PinMode.Output);

            try
            {
                _gpioController.Write(pin, PinValue.Low);
                Led led = new();
                led.LedPin = pin;
                await Task.Run(() =>
                {
                    led.LedValue = openPin.Read() == 1 ? "On" : "Off";
                });

                return Ok(led);

            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(new BadRequestResult());
            }

        }
    }  
}
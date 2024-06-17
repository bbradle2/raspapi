namespace raspapi.Controllers
{
    using System.Device.Gpio;
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using raspapi.DataObjects;
    using raspapi.Interfaces;
    using raspapi.Contants;

    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiGpioController: ControllerBase
    {
        private readonly ILogger<RaspberryPiGpioController> _logger;
        private readonly GpioController _gpioController;
        private readonly Dictionary<int, IPin> _pins;
        private readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);

        public RaspberryPiGpioController(ILogger<RaspberryPiGpioController> logger, GpioController gpioController, Dictionary<int, IPin> pins)
        {
            _logger = logger;
            _gpioController = gpioController;
            _pins = pins;
        }

        [HttpGet("GetLedStatus")]
        public IActionResult? GetLedStatus()
        {
            ArgumentNullException.ThrowIfNull(_gpioController);

            var openPin = _gpioController.OpenPin(_pins[RaspBerryPiContants.PIN23].Pin, PinMode.Output);

            try
            {
                _pins[RaspBerryPiContants.PIN23].Status = openPin.Read() == 1 ? "On" : "Off";
                //_logger.LogInformation("Returning Pins");
                return Ok(_pins);
            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(new BadRequestResult());
            }
        }

        [HttpPut("SetLedOn")]
        public IActionResult? SetLedOn()
        {
            ArgumentNullException.ThrowIfNull(_gpioController);

            var openPin = _gpioController.OpenPin(_pins[RaspBerryPiContants.PIN23].Pin, PinMode.Output);

            try
            {
                _gpioController.Write(_pins[RaspBerryPiContants.PIN23].Pin, PinValue.High);
                _pins[RaspBerryPiContants.PIN23].Status = openPin.Read() == 1 ? "On" : "Off";
                //_logger.LogInformation("Returning Pins");
                return Ok(_pins);

            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(new BadRequestResult());
            }

        }

        [HttpPut("SetLedOff")]
        public IActionResult? SetLedOff()
        {
            ArgumentNullException.ThrowIfNull(_gpioController);

            var openPin = _gpioController.OpenPin(_pins[RaspBerryPiContants.PIN23].Pin, PinMode.Output);

            try
            {
                _gpioController.Write(_pins[RaspBerryPiContants.PIN23].Pin, PinValue.Low);
                _pins[RaspBerryPiContants.PIN23].Status = openPin.Read() == 1 ? "On" : "Off";
                //_logger.LogInformation("Returning Pins");
                return Ok(_pins);

            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(new BadRequestResult());
            }

        }
    }  
}
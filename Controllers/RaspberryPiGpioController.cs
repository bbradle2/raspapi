namespace raspapi.Controllers
{
    using System.Device.Gpio;
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using raspapi.Interfaces;
    using raspapi.Constants.RaspberryPIConstants;

    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiGpioController: ControllerBase
    {
        private readonly ILogger<RaspberryPiGpioController> _logger;
        private readonly GpioController _gpioController;
        private readonly IDictionary<int, IGpioPin> _pins;
        private readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);
       
        public RaspberryPiGpioController(ILogger<RaspberryPiGpioController> logger, GpioController gpioController, Dictionary<int, IGpioPin> pins)
        {
            _logger = logger;
            _gpioController = gpioController;
            _pins = pins;
           
        }

        [HttpGet("GetLedStatus")]
        public IActionResult? GetLedStatus()
        {
            ArgumentNullException.ThrowIfNull(_gpioController);          

            try
            {
                var openPin23 = _gpioController.OpenPin(_pins[GpioPinConstants.PIN23].Pin, PinMode.Output);
                var openPin24 = _gpioController.OpenPin(_pins[GpioPinConstants.PIN24].Pin, PinMode.Output);

                _pins[GpioPinConstants.PIN23].Status = openPin23.Read() == 1 ? "On" : "Off";
                _pins[GpioPinConstants.PIN24].Status = openPin24.Read() == 1 ? "On" : "Off";

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
            
            try
            {
                var openPin23 = _gpioController.OpenPin(_pins[GpioPinConstants.PIN23].Pin, PinMode.Output);
                var openPin24 = _gpioController.OpenPin(_pins[GpioPinConstants.PIN24].Pin, PinMode.Output);

                _gpioController.Write(_pins[GpioPinConstants.PIN23].Pin, PinValue.High);
                _gpioController.Write(_pins[GpioPinConstants.PIN24].Pin, PinValue.High);

                _pins[GpioPinConstants.PIN23].Status = openPin23.Read() == 1 ? "On" : "Off";
                _pins[GpioPinConstants.PIN24].Status = openPin24.Read() == 1 ? "On" : "Off";

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

            try
            {
                var openPin23 = _gpioController.OpenPin(_pins[GpioPinConstants.PIN23].Pin, PinMode.Output);
                var openPin24 = _gpioController.OpenPin(_pins[GpioPinConstants.PIN24].Pin, PinMode.Output);

                _gpioController.Write(_pins[GpioPinConstants.PIN23].Pin, PinValue.Low);
                _gpioController.Write(_pins[GpioPinConstants.PIN24].Pin, PinValue.Low);

                _pins[GpioPinConstants.PIN23].Status = openPin23.Read() == 1 ? "On" : "Off";
                _pins[GpioPinConstants.PIN24].Status = openPin24.Read() == 1 ? "On" : "Off";

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
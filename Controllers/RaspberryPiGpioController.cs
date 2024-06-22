namespace raspapi.Controllers
{
    using System.Device.Gpio;
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using raspapi.DataObjects;
    using raspapi.Interfaces;
    using raspapi.Contants;
    using System.Text.Json.Serialization.Metadata;
    using raspapi.JsonDictionaryConverter;

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
                var openPin23 = _gpioController.OpenPin(_pins[RaspBerryPiContants.PIN23].Pin, PinMode.Output);
                var openPin24 = _gpioController.OpenPin(_pins[RaspBerryPiContants.PIN24].Pin, PinMode.Output);

                _pins[RaspBerryPiContants.PIN23].Status = openPin23.Read() == 1 ? "On" : "Off";
                _pins[RaspBerryPiContants.PIN24].Status = openPin24.Read() == 1 ? "On" : "Off";

                //var options = new JsonSerializerOptions();
                // _options.Converters.Add(new JsonDictionaryConverter.DictionaryInt32StringKeyValueConverter(_options));
                // var p = JsonSerializer.Serialize(_pins, _options);
                // //var t = JsonSerializer.Deserialize<Dictionary<int, GpioPin23>>(p, _options);


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
                var openPin23 = _gpioController.OpenPin(_pins[RaspBerryPiContants.PIN23].Pin, PinMode.Output);
                var openPin24 = _gpioController.OpenPin(_pins[RaspBerryPiContants.PIN24].Pin, PinMode.Output);

                _gpioController.Write(_pins[RaspBerryPiContants.PIN23].Pin, PinValue.High);
                _gpioController.Write(_pins[RaspBerryPiContants.PIN24].Pin, PinValue.High);

                _pins[RaspBerryPiContants.PIN23].Status = openPin23.Read() == 1 ? "On" : "Off";
                _pins[RaspBerryPiContants.PIN24].Status = openPin24.Read() == 1 ? "On" : "Off";

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
                var openPin23 = _gpioController.OpenPin(_pins[RaspBerryPiContants.PIN23].Pin, PinMode.Output);
                var openPin24 = _gpioController.OpenPin(_pins[RaspBerryPiContants.PIN24].Pin, PinMode.Output);

                _gpioController.Write(_pins[RaspBerryPiContants.PIN23].Pin, PinValue.Low);
                _gpioController.Write(_pins[RaspBerryPiContants.PIN24].Pin, PinValue.Low);

                _pins[RaspBerryPiContants.PIN23].Status = openPin23.Read() == 1 ? "On" : "Off";
                _pins[RaspBerryPiContants.PIN24].Status = openPin24.Read() == 1 ? "On" : "Off";

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
using System.Device.Gpio;
using Microsoft.AspNetCore.Mvc;

namespace raspapi.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiI2CController : ControllerBase
    {
        private readonly ILogger<RaspberryPiI2CController> _logger;
        private readonly GpioController _gpioController;

        public RaspberryPiI2CController(ILogger<RaspberryPiI2CController> logger, GpioController gpioController)
        {
            _logger = logger;
            _gpioController = gpioController;
        }

    }

}
using System.Device.Gpio;
using Microsoft.AspNetCore.Mvc;

namespace raspapi.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiI2CController(ILogger<RaspberryPiI2CController> logger, GpioController gpioController) : ControllerBase
    {
        private readonly ILogger<RaspberryPiI2CController> _logger = logger;
        private readonly GpioController _gpioController = gpioController;

    }

}
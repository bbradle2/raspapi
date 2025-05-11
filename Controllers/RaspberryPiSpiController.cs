using System.Device.Gpio;
using Microsoft.AspNetCore.Mvc;

namespace raspapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiSpiController : ControllerBase
    {
        private readonly ILogger<RaspberryPiSpiController> _logger;
        private readonly GpioController _gpioController;

        public RaspberryPiSpiController(ILogger<RaspberryPiSpiController> logger, GpioController gpioController)
        {
            _logger = logger;
            _gpioController = gpioController;
        }


    }


}
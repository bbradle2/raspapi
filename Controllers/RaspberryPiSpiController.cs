using System.Device.Gpio;
using Microsoft.AspNetCore.Mvc;

namespace raspapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiSpiController(ILogger<RaspberryPiSpiController> logger, GpioController gpioController) : ControllerBase
    {
        private readonly ILogger<RaspberryPiSpiController> _logger = logger;
        private readonly GpioController _gpioController = gpioController;

    }


}
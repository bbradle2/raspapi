using System.Device.Gpio;
using Microsoft.AspNetCore.Mvc;
using raspapi.Constants;

namespace raspapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiSpiController(ILogger<RaspberryPiSpiController> logger,
                                          [FromKeyedServices(SystemConstants.gpioControllerName)] GpioController gpioController) : ControllerBase
    {
        private readonly ILogger<RaspberryPiSpiController> _logger = logger;
        private readonly GpioController _gpioController = gpioController;
    }
}
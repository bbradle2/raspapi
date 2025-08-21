using System.Device.Gpio;
using Microsoft.AspNetCore.Mvc;
using raspapi.Constants;

namespace raspapi.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiI2CController(ILogger<RaspberryPiI2CController> logger,
                                          [FromKeyedServices(MiscConstants.gpioControllerName)] GpioController gpioController) : ControllerBase
    {
        private readonly ILogger<RaspberryPiI2CController> _logger = logger;
        private readonly GpioController _gpioController = gpioController;

    }

}
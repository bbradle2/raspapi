namespace raspapi.Controllers
{
    using System.Device.Gpio;
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

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
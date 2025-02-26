namespace raspapi.Controllers
{
    using System.Device.Gpio;
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

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
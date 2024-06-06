namespace raspapi.Controllers
{
    using System.Device.Gpio;
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using raspapi.DataObjects;

    [ApiController]
    [Route("[controller]")]
    public class RaspberryPiSpiController : ControllerBase
    {
        private readonly ILogger<RaspberryPiSpiController> _logger;
        private readonly GpioController _gpioController;
        private readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);

        public RaspberryPiSpiController(ILogger<RaspberryPiSpiController> logger, GpioController gpioController)
        {
            _logger = logger;
            _gpioController = gpioController;
        }

      
    }

    
}
namespace raspapi.Controllers
{
    using System.Device.Gpio;
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using raspapi.Interfaces;
    using raspapi.Constants.RaspberryPIConstants;
    using System.ComponentModel.DataAnnotations;
    using Nextended.Core.Extensions;
    using Nextended.Core.Helper;
    using raspapi.Utils;
    using raspapi.Extensions;

    [Route("[controller]")]
    [ApiController]
    public class RaspberryPiGpioController : ControllerBase
    {
        private readonly ILogger<RaspberryPiGpioController> _logger;
        private readonly GpioController _gpioController;
        private readonly IDictionary<int, IGpioPin> _ledpins;
        private readonly SemaphoreSlim _semaphoreGpioController;

        public RaspberryPiGpioController(ILogger<RaspberryPiGpioController> logger, GpioController gpioController, Dictionary<int, IGpioPin> ledpins, [FromKeyedServices(MiscConstants.gpioSemaphoreName)] SemaphoreSlim semaphoreGpioController)
        {
            _logger = logger;
            _gpioController = gpioController;
            _ledpins = ledpins;
            _semaphoreGpioController = semaphoreGpioController;

        }

        
        [HttpPut("SetLedOn")]
        public async Task<IActionResult?> SetLedOn()
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_semaphoreGpioController);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_ledpins);
                ArgumentNullException.ThrowIfNull(_logger);

                await _semaphoreGpioController.WaitAsync();

                _gpioController.OpenGpioPinOutput([.. _ledpins.Values]);
                _gpioController.GpioPinWriteHighValue([.. _ledpins.Values]);

                return Ok(_ledpins);

            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(e.Message);
            }
            finally
            {

                //_gpioController.CloseGpioPins([.. _pins.Values]);

                _semaphoreGpioController.Release();
            }
        }

        [HttpPut("BlinkLeds")]
        public async Task<IActionResult?> BlinkLeds()
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_semaphoreGpioController);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_ledpins);
                ArgumentNullException.ThrowIfNull(_logger);

                await _semaphoreGpioController.WaitAsync();

                _gpioController.GpioPinWriteLowValue([.. _ledpins.Values]);
                await Task.Delay(500);

                _gpioController.GpioPinWriteHighValue([.. _ledpins.Values]);
                await Task.Delay(500);

                _gpioController.GpioPinWriteLowValue([.. _ledpins.Values]);
                await Task.Delay(500);

                _gpioController.GpioPinWriteHighValue([.. _ledpins.Values]);                             
                await Task.Delay(500);
                
                return Ok(_ledpins);

            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(e.Message);
            }
            finally
            {
                //_gpioController.CloseGpioPins([.. _pins.Values]);
                _semaphoreGpioController.Release();
            }
        }

        [HttpPut("SetLedOff")]
        public async Task<IActionResult?> SetLedOff()
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_semaphoreGpioController);
                ArgumentNullException.ThrowIfNull(_gpioController);
                ArgumentNullException.ThrowIfNull(_ledpins);
                ArgumentNullException.ThrowIfNull(_logger);

                await _semaphoreGpioController.WaitAsync();

                _gpioController.OpenGpioPinOutput([.. _ledpins.Values]);
                _gpioController.GpioPinWriteLowValue([.. _ledpins.Values]);
             
                return Ok(_ledpins);
            }
            catch (Exception e)
            {
                _logger.LogCritical("{Message}", e.Message);
                return BadRequest(e.Message);
            }
            finally
            {
                //_gpioController.CloseGpioPins([.. _pins.Values]);
               
                _semaphoreGpioController.Release();
            }
        }
    }  
}
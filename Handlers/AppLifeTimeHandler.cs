using raspapi.Constants;
using raspapi.Interfaces;
using raspapi.Models;
using System.Collections.Concurrent;
using System.Device.Gpio;

namespace raspapi.Handlers
{

    public class AppLifeTimeHandler([FromKeyedServices(MiscConstants.gpioControllerName)] GpioController gpioController,
                              [FromKeyedServices(MiscConstants.gpioObjectsName)] ConcurrentQueue<GpioObject> gpioObjectList,
                              ILogger<AppLifeTimeHandler> logger,
                              IHostApplicationLifetime hostLifetTime) : IAppLifeTimeHandler
    {
        private readonly GpioController _gpioController = gpioController;
        private readonly ConcurrentQueue<GpioObject> _gpioObjectList = gpioObjectList;
        private readonly ILogger<AppLifeTimeHandler> _logger = logger;
        private readonly IHostApplicationLifetime _hostLifetTime = hostLifetTime;

        public void Run()
        {
            _ = _hostLifetTime!.ApplicationStopping.Register(() =>
           {
               _logger!.LogInformation("Waiting for client(s) to Disconnect");
           });

            _ = _hostLifetTime.ApplicationStopped.Register(() =>
            {
                try
                {
                    _logger!.LogInformation("Checking for Open Gpio's....");

                    if (_gpioObjectList == null)
                    {
                        return;
                    }

                    foreach (var gpioObject in _gpioObjectList!.DistinctBy(s => s.GpioNumber))
                    {
                        if (_gpioController!.IsPinOpen(gpioObject.GpioNumber))
                        {
                            _logger!.LogInformation("Closing Gpio {gpioObject.GpioNumber}", gpioObject.GpioNumber);
                            _gpioController.Write(gpioObject.GpioNumber, PinValue.Low);
                            _gpioController.ClosePin(gpioObject.GpioNumber);
                        }
                        else
                        {
                            _logger!.LogInformation("Gpio {gpioObject.GpioNumber} not Open", gpioObject.GpioNumber);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger!.LogError("Exception:{Message}", ex.Message);
                }
                finally
                {
                    _gpioController!.Dispose();
                }
            });
        }
    }
}
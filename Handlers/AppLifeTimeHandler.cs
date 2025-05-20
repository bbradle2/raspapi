using raspapi.Constants;
using raspapi.Interfaces;
using raspapi.Models;
using System.Device.Gpio;

namespace raspapi.Handlers
{

    public class AppLifeTimeHandler : IAppLifeTimeHandler
    {
        private readonly GpioController _gpioController;
        private readonly IList<GpioObject> _gpioObjectList;
        private readonly IGpioObjectsWaitEventHandler _gpioObjectsWaitEventHandler;
        private readonly IBinarySemaphoreSlimHandler _binarySemphoreSlimHandler;
        private readonly ILogger<AppLifeTimeHandler> _logger;
        private readonly IHostApplicationLifetime _hostLifetTime;

        public AppLifeTimeHandler([FromKeyedServices(MiscConstants.gpioControllerName)] GpioController gpioController,
                                  [FromKeyedServices(MiscConstants.gpioObjectsName)] IList<GpioObject> gpioObjectList,
                                  [FromKeyedServices(MiscConstants.gpioObjectsWaitEventName)] IGpioObjectsWaitEventHandler gpioObjectsWaitEventHandler,
                                  [FromKeyedServices(MiscConstants.gpioSemaphoreName)] IBinarySemaphoreSlimHandler binarySemaphoreSlimHandler,
                                  ILogger<AppLifeTimeHandler> logger,
                                  IHostApplicationLifetime hostLifetTime)
        {
            _gpioController = gpioController;
            _gpioObjectList = gpioObjectList;
            _gpioObjectsWaitEventHandler = gpioObjectsWaitEventHandler;
            _binarySemphoreSlimHandler = binarySemaphoreSlimHandler;
            _logger = logger;
            _hostLifetTime = hostLifetTime;
        }

        public void Handle()
        {
            _ = _hostLifetTime!.ApplicationStopping.Register(() =>
           {
               _logger!.LogInformation("Shutting down Gpio Wait Event Handler.");
               _logger!.LogInformation("Please wait.");
               _gpioObjectsWaitEventHandler!.Set();
               _logger!.LogInformation("Gpio Wait Event Handler shut down complete");
           });

            _ = _hostLifetTime.ApplicationStopped.Register(async () =>
            {
                try
                {
                    await _binarySemphoreSlimHandler!.WaitAsync();
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
                    _binarySemphoreSlimHandler!.Release();
                }
            });
        }
    }
}
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
        private readonly IAppShutdownWaitEventHandler _appShutdownWaitEventHandler;
        private readonly IBinarySemaphoreSlimHandler _binarySemphoreSlimHandler;
        private readonly IServiceProvider _services;
        private readonly ILogger<AppLifeTimeHandler> _logger;

        public AppLifeTimeHandler([FromKeyedServices(MiscConstants.gpioControllerName)] GpioController gpioController,
                                  [FromKeyedServices(MiscConstants.gpioObjectsName)] IList<GpioObject> gpioObjectList,
                                  [FromKeyedServices(MiscConstants.gpioObjectsWaitEventName)] IGpioObjectsWaitEventHandler gpioObjectsWaitEventHandler,
                                  [FromKeyedServices(MiscConstants.appShutdownWaitEventName)] IAppShutdownWaitEventHandler appShutdownWaitEventHandler,
                                  [FromKeyedServices(MiscConstants.gpioSemaphoreName)] IBinarySemaphoreSlimHandler binarySemaphoreSlimHandler,
                                  IServiceProvider services,
                                  ILogger<AppLifeTimeHandler> logger)
        {
            _gpioController = gpioController;
            _gpioObjectList = gpioObjectList;
            _gpioObjectsWaitEventHandler = gpioObjectsWaitEventHandler;
            _services = services;
            _appShutdownWaitEventHandler = appShutdownWaitEventHandler;
            _binarySemphoreSlimHandler = binarySemaphoreSlimHandler;
            _logger = logger;

        }

        public void Handle(WebApplication app)
        {
            
            _ = app.Lifetime.ApplicationStopping.Register(() =>
           {
               _logger!.LogInformation("Shutting down Gpio Wait Event Handler.");
               _logger!.LogInformation("Please wait.");
               _gpioObjectsWaitEventHandler!.Set();
               _appShutdownWaitEventHandler!.Set();
               _logger!.LogInformation("Gpio Wait Event Handler shut down complete");
           });

            _ = app.Lifetime.ApplicationStopped.Register(() =>
            {
                try
                {
                    _binarySemphoreSlimHandler!.WaitAsync().GetAwaiter();
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
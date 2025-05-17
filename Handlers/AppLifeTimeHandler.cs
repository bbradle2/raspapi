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

        public AppLifeTimeHandler([FromKeyedServices(MiscConstants.gpioControllerName)] GpioController gpioController,
                                  [FromKeyedServices(MiscConstants.gpioObjectsName)] IList<GpioObject> gpioObjectList,
                                  [FromKeyedServices(MiscConstants.gpioObjectsWaitEventName)] IGpioObjectsWaitEventHandler gpioObjectsWaitEventHandler,
                                  [FromKeyedServices(MiscConstants.appShutdownWaitEventName)] IAppShutdownWaitEventHandler appShutdownWaitEventHandler,
                                  [FromKeyedServices(MiscConstants.gpioSemaphoreName)] IBinarySemaphoreSlimHandler binarySemaphoreSlimHandler,
                                  IServiceProvider services)
        {
            _gpioController = gpioController;
            _gpioObjectList = gpioObjectList;
            _gpioObjectsWaitEventHandler = gpioObjectsWaitEventHandler;
            _services = services;
            _appShutdownWaitEventHandler = appShutdownWaitEventHandler;
            _binarySemphoreSlimHandler = binarySemaphoreSlimHandler;

        }

        public void Handle(WebApplication app)
        {
            var logger = _services.GetService<ILogger<AppLifeTimeHandler>>();

            _ = app.Lifetime.ApplicationStopping.Register(() =>
           {
               logger!.LogInformation("Shutting down Gpio Wait Event Handler.");
               logger!.LogInformation("Please wait.");
               _gpioObjectsWaitEventHandler!.Set();
               _appShutdownWaitEventHandler!.Set();
               logger!.LogInformation("Gpio Wait Event Handler shut down complete");
           });

            _ = app.Lifetime.ApplicationStopped.Register(() =>
            {
                try
                {
                    _binarySemphoreSlimHandler!.WaitAsync().GetAwaiter();
                    logger!.LogInformation("Checking for Open Gpio's....");

                    if (_gpioObjectList == null)
                    {
                        return;
                    }

                    foreach (var gpioObject in _gpioObjectList!.DistinctBy(s => s.GpioNumber))
                    {
                        if (_gpioController!.IsPinOpen(gpioObject.GpioNumber))
                        {
                            logger!.LogInformation("Closing Gpio {gpioObject.GpioNumber}", gpioObject.GpioNumber);
                            _gpioController.Write(gpioObject.GpioNumber, PinValue.Low);
                            _gpioController.ClosePin(gpioObject.GpioNumber);
                        }
                        else
                        {
                            logger!.LogInformation("Gpio {gpioObject.GpioNumber} not Open", gpioObject.GpioNumber);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger!.LogError("Exception:{Message}", ex.Message);
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
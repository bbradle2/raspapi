using System.Device.Gpio;
using raspapi.Constants;
using raspapi.Utils;
using raspapi.Models;
using raspapi.Intercepts;
using raspapi.Interfaces;
using raspapi.Handlers;

namespace raspapi
{
    class Program
    {
        private static ILogger<Program>? _logger;
        public static void OnSigInt(object? sender, ConsoleCancelEventArgs e)
        {
            _logger!.LogInformation("SIGINT Received...");
        }

        public static void OnSigTerm(object? sender, EventArgs e)
        {
            _logger!.LogInformation("SIGTERM Received...");
        }
        
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += OnSigTerm;
            Console.CancelKeyPress += OnSigInt;

            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.AddConsole();

            builder.Services.AddKeyedSingleton<GpioController>(MiscConstants.gpioControllerName);
            builder.Services.AddKeyedSingleton<IBinarySemaphoreSlimHandler, BinarySemaphoreSlimHandler>(MiscConstants.gpioSemaphoreName);
            builder.Services.AddKeyedSingleton<IList<GpioObject>,List<GpioObject>>(MiscConstants.gpioObjectsName);
            builder.Services.AddKeyedSingleton<IGpioObjectsWaitEventHandler,GpioObjectsWaitEventHandler>(MiscConstants.gpioObjectsWaitEventName);
            builder.Services.AddKeyedSingleton<IAppShutdownWaitEventHandler,AppShutdownWaitEventHandler>(MiscConstants.appShutdownWaitEventName);
            builder.Services.AddKeyedSingleton<IWebSocketHandler, WebSocketHandler>(MiscConstants.webSocketHandlerName);

            builder.Services.AddControllers();

            var app = builder.Build();
            app.UseMiddleware<ApiIntercept>();
            app.UseRouting();          
            app.UseWebSockets();
             
            var gpioController = app.Services.GetKeyedService<GpioController>(MiscConstants.gpioControllerName);
            var gpioSemaphore = app.Services.GetKeyedService<IBinarySemaphoreSlimHandler>(MiscConstants.gpioSemaphoreName);
            var gpioObjectList = app.Services.GetKeyedService<IList<GpioObject>>(MiscConstants.gpioObjectsName);
            var gpioObjectsWaitEventHandler = app.Services.GetKeyedService<IGpioObjectsWaitEventHandler>(MiscConstants.gpioObjectsWaitEventName);
            var appShutdownWaitEventHandler = app.Services.GetKeyedService<IAppShutdownWaitEventHandler>(MiscConstants.appShutdownWaitEventName);
            _logger = app.Services.GetRequiredService<ILogger<Program>>();

            app.MapControllers();
          
            CommandLineTask.RunCommandLineTask(app);

            _ = app.Lifetime.ApplicationStopping.Register(() =>
            {
                _logger!.LogInformation("Shutting down Gpio Wait Event Handler.");
                _logger!.LogInformation("Please wait.");
                gpioObjectsWaitEventHandler!.Set();
                appShutdownWaitEventHandler!.Set();
                _logger!.LogInformation("Gpio Wait Event Handler shut down complete");
            });


            _ = app.Lifetime.ApplicationStopped.Register(() =>
            {
                try
                {
                    gpioSemaphore!.WaitAsync().GetAwaiter();
                    _logger!.LogInformation("Checking for Open Gpio's....");

                    if (gpioObjectList == null)
                    {
                        return;
                    }

                    foreach (var gpioObject in gpioObjectList!.DistinctBy(s => s.GpioNumber))
                    {
                        if (gpioController!.IsPinOpen(gpioObject.GpioNumber))
                        {
                            _logger!.LogInformation("Closing Gpio {gpioObject.GpioNumber}", gpioObject.GpioNumber);
                            gpioController.Write(gpioObject.GpioNumber, PinValue.Low);
                            gpioController.ClosePin(gpioObject.GpioNumber);
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
                    gpioController!.Dispose();
                    gpioSemaphore!.Release();
                }
            });

            app.Run();
        }
    }
}

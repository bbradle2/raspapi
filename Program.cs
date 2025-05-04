using System.Device.Gpio;
using raspapi.Constants;
using System.Net.NetworkInformation;
using raspapi.Utils;
using raspapi.Models;
using System.Text.Json;
using raspapi.Intercepts;


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
            builder.Services.AddKeyedSingleton<BinarySemaphoreSlim>(MiscConstants.gpioSemaphoreName);
            builder.Services.AddKeyedSingleton<List<GpioObject>>(MiscConstants.gpioObjectsName);
            builder.Services.AddKeyedSingleton<GpioObjectsWaitEventHandler>(MiscConstants.gpioObjectsWaitEventName);
            builder.Services.AddKeyedSingleton<AppShutdownWaitEventHandler>(MiscConstants.appShutdownWaitEventName);

            builder.Services.AddControllers();

            var app = builder.Build();
            app.UseMiddleware<ApiIntercept>();
            app.UseRouting();
            app.UseWebSockets();

            _logger = app.Services.GetRequiredService<ILogger<Program>>();

            var gpioController = app.Services.GetKeyedService<GpioController>(MiscConstants.gpioControllerName);
            var gpioSemaphore = app.Services.GetKeyedService<BinarySemaphoreSlim>(MiscConstants.gpioSemaphoreName);
            var gpioObjectList = app.Services.GetKeyedService<List<GpioObject>>(MiscConstants.gpioObjectsName);
            var gpioObjectsWaitEventHandler = app.Services.GetKeyedService<GpioObjectsWaitEventHandler>(MiscConstants.gpioObjectsWaitEventName);
            var appShutdownWaitEventHandler = app.Services.GetKeyedService<AppShutdownWaitEventHandler>(MiscConstants.appShutdownWaitEventName);

            var gpioObjectConfig = File.ReadAllText("raspberrypi05_PinMapping.json");
            var gpioConfigList = JsonSerializer.Deserialize<List<GpioObject>>(gpioObjectConfig);

            foreach (var i in gpioConfigList!)
            {
                GpioObject gpioObject = i;
                gpioObject.GpioNumber = i.GpioNumber;
                gpioObject.GpioValue = null;

                gpioObjectList!.Add(gpioObject);
            }

            app.MapControllers();

            _ = app.Use(async (context, next) =>
            {

                if (context.Request.Path == "/GetGpios")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();


                        await WebSocketGpio.GetGpios(webSocket,
                                                    gpioObjectList!,
                                                    gpioObjectsWaitEventHandler!,
                                                    appShutdownWaitEventHandler!
                                                    );

                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    }
                }
                else
                {
                    await next(context);
                }

            });

            CommandLineTask.RunCommandLineTask(app,
                              gpioController!,
                              gpioObjectList!,
                              gpioObjectsWaitEventHandler!,
                              _logger);

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
                        //SendLogMessage($"Checking Gpio {gpioObject.GpioNumber}");
                        if (gpioController!.IsPinOpen(gpioObject.GpioNumber))
                        {
                            //SendLogMessage($"Gpio {gpioObject.GpioNumber} Open");
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

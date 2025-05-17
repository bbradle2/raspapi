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

            _logger = app.Services.GetRequiredService<ILogger<Program>>();

            app.UseMiddleware<ApiIntercept>();
            app.UseRouting();          
            app.UseWebSockets();
            app.MapControllers();

            var commandLineTaskHandler = ActivatorUtilities.GetServiceOrCreateInstance<CommandLineTaskHandler>(app.Services);
            var appLifeTimeHandler = ActivatorUtilities.GetServiceOrCreateInstance<AppLifeTimeHandler>(app.Services);

            commandLineTaskHandler!.Handle(app);
            appLifeTimeHandler!.Handle(app);

            app.Run();
        }
    }
}

using System.Device.Gpio;
using raspapi.Constants;
using raspapi.Models;
using raspapi.Intercepts;
using raspapi.Interfaces;
using raspapi.Handlers;
using System.Collections.Concurrent;

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

        static async Task Main(string[] args)
        {
            
            AppDomain.CurrentDomain.ProcessExit += OnSigTerm;
            Console.CancelKeyPress += OnSigInt;

            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.AddConsole();

            builder.Services.AddKeyedSingleton<GpioController>(MiscConstants.gpioControllerName);
            builder.Services.AddKeyedSingleton<ConcurrentQueue<GpioObject>>(MiscConstants.gpioObjectsName);
            builder.Services.AddKeyedSingleton<IAppLifeTimeHandler, AppLifeTimeHandler>(MiscConstants.appLifeTimeHandlerName);
            builder.Services.AddKeyedSingleton<ICommandLineTaskHandler, CommandLineTaskHandler>(MiscConstants.commandLineTaskHandlerName);
            builder.Services.AddKeyedSingleton<IBinarySemaphoreSlimHandler, BinarySemaphoreSlimHandler>(MiscConstants.binarySemaphoreSlimHandler);
            builder.Services.AddControllers();

            var app = builder.Build();

            _logger = app.Services.GetRequiredService<ILogger<Program>>();

            app.UseMiddleware<ApiIntercept>();
            app.UseRouting();
            app.MapControllers();

            var appLifeTimeHandler = app.Services.GetKeyedService<IAppLifeTimeHandler>(MiscConstants.appLifeTimeHandlerName);
            appLifeTimeHandler!.Run();

            if (app.Environment.IsDevelopment())
            {
                var commandLineTaskHandler = app.Services.GetKeyedService<ICommandLineTaskHandler>(MiscConstants.commandLineTaskHandlerName);
                commandLineTaskHandler!.Run();
            }

            await app.RunAsync();
        }
    }
}

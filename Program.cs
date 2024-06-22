using System.Device.Gpio;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using raspapi.Contants;
using raspapi.DataObjects;
using raspapi.Interfaces;

namespace raspapi
{
    
    class Program
    {

        public static ILogger<Program> ?logger;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += OnSigTerm;
            Console.CancelKeyPress += OnSigInt;

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Logging.AddConsole();

            builder.Services.AddSingleton<GpioPin23>();
            builder.Services.AddSingleton<GpioPin24>();
            builder.Services.AddSingleton<GpioController>();
            builder.Services.AddSingleton<Dictionary<int, IGpioPin>>();
            

            builder.Services.AddControllers();

            var app = builder.Build();

            var gpioController = app.Services.GetRequiredService<GpioController>();
            
            var pins = app.Services.GetRequiredService<Dictionary<int, IGpioPin>>(); 
            pins.Add(RaspBerryPiContants.PIN23 , app.Services.GetRequiredService<GpioPin23>());
            pins.Add(RaspBerryPiContants.PIN24, app.Services.GetRequiredService<GpioPin24>());

            logger = app.Services.GetRequiredService<ILogger<Program>>();
            
            app.UseSwagger();
            app.UseSwaggerUI();
            //app.UseMiddleware<ApiIntercept>();

            // if (app.Environment.IsDevelopment())
            // {

            // }

            logger.LogInformation("ASPNETCORE_ENVIRONMENT:{app.Environment.EnvironmentName}", app.Environment.EnvironmentName);

            app.MapControllers();

            //AssemblyName[] names = Assembly.GetExecutingAssembly().GetReferencedAssemblies();

            _ = app.Lifetime.ApplicationStopped.Register(() =>
            {
                
                SendMessageToTerminal("Checking for Open Pins....");

                foreach (var pin in pins)
                {
                    if (gpioController.IsPinOpen(pin.Value.Pin))
                    {
                        SendMessageToTerminal($"Turning Off and Closing Pin {pin.Key}");
                        gpioController.Write(pin.Value.Pin, PinValue.Low);
                        gpioController.ClosePin(pin.Value.Pin);
                    }
                }
            });

            app.Run();
        }

        private static void SendMessageToTerminal(string message)
        {
            
            logger?.LogInformation("{message}", message);
        }

        private static void OnSigInt(object? sender, ConsoleCancelEventArgs e)
        {
            SendMessageToTerminal("SIGINT Received...");
        }

        private static void OnSigTerm(object? sender, EventArgs e)
        {

            SendMessageToTerminal("SIGTERM Received...");
        }

    }
}

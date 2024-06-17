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

       

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += OnSigTerm;
            Console.CancelKeyPress += OnSigInt;          

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Logging.AddConsole();
            builder.Services.AddControllers();        

            builder.Services.AddSingleton<GpioController>();
            builder.Services.AddSingleton<Dictionary<int, IPin>>();


            var app = builder.Build();

            var gpioController = app.Services.GetRequiredService<GpioController>();


            var pins = app.Services.GetRequiredService<Dictionary<int, IPin>>();
            pins.Add(RaspBerryPiContants.PIN23 , new Pin23 { Status = "Off" });


            var logger = app.Logger;
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseMiddleware<ApiIntercept>();

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
                    if (gpioController.IsPinOpen(pin.Key))
                    {
                        Console.WriteLine($"Turning Off and Closing Pin {pin.Key}");
                        gpioController.Write(pin.Key, PinValue.Low);
                        gpioController.ClosePin(pin.Key);
                    }
                }
            });

            app.Run();
        }

        private static void SendMessageToTerminal(string message)
        {
            Console.WriteLine("\r\n" + message);
        }

        private static void OnSigInt(object? sender, ConsoleCancelEventArgs e)
        {

            SendMessageToTerminal("SIGINT Received...");
        }

        private static void OnSigTerm(object? sender, EventArgs e)
        {

            SendMessageToTerminal("SIGTERM Received...");
        }

        private void OnShutdown()
        {

            //Wait while the data is flushed
            System.Threading.Thread.Sleep(1000);
        }

    }
}

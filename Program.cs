using System.Device.Gpio;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using raspapi.Controllers;

namespace raspapi
{
    
    class Program
    {

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

            var app = builder.Build();
            var logger = app.Logger;
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseMiddleware<ApiIntercept>();
          
            // if (app.Environment.IsDevelopment())
            // {

            // }

            logger.LogInformation("ASPNETCORE_ENVIRONMENT:{app.Environment.EnvironmentName}", app.Environment.EnvironmentName);

            app.MapControllers();


            AssemblyName[] names = Assembly.GetExecutingAssembly().GetReferencedAssemblies();

            app.Lifetime.ApplicationStopped.Register(() => 
            {
                var gpioController = app.Services.GetRequiredService<GpioController>();
                SendMessageToTerminal("Checking for Open Pins....");

                if (gpioController.IsPinOpen(23))
                {
                    Console.WriteLine($"Turning Off and Closing Pin {23}");
                    gpioController.Write(23, PinValue.Low);
                    gpioController.ClosePin(23);
                }
            });

            app.Run();
            

        }

        private void OnShutdown()
        {

            //Wait while the data is flushed
            System.Threading.Thread.Sleep(1000);
        }

    }
}

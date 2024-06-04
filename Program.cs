using System.Device.Gpio;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            
            builder.Logging.AddConsole();
            builder.Services.AddControllers();
                     

            builder.Services.AddSingleton<GpioController>();

            //builder.Services.AddSingleton<IRaspberryPiGpioController, RaspberryPiGpioController>();

            var app = builder.Build();
            var logger = app.Logger;
            app.UseMiddleware<ApiIntercept>();

            // if (app.Environment.IsDevelopment())
            // {
                
            // }

            logger.LogInformation("ASPNETCORE_ENVIRONMENT:{app.Environment.EnvironmentName}", app.Environment.EnvironmentName);

            //app.Logger.LogInformation($"BRIAN_TEST:{app.Configuration["BRIAN_TEST"]}");
            //app.Logger.LogInformation($"AllowedHosts:{app.Configuration["AllowedHosts"]}");

            // var piController = app.Services.GetService<IRaspberryPiGpioController>();
            // ArgumentNullException.ThrowIfNull(piController);
            // piController.StartGpio(app);
            app.MapControllers();

            //AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
            AssemblyName[] names = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
           
            app.Run();

        }

        // private static void OnAssemblyLoad(object? sender, AssemblyLoadEventArgs args)
        // {
        //     if(args.LoadedAssembly.FullName.Contains("GitVersion"))
        //         Console.WriteLine(args.LoadedAssembly.FullName);
        // }
    }
}

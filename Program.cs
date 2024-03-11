using System.Device.Gpio;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;


namespace first_test
{
    using Interfaces;
    using Controllers;
    using Microsoft.AspNetCore.Hosting;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Microsoft.AspNetCore.Http.Json;

    class Program
    {

        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<GpioController>();
            builder.Services.AddSingleton<IRaspberryPiInfoController, RaspberryPiInfoController>();
            builder.Services.AddSingleton<IRaspberryPiGpioController, RaspberryPiGpioController>();

            var app = builder.Build();
                                   
            app.UseSwagger();
            app.UseSwaggerUI();

            Console.WriteLine($"ASPNETCORE_ENVIRONMENT:{app.Environment.EnvironmentName}");

            var piInfoController = app.Services.GetService<IRaspberryPiInfoController>();
            ArgumentNullException.ThrowIfNull(piInfoController);
            piInfoController.StartInfo(app);

            var piController = app.Services.GetService<IRaspberryPiGpioController>();
            ArgumentNullException.ThrowIfNull(piController);
            piController.StartGpio(app);

            app.Run();
        }
    }
}

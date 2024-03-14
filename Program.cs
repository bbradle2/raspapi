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
    using Microsoft.Extensions.Hosting;

    class Program
    {

        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        
            builder.Services.AddSingleton<GpioController>();
           //builder.Services.AddSingleton<IRaspberryPiGpioController, RaspberryPiGpioController>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            Console.WriteLine($"ASPNETCORE_ENVIRONMENT:{app.Environment.EnvironmentName}");
            Console.WriteLine($"BRIAN_TEST:{app.Configuration["BRIAN_TEST"]}");
            Console.WriteLine($"AllowedHosts:{app.Configuration["AllowedHosts"]}");

            // var piController = app.Services.GetService<IRaspberryPiGpioController>();
            // ArgumentNullException.ThrowIfNull(piController);
            // piController.StartGpio(app);
            app.MapControllers();
            app.Run();
        }
    }
}

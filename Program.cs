using System.Device.Gpio;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;


namespace raspapi
{
    using Interfaces;
    using Controllers;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    class Program
    {

        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.AddConsole();
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //builder.Services.AddSingleton<GpioController>();
            //builder.Services.AddSingleton<IRaspberryPiGpioController, RaspberryPiGpioController>();

            var app = builder.Build();

            

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.Logger.LogInformation($"ASPNETCORE_ENVIRONMENT:{app.Environment.EnvironmentName}");
            //app.Logger.LogInformation($"BRIAN_TEST:{app.Configuration["BRIAN_TEST"]}");
            //app.Logger.LogInformation($"AllowedHosts:{app.Configuration["AllowedHosts"]}");

            // var piController = app.Services.GetService<IRaspberryPiGpioController>();
            // ArgumentNullException.ThrowIfNull(piController);
            // piController.StartGpio(app);
            app.MapControllers();

            app.Logger.LogInformation($"Git Version {GitVersionInformation.SemVer}.");
            app.Run();
        }
    }
}

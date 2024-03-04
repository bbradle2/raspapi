using System.Device.Gpio;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;


namespace first_test
{
    using Interfaces;
    using Controllers;

    class Program
    {
       
        static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<GpioController>();
            builder.Services.AddSingleton<IRaspberryPiController, RaspberryPiController>();

            var app = builder.Build();

            var piController = app.Services.GetService<IRaspberryPiController>();
            ArgumentNullException.ThrowIfNull(piController);
            piController.Start(app);
            
            app.Run();

        }
    }
}

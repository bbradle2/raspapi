using System.Device.Gpio;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.Configuration;

namespace raspapi.Controllers
{

    using Interfaces;

    public class RaspberryPiGpioController: IRaspberryPiGpioController
    {
        private static readonly int pin = 23;
        public void StartGpio(WebApplication app)  
        {
       
            app.MapGet("/RaspberryPiGpio/ledstatus", (GpioController gpioController, IConfiguration config) =>
            {
                ArgumentNullException.ThrowIfNull(gpioController);

                var openPin = gpioController.OpenPin(pin, PinMode.Output);
                
                try
                {
                    return Results.Json(openPin.Read().ToString());
                }
                finally
                {
                  
                }
            });

            app.MapPut("/RaspberryPiGpio/ledon", (GpioController gpioController) =>
            {
                ArgumentNullException.ThrowIfNull(gpioController);

                var openPin = gpioController.OpenPin(pin, PinMode.Output);

                try
                {
                    gpioController.Write(pin, PinValue.High);
                    var pinValue = openPin.Read();
                    return Results.Json(pinValue.ToString());
                }
                finally
                {

                }
            });

            app.MapPut("/RaspberryPiGpio/ledoff", (HttpContext context, GpioController gpioController) =>
            {

                ArgumentNullException.ThrowIfNull(gpioController);

                var openPin = gpioController.OpenPin(pin, PinMode.Output);

                try
                {
                    gpioController.Write(pin, PinValue.Low);
                    var pinValue = openPin.Read();
                    return Results.Json(pinValue.ToString());
                }
                finally
                {

                }
            });


        }
    }

    
}

using System.Device.Gpio;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace raspapi.Controllers
{
    using Interfaces;
    using Microsoft.Extensions.Configuration;

    public class RaspberryPiGpioController: IRaspberryPiGpioController
    {
        private static readonly int pin = 23;
        public void StartGpio(WebApplication app)  
        {
       
            app.MapGet("/RaspberryPiGpioController/ledstatus", (GpioController gpioController, IConfiguration config) =>
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

            app.MapPut("/RaspberryPiGpioController/ledon", (GpioController gpioController) =>
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

            app.MapPut("/RaspberryPiGpioController/ledoff", (HttpContext context, GpioController gpioController) =>
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
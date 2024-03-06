
using System.Device.Gpio;
using System.Text;
using first_test.DataObjects;
using first_test.Interfaces;
using first_test.LinuxExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace first_test.Controllers
{
    public class RaspberryPiGpioController: IRaspberryPiGpioController
    {
        private static readonly int pin = 23;
        public void StartGpio(WebApplication app)  
        {
       
            app.MapGet("/RaspberryPiGpioController/ledstatus", (GpioController gpioController) =>
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

            app.MapGet("/RaspberryPiGpioController/ledon", (GpioController gpioController) =>
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

            app.MapGet("/RaspberryPiGpioController/ledoff", (HttpContext context, GpioController gpioController) =>
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
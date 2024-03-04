
using System.Device.Gpio;
using System.Text;
using first_test.DataObjects;
using first_test.Interfaces;
using first_test.LinuxExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace first_test.Controllers
{
    public class RaspberryPiController: IRaspberryPiController
    {
        private static readonly int pin = 23;
        public void Start(WebApplication app)  
        {
           app.MapGet("/cpuinfo", async (HttpContext context) =>
           {
                StringBuilder? retRes = new StringBuilder();

                try
                {
                    var commandRes = await Task.Run(() => "cat /proc/cpuinfo".Execute());

                    foreach (var c in commandRes)
                    {
                        if (c == '\n' || c == '\t') continue;

                        retRes.Append(c);
                    }

                    CPUObject cpuObject = new CPUObject { Call = "CPUInfo", Content = retRes.ToString() };

                    return Results.Ok(cpuObject);
                }
                finally
                {
                    retRes = null;
                }
            });

            app.MapGet("/ledstatus", (GpioController gpioController) =>
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

            app.MapGet("/ledon", (GpioController gpioController) =>
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

            app.MapGet("/ledoff", (HttpContext context, GpioController gpioController) =>
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
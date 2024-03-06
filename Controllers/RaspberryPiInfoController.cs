
using System.Device.Gpio;
using System.Text;
using first_test.DataObjects;
using first_test.Interfaces;
using first_test.LinuxExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace first_test.Controllers
{
    public class RaspberryPiInfoController : IRaspberryPiInfoController
    {
        public void StartInfo(WebApplication app)  
        {
           app.MapGet("/RaspberryPiInfoController/cpuinfo", async (HttpContext context) =>
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


            app.MapGet("/RaspberryPiInfoController/memoryinfo", async (HttpContext context) =>
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

                   MemoryObject cpuObject = new MemoryObject { Call = "MemoryInfo", Content = retRes.ToString() };

                   return Results.Ok(cpuObject);
               }
               finally
               {
                   retRes = null;
               }
           });





        }
    }

    
}
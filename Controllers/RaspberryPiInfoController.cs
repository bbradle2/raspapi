
using System.Data.Common;
using System.Device.Gpio;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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
            const string controllerName = "RaspberryPiInfoController";
            const string memoryInfo = "memoryinfo";
            const string cpuInfo = "cpuinfo";
            const string systemInfo = "systeminfo";

            _ = app.MapGet($"/{controllerName}/{systemInfo}", async (HttpContext context) =>
            {
                
                try
                {
                    string result = "sudo lshw -class system -json".Execute();
                    byte[] byteArray = Encoding.UTF8.GetBytes(result);
                    MemoryStream stream = new(byteArray);
                    var node = await JsonNode.ParseAsync(stream);

                    //var array = node!.AsArray();
                    //var id = array[0]!["id"];
                    //var claimed = array[0]!["claimed"];
                    //var cls = array[0]!["class"];

                    return TypedResults.Ok(node);
                }
                finally
                {
                }
            });

            _ = app.MapGet($"/{controllerName}/{cpuInfo}", async (HttpContext context) =>
            {
                try
                {
                    var result = "sudo lshw -class cpu -json".Execute();
                    byte[] byteArray = Encoding.UTF8.GetBytes(result);
                    MemoryStream stream = new MemoryStream(byteArray);
                    var node = await JsonNode.ParseAsync(stream);
                    return TypedResults.Json(node);
                } 
                finally
                {

                }

            });

            _ = app.MapGet($"/{controllerName}/{memoryInfo}", async (HttpContext context) =>
            {
                try
                {
                    var meminfoLines = await File.ReadAllLinesAsync("/proc/meminfo");
                    var memTotal = meminfoLines.SingleOrDefault(item => item.Contains("MemTotal:"));
                    var memFree = meminfoLines.SingleOrDefault(item => item.Contains("MemFree:"));
                    var memAvailable = meminfoLines.SingleOrDefault(item => item.Contains("MemAvailable:"));

                    MemoryObject memInfoObject = new()
                    {
                        Call = memoryInfo,
                        Description = "RaspBerry Pi 5 Memory Statistics",
                        MemTotal = memTotal!.Split(":")[1].Trim(),
                        MemFree = memFree!.Split(":")[1].Trim(),
                        MemAvailable = memAvailable!.Split(":")[1].Trim()
                    };

                    return TypedResults.Json<MemoryObject>(memInfoObject);
                }
                finally
                {

                }
            });
        }
    }   
}
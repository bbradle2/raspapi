using System.Text;
using System.Text.Json.Nodes;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace first_test.Controllers
{
    using System.Runtime.CompilerServices;
    using System.Text.Json;
    using DataObjects;
    using Interfaces;
    using LinuxExtensions;
    using Microsoft.AspNetCore.Http.Features;

    public class RaspberryPiInfoController : IRaspberryPiInfoController
    {
        private readonly JsonSerializerOptions options = new(JsonSerializerDefaults.Web);
        public void StartInfo(WebApplication app)  
        {
            const string controllerName = "RaspberryPiInfoController";
            const string memoryInfo = "memoryinfo";
            const string cpuInfo = "cpuinfo";
            const string systemInfo = "systeminfo";
            
            _ = app.MapGet($"/{controllerName}/{systemInfo}", async (HttpContext context) =>
            {
                //byte[] byteArray = Encoding.UTF8.GetBytes(result);
                //MemoryStream stream = new(byteArray);
                //var node = await JsonNode.ParseAsync(stream);


                //var array = node!.AsArray();
                //var id = array[0]!["id"];
                //var claimed = array[0]!["claimed"];
                //var cls = array[0]!["class"];

                try
                {
                    string result = "sudo lshw -class system -json".Execute();
                    byte[] byteArray = Encoding.UTF8.GetBytes(result);
                    MemoryStream stream = new(byteArray);

                    SystemInfoObject systemInfoObject = new()
                    {
                        SystemObjects = await JsonSerializer.DeserializeAsync<SystemInfoObject.SystemObject[]>(stream, options)
                    };

                    return TypedResults.Json(systemInfoObject);
                }
                finally
                {
                }
            });

            _ = app.MapGet($"/{controllerName}/{cpuInfo}", async (HttpContext context) =>
            {
                try
                {                 
                    string cpuInfoResult = "sudo lshw -class cpu -json".Execute();
                    byte[] cpuInfoByteArray = Encoding.UTF8.GetBytes(cpuInfoResult);
                    MemoryStream cpuInfoStream = new(cpuInfoByteArray);
                    

                    CPUInfoObject cpuInfoObject = new()
                    {
                        CPUObjects = await JsonSerializer.DeserializeAsync<CPUInfoObject.CPUObject[]>(cpuInfoStream, options)
                    };

                    return TypedResults.Json(cpuInfoObject);
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
                    var memTotal = meminfoLines.SingleOrDefault(item => item.Contains("memtotal:", StringComparison.CurrentCultureIgnoreCase));
                    var memFree = meminfoLines.SingleOrDefault(item => item.Contains("memfree:", StringComparison.CurrentCultureIgnoreCase));
                    var memAvailable = meminfoLines.SingleOrDefault(item => item.Contains("memavailable:", StringComparison.CurrentCultureIgnoreCase));

                    var cached = meminfoLines.SingleOrDefault(item => item.Contains("cached:", StringComparison.CurrentCultureIgnoreCase) 
                                                                      && 
                                                                      !item.Contains("swapcached:", StringComparison.CurrentCultureIgnoreCase));
                    var swapCached = meminfoLines.SingleOrDefault(item => item.Contains("swapcached:", StringComparison.CurrentCultureIgnoreCase));

                    var swapFree = meminfoLines.SingleOrDefault(item => item.Contains("swapfree:", StringComparison.CurrentCultureIgnoreCase));

                    var activeAnon = meminfoLines.SingleOrDefault(item => item.Contains("active(anon):", StringComparison.CurrentCultureIgnoreCase) 
                                                                          &&
                                                                          !item.Contains("inactive(anon):", StringComparison.CurrentCultureIgnoreCase));
                    var inActiveAnon = meminfoLines.SingleOrDefault(item => item.Contains("inactive(anon):", StringComparison.CurrentCultureIgnoreCase));

                    MemoryInfoObject memInfoObject = new()
                    {
                        MemoryTotal = memTotal!.Split(":")[1].Trim(),
                        MemoryFree = memFree!.Split(":")[1].Trim(),
                        MemoryAvailable = memAvailable!.Split(":")[1].Trim(),
                        Cached = cached!.Split(":")[1].Trim(),
                        SwapCached = swapCached!.Split(":")[1].Trim(),
                        SwapFree = swapFree!.Split(":")[1].Trim(),
                    };

                    return TypedResults.Json(memInfoObject);
                }
                finally
                {

                }
            });
        }
    }   
}
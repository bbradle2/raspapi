using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace first_test.Controllers
{
    using System.Text.Json;
    using DataObjects;
    using Interfaces;
    using LinuxExtensions;

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
                try
                {
                    MemoryStream memoryStream()
                    {
                        string systemInfoResult = "sudo lshw -class system -json".ExecuteBashScript();
                        byte[] systemInfoByteArray = Encoding.UTF8.GetBytes(systemInfoResult);
                        return new(systemInfoByteArray);
                    }

                    SystemInfoObject systemInfoObject = new()
                    {
                        SystemObjects = await JsonSerializer.DeserializeAsync<SystemInfoObject.SystemObject[]>(memoryStream(), options)
                    };

                    return TypedResults.Json<SystemInfoObject>(systemInfoObject);
                }
                catch(Exception e)
                {
                    return Results.Problem(e.Message);
                }
               
            });

            _ = app.MapGet($"/{controllerName}/{cpuInfo}", async (HttpContext context) =>
            {
                try
                {
                    MemoryStream memoryStream()
                    {
                        string cpuInfoResult = "sudo lshw -class cpu -json".ExecuteBashScript();
                        byte[] cpuInfoByteArray = Encoding.UTF8.GetBytes(cpuInfoResult);
                        return new MemoryStream(cpuInfoByteArray);
                    };

                    CPUInfoObject cpuInfoObject = new()
                    {
                        CPUObjects = await JsonSerializer.DeserializeAsync<CPUInfoObject.CPUObject[]>(memoryStream(), options)
                    };

                    return TypedResults.Json<CPUInfoObject>(cpuInfoObject);
                } 
                catch(Exception e)
                {
                    return Results.Problem(e.Message);
                }

            });

            _ = app.MapGet($"/{controllerName}/{memoryInfo}", async (HttpContext context) =>
            {
                try
                {
                    var meminfoLines = await File.ReadAllLinesAsync("/proc/meminfo");
                    var memInfoObject = MemoryInfoObject.ParseMemoryInfo(meminfoLines);
                    return TypedResults.Json<MemoryInfoObject>(memInfoObject);
                }
                catch (Exception e)
                {
                    return Results.Problem(e.Message);
                }
            });
        }
    }   
}
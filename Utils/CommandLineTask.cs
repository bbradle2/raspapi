using raspapi.Constants;
using raspapi.Interfaces;
using raspapi.Models;
using System.Device.Gpio;
using System.Net.NetworkInformation;

namespace raspapi.Utils
{
    public class CommandLineTask
    {
        public static void RunCommandLineTask(WebApplication app)
        {
            _ = Task.Factory.StartNew(async () =>
            {
                var gpioObjects = app.Services.GetKeyedService<IList<GpioObject>>(MiscConstants.gpioObjectsName);
                var gpioObjectsWaitEventHandler = app.Services.GetKeyedService<IGpioObjectsWaitEventHandler>(MiscConstants.gpioObjectsWaitEventName);
                var appShutdownWaitEventHandler = app.Services.GetKeyedService<IAppShutdownWaitEventHandler>(MiscConstants.appShutdownWaitEventName);
                var logger = app.Services.GetService<ILogger<CommandLineTask>>();
                var gpioController = app.Services.GetKeyedService<GpioController>(MiscConstants.gpioControllerName);

                bool runTask = true;

                while (runTask)
                {
                    var command = Console.ReadLine()!.Trim();

                    if (command!.Equals("INFO".Trim(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        logger!.LogInformation("ASPNETCORE_ENVIRONMENT:{EnvironmentName}", app.Environment.EnvironmentName);
                        logger!.LogInformation("APPICATION_NAME:{ApplicationName}", app.Environment.ApplicationName);
                        // _logger.LogInformation("WEB_ROOT_PATH:{WebRootPath}", app.Environment.WebRootPath);
                        var urls = app.Urls;

                        var endpoints = app
                                        .Services
                                        .GetServices<EndpointDataSource>()
                                        .SelectMany(x => x!.Endpoints);

                        foreach (var endpoint in endpoints)
                        {
                            if (endpoint is RouteEndpoint routeEndpoint)
                            {
                                var url = urls.FirstOrDefault();
                                var routepatternrawtext = routeEndpoint.RoutePattern.RawText;


                                if (routepatternrawtext!.StartsWith('/'))
                                    logger!.LogInformation("ENDPOINT:{url}{RawText}", url, routepatternrawtext);
                                else
                                    logger!.LogInformation("ENDPOINT:{url}/{RawText}", url, routepatternrawtext);

                            }

                            var routeNameMetadata = endpoint.Metadata.OfType<RouteNameMetadata>().FirstOrDefault();
                            var routName = routeNameMetadata?.RouteName;

                            var httpMethodsMetadata = endpoint.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault();
                            var httpMethods = httpMethodsMetadata?.HttpMethods;
                        }

                        var properties = IPGlobalProperties.GetIPGlobalProperties();
                        var httpConnections = from connection in properties.GetActiveTcpConnections()
                                              where connection.LocalEndPoint.Port == new Uri(urls.FirstOrDefault()!).Port
                                              select connection;

                        logger!.LogInformation("Local CONNECTIONS:{Count}", httpConnections.Count());

                    }

                    else if (command!.Equals("GPIO".Trim(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (gpioObjects != null && gpioObjects.Count > 0)
                        {
                            foreach (var gpioObject in gpioObjects!.DistinctBy(s => s.GpioNumber))
                            {

                                if (gpioController != null)
                                {
                                    if (gpioController!.IsPinOpen(gpioObject.GpioNumber))
                                    {
                                        logger!.LogInformation("Gpio {GpioNumber} Open", gpioObject.GpioNumber);
                                        PinValue pinValue = gpioController.Read(gpioObject.GpioNumber);
                                        logger!.LogInformation("Gpio Value is {pinValue} ", pinValue);

                                    }
                                    else
                                    {
                                        logger!.LogInformation("Gpio {GpioNumber} not Open", gpioObject.GpioNumber);
                                    }
                                }
                            }
                        }
                        else
                        {
                            logger!.LogInformation("No Gpios are Open");
                        }

                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (command!.Equals("QUIT".Trim(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        gpioObjectsWaitEventHandler!.Set();
                        runTask = false;
                        await app.StopAsync();

                    }
                    else
                    {
                        logger!.LogWarning("Invalid command. Valid commands are quit,info or gpio");
                    }
                }
            });
        }
    }
}

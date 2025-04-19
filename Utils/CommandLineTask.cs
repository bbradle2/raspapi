using raspapi.Models;
using System.Device.Gpio;
using System.Net.NetworkInformation;

namespace raspapi.Utils;
public class CommandLineTask
{
    public static void RunCommandLineTask(WebApplication app, GpioController gpioController, List<GpioObject> gpioObjectList, GpioObjectsWaitEventHandler gpioObjectsWaitEventHandler, ILogger logger)
    {
        _ = Task.Factory.StartNew(() =>
        {
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

                    logger!.LogInformation($"ENDPOINTS:");

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
                    foreach (var gpioObject in gpioObjectList!.DistinctBy(s => s.GpioNumber))
                    {
                        //SendLogMessage($"Checking Gpio {gpioObject.GpioNumber}");
                        if (gpioController!.IsPinOpen(gpioObject.GpioNumber))
                        {
                            logger.LogInformation("Gpio {GpioNumber} Open", gpioObject.GpioNumber);

                        }
                        else
                        {
                            logger.LogInformation("Gpio {GpioNumber} not Open", gpioObject.GpioNumber);
                        }
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (command!.Equals("QUIT".Trim(), StringComparison.CurrentCultureIgnoreCase))
                {
                    gpioObjectsWaitEventHandler.Set();
                    runTask = false;
                    app.StopAsync();

                }
                else
                {
                    logger!.LogWarning("Invalid command. Valid commands are quit,info or gpio");
                }
            }
        });
    }
}


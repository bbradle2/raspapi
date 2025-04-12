using System.Device.Gpio;
using raspapi.Constants.RaspberryPIConstants;
using System.Net.NetworkInformation;
using Scalar.AspNetCore;
using raspapi.Intercepts;
using raspapi.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Metadata;
using System.Text;
using System.Text.Json.Nodes;
using raspapi.Models;
using System.Text.Json;
using Iot.Device.Usb;
using System.IO.Pipelines;


namespace raspapi
{
    
    class Program
    {
        
        public static ILogger<Program>? logger;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += OnSigTerm;
            Console.CancelKeyPress += OnSigInt;
                      
            var builder = WebApplication.CreateBuilder(args);
           
            builder.Services.AddOpenApi();
            builder.Logging.AddConsole();

            builder.Services.AddKeyedSingleton<GpioController>(MiscConstants.gpioControllerName);
            builder.Services.AddKeyedSingleton<BinarySemaphoreSlim>(MiscConstants.gpioSemaphoreName);
            builder.Services.AddKeyedSingleton<List<GpioObject>>(MiscConstants.gpioObjectsName);
            builder.Services.AddKeyedSingleton<GpioObjectsWaitEventHandler>(MiscConstants.gpioObjectsWaitEventName);
            builder.Services.AddKeyedSingleton<AppShutdownWaitEventHandler>(MiscConstants.appShutdownWaitEventName);

            builder.Services.AddControllers();

            var app = builder.Build();
           
            app.UseRouting();          
            app.UseWebSockets();

            logger = app.Services.GetRequiredService<ILogger<Program>>();

            var gpioController = app.Services.GetKeyedService<GpioController>(MiscConstants.gpioControllerName);
            var gpioSemaphore = app.Services.GetKeyedService<BinarySemaphoreSlim>(MiscConstants.gpioSemaphoreName);
            var gpioObjectList = app.Services.GetKeyedService<List<GpioObject>>(MiscConstants.gpioObjectsName);
            var gpioObjectsWaitEventHandler = app.Services.GetKeyedService<GpioObjectsWaitEventHandler>(MiscConstants.gpioObjectsWaitEventName);
            var appShutdownWaitEventHandler = app.Services.GetKeyedService<AppShutdownWaitEventHandler>(MiscConstants.appShutdownWaitEventName);

            var gpioObjectConfig = File.ReadAllText("raspberrypi05_PinMapping.json");
            var gpioConfigList = JsonSerializer.Deserialize<List<GpioObject>>(gpioObjectConfig);          

            foreach (var i in gpioConfigList!)
            {
                GpioObject gpioObject = i;
                gpioObject.GpioNumber = i.GpioNumber;
                gpioObject.GpioValue = null;

                gpioObjectList!.Add(gpioObject);
            }
           
            app.MapControllers();
            _ = app.Use(async (context, next) =>
            {

                if (context.Request.Path == "/GetGpiosStatus")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();


                        await WebSocketGpioStatus.GetGpiosStatus(webSocket,
                                                               gpioController!,
                                                               gpioObjectList!,
                                                               gpioObjectsWaitEventHandler!,
                                                               appShutdownWaitEventHandler!
                                                               );
                       
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    }
                }
                else
                {
                    await next(context);
                }

            });
            
            RunCommandLineTask(app, logger, gpioController!, gpioObjectList!, gpioObjectsWaitEventHandler!);

            _ = app.Lifetime.ApplicationStopping.Register(() =>
            {
                SendLogMessage("Shutting down Gpio Wait Event Handler.");
                SendLogMessage("Please wait.");
                gpioObjectsWaitEventHandler!.Set();
                appShutdownWaitEventHandler!.Set();
                SendLogMessage("Gpio Wait Event Handler shut down complete");
            });

            
            _ = app.Lifetime.ApplicationStopped.Register(() =>
            {
                try
                {
                    // Wait on all calls to complete before shut down.
                    gpioSemaphore?.WaitAsync().GetAwaiter();
                    SendLogMessage("Checking for Open Gpio's....");

                    if (gpioObjectList == null)
                    {
                        return;
                    }

                    foreach (var gpioObject in gpioObjectList!.DistinctBy(s => s.GpioNumber))
                    {
                        SendLogMessage($"Checking Gpio {gpioObject.GpioNumber}");
                        if (gpioController!.IsPinOpen(gpioObject.GpioNumber))
                        {
                            SendLogMessage($"Gpio {gpioObject.GpioNumber} Open");
                            SendLogMessage($"Turning Off and Closing Gpio {gpioObject.GpioNumber}");
                            //gpioController.Write(gpioObject.GpioNumber, PinValue.Low);
                            gpioController.ClosePin(gpioObject.GpioNumber);
                        }
                        else
                        {
                            SendLogMessage($"Gpio {gpioObject.GpioNumber} not Open");
                        }
                    }
                }
                catch (Exception ex)
                {
                    SendLogErrorMessage($"Exception:  {ex.Message}");
                }
                finally
                {
                    gpioSemaphore?.Release();
                }
            });

            app.Run();
        }

        /*For debugging in development*/
        private static void RunCommandLineTask(WebApplication app, ILogger<Program> _logger, GpioController gpioController, List<GpioObject> gpioObjectList, GpioObjectsWaitEventHandler gpioEvent)
        {
            _ = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var command = Console.ReadLine()!.Trim();

                    if (command!.Equals("INFO".Trim(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        _logger.LogInformation("ASPNETCORE_ENVIRONMENT:{EnvironmentName}", app.Environment.EnvironmentName);
                        _logger.LogInformation("APPICATION_NAME:{ApplicationName}", app.Environment.ApplicationName);
                        // _logger.LogInformation("WEB_ROOT_PATH:{WebRootPath}", app.Environment.WebRootPath);
                        var urls = app.Urls;

                        var endpoints = app
                                        .Services
                                        .GetServices<EndpointDataSource>()
                                        .SelectMany(x => x!.Endpoints);

                        _logger.LogInformation($"ENDPOINTS:");

                        foreach (var endpoint in endpoints)
                        {
                            if (endpoint is RouteEndpoint routeEndpoint)
                            {
                                var url = urls.FirstOrDefault();
                                var routepatternrawtext = routeEndpoint.RoutePattern.RawText;


                                if (routepatternrawtext!.StartsWith('/'))
                                    _logger.LogInformation("ENDPOINT:{url}{RawText}", url, routepatternrawtext);
                                else
                                    _logger.LogInformation("ENDPOINT:{url}/{RawText}", url, routepatternrawtext);

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

                        _logger.LogInformation("Local CONNECTIONS:{Count}", httpConnections.Count());

                    }

                    else if (command!.Equals("GPIO".Trim(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        foreach (var gpioObject in gpioObjectList!.DistinctBy(s => s.GpioNumber))
                        {
                            SendLogMessage($"Checking Gpio {gpioObject.GpioNumber}");
                            if (gpioController!.IsPinOpen(gpioObject.GpioNumber))
                            {
                                SendLogMessage($"Gpio {gpioObject.GpioNumber} Open");

                            }
                            else
                            {
                                SendLogMessage($"Gpio {gpioObject.GpioNumber} not Open");
                            }
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (command!.Equals("QUIT".Trim(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        gpioEvent.Set();
                        app.StopAsync();
                        
                    }
                    else
                    {
                        _logger.LogWarning("Invalid command. Valid commands are quit or info");
                    }
                }
            });
        }

        private static void SendLogMessage(string message)
        {
            logger?.LogInformation("{message}", message);
        }

        private static void SendLogErrorMessage(string message)
        {
            logger?.LogError("{message}", message);
        }


        private static void OnSigInt(object? sender, ConsoleCancelEventArgs e)
        {
            SendLogMessage("SIGINT Received...");
        }
       
        private static void OnSigTerm(object? sender, EventArgs e)
        {
            SendLogMessage("SIGTERM Received...");
        }

    }
}

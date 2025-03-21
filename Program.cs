using System.Device.Gpio;
using raspapi.Constants.RaspberryPIConstants;
using raspapi.DataObjects;
using raspapi.Interfaces;
using raspapi.Intercepts;
//using System.Runtime.InteropServices;
//using Nextended.Core.Extensions;
//using System.ComponentModel.DataAnnotations;

namespace raspapi
{
    
    class Program
    {

        public static ILogger<Program> ?logger;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += OnSigTerm;
            Console.CancelKeyPress += OnSigInt;
            // PosixSignalRegistration.Create(PosixSignal.SIGCONT, (context) =>
            // {
            //     SendMessageToTerminal("Received SIGCONT");
            // });

           
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();
            //builder.Services.AddOpenApi();

            builder.Logging.AddConsole();
            

            builder.Services.AddSingleton<GpioPin23>();
            builder.Services.AddSingleton<GpioPin24>();
            builder.Services.AddSingleton<GpioPin25>();
            builder.Services.AddSingleton<GpioController>();
            builder.Services.AddSingleton<Dictionary<int, IGpioPin>>();
            builder.Services.AddKeyedSingleton(MiscConstants.gpioSemaphoreName, new SemaphoreSlim(1, 1));
            
            builder.Services.AddControllers();

            var app = builder.Build();

            var gpioController = app.Services.GetRequiredService<GpioController>();
            
            var pins = app.Services.GetRequiredService<Dictionary<int, IGpioPin>>(); 
            pins.Add(GpioPinConstants.PIN23 , app.Services.GetRequiredService<GpioPin23>());
            pins.Add(GpioPinConstants.PIN24, app.Services.GetRequiredService<GpioPin24>());
            pins.Add(GpioPinConstants.PIN25, app.Services.GetRequiredService<GpioPin25>());

            var gpioSemaphore = app.Services.GetRequiredKeyedService<SemaphoreSlim>(MiscConstants.gpioSemaphoreName);

            logger = app.Services.GetRequiredService<ILogger<Program>>();

            //app.UseSwagger();
            //app.UseSwaggerUI();
            
            app.UseMiddleware<ApiIntercept>();

            // _ = Task.Factory.StartNew(() =>
            // {
            //     while (true)
            //     {
            //         if (Console.ReadLine() == "QUIT")
            //         {
            //             app.StopAsync();
            //         }
            //     }
            // });

            

            //logger.LogInformation("ASPNETCORE_ENVIRONMENT:{app.Environment.EnvironmentName}", app.Environment.EnvironmentName);

            app.MapControllers();

            if (app.Environment.IsDevelopment())
            {
                _ = Task.Factory.StartNew(() =>
                {
                    while (true)
                    {

                        if (Console.ReadLine() == "INFO")
                        {
                            logger.LogInformation($"ASPNETCORE_ENVIRONMENT:{app.Environment.EnvironmentName}");
                            logger.LogInformation($"APPICATION_NAME:{app.Environment.ApplicationName}");
                            logger.LogInformation($"IS_DEV:{app.Environment.IsDevelopment().ToString()}");
                            logger.LogInformation($"WEB_ROOT_PATH:{app.Environment.WebRootPath}");


                            var endpoints = app
                            .Services
                            .GetServices<EndpointDataSource>()
                            .SelectMany(x => x!.Endpoints);

                            logger.LogInformation($"ENDPOINTS:");

                            foreach (var endpoint in endpoints)
                            {
                                if (endpoint is RouteEndpoint routeEndpoint)
                                {
                                    _ = routeEndpoint.RoutePattern.RawText;
                                    _ = routeEndpoint.RoutePattern.PathSegments;
                                    _ = routeEndpoint.RoutePattern.Parameters;
                                    _ = routeEndpoint.RoutePattern.InboundPrecedence;
                                    _ = routeEndpoint.RoutePattern.OutboundPrecedence;
                                    logger.LogInformation($"ENDPOINT:{routeEndpoint.RoutePattern.RawText}");
                                }

                                var routeNameMetadata = endpoint.Metadata.OfType<Microsoft.AspNetCore.Routing.RouteNameMetadata>().FirstOrDefault();
                                var routName = routeNameMetadata?.RouteName;

                                var httpMethodsMetadata = endpoint.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault();
                                var httpMethods = httpMethodsMetadata?.HttpMethods;

                            }


                        }

                    }

                });
            }

            //AssemblyName[] names = Assembly.GetExecutingAssembly().GetReferencedAssemblies();

            _ = app.Lifetime.ApplicationStopped.Register(() =>
            {

                try
                {
                    // Wait on all calls to complete before shut down.
                    gpioSemaphore?.Wait();
                    SendLogMessage("Checking for Open Pins....");

                    foreach (var pin in pins)
                    {
                        SendLogMessage($"Checking Pin {pin.Key}");
                        if (gpioController.IsPinOpen(pin.Value.Pin))
                        {
                            SendLogMessage($"Pin {pin.Key} Open");
                            SendLogMessage($"Turning Off and Closing Pin {pin.Key}");
                            gpioController.Write(pin.Value.Pin, PinValue.Low);
                            gpioController.ClosePin(pin.Value.Pin);
                        }
                        else
                        {
                            SendLogMessage($"Pin {pin.Key} not Open");
                        }
                    }
                }
                catch (Exception ex)
                {
                    SendLogMessage($"Exception:  {ex.Message}");
                }
                finally
                {
                    gpioSemaphore?.Release();
                    gpioSemaphore?.Dispose();
                }
            });

            app.Run();
        }

        private static void SendLogMessage(string message)
        {
            logger?.LogInformation("{message}", message);
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

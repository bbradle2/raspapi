using System.Device.Gpio;
using raspapi.Constants.RaspberryPIConstants;
using System.Net.NetworkInformation;
using Scalar.AspNetCore;
using raspapi.Intercepts;

//using System.ComponentModel.DataAnnotations;

namespace raspapi
{
    
    class Program
    {
        
        public static ILogger<Program>? logger;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += OnSigTerm;
            Console.CancelKeyPress += OnSigInt;
            // PosixSignalRegistration.Create(PosixSignal.SIGCONT, (context) =>
            // {
            //     SendMessageToTerminal("Received SIGCONT");
            // });

           
            var builder = WebApplication.CreateBuilder(args);
           
            builder.Services.AddOpenApi();
            builder.Logging.AddConsole();

            builder.Services.AddSingleton<GpioController>();
            builder.Services.AddKeyedSingleton(MiscConstants.gpioSemaphoreName, new SemaphoreSlim(1, 1));
            
            builder.Services.AddControllers();

            var app = builder.Build();
           
            //var t = builder.Configuration["ASPNETCORE_URLS"];

            app.UseRouting();

            var gpioController = app.Services.GetRequiredService<GpioController>();
            
            
            var gpioSemaphore = app.Services.GetRequiredKeyedService<SemaphoreSlim>(MiscConstants.gpioSemaphoreName);
            logger = app.Services.GetRequiredService<ILogger<Program>>();

            app.UseMiddleware<ApiIntercept>();                      
            app.MapControllers();

            if (app.Environment.IsDevelopment())
            {
                RunCommandLineTask(app, logger);
            }

            //AssemblyName[] names = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            
            app.Run();
        }

        /*For debugging in development*/
        private static void RunCommandLineTask(WebApplication app, ILogger<Program> _logger)
        {
            _ = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var command = Console.ReadLine();

                    if (command!.Equals("INFO", StringComparison.CurrentCultureIgnoreCase))
                    {
                        _logger.LogInformation("ASPNETCORE_ENVIRONMENT:{EnvironmentName}", app.Environment.EnvironmentName);
                        _logger.LogInformation("APPICATION_NAME:{ApplicationName}", app.Environment.ApplicationName);
                        _logger.LogInformation("WEB_ROOT_PATH:{WebRootPath}", app.Environment.WebRootPath);
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

                    if (command == "QUIT")
                    {
                        app.StopAsync();
                    }

                }

            });
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

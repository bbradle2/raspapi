using raspapi.Constants;
using raspapi.Interfaces;
using raspapi.Models;
using System.Device.Gpio;
using System.Net.NetworkInformation;

namespace raspapi.Handlers
{
   
    public class CommandLineTaskHandler : ICommandLineTaskHandler
    {
        private readonly GpioController _gpioController;
        private readonly IList<GpioObject> _gpioObjectList;
        private readonly IGpioObjectsWaitEventHandler _gpioObjectsWaitEventHandler;
        private readonly ILogger<CommandLineTaskHandler> _logger;
        private readonly IHost _host;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public CommandLineTaskHandler([FromKeyedServices(MiscConstants.gpioControllerName)] GpioController gpioController,
                                       [FromKeyedServices(MiscConstants.gpioObjectsName)] IList<GpioObject> gpioObjectList,
                                       [FromKeyedServices(MiscConstants.gpioObjectsWaitEventName)] IGpioObjectsWaitEventHandler gpioObjectsWaitEventHandler,
                                       ILogger<CommandLineTaskHandler> logger,
                                       IHost host,
                                       IWebHostEnvironment webHostEnvironment,
                                       IConfiguration configuration
                                       )
        {
            _gpioObjectList = gpioObjectList;
            _gpioController = gpioController;
            _gpioObjectsWaitEventHandler = gpioObjectsWaitEventHandler;
            _logger = logger;
            _host = host;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;

        }

        public void Handle()
        {
            _ = Task.Factory.StartNew(async () =>
            {

                bool runTask = true;

                while (runTask)
                {
                    var command = Console.ReadLine()!.Trim();

                    if (command!.Equals("INFO".Trim(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        _logger!.LogInformation("ASPNETCORE_ENVIRONMENT:{EnvironmentName}", _webHostEnvironment.EnvironmentName);
                        _logger!.LogInformation("APPICATION_NAME:{ApplicationName}", _webHostEnvironment.ApplicationName);

                        _logger!.LogInformation("Available Connection(s):{Urls}", _configuration["Urls"]!);
                        var urls = _configuration["Urls"]!.Split(',');
                       



                        var endpoints = _host.Services
                                        .GetServices<EndpointDataSource>()
                                        .SelectMany(x => x!.Endpoints);

                        foreach (var endpoint in endpoints)
                        {
                            if (endpoint is RouteEndpoint routeEndpoint)
                            {
                                var url = urls.FirstOrDefault();
                                var routepatternrawtext = routeEndpoint.RoutePattern.RawText;


                                if (routepatternrawtext!.StartsWith('/'))
                                    _logger!.LogInformation("ENDPOINT:{url}{RawText}", url, routepatternrawtext);
                                else
                                    _logger!.LogInformation("ENDPOINT:{url}/{RawText}", url, routepatternrawtext);

                            }

                            var routeNameMetadata = endpoint.Metadata.OfType<RouteNameMetadata>().FirstOrDefault();
                            var routName = routeNameMetadata?.RouteName;

                            var httpMethodsMetadata = endpoint.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault();
                            var httpMethods = httpMethodsMetadata?.HttpMethods;
                        }

                        try
                        {
                            var properties = IPGlobalProperties.GetIPGlobalProperties();
                            var httpConnections = from connection in properties.GetActiveTcpConnections()
                                                  where connection.LocalEndPoint.Port == new Uri(urls.FirstOrDefault()!).Port
                                                  select connection;

                            _logger!.LogInformation("Local CONNECTIONS:{Count}", httpConnections.Count());
                        }
                        catch(Exception ex)
                        {
                            _logger!.LogWarning("Problem Getting Http Connection Count. {ex.Message}", ex.Message);
                        }

                    }
                    else if (command!.Equals("GPIO".Trim(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (_gpioObjectList != null && _gpioObjectList.Count > 0)
                        {
                            foreach (var gpioObject in _gpioObjectList!.DistinctBy(s => s.GpioNumber))
                            {

                                if (_gpioController != null)
                                {
                                    if (_gpioController!.IsPinOpen(gpioObject.GpioNumber))
                                    {
                                        _logger!.LogInformation("Gpio {GpioNumber} Open", gpioObject.GpioNumber);
                                        PinValue pinValue = _gpioController.Read(gpioObject.GpioNumber);
                                        _logger!.LogInformation("Gpio Value is {pinValue} ", pinValue);

                                    }
                                    else
                                    {
                                        _logger!.LogInformation("Gpio {GpioNumber} not Open", gpioObject.GpioNumber);
                                    }
                                }
                            }
                        }
                        else
                        {
                            _logger!.LogInformation("No Gpios are Open");
                        }

                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (command!.Equals("QUIT".Trim(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        _gpioObjectsWaitEventHandler!.Set();
                        runTask = false;
                        await _host.StopAsync();

                    }
                    else
                    {
                        _logger!.LogWarning("Invalid command. Valid commands are quit,info or gpio");
                    }
                }
            });
        }
    }
}

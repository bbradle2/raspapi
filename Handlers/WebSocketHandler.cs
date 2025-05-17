using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using raspapi.Models;
using raspapi.Interfaces;
using raspapi.Constants;

namespace raspapi.Handlers
{
   
    public class WebSocketHandler : IWebSocketHandler
    {

        private readonly IList<GpioObject> _gpioObjects;
        private readonly IGpioObjectsWaitEventHandler _gpioObjectsWaitEventHandler;
        private readonly IAppShutdownWaitEventHandler _appShutdownWaitEventHandler;
        private readonly IBinarySemaphoreSlimHandler _binarySemphoreSlimHandler;
        private readonly IServiceProvider _services;
        private readonly ILogger _logger;

        public WebSocketHandler([FromKeyedServices(MiscConstants.gpioObjectsName)] IList<GpioObject> gpioObjects,
                                [FromKeyedServices(MiscConstants.gpioObjectsWaitEventName)] IGpioObjectsWaitEventHandler gpioObjectsWaitEventHandler,
                                [FromKeyedServices(MiscConstants.appShutdownWaitEventName)] IAppShutdownWaitEventHandler appShutdownWaitEventHandler,
                                [FromKeyedServices(MiscConstants.gpioSemaphoreName)] IBinarySemaphoreSlimHandler binarySemaphoreSlimHandler,
                                IServiceProvider services,
                                ILogger<WebSocketHandler> logger)
        {

            _gpioObjects = gpioObjects;
            _gpioObjectsWaitEventHandler = gpioObjectsWaitEventHandler;
            _services = services;
            _appShutdownWaitEventHandler = appShutdownWaitEventHandler;
            _binarySemphoreSlimHandler = binarySemaphoreSlimHandler;
            _logger = logger;

        }
        
        public async Task GetGpios(WebSocket webSocket)
        {

            while (webSocket!.State == WebSocketState.Open)
            {

                if (_appShutdownWaitEventHandler!.WaitOne(100))
                {

                    if (webSocket == null || webSocket!.State == WebSocketState.Closed || webSocket!.State == WebSocketState.Aborted)
                    {
                        return;
                    }
                    else
                    {
                        await webSocket.CloseAsync(
                                   WebSocketCloseStatus.NormalClosure,
                                   "Closed Web Socket",
                                   CancellationToken.None);
                        return;
                    }
                }

                var sendBuffer = JsonSerializer.Serialize(_gpioObjects);
                sendBuffer = sendBuffer.Replace("\0", string.Empty);

                if (webSocket != null)
                {
                    try
                    {
                        await webSocket.SendAsync(
                            new ArraySegment<byte>(Encoding.UTF8.GetBytes(sendBuffer), 0, sendBuffer.Length),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None);

                        if (webSocket.State == WebSocketState.Aborted || webSocket.State == WebSocketState.CloseSent || webSocket.State == WebSocketState.CloseReceived)
                        {
                            _logger!.LogInformation("WebSocket CloseSent, Aborted or CloseReceived..");
                            return;
                        }


                        var recvBuffer = new byte[1024 * 4];
                        var receiveResult = await webSocket.ReceiveAsync(
                            new ArraySegment<byte>(recvBuffer), CancellationToken.None);

                        if (receiveResult.EndOfMessage && recvBuffer[0] == '[')
                        {
                            // make sure message can de-serialize to an array of gpioObjects
                            var s = Encoding.UTF8.GetString(recvBuffer).Replace("\0", string.Empty);
                            var gpios = JsonSerializer.Deserialize<GpioObject[]?>(s);
                            _logger!.LogInformation("Received message {s}", s!);
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger!.LogInformation("Client Aborted Connection{ex.Message}", ex.Message);
                        return;
                    }
                }

                _gpioObjectsWaitEventHandler!.WaitOne(1);

            }

            _logger!.LogInformation("WebSocket Closed");
        }
    }
}
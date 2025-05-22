using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using raspapi.Models;
using raspapi.Interfaces;
using raspapi.Constants;

namespace raspapi.Handlers
{
   
    public class WebSocketHandler([FromKeyedServices(MiscConstants.gpioObjectsName)] IList<GpioObject> gpioObjects,
                                  [FromKeyedServices(MiscConstants.gpioObjectsWaitEventHandlerName)] IGpioObjectsWaitEventHandler gpioObjectsWaitEventHandler,
                                  [FromKeyedServices(MiscConstants.appShutdownWaitEventHandlerName)] IAppShutdownWaitEventHandler appShutdownWaitEventHandlerName,
                                  ILogger<WebSocketHandler> logger) : IWebSocketHandler
    {

        private readonly IList<GpioObject> _gpioObjects = gpioObjects;
        private readonly IGpioObjectsWaitEventHandler _gpioObjectsWaitEventHandler = gpioObjectsWaitEventHandler;
        private readonly IAppShutdownWaitEventHandler _appShutdownWaitEventHandlerName = appShutdownWaitEventHandlerName;
        private readonly ILogger _logger = logger;


        public async Task GetGpioStatus(WebSocket webSocket)
        {
            while (webSocket!.State == WebSocketState.Open)
            {
                try
                {
                    var sendBuffer = JsonSerializer.Serialize(_gpioObjects);
                    sendBuffer = sendBuffer.Replace("\0", string.Empty);

                    await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(sendBuffer), 0, sendBuffer.Length),
                                              WebSocketMessageType.Text,
                                              true,
                                              CancellationToken.None);

                    var recvBytes = new byte[1];
                    var res = await webSocket.ReceiveAsync(recvBytes,
                                                           CancellationToken.None);

                    if (res.MessageType == WebSocketMessageType.Close
                        && res.EndOfMessage
                        && recvBytes.Length == 1
                        && recvBytes[0] == MiscConstants.EOT)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                                                   res.CloseStatusDescription,
                                                   CancellationToken.None);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _logger!.LogWarning("Client Aborted Connection {ex.Message}", ex.Message);
                    return;
                }

                _gpioObjectsWaitEventHandler!.WaitOne(10);
               
                if (webSocket!.State != WebSocketState.Open) break;
            }

            _logger!.LogInformation("End GetGpioStatus");
        }
    }
}
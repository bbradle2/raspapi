using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using raspapi.Models;
using raspapi.Interfaces;
using raspapi.Constants;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace raspapi.Handlers
{
   
    public class WebSocketHandler : IWebSocketHandler
    {

        private readonly IList<GpioObject> _gpioObjects;
        //private readonly IGpioObjectsWaitEventHandler _gpioObjectsWaitEventHandler;
        private readonly ILogger _logger;

        public WebSocketHandler([FromKeyedServices(MiscConstants.gpioObjectsName)] IList<GpioObject> gpioObjects,
                                //[FromKeyedServices(MiscConstants.gpioObjectsWaitEventName)] IGpioObjectsWaitEventHandler gpioObjectsWaitEventHandler,
                                ILogger<WebSocketHandler> logger)
        {

            _gpioObjects = gpioObjects;
            //_gpioObjectsWaitEventHandler = gpioObjectsWaitEventHandler;
            _logger = logger;
            

        }
        
        public async Task GetGpioStatus(WebSocket webSocket)
        {

            while (webSocket!.State == WebSocketState.Open)
            {

                var sendBuffer = JsonSerializer.Serialize(_gpioObjects);
                sendBuffer = sendBuffer.Replace("\0", string.Empty);

                try
                {
                    await webSocket.SendAsync(
                        new ArraySegment<byte>(Encoding.UTF8.GetBytes(sendBuffer), 0, sendBuffer.Length),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);

                    var recvBytes = new byte[1];
                    var res = await webSocket.ReceiveAsync(recvBytes, CancellationToken.None);

                    if (res.MessageType == WebSocketMessageType.Close &&
                        res.EndOfMessage &&
                        recvBytes.Length == 1 &&
                        recvBytes[0] == 0x04)
                    {
                        await webSocket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            res.CloseStatusDescription,
                            CancellationToken.None
                        );
                        break;
                    }

                }
                catch (Exception ex)
                {
                    _logger!.LogWarning("Client Aborted Connection {ex.Message}.", ex.Message);
                    return;
                }

                //_gpioObjectsWaitEventHandler!.WaitOne(1000);

               

            }

            _logger!.LogInformation("End GetGpioStatus");
        }
    }
}
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using raspapi.Models;
using raspapi.Interfaces;
using raspapi.Constants;
using raspapi.Controllers;

namespace raspapi.Utils
{
   
    public class WebSocketHandler : IWebSocketHandler
    {
        public async Task GetGpios(WebSocket webSocket, IServiceProvider requestServices)
        {

            var gpioObjects = requestServices.GetKeyedService<IList<GpioObject>>(MiscConstants.gpioObjectsName);
            var gpioObjectsWaitEventHandler = requestServices.GetKeyedService<IGpioObjectsWaitEventHandler>(MiscConstants.gpioObjectsWaitEventName);
            var appShutdownWaitEventHandler = requestServices.GetKeyedService<IAppShutdownWaitEventHandler>(MiscConstants.appShutdownWaitEventName);
            var logger = requestServices.GetService<ILogger<WebSocketHandler>>();

            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (webSocket!.State == WebSocketState.Open)
            {

                if (appShutdownWaitEventHandler!.WaitOne(100))
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

                var sendBuffer = JsonSerializer.Serialize(gpioObjects);
                sendBuffer = sendBuffer.Replace("\0", string.Empty);

                if (webSocket != null)
                {
                    try
                    {
                        await webSocket.SendAsync(
                            new ArraySegment<byte>(Encoding.UTF8.GetBytes(sendBuffer), 0, sendBuffer.Length),
                            receiveResult.MessageType,
                            receiveResult.EndOfMessage,
                            CancellationToken.None);

                        if (webSocket.State == WebSocketState.Aborted || webSocket.State == WebSocketState.CloseSent || webSocket.State == WebSocketState.CloseReceived)
                        {
                            logger!.LogInformation("WebSocket CloseSent, Aborted or CloseReceived..");
                            return;
                        }


                        receiveResult = await webSocket.ReceiveAsync(
                            new ArraySegment<byte>(buffer), CancellationToken.None);

                        var s = Encoding.UTF8.GetString(buffer);

                        if (s.StartsWith("[]"))
                            logger!.LogInformation("Received Initalize message {s}", s);
                        else
                            logger!.LogInformation("Received message {s}", s);

                    }
                    catch (Exception)
                    {
                        logger!.LogInformation("Client Aborted Connection");
                        return;
                    }
                }

                gpioObjectsWaitEventHandler!.WaitOne(10);

            }

            logger!.LogInformation("WebSocket Closed");
        }
    }
}
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
        public async Task GetGpios(WebSocket webSocket, IServiceProvider requestServices)
        {

            var gpioObjects = requestServices.GetKeyedService<IList<GpioObject>>(MiscConstants.gpioObjectsName);
            var gpioObjectsWaitEventHandler = requestServices.GetKeyedService<IGpioObjectsWaitEventHandler>(MiscConstants.gpioObjectsWaitEventName);
            var appShutdownWaitEventHandler = requestServices.GetKeyedService<IAppShutdownWaitEventHandler>(MiscConstants.appShutdownWaitEventName);
            var logger = requestServices.GetService<ILogger<WebSocketHandler>>();

            var initBuffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(initBuffer), CancellationToken.None);

            while (webSocket!.State == WebSocketState.Open &&
                  receiveResult.EndOfMessage &&
                  initBuffer[0] == '[' &&
                  initBuffer[1] == ']')
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


                        var recvBuffer = new byte[1024 * 4];
                        receiveResult = await webSocket.ReceiveAsync(
                            new ArraySegment<byte>(recvBuffer), CancellationToken.None);

                        if (recvBuffer[0] == '[' &&
                            recvBuffer[1] == ']' &&
                            receiveResult.EndOfMessage)
                        {
                            logger!.LogInformation("Received Initalize message []");
                        }
                        else
                        {
                            if (receiveResult.EndOfMessage && recvBuffer[0] == '[')
                            {
                                // make sure message can de-serialize to an array of gpioObjects
                                var s = Encoding.UTF8.GetString(recvBuffer).Replace("\0", string.Empty);
                                var gpios = JsonSerializer.Deserialize<GpioObject[]?>(s);
                                logger!.LogInformation("Received message {s}", s!);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        logger!.LogInformation("Client Aborted Connection{ex.Message}", ex.Message);
                        return;
                    }
                }

                gpioObjectsWaitEventHandler!.WaitOne(1);

            }

            logger!.LogInformation("WebSocket Closed");
        }
    }
}
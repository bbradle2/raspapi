
namespace raspapi.Utils
{
    using System.Device.Gpio;
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;
    using raspapi.Models;

    public static class WebSocketGpioStatus
    {
        public static async Task GetGpiosStatus(WebSocket webSocket,
                                              GpioController gpioController,
                                              List<GpioObject> pObjects,
                                              GpioObjectsWaitEventHandler gpioWait,
                                              AppShutdownWaitEventHandler appShutdownWait
                                              )
        {

            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {

                if (appShutdownWait.WaitOne(100))
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
                    }

                    return;
                }

                var sendBuffer = JsonSerializer.Serialize(pObjects);
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

                        if (webSocket.State == WebSocketState.Aborted || webSocket.State == WebSocketState.Closed)
                        {
                            return;
                        }

                        receiveResult = await webSocket.ReceiveAsync(
                            new ArraySegment<byte>(buffer), CancellationToken.None);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Client Aborted Connection");
                        break;
                    }
                }
                              
                gpioWait.WaitOne(1000);

            }
        }
    }
}
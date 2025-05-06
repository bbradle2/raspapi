
namespace raspapi.Utils
{
    using System.Device.Gpio;
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;
    using raspapi.Models;

    public static class WebSocketGpio
    {
        public static async Task GetGpios(WebSocket webSocket,
                                         List<GpioObject> gpioObjects,
                                         GpioObjectsWaitEventHandler gpioObjectsWaitEventHandler,
                                         AppShutdownWaitEventHandler appShutdownWaitEventHandler
                                         )
        {

            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (webSocket!.State == WebSocketState.Open)
            {

                if (appShutdownWaitEventHandler.WaitOne(100))
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
                            Console.WriteLine($"WebSocket CloseSent, Aborted or CloseReceived..");
                            return;
                        }

                       
                        receiveResult = await webSocket.ReceiveAsync(
                            new ArraySegment<byte>(buffer), CancellationToken.None);
                        
                        var s = Encoding.UTF8.GetString(buffer);

                        if (s.StartsWith("[]"))
                            Console.WriteLine($"Received Initalize message {s}");
                        else
                            Console.WriteLine($"Received message {s}");

                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Client Aborted Connection");
                        return;
                    }
                }

                gpioObjectsWaitEventHandler.WaitOne(10);

            }

            Console.WriteLine($"WebSocket Closed");
        }
    }
}
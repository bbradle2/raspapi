using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using raspapi.Models;
using raspapi.Interfaces;
using raspapi.Constants;
using System.Collections.Concurrent;

namespace raspapi.Handlers
{
    public class WebSocketHandler([FromKeyedServices(MiscConstants.gpioObjectsName)] ConcurrentQueue<GpioObject> gpioObjects,
                                  ILogger<WebSocketHandler> logger) : IWebSocketHandler
    {
        private readonly IReadOnlyCollection<GpioObject> _gpioObjects = gpioObjects;
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
                                              WebSocketMessageType.Binary,
                                              true,
                                              CancellationToken.None);

                    var recvBytes = new byte[1];
                    var res = await webSocket.ReceiveAsync(recvBytes,
                                                           CancellationToken.None);

                    if (!res.EndOfMessage && res.Count != 1 && recvBytes[0] != MiscConstants.EOT)
                    {
                         _logger!.LogWarning("Did not receive End Of Transmission byte");
                    }

                    if (res.MessageType == WebSocketMessageType.Close && res.EndOfMessage)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                                                   res.CloseStatusDescription,
                                                   CancellationToken.None);
                        _logger!.LogInformation("{res.CloseStatusDescription}", res.CloseStatusDescription);

                        continue;
                    }
                }
                catch (Exception ex)
                {
                    _logger!.LogWarning("Client Aborted Connection {ex.Message}", ex.Message);
                    continue;
                }
            }

            _logger!.LogInformation("End GetGpioStatus");
        }
    }
}
using System.Net.WebSockets;

namespace raspapi.Interfaces
{
    public interface IWebSocketHandler
    {
        Task GetGpios(WebSocket webSocket);
    }
}
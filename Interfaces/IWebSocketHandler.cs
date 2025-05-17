using System.Net.WebSockets;

public interface IWebSocketHandler
{
    Task GetGpios(WebSocket webSocket);
}
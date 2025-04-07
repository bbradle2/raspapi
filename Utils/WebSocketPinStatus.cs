
namespace raspapi.Utils
{
    using System.Device.Gpio;
    using System.Net.WebSockets;
    using System.Reflection.Metadata.Ecma335;
    using System.Text;
    using System.Text.Json;
    using raspapi.Constants.RaspberryPIConstants;
    using raspapi.Models;

    public static class WebSocketPinStatus
    {
        public static async Task GetPinsStatus(WebSocket webSocket, GpioController gpioController, HashSet<PinObject> pObjects)
        {

            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            string pinsJson = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
            var pinObjs = JsonSerializer.Deserialize<PinObject[]>(pinsJson);
            
            foreach (var pinObj in pinObjs!)
            {
                PinObject gpioPin = new();
                if (!gpioController.IsPinOpen(pinObj.PinNumber))
                {
                    gpioPin.PinNumber = gpioController!.OpenPin(pinObj.PinNumber, PinMode.Output).PinNumber;
                    var pinVal = gpioController.Read(pinObj.PinNumber);
                    gpioPin.PinValue = (bool?)pinVal;


                }
                else
                {
                    gpioPin.PinNumber = pinObj.PinNumber;
                    var pinVal = gpioController.Read(gpioPin.PinNumber);
                    gpioPin.PinValue = (bool?)pinVal;
                }

                pObjects.Add(gpioPin);
            }
            var sendBuffer = JsonSerializer.Serialize(pObjects);

            while (!receiveResult.CloseStatus.HasValue)
            {

                await webSocket.SendAsync(
                     new ArraySegment<byte>(Encoding.UTF8.GetBytes(sendBuffer), 0, sendBuffer.Length),
                     receiveResult.MessageType,
                     receiveResult.EndOfMessage,
                     CancellationToken.None);

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);

            
        }
    }
}
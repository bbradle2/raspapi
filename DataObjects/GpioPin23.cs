using System.Text.Json.Serialization;
using raspapi.Contants;
using raspapi.Interfaces;

namespace raspapi.DataObjects
{
    public class GpioPin23: IGpioPin
    {
        
        public int Pin { get; } = GpioPinContants.PIN23;
        public string Status { get; set; } = "Off";
        public string Description { get; set; } = $"Led {GpioPinContants.PIN23} On or Off";

    }
}
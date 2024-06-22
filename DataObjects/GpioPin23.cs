using System.Text.Json.Serialization;
using raspapi.Contants;
using raspapi.Interfaces;

namespace raspapi.DataObjects
{
    public class GpioPin23: IGpioPin
    {
        
        public int Pin { get; } = RaspBerryPiContants.PIN23;
        public string Status { get; set; } = "Off";
        public string Description { get; set; } = $"Led {RaspBerryPiContants.PIN23} On or Off";

    }
}
using raspapi.Contants;
using raspapi.Interfaces;

namespace raspapi.DataObjects
{
    public class GpioPin24 : IGpioPin
    {
        public int Pin { get; } = GpioPinContants.PIN24;
        public string Status { get; set; } = "Off";
        public string Description { get; set; } = $"Led {GpioPinContants.PIN24} On or Off";

    }
}
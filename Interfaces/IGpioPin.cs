using System.Device.Gpio;

namespace raspapi.Interfaces
{
    public interface IGpioPin
    {
        public int PinNumber { get; }

        public PinMode Mode { get; set; }
        public bool Status { get; set; } 
        public string Description { get; set; }

    }
}
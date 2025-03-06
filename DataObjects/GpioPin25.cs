namespace raspapi.DataObjects
{
    using raspapi.Constants.RaspberryPIConstants;
    using raspapi.Interfaces;
    public class GpioPin25 : IGpioPin
    {
        public int Pin { get; } = GpioPinConstants.PIN25;
        public string Status { get; set; } = "Off";
        public string Description { get; set; } = $"Led {GpioPinConstants.PIN25} On or Off";

    }
}
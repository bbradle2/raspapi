namespace raspapi.DataObjects
{
    using raspapi.Constants.RaspberryPIConstants;
    using raspapi.Interfaces;
    public class GpioPin24 : IGpioPin
    {
        public int Pin { get; } = GpioPinConstants.PIN24;
        public string Status { get; set; } = "Off";
        public string Description { get; set; } = $"Led {GpioPinConstants.PIN24} On or Off";

    }
}
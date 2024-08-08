namespace raspapi.DataObjects
{
    using raspapi.Constants.RaspberryPIConstants;
    using raspapi.Interfaces;
    public class GpioPin23: IGpioPin
    {
        
        public int Pin { get; } = GpioPinConstants.PIN23;
        public string Status { get; set; } = "Off";
        public string Description { get; set; } = $"Led {GpioPinConstants.PIN23} On or Off";

    }
}
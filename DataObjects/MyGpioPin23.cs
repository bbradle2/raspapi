namespace raspapi.DataObjects
{
    using System.Device.Gpio;
    using raspapi.Constants.RaspberryPIConstants;
    using raspapi.Interfaces;
    public class MyGpioPin23: IGpioPin
    {
        
        public int PinNumber { get; } = GpioPinConstants.PIN23;
        public PinMode Mode { get; set; } = PinMode.Output;
        public bool Status { get; set; } = false;
        public string Description { get; set; } = $"Pin {GpioPinConstants.PIN23} true or false";

    }
}
namespace raspapi.DataObjects
{
    using System.Device.Gpio;
    using raspapi.Constants.RaspberryPIConstants;
    using raspapi.Interfaces;
    public class MyGpioPin24: IGpioPin
    {
        
        public int PinNumber { get; } = GpioPinConstants.PIN24;
        public PinMode Mode { get; set; } = PinMode.Output;
        public bool Status { get; set; } = false;
        public string Description { get; set; } = $"Pin {GpioPinConstants.PIN24} true or false";

    }
}
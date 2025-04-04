namespace raspapi.Extensions
{
    using System.Device.Gpio;
    using raspapi.DataObjects;

    public static class GpioExtensions
    {
        private static void CloseGpioPin(this GpioController gpioController, PinObject pin)
        {
            if (gpioController.IsPinOpen(pin.PinNumber))
            {
                gpioController.GpioPinWriteLowValue(pin);
                gpioController.ClosePin(pin.PinNumber);
            }
        }

        public static void CloseGpioPin(this GpioController gpioController, PinObject[] gpioPins)
        {
            foreach (var gpioPin in gpioPins)
            {
                CloseGpioPin(gpioController, gpioPin);
            }
        }

        private static PinObject OpenGpioPinOutput(this GpioController gpioController, PinObject pin, PinMode pinMode = PinMode.Output)
        {
            if (!gpioController.IsPinOpen(pin.PinNumber))
                gpioController!.OpenPin(pin.PinNumber, pinMode);

            return pin;
        }

        public static PinObject[] OpenGpioPinOutput(this GpioController gpioController, PinObject[] gpioPins)
        {

            foreach (var gpioPin in gpioPins)
            {
                OpenGpioPinOutput(gpioController, gpioPin);
            }

            return gpioPins;
        }

        private static void OpenGpioPinInput(this GpioController gpioController, PinObject pin, PinMode pinMode = PinMode.Input)
        {
            if (!gpioController.IsPinOpen(pin.PinNumber))
                gpioController.OpenPin(pin.PinNumber, pinMode);
        }

        public static void OpenGpioPinInput(this GpioController gpioController, PinObject[] gpioPins)
        {
            foreach (var gpioPin in gpioPins)
            {
                OpenGpioPinInput(gpioController, gpioPin);
            }
        }

        private static void OpenGpioPinInputPullDown(this GpioController gpioController, PinObject pin, PinMode pinMode = PinMode.InputPullDown)
        {

            if (!gpioController.IsPinOpen(pin.PinNumber))
                gpioController.OpenPin(pin.PinNumber, pinMode);
        }

        public static void OpenGpioPinInputPullDown(this GpioController gpioController, PinObject[] gpioPins)
        {
            foreach (var gpioPin in gpioPins)
            {
                OpenGpioPinInputPullDown(gpioController, gpioPin);
            }
        }

        private static void OpenGpioPinInputPullUp(this GpioController gpioController, PinObject pin, PinMode pinMode = PinMode.InputPullUp)
        {
            if (!gpioController.IsPinOpen(pin.PinNumber))
                gpioController.OpenPin(pin.PinNumber, pinMode);
        }

        public static void OpenGpioPinInputPullUp(this GpioController gpioController, PinObject[] gpioPins)
        {
            foreach (var gpioPin in gpioPins)
            {
                OpenGpioPinInputPullUp(gpioController, gpioPin);
            }
        }

        private static bool GpioPinWriteLowValue(this GpioController gpioController, PinObject pin)
        {
            if (gpioController.IsPinOpen(pin.PinNumber))
            {
                var status = gpioController.GpioGetPinValue(pin);
                if (status != PinValue.Low)
                {
                    gpioController.Write(pin.PinNumber, PinValue.Low);

                }
            }
            else
            {
                gpioController.OpenGpioPinOutput(pin);
                var status = gpioController.GpioGetPinValue(pin);
                if (status != PinValue.Low)
                {
                    gpioController.Write(pin.PinNumber, PinValue.Low);

                }
            }

            return false;

        }

        public static Dictionary<int, bool?> GpioPinWriteLowValue(this GpioController gpioController, PinObject[] pins)
        {

            Dictionary<int, bool?> retVals = [];

            foreach (var pin in pins)
            {
                var status = gpioController.GpioPinWriteLowValue(pin);
                retVals.Add(pin.PinNumber, status);
            }

            return retVals;
        }

        private static bool GpioPinWriteHighValue(this GpioController gpioController, PinObject pin)
        {
            if (gpioController.IsPinOpen(pin.PinNumber))
            {
                var status = gpioController.GpioGetPinValue(pin);
                if (status != PinValue.High)
                {
                    gpioController.Write(pin.PinNumber, PinValue.High);

                }
            }
            else
            {
                gpioController.OpenGpioPinOutput(pin);
                var status = gpioController.GpioGetPinValue(pin);
                if (status != PinValue.High)
                {
                    gpioController.Write(pin.PinNumber, PinValue.High);

                }
            }


            return true;

        }

        public static Dictionary<int, bool?> GpioPinWriteHighValue(this GpioController gpioController, PinObject[] pins)
        {
            Dictionary<int, bool?> retVals = [];

            foreach (var pin in pins)
            {
                var status = gpioController.GpioPinWriteHighValue(pin);
                retVals.Add(pin.PinNumber, status);
            }

            return retVals;

        }

        private static bool? GpioGetPinValue(this GpioController gpioController, PinObject pin)
        {
            if (gpioController.IsPinOpen(pin.PinNumber))
            {
               
                return gpioController.Read(pin.PinNumber) == PinValue.High ? true : false;

            }

            return false;
        }

        public static Dictionary<int, bool?> GpioGetPinValue(this GpioController gpioController, PinObject[] pins)
        {
            Dictionary<int, bool?> retVals = [];
            foreach (var pin in pins)
            {
                retVals.Add(pin.PinNumber, GpioGetPinValue(gpioController, pin));
            }

            return retVals;
        }

        public static List<PinObject> GetPinsStatusArray(this GpioController gpioController, Dictionary<int, bool?> pinStates)
        {
            List<PinObject> pinStatusArray = [];
            foreach (KeyValuePair<int, bool?> kvp in pinStates)
            {
                PinObject pinObject = new()
                {
                    PinNumber = kvp.Key,
                    PinValue = kvp.Value
                };

                pinStatusArray.Add(pinObject);
            }
            return pinStatusArray;
        }


    }
}
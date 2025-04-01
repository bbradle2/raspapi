namespace raspapi.Extensions
{
    using System.Device.Gpio;

      public static class GpioExtensions
    {
        private static void CloseGpioPin(this GpioController gpioController, int pin)
        {
            if (gpioController.IsPinOpen(pin))
            {
                gpioController.GpioPinWriteLowValue(pin);
                gpioController.ClosePin(pin);
            }
        }

        public static void CloseGpioPin(this GpioController gpioController, int [] gpioPins)
        {
            foreach (var gpioPin in gpioPins)
            {
                CloseGpioPin(gpioController, gpioPin);
            }
        }

        private static int OpenGpioPinOutput(this GpioController gpioController, int pin, PinMode pinMode = PinMode.Output)
        {
            if (!gpioController.IsPinOpen(pin))
                gpioController.OpenPin(pin, pinMode);

            return pin;
        }

        public static int[] OpenGpioPinOutput(this GpioController gpioController, int[] gpioPins)
        {

            foreach (var gpioPin in gpioPins)
            {
                OpenGpioPinOutput(gpioController, gpioPin);
            }

            return gpioPins;
        }


        private static void OpenGpioPinInput(this GpioController gpioController, int pin, PinMode pinMode = PinMode.Input)
        {
            if (!gpioController.IsPinOpen(pin))
                gpioController.OpenPin(pin, pinMode);
        }

        public static void OpenGpioPinInput(this GpioController gpioController, int[] gpioPins)
        {
            foreach (var gpioPin in gpioPins)
            {
                OpenGpioPinInput(gpioController, gpioPin);
            }
        }

        private static void OpenGpioPinInputPullDown(this GpioController gpioController, int pin, PinMode pinMode = PinMode.InputPullDown)
        {

            if (!gpioController.IsPinOpen(pin))
                gpioController.OpenPin(pin, pinMode);
        }

        public static void OpenGpioPinInputPullDown(this GpioController gpioController, int[] gpioPins)
        {
            foreach (var gpioPin in gpioPins)
            {
                OpenGpioPinInputPullDown(gpioController, gpioPin);
            }
        }


        private static void OpenGpioPinInputPullUp(this GpioController gpioController, int pin, PinMode pinMode = PinMode.InputPullUp)
        {
            if (!gpioController.IsPinOpen(pin))
                gpioController.OpenPin(pin, pinMode);
        }

        public static void OpenGpioPinInputPullUp(this GpioController gpioController, int[] gpioPins)
        {
            foreach (var gpioPin in gpioPins)
            {
                OpenGpioPinInputPullUp(gpioController, gpioPin);
            }
        }


        private static void GpioPinWriteLowValue(this GpioController gpioController, int pin)
        {
            if (gpioController.IsPinOpen(pin))
            {
                var status = gpioController.GpioGetPinValue(pin);
                if (status != PinValue.Low)
                {
                    gpioController.Write(pin, PinValue.Low);
                    
                }
            }
            else
            {
                gpioController.OpenGpioPinOutput(pin);
                var status = gpioController.GpioGetPinValue(pin);
                if (status != PinValue.Low)
                {
                    gpioController.Write(pin, PinValue.Low);
                    
                }
            }

        }

        public static void GpioPinWriteLowValue(this GpioController gpioController, int[] pins)
        {

            foreach (var pin in pins)
            {
                gpioController.GpioPinWriteLowValue(pin);
            }
        }

        private static void GpioPinWriteHighValue(this GpioController gpioController, int pin)
        {
            if (gpioController.IsPinOpen(pin))
            {
                var status = gpioController.GpioGetPinValue(pin);
                if (status != PinValue.High)
                {
                    gpioController.Write(pin, PinValue.High);
                }
            }
            else
            {
                gpioController.OpenGpioPinOutput(pin);
                var status = gpioController.GpioGetPinValue(pin);
                if (status != PinValue.High)
                {
                    gpioController.Write(pin, PinValue.High);
                   
                }
            }

            
        }

        public static void GpioPinWriteHighValue(this GpioController gpioController, int[] pins)
        {
            foreach (var pin in pins)
            {
                gpioController.GpioPinWriteHighValue(pin);
            }
        }


        private static bool? GpioGetPinValue(this GpioController gpioController, int pin)
        {
            if (gpioController.IsPinOpen(pin))
            {
                if (gpioController.Read(pin) == PinValue.High)
                    return true;

                return false;
            }

            return null;
        }

        public static bool?[] GpioGetPinValue(this GpioController gpioController, int[] pins)
        {
            List<bool?> retVals = [];
            foreach (var pin in pins)
            {
                retVals.Add(GpioGetPinValue(gpioController, pin));
            }

            return [.. retVals];
        }

    }
}
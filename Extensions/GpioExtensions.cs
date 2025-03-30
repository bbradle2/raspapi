using System.Device.Gpio;
using raspapi.Interfaces;

namespace raspapi.Extensions
{
    public static class GpioExtensions
    {
        private static void CloseGpioPin(this GpioController gpioController, IGpioPin pin)
        {
            if (gpioController.IsPinOpen(pin.PinNumber))
            {
                gpioController.GpioPinWriteLowValue(pin);
                gpioController.ClosePin(pin.PinNumber);
            }
        }

        public static void CloseGpioPin(this GpioController gpioController, IGpioPin[] gpioPins)
        {
            foreach (var gpioPin in gpioPins)
            {
                CloseGpioPin(gpioController, gpioPin);
            }
        }

        private static IGpioPin OpenGpioPinOutput(this GpioController gpioController, IGpioPin pin, PinMode pinMode = PinMode.Output)
        {
            if (!gpioController.IsPinOpen(pin.PinNumber))
                gpioController.OpenPin(pin.PinNumber, pinMode);

            return pin;
        }

        public static IGpioPin[] OpenGpioPinOutput(this GpioController gpioController, IGpioPin[] gpioPins)
        {

            foreach (var gpioPin in gpioPins)
            {
                OpenGpioPinOutput(gpioController, gpioPin);
            }

            return gpioPins;
        }


        private static void OpenGpioPinInput(this GpioController gpioController, IGpioPin pin, PinMode pinMode = PinMode.Input)
        {
            if (!gpioController.IsPinOpen(pin.PinNumber))
                gpioController.OpenPin(pin.PinNumber, pinMode);
        }

        public static void OpenGpioPinInput(this GpioController gpioController, IGpioPin[] gpioPins)
        {
            foreach (var gpioPin in gpioPins)
            {
                OpenGpioPinInput(gpioController, gpioPin);
            }
        }

        private static void OpenGpioPinInputPullDown(this GpioController gpioController, IGpioPin pin, PinMode pinMode = PinMode.InputPullDown)
        {

            if (!gpioController.IsPinOpen(pin.PinNumber))
                gpioController.OpenPin(pin.PinNumber, pinMode);
        }

        public static void OpenGpioPinInputPullDown(this GpioController gpioController, IGpioPin[] gpioPins)
        {
            foreach (var gpioPin in gpioPins)
            {
                OpenGpioPinInputPullDown(gpioController, gpioPin);
            }
        }


        private static void OpenGpioPinInputPullUp(this GpioController gpioController, IGpioPin pin, PinMode pinMode = PinMode.InputPullUp)
        {
            if (!gpioController.IsPinOpen(pin.PinNumber))
                gpioController.OpenPin(pin.PinNumber, pinMode);
        }

        public static void OpenGpioPinInputPullUp(this GpioController gpioController, IGpioPin[] gpioPins)
        {
            foreach (var gpioPin in gpioPins)
            {
                OpenGpioPinInputPullUp(gpioController, gpioPin);
            }
        }


        private static void GpioPinWriteLowValue(this GpioController gpioController, IGpioPin pin)
        {
            if (gpioController.IsPinOpen(pin.PinNumber))
            {
                gpioController.Write(pin.PinNumber, PinValue.Low);
                pin.Status = false;
            }
            else
            {
                gpioController.OpenGpioPinOutput(pin);
                gpioController.Write(pin.PinNumber, PinValue.Low);
                pin.Status = false;
            }
        }
        
        public static void GpioPinWriteLowValue(this GpioController gpioController, IGpioPin[] pins)
        {
            foreach (var pin in pins)
            {
                gpioController.GpioPinWriteLowValue(pin);
            }
        }

        private static void GpioPinWriteHighValue(this GpioController gpioController, IGpioPin pin)
        {
            if (gpioController.IsPinOpen(pin.PinNumber))
            {
                gpioController.Write(pin.PinNumber, PinValue.High);
                pin.Status = true;
            }
            else
            {
                gpioController.OpenGpioPinOutput(pin);
                gpioController.Write(pin.PinNumber, PinValue.High);
                pin.Status = true;
            }
        }

        public static void GpioPinWriteHighValue(this GpioController gpioController, IGpioPin[] pins)
        {
            foreach (var pin in pins)
            {
                gpioController.GpioPinWriteHighValue(pin);
            }
        }


        private static bool? GpioGetPinValue(this GpioController gpioController, IGpioPin pin)
        {
            if (gpioController.IsPinOpen(pin.PinNumber))
            {
                if (gpioController.Read(pin.PinNumber) == PinValue.High)
                    return true;

                return false;
            }

            return null;
        }

        public static bool? [] GpioGetPinValue(this GpioController gpioController, IGpioPin [] pins)
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
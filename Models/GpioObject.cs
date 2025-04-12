using Iot.Device.Ili934x;

namespace raspapi.Models
{
    public class GpioObject
    {
        public int GpioNumber { get; set; }
        public bool? GpioValue { get; set; }
    }
}
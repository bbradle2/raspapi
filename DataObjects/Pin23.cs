using raspapi.Interfaces;

namespace raspapi.DataObjects
{
    public class Pin23: IPin
    {
        public int Pin { get; } = 23;
        public string Status { get; set; } = "Off";

        public string Function { get; set; } = "Led";

    }
}
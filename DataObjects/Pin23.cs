using raspapi.Contants;
using raspapi.Interfaces;

namespace raspapi.DataObjects
{
    public class Pin23: IPin
    {
        public int Pin { get; } = RaspBerryPiContants.PIN23;
        public string Status { get; set; } = "Off";

        public string Function { get; set; } = "Led";

    }
}
namespace raspapi.Models
{
    public class TemperatureInfoObject : BaseInfoObject
    {
        public decimal TemperatureFahrenheit { get; set; }
        public decimal TemperatureCelcius { get; set; }
    }
}
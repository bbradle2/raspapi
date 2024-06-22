namespace raspapi.Interfaces
{
    public interface IGpioPin
    {
        public int Pin { get; }
        public string Status { get; set; } 
        public string Description { get; set; }

    }
}
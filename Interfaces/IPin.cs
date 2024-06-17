namespace raspapi.Interfaces
{
    public interface IPin
    {
        public int Pin { get; }
        public string Status { get; set; } 

        public string Function { get; set; }

    }
}
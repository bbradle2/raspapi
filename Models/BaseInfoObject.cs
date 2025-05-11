namespace raspapi.Models
{
    public abstract class BaseInfoObject
    {
        public required string ProductName { get; set; }
        public required string Description { get; set; }
    }

}

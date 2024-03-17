using System.Text.Json.Serialization;

namespace raspapi.DataObjects
{
    public abstract class BaseInfoObject
    {

        public string? Name { get; set; }
        public string? Description { get; set; }
    }

}

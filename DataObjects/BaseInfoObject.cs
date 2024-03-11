using System.Text.Json.Serialization;

namespace first_test.DataObjects
{
    public abstract class BaseInfoObject
    {

        public string? Name { get; set; }
        public string? Description { get; set; }
    }

}

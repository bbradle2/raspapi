using System.Text.Json.Serialization;

namespace first_test.DataObjects
{
    public class SystemInfoObject : BaseInfoObject
    {

        public SystemInfoObject()
        {
            Name = "systeminfo";
            Description = "Raspberry PI 5 systeminfo";
        }

        public SystemObject[]? SystemObjects { get; set; }
        
        // public class Capabilities
        // {
        //     public string? Smp { get; set; }
        //     public bool Cp15Barrier { get; set; }
        //     public bool Setend { get; set; }
        //     public bool Swp { get; set; }
        //     public bool TaggedAddrDisabled { get; set; }
        // }

        public class SystemObject
        {
            public string? Id { get; set; }
            public string? Class { get; set; }
            public bool Claimed { get; set; }
            public string? Description { get; set; }
            public string? Product { get; set; }
            public string? Serial { get; set; }
            public int Width { get; set; }
           //public Capabilities? Capabilities { get; set; }
        }

    }
}
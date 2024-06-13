namespace raspapi.DataObjects
{
    public class CPUInfoObject : BaseInfoObject
    {
       
        public CPUObject[]? CPUObjects { get; set; }

        public class Capabilities
        {
            public string? Fp { get; set; }
            public string? Asimd { get; set; }
            public string? Evtstrm { get; set; }
            public string? Aes { get; set; }
            public string? Pmull { get; set; }
            public string? Sha1 { get; set; }
            public string? Sha2 { get; set; }
            public string? Crc32 { get; set; }
            public bool Atomics { get; set; }
            public bool Fphp { get; set; }
            public bool Asimdhp { get; set; }
            public bool Cpuid { get; set; }
            public bool Asimdrdm { get; set; }
            public bool Lrcpc { get; set; }
            public bool Dcpop { get; set; }
            public bool Asimddp { get; set; }
            public string? Cpufreq { get; set; }
        }

        public class CPUObject
        {
            public string? Id { get; set; }
            public string? Class { get; set; }
            public bool? Claimed { get; set; }
            public string? Description { get; set; }
            public string? Product { get; set; }
            public string? Physid { get; set; }
            public string? Businfo { get; set; }
            public string? Units { get; set; }
            public object? Size { get; set; }
            public object? Capacity { get; set; }
            public Capabilities? Capabilities { get; set; }
            public bool? Disabled { get; set; }
        }
    }
}
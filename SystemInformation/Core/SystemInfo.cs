using GameBoost.SystemInformation.Components;

namespace GameBoost.SystemInformation.Core
{
    public class SystemInfo()
    {
        public bool IsAdministrator { get; set; } = false;
        public OSInfo OS { get; set; }
        public CpuInfo CPU { get; set; }
        public GpuInfo GPU { get; set; }
        public MemoryInfo Memory { get; set; }
        public MotherboardInfo Motherboard { get; set; }
        public NetworkInfo Network { get; set; }
    }
}

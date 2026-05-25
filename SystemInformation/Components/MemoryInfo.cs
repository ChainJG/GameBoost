namespace GameBoost.SystemInformation.Components
{
    public class MemoryInfo
    {
        public ulong TotalPhysicalMemory { get; set; }
        public ulong AvailablePhysicalMemory { get; set; }
        public ulong TotalVirtualMemory { get; set; }
        public ulong AvailableVirtualMemory { get; set; }

        public string MemoryType { get; set; }        // DDR4 / DDR5
        public int SpeedMHz { get; set; }
        public int SlotsUsed { get; set; }
        public int TotalSlots { get; set; }

        public List<RamStickInfo> Sticks { get; set; }
    }
    public class RamStickInfo
    {
        public string Manufacturer { get; set; }
        public string Capacity { get; set; }
        public string Speed { get; set; }
        public string SerialNumber { get; set; } 
        public string PartNumber { get; set; }
    }
}

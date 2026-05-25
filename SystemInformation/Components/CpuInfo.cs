namespace GameBoost.SystemInformation.Components
{
    public class CpuInfo
    {
        public string Name { get; set; }              // Intel / AMD model
        public int CoreCount { get; set; }
        public int LogicalProcessors { get; set; }
        public string Manufacturer { get; set; }      // Intel / AMD
        public string Architecture { get; set; }      // x64
        public string Socket { get; set; }            // CPU socket type
        public string L2Cache { get; set; }
        public string L3Cache { get; set; }
        public string MaxClockSpeed { get; set; }     // MHz
        public string CurrentClockSpeed { get; set; } // MHz
        public string ProcessorId { get; set; }       // IMPORTANT (fingerprint)
    }
}

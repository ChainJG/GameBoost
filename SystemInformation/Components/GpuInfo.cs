namespace GameBoost.SystemInformation.Components
{
    public class GpuInfo
    {
        public string Name { get; set; }              // RTX 4070 etc
        public string DriverVersion { get; set; }
        public string VideoProcessor { get; set; }
        public string AdapterRAM { get; set; }
        public string VideoModeDescription { get; set; }
        public string PNPDeviceID { get; set; }       // useful for fingerprinting
        public int RefreshRate { get; set; }
        public string Status { get; set; }
    }
}

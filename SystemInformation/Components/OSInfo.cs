namespace GameBoost.SystemInformation.Components
{
    public class OSInfo
    {
        public string Name { get; set; }              // Windows 10 / 11
        public string Version { get; set; }           // 10.0.22631
        public string BuildNumber { get; set; }       // 22631
        public string Architecture { get; set; }      // 64-bit
        public string InstallDate { get; set; }       // WMI
        public string SystemDirectory { get; set; }   // C:\Windows\System32
        public string BootDevice { get; set; }        // \Device\HarddiskVolume1
        public bool Is64Bit { get; set; }
        public TimeSpan Uptime { get; set; }
    }
}

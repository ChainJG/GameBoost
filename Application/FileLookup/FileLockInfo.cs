namespace GameBoost.Application.FileLookup
{
    public sealed class FileLockInfo
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; } = "";
        public string ApplicationName { get; set; } = "";
        public string ServiceName { get; set; } = "";
        public string ApplicationType { get; set; } = "";
        public bool Restartable { get; set; }
    }
}

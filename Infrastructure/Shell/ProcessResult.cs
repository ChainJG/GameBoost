namespace GameBoost.Infrastructure.Shell
{
    public class ProcessResult
    {
        public bool Success { get; set; }

        public int ExitCode { get; set; }

        public string Output { get; set; }

        public string Error { get; set; }
    }
}

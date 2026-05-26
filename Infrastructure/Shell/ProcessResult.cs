namespace GameBoost.Infrastructure.Shell
{
    public class ProcessResult
    {
        public bool Success { get; set; }

        public int ExitCode { get; set; }

        public string Output { get; set; }

        public string Error { get; set; }


        public static ProcessResult Successful(int exitCode, string output = default) => new() { Success = exitCode == 0, Output = output, ExitCode = exitCode };
        public static ProcessResult Failed(int exitCode, string output = default) => new() { Success = false, Error = output, ExitCode = exitCode };
    }
}

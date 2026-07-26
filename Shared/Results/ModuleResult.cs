using GameBoost.Core.Interfaces;

namespace GameBoost.Shared.Results
{
    public class ModuleResult : IResult
    {
        public bool Success { get; init; } = true;
        public bool Cancelled { get; init; } = false;
        public string Message { get; init; } = string.Empty;
        public int ExitCode { get; init; } = 0;
        public ResultType Status { get; init; } = ResultType.Unknown;
        public static ModuleResult Successful(string message = "Successfully completed") =>
            new() { Success = true, Status = ResultType.Successful, Message = message };
        public static ModuleResult Failed(string message = "Failed to complete", ResultType status = ResultType.Failed) =>
            new() { Success = false, Status = status, Message = message };
        public static ModuleResult Cancel(string message = "Operation cancelled") =>
            new() { Success = false, Cancelled = true, Status = ResultType.Cancelled, Message = message };
    }
}

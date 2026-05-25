using GameBoost.Core.Interfaces;

namespace GameBoost.Shared.Results
{
    public class ModuleResult : IResult
    {
        public bool Success { get; init; } = true;
        public string Message { get; init; } = string.Empty;
        public ResultType Status { get; init; } = ResultType.Unknown;

        public static ModuleResult Successful(string message = "Successfully completed") =>
            new() { Success = true, Status = ResultType.Successful, Message = message };

        public static ModuleResult Failed(string message = "Failed to complete", ResultType status = ResultType.Failed) =>
            new() { Success = false, Status = status, Message = message };
    }
}

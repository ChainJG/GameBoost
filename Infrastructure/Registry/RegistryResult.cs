using GameBoost.Core.Interfaces;
using Microsoft.Win32;

namespace GameBoost.Infrastructure.Registry
{
    public class RegistryResult : IResult
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public RegistryKey? Key { get; init; }
        public object? Value { get; init; }
        public ResultType Status { get; init; } = ResultType.Unknown;

        public static RegistryResult Successful(string message, object value = default) =>
            new() { Success = true, Status = ResultType.Successful, Message = message, Value = value };

        public static RegistryResult Failed(string message, ResultType status = ResultType.Failed) =>
            new() { Success = false, Status = status, Message = message };
    }
}

using GameBoost.Scripts.Models;

namespace GameBoost.Scripts.Helpers
{
    public static class ModuleHelper
    {
        public static ModuleResult CreateSuccessfulResult(string message) =>
            new() { Success = true, Status = ResultType.Successful, Message = message };

        public static ModuleResult CreateFailedResult(string message, ResultType status = ResultType.Failed) =>
            new() { Success = false, Status = status, Message = message };
    }
}

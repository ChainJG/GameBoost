namespace GameBoost.Scripts.Models
{
    public class ModuleResult
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public ResultType Status { get; set; } = ResultType.Unknown;
    }
}

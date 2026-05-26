namespace GameBoost.Shared.Results
{
    public class ModuleShareResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new();

        public void AddFailure(string message)
        {
            Success = false;
            Errors.Add(message);
        }
    }
}

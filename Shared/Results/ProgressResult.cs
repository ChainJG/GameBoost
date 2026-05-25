namespace GameBoost.Shared.Results
{
    public class ProgressResult
    {
        public string Status { get; set; } = string.Empty;
        public int Percent { get; set; } = 0;

        public ProgressResult(string status, int percent)
        {
            Status = status;
            Percent = percent;
        }
    }
}

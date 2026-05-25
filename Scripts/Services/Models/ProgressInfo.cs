namespace GameBoost.Scripts.Services.Models
{
    public class ProgressInfo
    {
        public string Status { get; set; } = string.Empty;
        public int Percent { get; set; } = 0;

        public ProgressInfo(string status, int percent)
        {
            Status = status;
            Percent = percent;
        }
    }
}

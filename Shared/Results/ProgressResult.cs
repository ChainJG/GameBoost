namespace GameBoost.Shared.Results
{
    public class ProgressResult(string StatusText, int Percentage = 0)
{
        public string Status => StatusText;
        public int Percent => Percentage;
    }
}

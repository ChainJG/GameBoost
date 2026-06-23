using System.Text.RegularExpressions;

namespace GameBoost.Shared.Helpers
{
    public static class StringHelper
    {
        public static string CleanDisplayName(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var result = value;

            result = Regex.Replace(result, @"\([^)]*\)", " ");
            result = result.Replace("-", " ").Replace("_", " ").Replace(".", " ");
            result = Regex.Replace(result, @"[^\p{L}\p{N}\s]", " ");
            result = Regex.Replace(result, @"\s+", " ");

            return result.Trim();
        }

        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan < TimeSpan.Zero)
                return $"Invalid negative time span ({timeSpan})";

            if (timeSpan.TotalDays >= 1)
                return $"{timeSpan.TotalDays:N1} days";

            if (timeSpan.TotalHours >= 1)
                return $"{timeSpan.TotalHours:N1} hours";

            if (timeSpan.TotalMinutes >= 1)
                return $"{timeSpan.TotalMinutes:N1} minutes";

            return $"{timeSpan.TotalSeconds:N0} seconds";
        }
    }
}
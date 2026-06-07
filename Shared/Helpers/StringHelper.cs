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
    }
}
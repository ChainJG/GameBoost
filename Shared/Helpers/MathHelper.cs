namespace GameBoost.Shared.Helpers
{
    public static class MathHelper
    {
        private const long BytesPerKilobyte = 1024L;
        private const long BytesPerMegabyte = BytesPerKilobyte * BytesPerKilobyte;
        private const long BytesPerGigabyte = BytesPerMegabyte * BytesPerKilobyte;

        public static long GigabytesToBytes(double gigabytes)
        {
            if (gigabytes < 0)
                throw new ArgumentOutOfRangeException(nameof(gigabytes), "Gigabytes cannot be negative");

            return (long)(gigabytes * BytesPerGigabyte);
        }

        public static int ToPercentageInt(double value, double max)
        {
            if (max == 0)
                return 0;

            return (int)Math.Round((value / max) * 100.0);
        }
        public static double ToPercentage(double value, double max)
        {
            if (max == 0)
                return 0;

            var percent = (value / max) * 100.0;

            return Math.Clamp(percent, 0, 100);
        }

        public static string FormatBytes(ulong? bytes)
        {
            return bytes is null
                ? "Unknown"
                : FormatBytes((double)bytes.Value);
        }

        public static string FormatBytes(long bytes)
        {
            return FormatBytes((double)bytes);
        }

        public static string FormatBytes(double bytes)
        {
            string[] units = ["B", "KB", "MB", "GB", "TB"];

            var unitIndex = 0;

            while (bytes >= 1024 && unitIndex < units.Length - 1)
            {
                bytes /= 1024;
                unitIndex++;
            }

            return $"{FormatUnitValue(bytes)} {units[unitIndex]}";
        }

        public static string FormatMilliseconds(double milliseconds)
        {
            if (milliseconds < 1)
                return "<1ms";

            if (milliseconds < 1000)
                return $"{milliseconds:0.##}ms";

            var time = TimeSpan.FromMilliseconds(milliseconds);

            if (time.TotalMinutes < 1)
                return $"{time.TotalSeconds:0.##}s";

            if (time.TotalHours < 1)
                return $"{time.TotalMinutes:0.##}m";

            return $"{time.TotalHours:0.##}h";
        }

        private static string FormatUnitValue(double value)
        {
            var nearestWholeNumber = Math.Round(value);
            var distanceFromWholeNumber = Math.Abs(value - nearestWholeNumber);

            // If the value is very close to a whole number, show it as whole.
            if (distanceFromWholeNumber <= 0.05)
                return nearestWholeNumber.ToString("0");

            return value.ToString("0.##");
        }
    }
}

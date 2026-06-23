namespace GameBoost.Shared.Helpers
{
    public static class MathHelper
    {
        private const long BytesPerKilobyte = 1024L;
        private const long BytesPerMegabyte = BytesPerKilobyte * BytesPerKilobyte;
        private const long BytesPerGigabyte = BytesPerMegabyte * BytesPerKilobyte;

        public static long GigabytesToBytes(double gigabytes)
        {
            if (double.IsNaN(gigabytes) || double.IsInfinity(gigabytes))
                throw new ArgumentOutOfRangeException(nameof(gigabytes), "Gigabytes must be a valid number");

            if (gigabytes < 0)
                throw new ArgumentOutOfRangeException(nameof(gigabytes), "Gigabytes cannot be negative");

            return checked((long)(gigabytes * BytesPerGigabyte));
        }

        public static long MegabytesToBytes(double megabytes)
        {
            if (double.IsNaN(megabytes) || double.IsInfinity(megabytes))
                throw new ArgumentOutOfRangeException(nameof(megabytes), "Megabytes must be a valid number");

            if (megabytes < 0)
                throw new ArgumentOutOfRangeException(nameof(megabytes), "Megabytes cannot be negative");

            return checked((long)(megabytes * BytesPerMegabyte));
        }

        public static int ToPercentageInt(double value, double max)
        {
            if (max == 0)
                return 0;

            var percent = ToPercentage(value, max);

            return (int)Math.Round(percent);
        }
        public static double ToPercentage(double value, double max)
        {
            if (max == 0)
                return 0;

            var percent = (value / max) * 100.0;

            return Math.Clamp(percent, 0, 100);
        }
        public static int ClampToPercentage(double value)
        {
            return Math.Clamp(
                Convert.ToInt32(Math.Round(value)),
                0,
                100);
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

            if (bytes <= 0)
                return $"Empty";

            while (bytes >= 1024 && unitIndex < units.Length - 1)
            {
                bytes /= 1024;
                unitIndex++;
            }

            return $"{FormatUnitValue(bytes)} {units[unitIndex]}";
        }

        public static double ToBytes(double value, string unit)
        {
            return unit.ToUpperInvariant() switch
            {
                "KB" => value * 1024d,
                "MB" => value * 1024d * 1024d,
                "GB" => value * 1024d * 1024d * 1024d,
                _ => value
            };
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

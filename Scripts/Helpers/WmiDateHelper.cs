using System.Management;

namespace GameBoost.Scripts.Helpers
{
    public static class WmiDateHelper
    {
        /// Converts a WMI datetime string into a formatted date string.
        public static string FormatDate(
            string? wmiDate,
            string format = "yyyy-MM-dd")
        {
            if (string.IsNullOrWhiteSpace(wmiDate))
                return "Unknown";

            try
            {
                var date =
                    ManagementDateTimeConverter
                    .ToDateTime(wmiDate);

                return date.ToString(format);
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}

using System.Net.NetworkInformation;

namespace GameBoost.SystemInformation.Providers
{
    public static class SystemHealthProvider
    {
        public static bool IsInternetAvailable()
        {
            try
            {
                using var ping = new Ping();
                var reply = ping.Send("8.8.8.8", 1000);
                return reply?.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }
    }
}

using GameBoost.Shared.Results;

namespace GameBoost.Application.Startup
{
    public static class StartupProgressMapper
    {
        public static IProgress<ProgressResult> CreateRange(
            IProgress<ProgressResult> parentProgress,
            int startPercent,
            int endPercent,
            string? fallbackStatus = null)
        {
            return new Progress<ProgressResult>(progress =>
            {
                var localPercent = Math.Clamp(progress.Percent, 0, 100);

                var mappedPercent = MapPercent(
                    localPercent,
                    startPercent,
                    endPercent);

                parentProgress.Report(
                    new ProgressResult(
                        string.IsNullOrWhiteSpace(progress.Status)
                            ? fallbackStatus ?? "Initialising..."
                            : progress.Status,
                        mappedPercent));
            });
        }

        public static (int StartPercent, int EndPercent) GetRange(
            int index,
            int totalUnits)
        {
            if (totalUnits <= 0)
                return (0, 100);

            var start = (int)Math.Round(index * 100d / totalUnits);
            var end = (int)Math.Round((index + 1) * 100d / totalUnits);

            return (
                Math.Clamp(start, 0, 100),
                Math.Clamp(end, 0, 100));
        }

        private static int MapPercent(
            int localPercent,
            int startPercent,
            int endPercent)
        {
            var range = endPercent - startPercent;

            return Math.Clamp(
                startPercent + (int)Math.Round(range * (localPercent / 100d)),
                0,
                100);
        }
    }
}
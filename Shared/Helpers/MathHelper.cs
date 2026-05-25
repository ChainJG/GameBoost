namespace GameBoost.Shared.Helpers
{
    public static class MathHelper
    {
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
    }
}

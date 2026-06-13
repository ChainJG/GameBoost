namespace GameBoost.Features.Modules.WindowsModules.PowerOptions.Options
{
    public sealed class PciExpressLinkStateOption
    {
        public string Name { get; }

        public int Value { get; }

        private PciExpressLinkStateOption(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public static PciExpressLinkStateOption Off { get; } =
            new("Off", 0);

        public static PciExpressLinkStateOption ModeratePowerSavings { get; } =
            new("Moderate power savings", 1);

        public static PciExpressLinkStateOption MaximumPowerSavings { get; } =
            new("Maximum power savings", 2);

        public static PciExpressLinkStateOption Unknown { get; } =
            new("Unknown", -1);

        public override string ToString()
        {
            return Name;
        }
    }
}

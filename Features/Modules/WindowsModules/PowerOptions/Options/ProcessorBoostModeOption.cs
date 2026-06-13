namespace GameBoost.Features.Modules.WindowsModules.PowerOptions.Options
{
    public sealed class ProcessorBoostModeOption
    {
        public string Name { get; }

        public int Value { get; }

        public string Description { get; }

        private ProcessorBoostModeOption(
            string name,
            int value,
            string description)
        {
            Name = name;
            Value = value;
            Description = description;
        }

        public static ProcessorBoostModeOption Disabled { get; } =
            new(
                "Disabled",
                0,
                "Disables processor boost behaviour. Lower heat and power usage, but reduced peak CPU performance");

        public static ProcessorBoostModeOption Enabled { get; } =
            new(
                "Enabled",
                1,
                "Allows normal processor boost behaviour");

        public static ProcessorBoostModeOption Aggressive { get; } =
            new(
                "Aggressive",
                2,
                "Uses a more aggressive boost policy for stronger gaming-focused CPU responsiveness");

        public static ProcessorBoostModeOption EfficientEnabled { get; } =
            new(
                "Efficient Enabled",
                3,
                "Allows boost behaviour with a more efficiency-focused policy");

        public static ProcessorBoostModeOption EfficientAggressive { get; } =
            new(
                "Efficient Aggressive",
                4,
                "Uses aggressive boost behaviour while still trying to remain more efficient");

        public static ProcessorBoostModeOption AggressiveAtGuaranteed { get; } =
            new(
                "Aggressive At Guaranteed",
                5,
                "Uses aggressive boost behaviour once the processor reaches its guaranteed performance level");

        public static ProcessorBoostModeOption EfficientAggressiveAtGuaranteed { get; } =
            new(
                "Efficient Aggressive At Guaranteed",
                6,
                "Uses efficient aggressive boost behaviour once the processor reaches its guaranteed performance level");

        public static ProcessorBoostModeOption Unknown { get; } =
            new(
                "Unknown",
                -1,
                "The current processor boost mode could not be detected");

        public override string ToString()
        {
            return Name;
        }
    }
}
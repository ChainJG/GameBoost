namespace GameBoost.Features.Modules.WindowsModules.PowerOptions.Options
{
    public sealed class PowerPlanOptions
    {
        public required string Guid { get; init; }

        public required string Name { get; init; }

        public bool IsActive { get; init; }

        public override string ToString()
        {
            return Name;
        }
    }
}
namespace GameBoost.Core.Interfaces
{
    public interface IRequiredModule
    {
        bool SystemReboot { get; }
        bool Admin { get; }
    }
}

namespace GameBoost.Core.Interfaces
{
    public interface IRequireModule
    {
        bool SystemReboot { get; }
        bool Admin { get; }
    }
}


using GameBoost.SystemInformation.Core;

namespace GameBoost.Core.Interfaces
{
    public interface ISystemInfoStep
    {
        string Name { get; }
        Task ExecuteAsync(SystemInfo info);
    }
}

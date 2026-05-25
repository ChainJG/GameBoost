
using GameBoost.SystemInformation.Core;

namespace GameBoost.Core.Interfaces
{
    public interface ISystemInfoStep
    {
        string Name { get; }
        Task EcecuteAsync(SystemInfo info);
    }
}

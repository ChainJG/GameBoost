using GameBoost.Infrastructure.Registry;
using GameBoost.Infrastructure.Services;
using GameBoost.Shared.Results;

namespace GameBoost.Core.Interfaces
{
    public interface IActionModule 
    {
        string Status { get; set; }

        ToggleType ToggleType { get; set; }

        RegistryEditInfo[] RegistryEdits { get; set; }
        ServiceEditInfo[] ServiceEdits { get; set; }

        Task<ModuleResult> ExecuteAsync();
    }
}

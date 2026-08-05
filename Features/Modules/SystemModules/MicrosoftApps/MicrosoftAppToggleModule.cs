using GameBoost.Core.Interfaces;
using GameBoost.Infrastructure.MicrosoftApps.Models;
using GameBoost.Infrastructure.MicrosoftApps.Services;
using GameBoost.Shared.Results;

namespace GameBoost.Features.Modules.SystemModules.MicrosoftApps
{
    public sealed class MicrosoftAppToggleModule(MicrosoftAppDefinition app) : IActionModule, IRequiredModule
    {
        private readonly MicrosoftAppDefinition _app = app;

        public string Name => $"{_app.DisplayName}";

        #region IRequiredModule
        public bool Admin => true;
        public bool SystemReboot => false;
        #endregion

        public async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var installed = await AppxPackageOperationService.IsInstalledForCurrentUserAsync(
                _app.PackageName,
                token);

            return ActionRefreshResult.Status(installed ? "Installed" : "Not installed");
        }

        public async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var installed = await AppxPackageOperationService.IsInstalledForCurrentUserAsync(
                _app.PackageName,
                token);

            if (installed)
            {
                return await AppxPackageOperationService.UninstallPackageAsync(
                    _app,
                    token);
            }

            return await AppxPackageOperationService.InstallPackageAsync(
                _app,
                token);
        }
    }
}
using GameBoost.Infrastructure.Installers.Models;
using GameBoost.Shared.Results;

namespace GameBoost.Infrastructure.Installers.Services
{
    public sealed class ApplicationInstallerService
    {
        public static Task<ModuleResult> InstallAsync(AppInstallDefinition app, IProgress<ProgressResult>? progress, CancellationToken token)
        {
            return app.Provider switch
            {
                AppInstallProvider.Winget => WingetInstallProvider.InstallAsync(app, progress, token),
                _ => Task.FromResult(ModuleResult.Failed($"Unsupported installer provider: {app.Provider}"))
            };
        }
    }
}
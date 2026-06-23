using GameBoost.Infrastructure.Installers.Models;
using GameBoost.Shared.Results;

namespace GameBoost.Infrastructure.Installers.Services
{
    public sealed class ApplicationInstallerService
    {
        private readonly WingetInstallProvider _wingetProvider = new();

        public Task<AppInstallResult> InstallAsync(AppInstallDefinition app, IProgress<ProgressResult>? progress, CancellationToken token)
        {
            return app.Provider switch
            {
                AppInstallProvider.Winget => _wingetProvider.InstallAsync(app, progress, token),
                _ => Task.FromResult(new AppInstallResult
                {
                    AppId = app.Id,
                    DisplayName = app.DisplayName,
                    Success = false,
                    Cancelled = false,
                    Message = $"Unsupported installer provider: {app.Provider}"
                })
            };
        }
    }
}
using GameBoost.Application;
using GameBoost.Application.Diagnostics;
using GameBoost.Application.Startup;
using GameBoost.Core.Debugger;
using GameBoost.MVVM.Core;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Results;
using System.Diagnostics;
using System.Reflection;

namespace GameBoost.MVVM.SplashScreen
{
    public class SplashScreenViewModel(StartupService startupService) : ObservableObject
    {
        private readonly StartupService _startupService = startupService;

        private string _statusMessage = "Initialising...";
        public string StatusMessage
        {
            get => _statusMessage;
            set => Set(ref _statusMessage, value);
        }

        private int _progressPercentage = 0;
        public int ProgressPercentage
        {
            get => _progressPercentage;
            set => Set(ref _progressPercentage, value);
        }

        private string _versionText = UIHelper.VersionText;
        public string VersionText
        {
            get => _versionText;
            set => Set(ref _versionText, value);
        }

        public event Action<bool>? StartupCompleted;

        public async Task InitialiseApplicationAsync(
            Func<IProgress<ProgressResult>, CancellationToken, Task>? initialiseMainViewModel = null,
            CancellationToken token = default)
        {
            try
            {
                var progress = new Progress<ProgressResult>(UpdateProgress);

                var result = await GameBoostContext.Diagnostic.TrackAsync(
                    category: "Startup",
                    operationType: DiagnosticOperationType.Initialise,
                    name: "Application Startup",
                    source: GetType().Name,
                    operation: innerToken => _startupService.InitialiseAsync(
                        progress,
                        initialiseMainViewModel,
                        innerToken),
                    token: token,
                    metadata: new Dictionary<string, string?>
                    {
                        ["StartupType"] = "SplashScreenInitialisation",
                        ["HasMainViewModelInitialiser"] = (initialiseMainViewModel is not null).ToString(),
                        ["ViewModel"] = GetType().FullName
                    });

                CompleteStartupAsync(result.Success);
            }
            catch (Exception ex)
            {
#if DEBUG
                GameBoostDebug.Error("Error initialising application", ex);
#endif

                StartupCompleted?.Invoke(false);
            }
        }

        private void UpdateProgress(ProgressResult info)
        {
            StatusMessage = info.Status;
            ProgressPercentage = info.Percent;
        }

        private void CompleteStartupAsync(bool success)
        {
            StartupCompleted?.Invoke(success);
        }
    }
}

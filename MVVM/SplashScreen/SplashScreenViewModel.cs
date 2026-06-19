using GameBoost.Application.Startup;
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

                var result = await _startupService.InitialiseAsync(
                    progress,
                    initialiseMainViewModel,
                    token);

                await CompleteStartupAsync(result.Success);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error initialising application: {ex.Message}");
#endif

                StartupCompleted?.Invoke(false);
            }
        }

        private void UpdateProgress(ProgressResult info)
        {
            StatusMessage = info.Status;
            ProgressPercentage = info.Percent;
        }

        private async Task CompleteStartupAsync(bool success)
        {
            StartupCompleted?.Invoke(success);
        }
    }
}

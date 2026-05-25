using GameBoost.Core.Services;
using GameBoost.MVVM.Core;
using GameBoost.Scripts.Helper;
using GameBoost.Scripts.Services.Models;
using GameBoost.Scripts.Services.RestorePoint;
using GameBoost.SystemInformation.Core;
using GameBoost.Update;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace GameBoost.MVVM.SplashScreen
{
    public class SplashScreenViewModel : ObservableObject
    {
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

        private string _versionText = $"v{Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "Unknown"}";
        public string VersionText
        {
            get => _versionText;
            set => Set(ref _versionText, value);
        }

        public event Action<bool>? InitialisationComplete;

        public async Task InitialiseApplicationAsync()
        {
            try
            {
                var progress = new Progress<ProgressInfo>(info =>
                {
                    StatusMessage = info.Status;
                    ProgressPercentage = info.Percent;
                });

                await LoadSystemInfo(progress);

                await CheckUpdate(progress);

                await CheckRestorePoints(progress);

                await CompleteInitialise(progress);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error initialising application: {ex.Message}");
#endif

                InitialisationComplete?.Invoke(false);
            }
        }

        private async Task CompleteInitialise(IProgress<ProgressInfo> progress)
        {
            await Task.Delay(500);

            progress.Report(new ProgressInfo("Initialisation complete", 100));

            await Task.Delay(500);
            InitialisationComplete?.Invoke(true);
        }

        private static async Task LoadSystemInfo(Progress<ProgressInfo> progress)
        {
            var systemLoader = new SystemInfoLoader();
            var sysInfo = await systemLoader.LoadAsync(progress);

            AppServices.SystemInfo = sysInfo;
        }

        private static async Task CheckUpdate(Progress<ProgressInfo> progress)
        {
            // Checking GitHub releases for updates
            var update = await GitHubUpdateChecker.CheckForUpdatesAsync(progress);

            // Cache the result
            AppServices.UpdateInfo = update;
        }

        private static async Task CheckRestorePoints(Progress<ProgressInfo> progress)
        {
            // Check if we have an active restore point (GameBoost Description)
            var activeRestorePoint = await RestorePointService.CheckActiveRestorePoint(progress);

            if (!activeRestorePoint)
            {
                // Prompt to create a restore point
                var wantsRestore = MessageBox.Show(
                    "Your system does not have an active restore point\nDo you want to create one now?",
                    "Restore Point",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes;

                if (wantsRestore)
                {
                    // Create a restore point
                    await RestorePointService.CreateProtectionPointAsync(progress);
                }
            }

            // Cache the result
            AppServices.HasActiveRestorePoint = activeRestorePoint;
        }
    }
}

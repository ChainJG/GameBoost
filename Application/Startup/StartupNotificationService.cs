using GameBoost.Application.Titlebar;
using GameBoost.Core;
using GameBoost.Features.Updates;
using GameBoost.MVVM.Core;
using GameBoost.MVVM.UserControls.Shared.Titlebar;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;

namespace GameBoost.Application.Startup
{
    public sealed class StartupNotificationService(TitleBarActionService titleBarActionService)
    {
        private readonly TitleBarActionService _titleBarActions = titleBarActionService;

        public void AddStartupActions()
        {
            AddUpdateAvailableAction(GameBoostContext.UpdateInfo);
            AddRestorePointAction();
        }

        private void AddUpdateAvailableAction(UpdateReleaseInfo? updateInfo)
        {
            if (updateInfo is null || !updateInfo.IsUpdateAvailable)
                return;

            TitleBarActionViewModel? action = null;

            action = new TitleBarActionViewModel
            {
                Key = "UpdateAvailable",
                Title = "Update available",
                Message = $"A new update is available: {updateInfo.Version}\n\n{updateInfo.Notes}",
                Icon = PackIconKind.ArrowDownCircle,
                Foreground = GetBrush("GreenColour", Brushes.ForestGreen),
                Command = new AsyncRelayCommand(async () =>
                {
                    if (action is null)
                        return;

                    await _titleBarActions.RunAsync(
                        action,
                        () => GameBoostServices.ShowUpdateDialog(updateInfo));
                })
            };
            _titleBarActions.AddOrReplace(action);
        }

        private void AddRestorePointAction()
        {
            if (GameBoostContext.HasActiveRestorePoint)
                return;

            TitleBarActionViewModel? action = null;

            action = new TitleBarActionViewModel
            {
                Key = "RestorePoint",
                Title = "Restore point recommended",
                Message = "Your system does not have an active GameBoost restore point.",
                Icon = PackIconKind.BackupRestore,
                Foreground = GetBrush("DangerColour", Brushes.LightGoldenrodYellow),
                Command = new AsyncRelayCommand(async () =>
                {
                    if (action is null)
                        return;

                    await _titleBarActions.RunAsync(
                        action,
                        () => GameBoostServices.ShowRestorePointDialog());
                })
            };

            _titleBarActions.AddOrReplace(action);
        }

        private static Brush GetBrush(string resourceKey, Brush fallback) => System.Windows.Application.Current.TryFindResource(resourceKey) as Brush ?? fallback;
    }
}
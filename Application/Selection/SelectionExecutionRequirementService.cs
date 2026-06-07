using GameBoost.Application.Titlebar;
using GameBoost.Core;
using GameBoost.MVVM.Core;
using GameBoost.MVVM.UserControls.Shared.Titlebar;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions.Misc;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;

namespace GameBoost.Application.Selection
{
    public sealed class SelectionExecutionRequirementService(TitleBarActionService titleBarActions)
    {
        private readonly TitleBarActionService _titleBarActions = titleBarActions;
        public void HandleRequirements(ExecutionRequirementsEventArgs args)
        {
            if (args.RequiresAdmin)
                AddAdminRequiredAction(args.AdminRequiredActions);

            if (args.RequiresRestart)
                AddRestartRequiredAction(args.RestartRequiredActions);
        }

        private void AddAdminRequiredAction(IReadOnlyList<string> actionTitles)
        {
            var message = actionTitles.Count == 0
                ? "Some optimisations require administrator permission."
                : "Some optimisations require administrator permission:\n\n" +
                  string.Join(Environment.NewLine, actionTitles.Select(title => $"• {title}"));

            TitleBarActionViewModel? action = null;

            action = new TitleBarActionViewModel
            {
                Key = "AdminRequired",
                Title = "Administrator required",
                Message = message,
                Icon = PackIconKind.ShieldAlert,
                Foreground = GetBrush("InfoColour", Brushes.AliceBlue),
                Command = new AsyncRelayCommand(async () =>
                {
                    if (action is null)
                        return;

                    await _titleBarActions.RunAsync(
                        action,
                        () => GameBoostServices.ShowRestartAdministratorDialog(message));
                })
            };

            _titleBarActions.AddOrReplace(action);
        }

        private void AddRestartRequiredAction(IReadOnlyList<string> actionTitles)
        {
            var message = actionTitles.Count == 0
                ? "Some optimisations require a restart before they fully take effect."
                : "Some optimisations require a restart before they fully take effect:\n\n" +
                  string.Join(Environment.NewLine, actionTitles.Select(title => $"• {title}"));

            TitleBarActionViewModel? action = null;

            action = new TitleBarActionViewModel
            {
                Key = "RestartRequired",
                Title = "Restart required",
                Message = message,
                Icon = PackIconKind.AlertCircle,
                Foreground = GetBrush("DangerColour", Brushes.IndianRed),
                Command = new AsyncRelayCommand(async () =>
                {
                    if (action is null)
                        return;

                    await _titleBarActions.RunAsync(
                        action,
                        () => GameBoostServices.ShowRestartDialog(message));
                })
            };

            _titleBarActions.AddOrReplace(action);
        }

        private static Brush GetBrush(string resourceKey, Brush fallback)
        {
            return System.Windows.Application.Current.TryFindResource(resourceKey) as Brush ?? fallback;
        }
    }
}
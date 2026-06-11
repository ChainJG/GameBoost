using GameBoost.Application.Titlebar;
using GameBoost.Core;
using GameBoost.Core.EventArguments;
using GameBoost.MVVM.Core;
using GameBoost.MVVM.UserControls.Shared.Titlebar;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;

namespace GameBoost.Application.Selection.Services
{
    public sealed class SelectionExecutionRequirementService(TitleBarActionService titleBarActions)
    {
        private readonly TitleBarActionService _titleBarActions = titleBarActions;

        private readonly HashSet<string> _adminRequiredActions = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _restartRequiredActions = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterExecutedAction(SelectionActionCardViewModelBase action)
        {
            if (ShouldShowAdminRequiredAction(action))
                _adminRequiredActions.Add(action.Title);

            if (action.RequiresReboot)
                _restartRequiredActions.Add(action.Title);

            RefreshTitleBarActions();
        }

        public void RegisterExecutedActions(IEnumerable<SelectionActionCardViewModelBase> actions)
        {
            foreach (var action in actions)
            {
                if (ShouldShowAdminRequiredAction(action))
                    _adminRequiredActions.Add(action.Title);

                if (action.RequiresReboot)
                    _restartRequiredActions.Add(action.Title);
            }

            RefreshTitleBarActions();
        }

        public void HandleRequirements(ExecutionRequirementsEventArgs args)
        {
            foreach (var title in args.AdminRequiredActions)
                _adminRequiredActions.Add(title);

            foreach (var title in args.RestartRequiredActions)
                _restartRequiredActions.Add(title);

            RefreshTitleBarActions();
        }

        public void Clear()
        {
            _adminRequiredActions.Clear();
            _restartRequiredActions.Clear();

            _titleBarActions.Remove("AdminRequired");
            _titleBarActions.Remove("RestartRequired");
        }

        private void RefreshTitleBarActions()
        {
            if (_adminRequiredActions.Count > 0)
                AddAdminRequiredAction(_adminRequiredActions.OrderBy(title => title).ToList());

            if (_restartRequiredActions.Count > 0)
                AddRestartRequiredAction(_restartRequiredActions.OrderBy(title => title).ToList());
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

        private static bool ShouldShowAdminRequiredAction(SelectionActionCardViewModelBase action)
            => action.RequiresAdmin &&
                   !GameBoostServices.IsAdministrator();

        private static Brush GetBrush(string resourceKey, Brush fallback)
            => System.Windows.Application.Current.TryFindResource(resourceKey) as Brush ?? fallback;
    }
}
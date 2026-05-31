using GameBoost.Core.Interfaces;
using GameBoost.MVVM.Core;
using GameBoost.Shared.Results;
using MaterialDesignThemes.Wpf;
using System.Diagnostics;

namespace GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions
{
    public abstract class SelectionActionCardViewModelBase : ObservableObject, ISelectionButton
    {
        public SelectionFeatureViewModel? Parent { get; internal set; }

        public required string Title { get; set; }
        public required PackIconKind Icon { get; set; }

        private string _status = string.Empty;
        public string Status { get => _status; set => Set(ref _status, value); }

        private bool _isChecked = false;
        public bool IsChecked 
        {
            get => _isChecked;
            set
            {
                if (!Set(ref _isChecked, value))
                    return;

                Parent?.OnActionSelectionChanged(this);
            }
        }

        private ModuleResult? _lastResult;
        public ModuleResult? LastResult { get => _lastResult; set => Set(ref _lastResult, value); }

        public IActionModule? Module { get; set; }

        public async Task RefreshStatusSafeAsync(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                Status = await RefreshStatusAsync(token);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error in RefreshStatusAsync: {ex.Message}");
#endif
                Status = "Failed to refresh";
            }
        }

        public async Task<ModuleResult> ExecuteSafeAsync(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                LastResult = await ExecuteAsync(token);

                Status = await RefreshStatusAsync(token);

                return LastResult;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error in ExecuteAsync: {ex.Message}");
#endif
                LastResult = ModuleResult.Failed(ex.Message);

                return LastResult;
            }

        }

        protected abstract Task<string> RefreshStatusAsync(CancellationToken token);

        protected abstract Task<ModuleResult> ExecuteAsync(CancellationToken token);
    }
}

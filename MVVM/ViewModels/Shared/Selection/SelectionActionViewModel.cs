using GameBoost.Core.Interfaces;
using GameBoost.MVVM.Core;
using GameBoost.Shared.Results;
using MaterialDesignThemes.Wpf;
using System.Diagnostics;

namespace GameBoost.MVVM.ViewModels.Shared.Selection
{
    public class SelectionActionViewModel : ObservableObject, ISelectionButton
    {
        public SelectionFeatureViewModel? Parent { get; internal set; }

        public required string Title { get; set; }
        public required PackIconKind Icon { get; set; }

        private string _status = string.Empty;
        public string Status { get => _status; set => Set(ref _status, value); }

        private bool _isChecked = false;
        public bool IsChecked { get => _isChecked; set => Set(ref _isChecked, value); }

        private ModuleResult? _lastResult;
        public ModuleResult? LastResult { get => _lastResult; set => Set(ref _lastResult, value); }

        public IActionModule? Module { get; set; }

        public async Task RefreshStatusAsync(CancellationToken token)
        {
            if (Module is null)
                return;

            try
            {
                token.ThrowIfCancellationRequested();

                Status = await Module.RefreshStatusAsync(token);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error in RefreshStatusAsync: {ex.Message}");
#endif
                Status = "Failed to refresh";
            }
        }

        public async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            if (Module is null)
                return ModuleResult.Failed("No Module To Execute");

            try
            {
                token.ThrowIfCancellationRequested();

                LastResult = await Module.ExecuteAsync(token);

                Status = await Module.RefreshStatusAsync(token);

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
    }
}

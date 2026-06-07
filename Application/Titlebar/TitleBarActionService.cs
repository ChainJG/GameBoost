using GameBoost.Application.Operations;
using GameBoost.MVVM.UserControls.Shared.Titlebar;
using GameBoost.Shared.Results;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace GameBoost.Application.Titlebar
{
    public sealed class TitleBarActionService(GlobalOperationService globalOperation)
    {
        private readonly GlobalOperationService _globalOperations = globalOperation;

        public ObservableCollection<TitleBarActionViewModel> Actions { get; } = [];

        public void AddOrReplace(TitleBarActionViewModel action)
        {
            var exisitingAction = Actions.FirstOrDefault(item => item.Key == action.Key);

            if (exisitingAction is not null)
                Actions.Remove(exisitingAction);

            Actions.Add(action);
        }

        public void Remove(string key)
        {
            var existingAction = Actions.FirstOrDefault(item => item.Key == key);

            if (existingAction is not null)
                Actions.Remove(existingAction);
        }

        public async Task RunAsync(
            TitleBarActionViewModel action,
            Func<Task<ModuleResult>> operation)
        {
            if (action.IsBusy)
                return;

            try
            {
                action.IsBusy = true;
                _globalOperations.BeginOperation();

                var result = await operation();

                if (result.Success)
                    Remove(action.Key);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Title bar action '{action.Key}' failed: {ex.Message}");
#endif
                MessageBox.Show(
                    ex.Message,
                    "GameBoost Action Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            finally
            {
                _globalOperations.EndOperation();
                action.IsBusy = false;
            }
        }
    }
}

using GameBoost.MVVM.Core;

namespace GameBoost.Application.Operations
{
    public sealed class GlobalOperationService : ObservableObject
    {
        private int _operationCount;
        private bool _isSelectionExecutionActive;

        public bool IsGlobalProgressVisible =>
            _operationCount > 0 || _isSelectionExecutionActive;

        public void BeginOperation()
        {
            _operationCount++;
            OnPropertyChanged(nameof(IsGlobalProgressVisible));
        }

        public void EndOperation()
        {
            if (_operationCount > 0)
                _operationCount--;

            OnPropertyChanged(nameof(IsGlobalProgressVisible));
        }

        public void SetOperationBoolean(bool value)
        {
            if (value)
                BeginOperation();
            else
                EndOperation();
        }

        public void SetSelectionExecutionActive(bool isActive)
        {
            if (_isSelectionExecutionActive == isActive)
                return;

            _isSelectionExecutionActive = isActive;
            OnPropertyChanged(nameof(IsGlobalProgressVisible));
        }
    }
}

using GameBoost.Core.Debugger;
using System.Windows.Input;

namespace GameBoost.MVVM.Core
{
    public sealed class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool>? _canExecute;
        private bool _isExecuting;

        public AsyncRelayCommand(
            Func<Task> execute,
            Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke() ?? true);
        }

        public async void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
                return;

            try
            {
                _isExecuting = true;

                await _execute();
            }
            catch (OperationCanceledException)
            {
                // Cancellation is an expected outcome, not a failure.
            }
            catch (Exception ex)
            {
                GameBoostDebug.Error("AsyncRelayCommand failed", ex);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public sealed class AsyncRelayCommand<T> : ICommand
    {
        private readonly Func<T?, Task> _execute;
        private readonly Func<T?, bool>? _canExecute;
        private bool _isExecuting;

        public AsyncRelayCommand(
            Func<T?, Task> execute,
            Func<T?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (_isExecuting)
                return false;

            return _canExecute?.Invoke(ConvertParameter(parameter)) ?? true;
        }

        public async void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
                return;

            try
            {
                _isExecuting = true;

                await _execute(ConvertParameter(parameter));
            }
            catch (OperationCanceledException)
            {
                // Cancellation is an expected outcome, not a failure.
            }
            catch (Exception ex)
            {
                GameBoostDebug.Error($"AsyncRelayCommand<{typeof(T).Name}> failed", ex);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private static T? ConvertParameter(object? parameter)
        {
            if (parameter is null)
                return default;

            if (parameter is T typedParameter)
                return typedParameter;

            return default;
        }
    }
}
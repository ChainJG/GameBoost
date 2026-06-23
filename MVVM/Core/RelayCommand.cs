using System.Diagnostics;
using System.Windows.Input;

namespace GameBoost.MVVM.Core
{
    public class RelayCommand<T>(Action<T> execute,Func<T, bool>? canExecute = null) : ICommand
    {
        private readonly Action<T> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        private readonly Func<T, bool>? _canExecute = canExecute;

        public bool CanExecute(object? parameter)
        {
            try
            {
                return _canExecute?.Invoke(ConvertParameter(parameter)) ?? true;
            }
            catch
            {
                return false;
            }
        }

        public void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
                return;

            try
            {
                _execute(ConvertParameter(parameter)!);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"RelayCommand<{typeof(T).Name}> failed: {ex.Message}");
#endif
            }
            finally
            {
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

    public class RelayCommand(Action execute, Func<bool>? canExecute = null) : ICommand
    {
        private readonly Action _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        private readonly Func<bool>? _canExecute = canExecute;

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
                return;

            try
            {
                _execute();

            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"RelayCommand failed: {ex.Message}");
#endif
            }
            finally
            {
                RaiseCanExecuteChanged();
            }
        }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
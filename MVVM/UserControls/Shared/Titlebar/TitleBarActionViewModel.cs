using GameBoost.MVVM.Core;
using MaterialDesignThemes.Wpf;
using System.Windows.Input;
using System.Windows.Media;

namespace GameBoost.MVVM.UserControls.Shared.Titlebar
{
    public sealed class TitleBarActionViewModel : ObservableObject
    {
        public required string Key { get; init; }

        public required string Title { get; init; }

        public required string Message { get; init; }

        public required PackIconKind Icon { get; init; }

        public required ICommand Command { get; init; }

        public Brush Foreground { get; init; } = System.Windows.Application.Current?.TryFindResource("IconKindColor") as Brush ?? Brushes.White;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (!Set(ref _isBusy, value)) return;

                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        public bool IsEnabled => !IsBusy;
    }
}

using GameBoost.MVVM.Core;
using GameBoost.Shared.Helpers.ProcessHelpers;
using MaterialDesignThemes.Wpf;

namespace GameBoost.Application.FileLookup
{
    public sealed class FileLockInfoViewModel : ObservableObject
    {
        public string Title { get; set; }
        public PackIconKind Icon { get; set; } = PackIconKind.Application;
        public FileLockInfo FileLockInfo { get; set; }

        public PackIconKind InfoIcon { get; init; } = PackIconKind.HelpRhombus;
        public string InfoToolTip { get; init; } = string.Empty;

        public string RecommendationToolTip { get; init; } = string.Empty;
        public bool IsRecommendedState { get; init; } = false;

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set => Set(ref _isChecked, value);
        }

        private string _status;
        public string Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        public FileLockInfoViewModel(FileLockInfo fileLockInfo)
        {
            FileLockInfo = fileLockInfo;

            Title = ProcessDisplayHelper.GetTitle(fileLockInfo);
            Icon = ProcessDisplayHelper.GetIcon(fileLockInfo);
            InfoToolTip = ProcessDisplayHelper.GetInfoToolTip(fileLockInfo);
        }
    }
}

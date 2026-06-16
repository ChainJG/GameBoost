using GameBoost.MVVM.Core;
using MaterialDesignThemes.Wpf;

namespace GameBoost.MVVM.ViewModels.Storage
{
    public sealed class StoragePathSegmentViewModel : ObservableObject
    {
        public required string DisplayName { get; init; }

        public required string FullPath { get; init; }

        public bool IsRoot { get; init; }

        public bool IsCurrent { get; init; }

        public StorageFolderNodeViewModel? OpenedFolder { get; init; }

        public StorageFolderNavigationSnapshot? CachedSnapshot { get; init; }

        public bool HasCachedSnapshot => CachedSnapshot is not null;

        public PackIconKind Icon =>
            IsRoot ? PackIconKind.Harddisk : PackIconKind.Folder;
    }
}

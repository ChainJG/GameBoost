using GameBoost.Features.Storage.Models;
using GameBoost.MVVM.Core;
using GameBoost.Shared.Helpers;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;

namespace GameBoost.MVVM.ViewModels.Storage
{
    public sealed class StorageFolderNodeViewModel(StorageFolderNode model, long parentSizeBytes) : ObservableObject
    {
        public StorageFolderNode Model { get; } = model;

        public ObservableCollection<StorageFolderNodeViewModel> Children { get; } =
        [
            ..model.Children.Select(child =>
                new StorageFolderNodeViewModel(child, model.SizeBytes))
        ];

        public string Name => Model.Name;

        public string FullPath => Model.FullPath;

        public long SizeBytes => Model.SizeBytes;

        public string SizeText => MathHelper.FormatBytes(Model.SizeBytes);

        public int FileCount => Model.FileCount;

        public int FolderCount => Model.FolderCount;

        public bool IsAccessible => Model.IsAccessible;

        public double PercentageOfParent { get; } = parentSizeBytes <= 0
                ? 0
                : model.SizeBytes / (double)parentSizeBytes * 100;

        public string PercentageText => $"{PercentageOfParent:0.#}%";

        public PackIconKind Icon => IsAccessible
            ? PackIconKind.Folder
            : PackIconKind.FolderAlert;
    }
}
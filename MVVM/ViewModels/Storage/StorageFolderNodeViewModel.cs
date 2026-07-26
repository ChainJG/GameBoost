using GameBoost.Features.Storage.Models;
using GameBoost.MVVM.Core;
using GameBoost.Shared.Helpers;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualBasic.FileIO;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace GameBoost.MVVM.ViewModels.Storage
{
    public sealed class StorageFolderNodeViewModel : ObservableObject
    {
        private readonly Action<StorageFolderNodeViewModel>? _removeFromParent;

        public StorageFolderNodeViewModel(StorageFolderNode model, long parentSizeBytes, Action<StorageFolderNodeViewModel>? removeFromParent = null)
        {
            Model = model;
            _removeFromParent = removeFromParent;

            PercentageOfParent = parentSizeBytes <= 0
                ? 0
                : model.SizeBytes / (double)parentSizeBytes * 100;

            Children = [];

            foreach (var child in model.Children)
            {
                var childViewModel = new StorageFolderNodeViewModel(
                    child,
                    model.SizeBytes,
                    removedChild => Children.Remove(removedChild));

                Children.Add(childViewModel);
            }

            OpenFolderCommand = new RelayCommand(OpenFolder, () => IsAccessible);
            DeleteFolderCommand = new AsyncRelayCommand(DeleteFolderAsync, CanDeleteFolder);
        }


        public StorageFolderNode Model { get; }

        public ObservableCollection<StorageFolderNodeViewModel> Children { get; }

        public string Name => Model.Name;

        public string FullPath => Model.FullPath;

        public ICommand OpenFolderCommand { get; }
        public ICommand DeleteFolderCommand { get; }

        public long SizeBytes => Model.SizeBytes;

        public string SizeText => MathHelper.FormatBytes(Model.SizeBytes);

        public int FileCount => Model.FileCount;

        public int FolderCount => Model.FolderCount;

        public bool IsAccessible => Model.IsAccessible;

        public double PercentageOfParent { get; }

        public string PercentageText => $"{PercentageOfParent:0.#}%";

        public PackIconKind Icon => IsAccessible
            ? PackIconKind.Folder
            : PackIconKind.FolderAlert;

        private bool _isDeleting;
        public bool IsDeleting
        {
            get => _isDeleting;
            private set
            {
                if (!Set(ref _isDeleting, value))
                    return;

                if (DeleteFolderCommand is AsyncRelayCommand deleteCommand)
                    deleteCommand.RaiseCanExecuteChanged();
            }
        }
        private bool CanDeleteFolder()
        {
            return !IsDeleting &&
                IsAccessible &&
                !string.IsNullOrWhiteSpace(FullPath) &&
                Directory.Exists(FullPath) && 
                !DirectoryHelper.IsRootDirectory(FullPath);
        }

        private void OpenFolder() =>
            DirectoryHelper.OpenFolderInExplorer(FullPath);

        private async Task DeleteFolderAsync()
        {
            if (!CanDeleteFolder())
                return;

            try
            {
                IsDeleting = true;
                bool canceled = false;

                await Task.Run(() =>
                {
                    try
                    {
                        FileSystem.DeleteDirectory(
                            FullPath,
                            UIOption.AllDialogs,
                            RecycleOption.SendToRecycleBin);
                    } 
                    catch 
                    {
                        canceled = true;
                    }
                });

                if (canceled)
                    return;

                _removeFromParent?.Invoke(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while trying to delete the folder:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                IsDeleting = false;
            }
        }
    }
}

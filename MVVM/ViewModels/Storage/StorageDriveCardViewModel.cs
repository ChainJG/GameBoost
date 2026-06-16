using GameBoost.Features.Storage.Models;
using GameBoost.MVVM.Core;
using GameBoost.Shared.Helpers;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;

namespace GameBoost.MVVM.ViewModels.Storage
{
    public sealed class StorageDriveCardViewModel(StorageDriveInfo model) : ObservableObject
    {
        public StorageDriveInfo Model { get; } = model;

        public PackIconKind Icon => PackIconKind.Harddisk;

        public string Name => Model.Name;

        public string RootPath => Model.RootPath;

        public long TotalBytes => Model.TotalBytes;

        public long UsedBytes => Model.UsedBytes;

        public long FreeBytes => Model.FreeBytes;

        public string UsedText => MathHelper.FormatBytes(Model.UsedBytes);

        public string TotalText => MathHelper.FormatBytes(Model.TotalBytes);

        public string FreeText => MathHelper.FormatBytes(Model.FreeBytes);

        public double UsedPercentage => MathHelper.ToPercentageInt(Model.UsedBytes, Model.TotalBytes);

        public string SummaryText => $"{FreeText} free of {TotalText}";

        public Brush ProgressBackgroundColour => IsAlmostFull ? Brushes.Red : IsHighUsage ? Brushes.Orange : Brushes.Green;

        public bool IsAlmostFull => UsedPercentage >= 85;
        public bool IsHighUsage => UsedPercentage >= 70;
        public bool IsHealthy => UsedPercentage < 70;



    }
}
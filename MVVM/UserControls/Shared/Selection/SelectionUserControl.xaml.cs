using GameBoost.MVVM.ViewModels.Shared;
using System.Windows;
using System.Windows.Controls;

namespace GameBoost.MVVM.UserControls.Shared.ViewModels;

public partial class SelectionUserControl : UserControl
{
    public SelectionUserControl()
    {
        InitializeComponent();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is SelectionViewModel viewModel)
        {
            viewModel.StateChanged += OnStateChanged;
            await viewModel.InitialiseAsync();
        }
    }
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is SelectionViewModel viewModel)
            viewModel.StateChanged -= OnStateChanged;
    }

    private void OnStateChanged(SelectionScreenType type) => VisualStateManager.GoToElementState(PageRoot, type.ToString(), true);
}

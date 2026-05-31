using GameBoost.MVVM.ViewModels.Shared.Selection;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace GameBoost.MVVM.UserControls.Shared.Selection;

public partial class SelectionUserControl : UserControl
{
    private SelectionViewModel? _viewModel;

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

            _viewModel = viewModel;
            await viewModel.InitialiseAsync();
        }
    }
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_viewModel is not null)
        {
            _viewModel.StateChanged -= OnStateChanged;
            _viewModel.CancelExecution();
            _viewModel = null;
        }
    }

    private void OnStateChanged(SelectionScreenType type) => VisualStateManager.GoToElementState(PageRoot, type.ToString(), true);
}

using GameBoost.MVVM.ViewModels.Shared.Selection;
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

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not SelectionViewModel viewModel)
            return;

        _viewModel = viewModel;

        // Screen transitions are driven by VisualStateAssist bound to DisplayScreenType.
        _ = viewModel.InitialiseAsync();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_viewModel is null)
            return;

        _viewModel.CancelExecution();
        _viewModel = null;
    }
}

using GameBoost.Core;
using GameBoost.MVVM.ViewModels;
using GameBoost.Shared.Helpers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace GameBoost.MVVM.Windows
{
    public partial class ProcessLookupWindow : Window
    {
        #region Application Functions
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWind, int wMsg, int wParam, int lParam);
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, 161, 2, 0);
        }

        private void BtnMinimiseWindow_Click(object sender, RoutedEventArgs e) =>
            this.WindowState = WindowState.Minimized;

        private void BtnExitApplication_Click(object sender, RoutedEventArgs e) =>
            GameBoostServices.Shutdown();
        #endregion

        private readonly ProcessLookupViewModel _processLookupViewModel;

        public ProcessLookupWindow(string filePath)
        {
            InitializeComponent();

            _processLookupViewModel = new ProcessLookupViewModel(filePath);
            DataContext = _processLookupViewModel;

        }

        private void SelectedFileText_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TextBlock textBlock) return;

            DirectoryHelper.OpenFileInExplorer(textBlock.Text);
        }

    }
}

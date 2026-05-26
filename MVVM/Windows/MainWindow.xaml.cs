using GameBoost.Core;
using GameBoost.MVVM.ViewModels;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace GameBoost.MVVM.Windows
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _mainViewModel = new();
        public MainWindow()
        {
            DataContext = _mainViewModel;
            InitializeComponent();
        }

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
    }
}
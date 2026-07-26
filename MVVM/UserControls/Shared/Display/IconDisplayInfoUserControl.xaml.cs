using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace GameBoost.MVVM.UserControls.Shared.Display
{
    public partial class IconDisplayInfoUserControl : UserControl
    {
        public IconDisplayInfoUserControl()
        {
            InitializeComponent();
        }
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public string SubTitle
        {
            get => (string)GetValue(SubTitleProperty);
            set => SetValue(SubTitleProperty, value);
        }
        public PackIconKind Icon
        {
            get => (PackIconKind)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(IconDisplayInfoUserControl),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty SubTitleProperty =
            DependencyProperty.Register(
                nameof(SubTitle),
                typeof(string),
                typeof(IconDisplayInfoUserControl),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                nameof(Icon),
                typeof(PackIconKind),
                typeof(IconDisplayInfoUserControl),
                new PropertyMetadata(PackIconKind.QuestionMark));
    }
}

using System.Windows;
using System.Windows.Controls;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for CustomWindow.xaml
    /// </summary>
    public partial class CustomWindow : Window
    {
        public CustomWindow()
        {
            InitializeComponent();
        }
        public CustomWindow(string title,object windowContent, bool WithoutClose = false) : this()
        {
            ContentControl.Content = windowContent;
            if(WithoutClose)
                CloseButton.Visibility = Visibility.Collapsed;
            titleText.Text = title;
            Application.Current.MainWindow.Opacity = 0.2;
            this.Closing += (s, e) => Application.Current.MainWindow.Opacity = 1;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Opacity = 1;
            this.Close();
        }

        private void OnDrag(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}

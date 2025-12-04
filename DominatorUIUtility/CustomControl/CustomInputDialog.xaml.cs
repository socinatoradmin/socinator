using System.Windows;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for CustomInputDialog.xaml
    /// </summary>
    public partial class CustomInputDialog : Window
    {
        public string DefaultText {  get; set; }
        public CustomInputDialog()
        {
            InitializeComponent();
        }
        public CustomInputDialog(string title,string message,string firstButton,string secondButton,string defaultText) : this()
        {
            DefaultText = defaultText;
            titleText.Text = title;
            MessageTextBox.Text = message;
            Application.Current.MainWindow.Opacity = 0.2;
            this.Closing += (s,e)=> Application.Current.MainWindow.Opacity = 1;
            TextBox.Text = defaultText;
            YesButton.Content = firstButton;
            NoButton.Content = secondButton;
            DataContext = this;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            DefaultText = TextBox.Text;
            this.Close();
        }

        private void OnDragWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}

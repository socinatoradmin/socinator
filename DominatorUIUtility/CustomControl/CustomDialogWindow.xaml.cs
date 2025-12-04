using System.Windows;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for CustomDialogWindow.xaml
    /// </summary>
    public partial class CustomDialogWindow : Window
    {
        public CustomDialogWindow(string title, string message, string affirmativeText = "OK", string negativeText = "Cancel")
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            // Set window properties for modal behavior
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.ShowInTaskbar = false;
            this.ResizeMode = ResizeMode.NoResize;
            TitleText.Text = title;
            MessageText.Text = message;
            YesButton.Content = affirmativeText;
            NoButton.Content = negativeText;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // Optional: Handle window close button (X) click
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // If DialogResult is null, it means user clicked X button
            if (DialogResult == null)
            {
                DialogResult = false; // Or set to null to indicate cancellation
            }
            base.OnClosing(e);
        }

    }
}

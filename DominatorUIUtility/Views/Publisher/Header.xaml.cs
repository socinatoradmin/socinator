using System.Windows;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    ///     Interaction logic for Header.xaml
    /// </summary>
    public partial class Header : UserControl
    {
        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(Header), new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true
            });

        public Header()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            // cmbAccounts.ItemsSource = AccountsFileManager.GetUsers();
        }

        public string HeaderText
        {
            get => (string) GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        private void BtnBackToCampaign_Click(object sender, RoutedEventArgs e)
        {
            var objPublisher = PublisherIndexPage.Instance;
            objPublisher.PublisherIndexPageViewModel.SelectedUserControl = Home.GetSingletonHome();
        }
    }
}
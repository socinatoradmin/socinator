using System.Windows.Controls;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    ///     Interaction logic for PublisherIndexPage.xaml
    /// </summary>
    public partial class PublisherIndexPage : UserControl
    {
        private static PublisherIndexPage _indexPage;

        private PublisherIndexPage()
        {
            InitializeComponent();
            PublisherIndexPageViewModel = new PublisherIndexPageViewModel();
            PublisherIndex.DataContext = PublisherIndexPageViewModel;
        }

        public static PublisherIndexPage Instance { get; set; }
            = _indexPage ?? (_indexPage = new PublisherIndexPage());

        public PublisherIndexPageViewModel PublisherIndexPageViewModel { get; set; }
    }

    public class PublisherIndexPageViewModel : BindableBase
    {
        private UserControl _selectedUserControl = Home.GetSingletonHome();

        public UserControl SelectedUserControl
        {
            get => _selectedUserControl;
            set
            {
                if (_selectedUserControl != null && Equals(_selectedUserControl, value))
                    return;

                SetProperty(ref _selectedUserControl, value);
            }
        }
    }
}
using System.Windows.Controls;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    ///     Interaction logic for PublisherHome.xaml
    /// </summary>
    public partial class PublisherHome : UserControl
    {
        private static PublisherHome _indexPage;

        private PublisherHome()
        {
            InitializeComponent();
            PublisherHomeViewModel.SetDefaultHomePage();
            UserControlPublisherHome.DataContext = PublisherHomeViewModel.PublisherHomeModel;
        }

        public static PublisherHome Instance { get; set; }
            = _indexPage ?? (_indexPage = new PublisherHome());

        public PublisherHomeViewModel PublisherHomeViewModel { get; set; } = new PublisherHomeViewModel();
    }
}
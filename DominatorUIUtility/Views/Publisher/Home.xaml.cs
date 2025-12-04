using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.ViewModel;

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    ///     Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        private Home()
        {
            InitializeComponent();

            PublisherDetailCollection = CollectionViewSource.GetDefaultView(PostFileManager.GetAllPost());

            publisherDetail.ItemsSource = PublisherDetailCollection;
        }

        public ICollectionView PublisherDetailCollection { get; set; }


        private static Home ObjHome { get; set; }

        public static Home GetSingletonHome()
        {
            return ObjHome ?? (ObjHome = new Home());
        }

        private void btnCreateCampaign_Click(object sender, RoutedEventArgs e)
        {
            PublisherIndexPage.Instance.PublisherIndexPageViewModel.SelectedUserControl = new Campaigns();
        }


        private void btnManageDestination_Click(object sender, RoutedEventArgs e)
        {
            var manageDestination = ManageDestinationIndex.Instance;
            manageDestination.SelectedControl = new ManageDestination();
            PublisherIndexPage.Instance.PublisherIndexPageViewModel.SelectedUserControl = manageDestination;
        }

        private void btnManagePosts_Click(object sender, RoutedEventArgs e)
        {
            var managePosts = new ManagePosts();
            var ObjAddPostViewModel = new AddPostViewModel();
            managePosts.MainGrid.DataContext = ObjAddPostViewModel.AddPostModel;
            PublisherIndexPage.Instance.PublisherIndexPageViewModel.SelectedUserControl = new ManagePosts();
        }
    }
}
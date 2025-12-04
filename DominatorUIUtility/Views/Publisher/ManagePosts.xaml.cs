using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.ViewModel;

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    ///     Interaction logic for ManagePosts.xaml
    /// </summary>
    public partial class ManagePosts : UserControl
    {
        public AddPostViewModel ObjAddPostViewModel = new AddPostViewModel();

        public ManagePosts()
        {
            InitializeComponent();
            publishersHeader.HeaderText = FindResource("LangKeyManagePosts").ToString();
            MainGrid.DataContext = ObjAddPostViewModel.AddPostModel;
            ObjAddPostViewModel.AddPostModel.PostsDetailCollection =
                CollectionViewSource.GetDefaultView(PostFileManager.GetAllPost());
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Continue");
        }
    }
}
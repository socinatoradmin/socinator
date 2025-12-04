using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for PostLikerCommentor.xaml
    /// </summary>
    public partial class PostLikerCommentor : UserControl
    {
        public PostLikerCommentor(IPostLikerCommentorViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
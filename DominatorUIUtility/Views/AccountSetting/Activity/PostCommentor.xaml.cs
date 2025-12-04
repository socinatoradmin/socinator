using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for PostCommentor.xaml
    /// </summary>
    public partial class PostCommentor : UserControl
    {
        public PostCommentor(IPostCommentorViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
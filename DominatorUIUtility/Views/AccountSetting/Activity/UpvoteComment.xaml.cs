using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for UpvoteComment.xaml
    /// </summary>
    public partial class UpvoteComment : UserControl
    {
        public UpvoteComment(IUpvoteCommentViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
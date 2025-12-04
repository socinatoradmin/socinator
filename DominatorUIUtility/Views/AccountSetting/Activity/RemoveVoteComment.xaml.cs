using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for RemoveVoteComment.xaml
    /// </summary>
    public partial class RemoveVoteComment : UserControl
    {
        public RemoveVoteComment(IRemoveVoteCommentViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
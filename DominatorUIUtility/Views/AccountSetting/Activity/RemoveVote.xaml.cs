using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for RemoveVote.xaml
    /// </summary>
    public partial class RemoveVote : UserControl
    {
        public RemoveVote(IRemoveVoteViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
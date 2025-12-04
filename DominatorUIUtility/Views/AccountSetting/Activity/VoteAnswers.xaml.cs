using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for VoteAnswers.xaml
    /// </summary>
    public partial class VoteAnswers : UserControl
    {
        public VoteAnswers(IVoteAnswersViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
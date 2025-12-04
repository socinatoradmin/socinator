using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for UpvoteAnswers.xaml
    /// </summary>
    public partial class UpvoteAnswers : UserControl
    {
        public UpvoteAnswers(IUpvoteAnswersViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
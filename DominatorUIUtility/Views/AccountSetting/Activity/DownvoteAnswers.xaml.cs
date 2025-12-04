using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for DownvoteAnswers.xaml
    /// </summary>
    public partial class DownvoteAnswers : UserControl
    {
        public DownvoteAnswers(IDownvoteAnswersViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
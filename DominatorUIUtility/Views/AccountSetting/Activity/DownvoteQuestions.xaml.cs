using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for DownvoteQuestions.xaml
    /// </summary>
    public partial class DownvoteQuestions : UserControl
    {
        public DownvoteQuestions(IDownvoteQuestionsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
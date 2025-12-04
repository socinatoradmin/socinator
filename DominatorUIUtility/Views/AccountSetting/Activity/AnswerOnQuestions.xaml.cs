using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for AnswerOnQuestions.xaml
    /// </summary>
    public partial class AnswerOnQuestions : UserControl
    {
        public AnswerOnQuestions(IAnswerOnQuestionsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for ReportQuestions.xaml
    /// </summary>
    public partial class ReportQuestions : UserControl
    {
        public ReportQuestions(IReportQuestionsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
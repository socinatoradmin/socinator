using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for ReportAnswers.xaml
    /// </summary>
    public partial class ReportAnswers : UserControl
    {
        public ReportAnswers(IReportAnswersViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
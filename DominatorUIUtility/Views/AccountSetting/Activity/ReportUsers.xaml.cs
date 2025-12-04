using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for ReportUsers.xaml
    /// </summary>
    public partial class ReportUsers : UserControl
    {
        public ReportUsers(IReportUsersViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for CancelSentRequest.xaml
    /// </summary>
    public partial class WithdrawSentRequest : UserControl
    {
        public WithdrawSentRequest(ICancelSentRequestViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for WithdrawConnectionRequest.xaml
    /// </summary>
    public partial class WithdrawConnectionRequest : UserControl
    {
        public WithdrawConnectionRequest(IWithdrawConnectionRequestViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
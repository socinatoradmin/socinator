using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for WatchPartyInviter.xaml
    /// </summary>
    public partial class WatchPartyInviter : UserControl
    {
        public WatchPartyInviter(IWatchPartyInviterViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
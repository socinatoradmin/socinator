using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for ConnectionRequest.xaml
    /// </summary>
    public partial class ConnectionRequest : UserControl
    {
        public ConnectionRequest(IConnectionRequestViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for AcceptConnectionRequest.xaml
    /// </summary>
    public partial class AcceptConnectionRequest : UserControl
    {
        public AcceptConnectionRequest(IAcceptConnectionRequestViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
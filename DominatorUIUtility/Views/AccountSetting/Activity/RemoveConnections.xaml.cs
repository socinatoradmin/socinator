using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for RemoveConnections.xaml
    /// </summary>
    public partial class RemoveConnections : UserControl
    {
        public RemoveConnections(IRemoveConnectionsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
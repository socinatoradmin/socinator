using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for BroadcastMessages.xaml
    /// </summary>
    public partial class BroadcastMessages : UserControl
    {
        public BroadcastMessages(IBroadcastMessagesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for MessageToFanpages.xaml
    /// </summary>
    public partial class MessageToFanpages : UserControl
    {
        public MessageToFanpages(IMessageToFanpagesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
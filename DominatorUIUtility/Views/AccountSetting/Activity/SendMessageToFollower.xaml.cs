using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for SendMessageToFollower.xaml
    /// </summary>
    public partial class SendMessageToFollower : UserControl
    {
        public SendMessageToFollower(ISendMessageToFollowerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for SendFriendRequest.xaml
    /// </summary>
    public partial class SendFriendRequest : UserControl
    {
        public SendFriendRequest(ISendFriendRequestViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
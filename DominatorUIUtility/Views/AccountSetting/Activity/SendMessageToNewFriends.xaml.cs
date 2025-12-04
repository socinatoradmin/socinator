using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for SendMessageToNewFriends.xaml
    /// </summary>
    public partial class SendMessageToNewFriends : UserControl
    {
        public SendMessageToNewFriends(ISendMessageToNewFriendsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
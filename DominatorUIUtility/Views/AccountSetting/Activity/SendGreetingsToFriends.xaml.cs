using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for SendGreetingsToFriends.xaml
    /// </summary>
    public partial class SendGreetingsToFriends : UserControl
    {
        public SendGreetingsToFriends(ISendGreetingsToFriendsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
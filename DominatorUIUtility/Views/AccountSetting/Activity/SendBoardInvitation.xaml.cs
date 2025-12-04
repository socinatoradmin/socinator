using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for SendBoardInvitation.xaml
    /// </summary>
    public partial class SendBoardInvitation : UserControl
    {
        public SendBoardInvitation(ISendBoardInvitationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
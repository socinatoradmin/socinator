using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for AcceptBoardInvitation.xaml
    /// </summary>
    public partial class AcceptBoardInvitation : UserControl
    {
        public AcceptBoardInvitation(IAcceptBoardInvitationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
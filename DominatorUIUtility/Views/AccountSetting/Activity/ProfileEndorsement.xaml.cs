using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for ProfileEndorsement.xaml
    /// </summary>
    public partial class ProfileEndorsement : UserControl
    {
        public ProfileEndorsement(IProfileEndorsementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
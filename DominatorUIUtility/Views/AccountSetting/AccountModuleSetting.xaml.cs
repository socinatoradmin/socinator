using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel.Startup;

namespace DominatorUIUtility.Views.AccountSetting
{
    /// <summary>
    ///     Interaction logic for AccountModuleSetting.xaml
    /// </summary>
    public partial class AccountModuleSetting
    {
        public AccountModuleSetting(SocialNetworks AccountNetwork)
        {
            InitializeComponent();
            var viewModel = InstanceProvider.GetInstance<ISelectActivityViewModel>();
            viewModel.SelectedNetwork = AccountNetwork.ToString();
        }
    }
}
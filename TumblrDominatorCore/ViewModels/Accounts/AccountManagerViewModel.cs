using DominatorHouseCore.Utility;

namespace TumblrDominatorCore.ViewModels.Accounts
{
    public class AccountManagerViewModel : BindableBase
    {
        private static AccountManagerViewModel _accountManagerViewModel;

        public static AccountManagerViewModel GetAccountManagerViewModel()
        {
            return _accountManagerViewModel ?? (_accountManagerViewModel = new AccountManagerViewModel());
        }
    }
}
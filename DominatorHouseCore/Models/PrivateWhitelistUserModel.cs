#region

using System.Collections.ObjectModel;

#endregion

namespace DominatorHouseCore.Models
{
    public class PrivateWhitelistUserModel : WhitelistUserModel
    {
        private ObservableCollection<DominatorAccountModel> _lstAccountModels;

        public ObservableCollection<DominatorAccountModel> LstAccountModel
        {
            get => _lstAccountModels;
            set => SetProperty(ref _lstAccountModels, value);
        }

        private DominatorAccountModel _selectedAccount;

        public DominatorAccountModel SelectedAccount
        {
            get => _selectedAccount;
            set => SetProperty(ref _selectedAccount, value);
        }
    }
}
using System;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;

namespace DominatorUIUtility.ViewModel
{
    public class AccountManagerViewModel : BindableBase
    {
        private UserControl _selectedUserControl;
        private static AccountManagerViewModel ObjAccountManagerViewModel { get; set; }
        public string LastControlType { get; set; }

        public UserControl SelectedUserControl
        {
            get => _selectedUserControl;
            set => SetProperty(ref _selectedUserControl, value);
        }

        public static AccountManagerViewModel GetSingletonAccountManagerViewModel()
        {
            return ObjAccountManagerViewModel ?? (ObjAccountManagerViewModel = new AccountManagerViewModel());
        }

        public void CallRespectiveView(string controlType, [CanBeNull] DominatorAccountModel dominatorAccountModel,
            SocialNetworks network)
        {
            try
            {
                LastControlType = controlType;
                if (controlType == "AccountManager")
                {
                    var accountCustomControl = AccountCustomControl.GetAccountCustomControl(network);
                    SelectedUserControl = null;
                    SelectedUserControl = accountCustomControl;
                }
                else
                {
                    SelectedUserControl = new AccountDetail(dominatorAccountModel);
                    LastControlType = "AccountDetail";
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
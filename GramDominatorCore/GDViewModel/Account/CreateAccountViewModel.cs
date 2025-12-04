using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using System;
using System.Windows.Controls;

namespace GramDominatorCore.GDViewModel.Account
{
    public class CreateAccountViewModel: BindableBase
    {
        private static CreateAccountViewModel ObjCreateAccountViewModel { get; set; }

        public static CreateAccountViewModel GetSingletonCreateAccountViewModel()
            => ObjCreateAccountViewModel ?? (ObjCreateAccountViewModel = new CreateAccountViewModel());

        private UserControl _selectedUserControl;

        public UserControl SelectedUserControl
        {
            get
            {
                return _selectedUserControl;
            }
            set
            {
                SetProperty(ref _selectedUserControl, value);
            }
        }

        public void CallRespectiveView()
        {
            try
            {             
                    //var accountCustomControl = CreateAccountCustomControl.GetAccountCustomControl();
                    SelectedUserControl = null;
                   // SelectedUserControl = accountCustomControl;               
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}

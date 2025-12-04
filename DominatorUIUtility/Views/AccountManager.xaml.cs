using System.ComponentModel;
using System.Runtime.CompilerServices;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.Views
{
    /// <summary>
    ///     Interaction logic for DominatorAutoActivity.xaml
    /// </summary>
    public partial class AccountManager : INotifyPropertyChanged
    {
        public static AccountManager ObjAccountManager;
        private AccountManagerViewModel _accountManagerViewModel;

        private AccountManager()
        {
            InitializeComponent();
            AccountManagerViewModel = AccountManagerViewModel.GetSingletonAccountManagerViewModel();
            AccountManagerPage.DataContext = AccountManagerViewModel;
        }

        public AccountManagerViewModel AccountManagerViewModel
        {
            get => _accountManagerViewModel;
            set
            {
                _accountManagerViewModel = value;
                OnPropertyChanged(nameof(AccountManagerViewModel));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public static AccountManager GetSingletonAccountManager(string controlType,
            [CanBeNull] DominatorAccountModel dominatorAccountModel, SocialNetworks network)
        {
            if (ObjAccountManager == null)
                ObjAccountManager = new AccountManager();

            ObjAccountManager.AccountManagerViewModel.CallRespectiveView(controlType, dominatorAccountModel, network);

            return ObjAccountManager;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
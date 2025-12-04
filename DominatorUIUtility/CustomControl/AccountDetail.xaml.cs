using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for AddUpdateAccountControl.xaml
    /// </summary>
    public partial class AccountDetail : INotifyPropertyChanged
    {
        private AccountDetailsViewModel _accountDetailsViewModel;

        /// <summary>
        ///     Constructor with default data context
        /// </summary>
        public AccountDetail()
        {
            InitializeComponent();
        }

        public AccountDetail(DominatorAccountModel dataContext) : this()
        {
            AccountDetailsViewModel = new AccountDetailsViewModel(dataContext);
            DataContext = AccountDetailsViewModel;
            AccountDetailsViewModel.CodeSectionVisibility =
                dataContext.AccountBaseModel.Status == AccountStatus.TwoFactorLoginAttempt
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        public AccountDetailsViewModel AccountDetailsViewModel
        {
            get => _accountDetailsViewModel;
            set
            {
                _accountDetailsViewModel = value;
                OnPropertyChanged(nameof(AccountDetailsViewModel));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) btnSave.IsDefault = true;
        }

        private void OnVerificationKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) btnVerifyAccount.IsDefault = true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
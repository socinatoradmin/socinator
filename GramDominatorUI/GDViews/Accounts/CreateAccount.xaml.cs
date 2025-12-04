using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using GramDominatorCore.GDViewModel.Account;

namespace GramDominatorUI.GDViews.Accounts
{
    /// <summary>
    ///     Interaction logic for CreateAccount.xaml
    /// </summary>
    public partial class CreateAccount : UserControl, INotifyPropertyChanged
    {
        public static CreateAccount ObjCreateAccount;
        private CreateAccountViewModel _accountManagerViewModel;

        public CreateAccount()
        {
            InitializeComponent();
            CreatAccountViewModel = CreateAccountViewModel.GetSingletonCreateAccountViewModel();
            //CreateAccountPage.DataContext = CreatAccountViewModel;
        }

        public CreateAccountViewModel CreatAccountViewModel
        {
            get => _accountManagerViewModel;
            set
            {
                _accountManagerViewModel = value;
                OnPropertyChanged(nameof(CreatAccountViewModel));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public static CreateAccount GetSingletonCreateAccount()
        {
            if (ObjCreateAccount == null)
                ObjCreateAccount = new CreateAccount();
            ObjCreateAccount.CreatAccountViewModel.CallRespectiveView();
            return ObjCreateAccount;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
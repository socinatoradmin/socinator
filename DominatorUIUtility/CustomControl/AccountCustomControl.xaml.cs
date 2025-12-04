using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommonServiceLocator;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for AccountCustomControl.xaml
    /// </summary>
    public partial class AccountCustomControl : INotifyPropertyChanged
    {
        private static AccountCustomControl _accountCustomInstance;
        private DominatorAccountViewModel _dominatorAccountViewModel;

        private AccountCustomControl()
        {
            _accountCustomInstance = this;
            _dominatorAccountViewModel =
                (DominatorAccountViewModel)InstanceProvider.GetInstance<IDominatorAccountViewModel>();
            InitializeComponent();
            AccountModule.DataContext = DominatorAccountViewModel;
        }

        #region Property

        public DominatorAccountViewModel DominatorAccountViewModel
        {
            get => _dominatorAccountViewModel;
            set
            {
                _dominatorAccountViewModel = value;
                OnPropertyChanged(nameof(DominatorAccountViewModel));
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public static AccountCustomControl GetAccountCustomControl(SocialNetworks socialNetworks,
            AccessorStrategies strategies)
        {
            if (_accountCustomInstance == null)
                _accountCustomInstance = new AccountCustomControl();

            UncheckAll();
            return _accountCustomInstance;
        }

        public static AccountCustomControl GetAccountCustomControl(SocialNetworks socialNework)
        {
            UncheckAll();
            return _accountCustomInstance ?? (_accountCustomInstance = new AccountCustomControl());
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static void UncheckAll()
        {
            InstanceProvider.GetInstance<IAccountCollectionViewModel>().GetCopySync().ForEach(x =>
            {
                x.IsAccountManagerAccountSelected = false;
            });
        }
    }
}
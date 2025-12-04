using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for AccountDetailsSelector.xaml
    /// </summary>
    public partial class AccountDetailsSelector : INotifyPropertyChanged
    {
        private readonly Action<AccountDetailsSelector> _updateAllDetails;

        private readonly Func<AccountDetailsSelector, PublisherCreateDestinationSelectModel, Task> _updateSinlgeDetails;
        private readonly Func<AccountDetailsSelector, AccountDetailsSelectorModel, Task> _updateSinlgeBoardSection;

        private AccountDetailsSelectorViewModel
            _accountDetailsSelectorViewModel = new AccountDetailsSelectorViewModel();

        private AccountDetailsSelectorModel _accountDetailsSelectorModel = new AccountDetailsSelectorModel();
        private readonly PublisherCreateDestinationSelectModel _publisherCreateDestinationSelectModel =
            new PublisherCreateDestinationSelectModel();

        public AccountDetailsSelector(bool isPageOptionVisible = false)
        {
            InitializeComponent();
            AccountDetailsSelectors.DataContext = AccountDetailsSelectorViewModel;
            AccountDetailsSelectorViewModel.IsPageOptionVisible = isPageOptionVisible;
        }


        public AccountDetailsSelector(Action<AccountDetailsSelector> updateAllData
            , string detailsType = "")
        {
            InitializeComponent();
            AccountDetailsSelectors.DataContext = AccountDetailsSelectorViewModel;
            AccountDetailsSelectorViewModel.IsPageOptionVisible = detailsType == "Page" ? true : false;
            AccountDetailsSelectorViewModel.IsGroupOptionVisible = detailsType == "Group" ? true : false;
            _updateAllDetails = updateAllData;
        }


        public AccountDetailsSelector(
            Func<AccountDetailsSelector, PublisherCreateDestinationSelectModel, Task> updateSingleData,
            PublisherCreateDestinationSelectModel publisherCreateDestinationSelectModel, string detailsType = "")
        {
            InitializeComponent();
            AccountDetailsSelectors.DataContext = AccountDetailsSelectorViewModel;
            AccountDetailsSelectorViewModel.IsPageOptionVisible = detailsType == "Page" ? true : false;
            AccountDetailsSelectorViewModel.IsGroupOptionVisible = detailsType == "Group" ? true : false;
            _updateSinlgeDetails = updateSingleData;
            _publisherCreateDestinationSelectModel = publisherCreateDestinationSelectModel;
        }

        public AccountDetailsSelector(Func<AccountDetailsSelector, AccountDetailsSelectorModel, Task> UpdateSingleBoardSection, AccountDetailsSelectorModel accountDetailsSelectorModel, string detailsType = "")
        {
            InitializeComponent();
            AccountDetailsSelectors.DataContext = AccountDetailsSelectorViewModel;
            AccountDetailsSelectorViewModel.IsPageOptionVisible = detailsType == "Page" ? true : false;
            AccountDetailsSelectorViewModel.IsGroupOptionVisible = detailsType == "Group" ? true : false;
            _accountDetailsSelectorModel = accountDetailsSelectorModel;
            _updateSinlgeBoardSection = UpdateSingleBoardSection;
        }

        public AccountDetailsSelectorViewModel AccountDetailsSelectorViewModel
        {
            get => _accountDetailsSelectorViewModel;
            set
            {
                if (AccountDetailsSelectorViewModel == value)
                    return;
                _accountDetailsSelectorViewModel = value;
                OnPropertyChanged(nameof(AccountDetailsSelectorViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateUiAllData()
        {
            ThreadFactory.Instance.Start(() => { _updateAllDetails.Invoke(this); });
        }

        public void UpdateUiSingleData()
        {
            ThreadFactory.Instance.Start(() =>
            {
                _updateSinlgeDetails.Invoke(this, _publisherCreateDestinationSelectModel);
            });
        }

        public void UpdateSingleBoardSection()
        {
            ThreadFactory.Instance.Start(() =>
            {
                _updateSinlgeBoardSection.Invoke(this, _accountDetailsSelectorModel);
            });
        }
    }
}
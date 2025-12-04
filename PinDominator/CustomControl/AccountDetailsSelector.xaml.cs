using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Utility;
using PinDominator.ViewModel;
using PinDominatorCore.PDModel;

namespace PinDominator.CustomControl
{
    /// <summary>
    ///     Interaction logic for AccountDetailsSelector.xaml
    /// </summary>
    public partial class AccountDetailsSelector : INotifyPropertyChanged
    {
        private readonly string _accountId;
        private readonly string _accountName;

        private readonly RepinCreateDestinationSelectModel _publisherCreateDestinationSelectModel =
            new RepinCreateDestinationSelectModel();
        private readonly AccountDetailsSelectorModel _accountDetailsSelectorModel = new AccountDetailsSelectorModel();
        private readonly Action<AccountDetailsSelector> _updateAllDetails;

        private readonly Func<AccountDetailsSelector, RepinCreateDestinationSelectModel, Task> _updateSinlgeDetails;
        private readonly Func<AccountDetailsSelector, AccountDetailsSelectorModel, Task> _updatSingleSectionDetails;

        private readonly Func<string, string, AccountDetailsSelector, Task> _updateUiDetails;

        private AccountDetailsSelectorViewModel
            _accountDetailsSelectorViewModel = new AccountDetailsSelectorViewModel();
        public AccountDetailsSelector() { }
        public AccountDetailsSelector(Func<string, string, AccountDetailsSelector, Task> updateUiData, string accountId,
            string accountName)
        {
            InitializeComponent();
            AccountDetailsSelectors.DataContext = AccountDetailsSelectorViewModel;
            _accountId = accountId;
            _accountName = accountName;
            _updateUiDetails = updateUiData;
        }


        public AccountDetailsSelector(Action<AccountDetailsSelector> updateAllData)
        {
            InitializeComponent();
            AccountDetailsSelectors.DataContext = AccountDetailsSelectorViewModel;
            _updateAllDetails = updateAllData;
        }


        public AccountDetailsSelector(
            Func<AccountDetailsSelector, RepinCreateDestinationSelectModel, Task> updateSingleData,
            RepinCreateDestinationSelectModel publisherCreateDestinationSelectModel)
        {
            InitializeComponent();
            AccountDetailsSelectors.DataContext = AccountDetailsSelectorViewModel;
            _updateSinlgeDetails = updateSingleData;
            _publisherCreateDestinationSelectModel = publisherCreateDestinationSelectModel;
        }
        public AccountDetailsSelector(
            Func<AccountDetailsSelector, AccountDetailsSelectorModel, Task> updateSingleSectionDetails,
            AccountDetailsSelectorModel accountDetailsSelectorModel)
        {
            InitializeComponent();
            AccountDetailsSelectors.DataContext = AccountDetailsSelectorViewModel;
            _updatSingleSectionDetails = updateSingleSectionDetails;
            _accountDetailsSelectorModel = accountDetailsSelectorModel;
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
        public void UpdateSingleSectionDetails() => ThreadFactory.Instance.Start(()=> { _updatSingleSectionDetails.Invoke(this, _accountDetailsSelectorModel); });
        private void chkQuery_Checked(object sender, RoutedEventArgs e)
        {
            CheckUnCheckQuery(sender, true);
        }


        private void chkQuery_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckUnCheckQuery(sender, false);
        }

        private void CheckUnCheckQuery(object sender, bool isSelect)
        {
            var dataContext = (RepinQueryContent) ((FrameworkElement) sender).DataContext;

            try
            {
                var get = AccountDetailsSelectorViewModel
                    .ListAccountDetailsSelectorModels.ToList()
                    .FirstOrDefault(x => x.QueryType.Any(y => y.BoardUrl == dataContext.BoardUrl));
                if (isSelect)
                {
                    var noSExceptAll = dataContext.Content.Equals("All");
                    var allSExceptAll =
                        get != null && get.QueryType.Where(x => x.Content != "All" && x.IsContentSelected)
                            .Select(y => true).Count() ==
                        get.QueryType.Count - 1;

                    if (noSExceptAll)
                        get.QueryType.ForEach(x => { x.IsContentSelected = true; });
                    else if (allSExceptAll)
                        get.QueryType.FirstOrDefault(x => x.Content == "All").IsContentSelected = true;
                }
                else
                {
                    var dall = get != null && get.QueryType.Any(x => x.Content == "All" && !x.IsContentSelected)
                                           && get.QueryType.Any(x => x.Content != "All" && x.IsContentSelected);
                    var allSExpOne = get.QueryType.Where(x => x.Content != "All" && !x.IsContentSelected)
                                         .Select(y => true).Count() == 1;
                    var allSExpAll = get.QueryType.Where(x => !x.IsContentSelected).Select(y => true).Count() == 1;

                    if (dall && allSExpAll)
                    {
                        get.QueryType.ForEach(x => { x.IsContentSelected = false; });
                    }
                    else if (allSExpOne)
                    {
                        var repinQueryContent = get.QueryType.FirstOrDefault(x => x.Content == "All");
                        if (repinQueryContent != null) repinQueryContent.IsContentSelected = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
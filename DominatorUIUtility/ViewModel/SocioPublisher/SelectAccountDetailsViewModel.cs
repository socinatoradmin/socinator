using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.Views.SocioPublisher;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class SelectAccountDetailsViewModel : BindableBase
    {
        private readonly IAccountsFileManager _accountsFileManager;

        //ConstructorS
        public SelectAccountDetailsViewModel()
        {
            _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            NavigationCommand = new BaseCommand<object>(NavigationCanExecute, NavigationExecute);
            GetSingleAccountGroupsCommand =
                new BaseCommand<object>(GetSingleAccountGroupsCanExecute, GetSingleAccountGroupsExecute);
            GetSingleAccountPagesOrBoardsCommand = new BaseCommand<object>(GetSingleAccountPagesOrBoardsCanExecute,
                GetSingleAccountPagesOrBoardsExecute);
            GetSingleAccountFriendsCommand =
                new BaseCommand<object>(GetSingleAccountFriendsCanExecute, GetSingleAccountFriendsExecute);
            SelectionCommand = new BaseCommand<object>(SelectionCanExecute, SelectionExecute);
            OpenContextMenuCommand = new BaseCommand<object>(OpenContextMenuCanExecute, OpenContextMenuExecute);
            SelectAllAccountDetailsCommand =
                new BaseCommand<object>(SelectAccountDetailsCanExecute, SelectAccountDetailsExecute);
            SaveDestinationCommand = new BaseCommand<object>(SaveDestinationCanExecute, SaveDestinationExecute);
            ClearCommand = new BaseCommand<object>(ClearCanExecute, ClearExecute);
            StatusSyncCommand = new BaseCommand<object>(SyncCanExecute, SyncExecute);
            AddFreshAccounts = new BaseCommand<object>(AddFreshAccountCanExecute, AddFreshAccountExecute);
            AddCustomDestinationCommand =
                new BaseCommand<object>(AddCustomDestinationCanExecute, AddCustomDestinationExecute);
            InitializeProperties();
            InitializeDestinationList();
            IsSavedDestination = false;
        }

        public void InitializeProperties()
        {
            Title = "LangKeyCreateDestination".FromResourceDictionary();
            IsAllDestinationSelected = false;
            EditDestinationId = string.Empty;
            IsSavedDestination = false;
            SelectAccountDetailsModel = SelectAccountDetailsModel.DestinationDefaultBuilder();
        }

        #region Validate Destinations

        public bool IsDuplicate()
        {
            if (!string.IsNullOrEmpty(EditDestinationId))
                return false;

            var availableCount = PublisherManageDestinations.Instance().PublisherManageDestinationViewModel
                .ListPublisherManageDestinationModels.Count;

            if (availableCount == 0)
                return false;

            // check destination name is already present or not 
            var isPresent = false;

            foreach (var x in PublisherManageDestinations.Instance().PublisherManageDestinationViewModel
                .ListPublisherManageDestinationModels)
            {
                if (x.DestinationName == SelectAccountDetailsModel.DestinationName)
                    isPresent = true;

                if (isPresent)
                    break;
            }

            return isPresent;
        }

        #endregion

        #region Edit Destination

        public void EditDestination()
        {
            Title = "LangKeyEditDestination".FromResourceDictionary();

            var saveDestination = SelectAccountDetailsModel.DeepCloneObject();

            InitializeDestinationList();

            var currentlyAvailableAccounts =
                SelectAccountDetailsModel.ListSelectDestination.Select(x => x.AccountId).ToList();

            foreach (var savedDestination in saveDestination.ListSelectDestination)
            {
                if (!currentlyAvailableAccounts.Contains(savedDestination.AccountId))
                {
                    savedDestination.StatusSyncContent = ConstantVariable.NotAvailableAccountSync;
                    continue;
                }

                var currentAccountDetails =
                    SelectAccountDetailsModel.ListSelectDestination.FirstOrDefault(x =>
                        x.AccountId == savedDestination.AccountId);

                if (currentAccountDetails == null)
                    return;

                if (savedDestination.TotalGroups != currentAccountDetails.TotalGroups ||
                    savedDestination.TotalPagesOrBoards != currentAccountDetails.TotalPagesOrBoards)
                    savedDestination.StatusSyncContent = ConstantVariable.NeedUpdateStatusSync;

                // currentAccountDetails = savedDestination;
            }

            SelectAccountDetailsModel = saveDestination;

            // SelectAccountDetailsModel.UpdateDestination(saveDestination);

            DestinationCollectionView =
                CollectionViewSource.GetDefaultView(SelectAccountDetailsModel.ListSelectDestination);
        }

        #endregion

        #region Properties

        private SelectAccountDetailsModel _selctAccountDetailsModel =
            SelectAccountDetailsModel.DestinationDefaultBuilder();

        public SelectAccountDetailsModel SelectAccountDetailsModel
        {
            get => _selctAccountDetailsModel;
            set
            {
                if (_selctAccountDetailsModel == value)
                    return;
                _selctAccountDetailsModel = value;
                OnPropertyChanged(nameof(SelectAccountDetailsModel));
            }
        }

        private ICollectionView _destinationCollectionView;

        public ICollectionView DestinationCollectionView
        {
            get => _destinationCollectionView;
            set
            {
                if (_destinationCollectionView != null && _destinationCollectionView == value)
                    return;
                SetProperty(ref _destinationCollectionView, value);
            }
        }

        public string EditDestinationId { get; set; } = string.Empty;

        public bool IsSavedDestination { get; set; }

        private string _title = string.Empty;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string _pageColWidth = "130";
        private string _groupColWidth = "130";
        private string _friendColWidth = "130";
        private string _customColWidth = "130";

        private bool _isPageMenuVisible = true;
        private bool _isGroupMenuVisible = true;
        private bool _isFriendMenuVisible = true;

        private bool _isFanpage;

        private bool _IsHeaderNeeded = true;

        private string _InviteForPagesText = "LangKeyInviteForPages".FromResourceDictionary();

        public bool GroupMenuVisible
        {
            get => _isGroupMenuVisible;
            set
            {
                if (_isGroupMenuVisible == value)
                    return;
                _isGroupMenuVisible = value;
                OnPropertyChanged(nameof(GroupMenuVisible));
            }
        }


        public bool PageMenuVisible
        {
            get => _isPageMenuVisible;
            set
            {
                if (_isPageMenuVisible == value)
                    return;
                _isPageMenuVisible = value;
                OnPropertyChanged(nameof(PageMenuVisible));
            }
        }


        public bool FriendMenuVisible
        {
            get => _isFriendMenuVisible;
            set
            {
                if (_isFriendMenuVisible == value)
                    return;
                _isFriendMenuVisible = value;
                OnPropertyChanged(nameof(FriendMenuVisible));
            }
        }

        public string InviteForPagesText
        {
            get => _InviteForPagesText;
            set
            {
                if (_InviteForPagesText == value)
                    return;
                _InviteForPagesText = value;
                OnPropertyChanged(nameof(InviteForPagesText));
            }
        }

        public string PageColWidth
        {
            get => _pageColWidth;
            set
            {
                if (_pageColWidth == value)
                    return;
                _pageColWidth = value;
                OnPropertyChanged(nameof(PageColWidth));
                if (value == "0")
                    PageMenuVisible = false;
            }
        }


        public string GroupColWidth
        {
            get => _groupColWidth;
            set
            {
                if (_groupColWidth == value)
                    return;
                _groupColWidth = value;
                OnPropertyChanged(nameof(GroupColWidth));
                if (value == "0")
                    GroupMenuVisible = false;
            }
        }


        public string FriendColWidth
        {
            get => _friendColWidth;
            set
            {
                if (_friendColWidth == value)
                    return;
                _friendColWidth = value;
                OnPropertyChanged(nameof(FriendColWidth));
                if (value == "0")
                    FriendMenuVisible = false;
            }
        }


        public string CustomColumnWidth
        {
            get => _customColWidth;
            set
            {
                if (_customColWidth == value)
                    return;
                _customColWidth = value;
                OnPropertyChanged(nameof(CustomColumnWidth));
            }
        }


        public bool IsFanpage
        {
            get => _isFanpage;
            set
            {
                if (_isFanpage == value)
                    return;
                _isFanpage = value;
                OnPropertyChanged(nameof(IsFanpage));
            }
        }


        public bool IsHeaderNeeded
        {
            get => _IsHeaderNeeded;
            set
            {
                if (_IsHeaderNeeded == value)
                    return;
                _IsHeaderNeeded = value;
                OnPropertyChanged(nameof(IsHeaderNeeded));
            }
        }

        private bool _isSelectedSingleAccount;

        public bool IsSelectedSingleAccount
        {
            get => _isSelectedSingleAccount;
            set
            {
                if (_isSelectedSingleAccount == value)
                    return;
                _isSelectedSingleAccount = value;
                SelectAccountDetailsModel.IsDisplaySingleAccount = true;
                OnPropertyChanged(nameof(IsSelectedSingleAccount));
            }
        }

        private string _DisplayAccount = string.Empty;

        public string DisplayAccount
        {
            get => _DisplayAccount;
            set
            {
                if (_DisplayAccount == value)
                    return;
                _DisplayAccount = value;
                SelectAccountDetailsModel.DisplayAccount = value;
                OnPropertyChanged(nameof(FriendColWidth));
            }
        }


        private bool _isAllDestinationSelected;

        public bool IsAllDestinationSelected
        {
            get => _isAllDestinationSelected;
            set
            {
                if (_isAllDestinationSelected == value)
                    return;
                SetProperty(ref _isAllDestinationSelected, value);
                SelectAllDestination(_isAllDestinationSelected);
                _isUncheckedFromList = false;
            }
        }

        public ICommand AddFreshAccounts { get; set; }

        public ICommand StatusSyncCommand { get; set; }

        public ICommand ClearCommand { get; set; }

        public ICommand NavigationCommand { get; set; }

        public ICommand GetSingleAccountGroupsCommand { get; set; }

        public ICommand SelectAllAccountDetailsCommand { get; set; }

        public ICommand OpenContextMenuCommand { get; set; }

        public ICommand SelectionCommand { get; set; }

        public ICommand GetSingleAccountPagesOrBoardsCommand { get; set; }

        public ICommand GetSingleAccountFriendsCommand { get; set; }

        public ICommand SaveDestinationCommand { get; set; }

        public ICommand AddCustomDestinationCommand { get; set; }

        private List<string> _needToUpdateAccounts = new List<string>();


        public List<string> GroupsAvailableInNetworks { get; set; } =
            new List<string> {"Facebook", "LinkedIn", "Reddit"};

        public List<string> WallAvailableInNetworks { get; set; } = new List<string> {"Pinterest", "Tumblr"};

        public List<string> ScrapingAvailableInNetworks { get; set; } =
            new List<string> {"Facebook", "Pinterest", "Twitter", "Reddit"};

        public List<string> BoardsOrPagesAvailableInNetworks { get; set; } = new List<string>
            {"Facebook", "YouTube", "Pinterest", "LinkedIn", "Gplus", "Tumblr"};

        public List<string> FriendsAvailableInNetworks { get; set; } = new List<string> {"Facebook", "LinkedIn"};

        #endregion

        #region Navigation

        private bool NavigationCanExecute(object sender)
        {
            return true;
        }

        private void NavigationExecute(object sender)
        {
            var module = sender.ToString();
            switch (module)
            {
                // Send back to manage destinations
                case "Back":
                    ClearCurrentDestination();
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl
                        = PublisherManageDestinations.Instance();
                    break;
            }
        }

        #endregion

        #region Get Single Account Groups Details

        private bool GetSingleAccountGroupsCanExecute(object sender)
        {
            return true;
        }

        private void GetSingleAccountGroupsExecute(object sender)
        {
            // get the selected accounts destinations model
            var allAccountDetailsSelectModel = (PublisherCreateDestinationSelectModel) sender;

            // get already selected group pairs
            var valuePairs = SelectAccountDetailsModel.AccountGroupPair
                .Where(x => x.Key == allAccountDetailsSelectModel.AccountId).ToList();

            var alreadySelectedGroups = valuePairs.Select(x => x.Value).ToList();

            // Get the initial selector details and also passing the action for getting the group details
            var accountDetailsSelector = new AccountDetailsSelector(UpdateSingleAccountGroupsDetails,
                allAccountDetailsSelectModel, "Group")
            {
                AccountDetailsSelectorViewModel =
                {
                    Title = "LangKeySelectGroup".FromResourceDictionary(),
                    DetailsUrlHeader = "LangKeyGroupUrl".FromResourceDictionary(),
                    DetailsNameHeader = "LangKeyGroupName".FromResourceDictionary(),
                    AlreadySelectedList = alreadySelectedGroups
                }
            };

            var dialog = new Dialog();

            // display the dialog window
            var window = dialog.GetMetroWindow(accountDetailsSelector, "LangKeySelectGroup".FromResourceDictionary());

            accountDetailsSelector.btnSave.Click += (senderDetails, events) =>
            {
                // Remove already saved group pairs 
                valuePairs.ForEach(x =>
                {
                    SelectAccountDetailsModel.AccountGroupPair.Remove(x);
                    SelectAccountDetailsModel.DestinationDetailsModels.RemoveAll(y =>
                        x.Key == y.AccountId && y.DestinationType == ConstantVariable.Group);
                });

                // get currectly selected groups from UI objects
                var keyValuePairs = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItems().ToList();

                // get the full destination details
                var destinationDetails = accountDetailsSelector.AccountDetailsSelectorViewModel
                    .GetSelectedItemsDestinations(ConstantVariable.Group).ToList();

                // Append with destination details of the accounts
                SelectAccountDetailsModel.DestinationDetailsModels.AddRange(destinationDetails);

                // Add to account's group pair
                SelectAccountDetailsModel.AccountGroupPair.AddRange(keyValuePairs);

                alreadySelectedGroups = SelectAccountDetailsModel.AccountGroupPair
                    .Where(x => x.Key == allAccountDetailsSelectModel.AccountId).Select(x => x.Value).ToList();

                var list = new ObservableCollection<PublisherCreateDestinationSelectModel>();

                var currentAccountId = string.Empty;

                if (keyValuePairs.Count > 0) currentAccountId = keyValuePairs[0].Key.ToString();

                SelectAccountDetailsModel.ListSelectDestination.ForEach(x =>
                {
                    if (x.AccountId == currentAccountId) x.SelectedGroups = keyValuePairs.Count;

                    list.Add(x);
                });

                SelectAccountDetailsModel.ListSelectDestination = list;

                var createDestinationSelectModel =
                    SelectAccountDetailsModel.ListSelectDestination.FirstOrDefault(x =>
                        x.AccountId == allAccountDetailsSelectModel.AccountId);

                // Get the group selector details
                if (createDestinationSelectModel != null)
                    createDestinationSelectModel.GroupSelectorText =
                        $"{alreadySelectedGroups.Count}/{createDestinationSelectModel.TotalGroups}";

                window.Close();
            };

            accountDetailsSelector.btnCancel.Click += (senderDetails, events) => { window.Close(); };

            window.Show();

            // Trigger the action
            accountDetailsSelector.UpdateUiSingleData();
        }

        private async Task UpdateSingleAccountGroupsDetails(AccountDetailsSelector accountDetailsSelector,
            PublisherCreateDestinationSelectModel allAccountDetailsSelectModel)
        {
            // Get the account group pair
            var valuePairs = SelectAccountDetailsModel.AccountGroupPair
                .Where(x => x.Key == allAccountDetailsSelectModel.AccountId).ToList();
            ;

            // Get already selected groups
            var alreadySelectedGroups = valuePairs.Select(x => x.Value).ToList();

            if (GroupsAvailableInNetworks.Contains(allAccountDetailsSelectModel.SocialNetworks.ToString()))
            {
                // Get the factory for account selector for a network
                var accountsDetailsSelector = SocinatorInitialize
                    .GetSocialLibrary(allAccountDetailsSelectModel.SocialNetworks)
                    .GetNetworkCoreFactory().AccountDetailsSelectors;

                // fetch the groups details for particular accounts
                var groups = await accountsDetailsSelector.GetGroupsDetails(allAccountDetailsSelectModel.AccountId,
                    allAccountDetailsSelectModel.AccountName, alreadySelectedGroups);

                groups.ForEach(group =>
                {
                    group.CurrentIndex = accountDetailsSelector.AccountDetailsSelectorViewModel
                                             .ListAccountDetailsSelectorModels.Count + 1;
                    group.Network = allAccountDetailsSelectModel.SocialNetworks;

                    // Add the group details to Ui's view model 
                    if (!Application.Current.Dispatcher.CheckAccess())
                        Application.Current.Dispatcher.Invoke(() =>
                            accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(
                                group));
                    else
                        accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels
                            .Add(group);
                });
            }

            // Update the status of details selector
            UpdateStatus(accountDetailsSelector);
        }

        #endregion

        #region Get Single Account Page Details

        private bool GetSingleAccountPagesOrBoardsCanExecute(object sender)
        {
            return true;
        }

        private void GetSingleAccountPagesOrBoardsExecute(object sender)
        {
            var allAccountDetailsSelectModel = (PublisherCreateDestinationSelectModel) sender;

            // get the page or board pair from collection with the account Id
            var valuePairs = SelectAccountDetailsModel.AccountPagesBoardsPair
                .Where(x => x.Key == allAccountDetailsSelectModel.AccountId).ToList();

            // get the factory pattern for the network of an account
            var accountsDetailsSelector = SocinatorInitialize
                .GetSocialLibrary(allAccountDetailsSelectModel.SocialNetworks)
                .GetNetworkCoreFactory().AccountDetailsSelectors;

            // Fetch the page details only
            var alreadySelectedPages = valuePairs.Select(x => x.Value).ToList();

            // Pass the fetching activity functions as action to UI
            var accountDetailsSelector = new AccountDetailsSelector(UpdateSingleAccountPagesDetails,
                allAccountDetailsSelectModel, "Page")
            {
                // Find whether page or board, its vary based on each network
                AccountDetailsSelectorViewModel =
                {
                    Title = string.Format("LangKeySelectAnything".FromResourceDictionary(),
                        accountsDetailsSelector.DisplayAsPageOrBoards),
                    DetailsUrlHeader = string.Format("LangKeyAnyUrl".FromResourceDictionary(),
                        accountsDetailsSelector.DisplayAsPageOrBoards),
                    DetailsNameHeader = string.Format("LangKeyAnyName".FromResourceDictionary(),
                        accountsDetailsSelector.DisplayAsPageOrBoards),
                    AlreadySelectedList = alreadySelectedPages
                }
            };

            var dialog = new Dialog();

            var window = dialog.GetMetroWindow(accountDetailsSelector,
                string.Format("LangKeySelectAnything".FromResourceDictionary(),
                    accountsDetailsSelector.DisplayAsPageOrBoards));

            // Defining the save buttons click events
            accountDetailsSelector.btnSave.Click += (senderDetails, events) =>
            {
                // Remove all the saved accounts page or boards pair
                valuePairs.ForEach(x =>
                {
                    SelectAccountDetailsModel.AccountPagesBoardsPair.Remove(x);
                    SelectAccountDetailsModel.DestinationDetailsModels.RemoveAll(y =>
                        x.Key == y.AccountId && y.DestinationType == ConstantVariable.PageOrBoard);
                });

                // Get the selected pairs
                var keyValuePairs = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItems().ToList();

                // Add to key value pair
                SelectAccountDetailsModel.AccountPagesBoardsPair.AddRange(keyValuePairs);

                // Get the destination full details of a page or board
                var destinationDetails = accountDetailsSelector.AccountDetailsSelectorViewModel
                    .GetSelectedItemsDestinations(ConstantVariable.PageOrBoard).ToList();

                // Update with destination details
                SelectAccountDetailsModel.DestinationDetailsModels.AddRange(destinationDetails);

                // Get the already selected page details 
                alreadySelectedPages = SelectAccountDetailsModel.AccountPagesBoardsPair
                    .Where(x => x.Key == allAccountDetailsSelectModel.AccountId).Select(x => x.Value).ToList();

                //
                var createDestinationSelectModel =
                    SelectAccountDetailsModel.ListSelectDestination.FirstOrDefault(x =>
                        x.AccountId == allAccountDetailsSelectModel.AccountId);

                if (createDestinationSelectModel != null)
                    createDestinationSelectModel.PagesOrBoardsSelectorText =
                        $"{alreadySelectedPages.Count}/{createDestinationSelectModel.TotalPagesOrBoards}";

                window.Close();
            };

            accountDetailsSelector.btnCancel.Click += (senderDetails, events) => { window.Close(); };

            window.Show();

            accountDetailsSelector.UpdateUiSingleData();
        }

        private async Task UpdateSingleAccountPagesDetails(AccountDetailsSelector accountDetailsSelector,
            PublisherCreateDestinationSelectModel allAccountDetailsSelectModel)
        {
            var valuePairs = SelectAccountDetailsModel.AccountPagesBoardsPair
                .Where(x => x.Key == allAccountDetailsSelectModel.AccountId).ToList();
            ;

            var alreadySelectedPages = valuePairs.Select(x => x.Value).ToList();

            if (BoardsOrPagesAvailableInNetworks.Contains(allAccountDetailsSelectModel.SocialNetworks.ToString()))
            {
                var accountsDetailsSelector = SocinatorInitialize
                    .GetSocialLibrary(allAccountDetailsSelectModel.SocialNetworks)
                    .GetNetworkCoreFactory().AccountDetailsSelectors;

                var pagesOrBoards = await accountsDetailsSelector.GetPagesDetails(
                    allAccountDetailsSelectModel.AccountId, allAccountDetailsSelectModel.AccountName,
                    alreadySelectedPages);

                pagesOrBoards.ForEach(page =>
                {
                    page.CurrentIndex = accountDetailsSelector.AccountDetailsSelectorViewModel
                                            .ListAccountDetailsSelectorModels.Count + 1;
                    page.Network = allAccountDetailsSelectModel.SocialNetworks;

                    if (!Application.Current.Dispatcher.CheckAccess())
                        Application.Current.Dispatcher.Invoke(() =>
                            accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels
                                .Add(page));
                    else
                        accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels
                            .Add(page);
                });
            }

            UpdateStatus(accountDetailsSelector);
        }

        #endregion


        #region Get Single Account Friends Details

        private bool GetSingleAccountFriendsCanExecute(object sender)
        {
            return true;
        }

        private void GetSingleAccountFriendsExecute(object sender)
        {
            // get the selected accounts destinations model
            var allAccountDetailsSelectModel = (PublisherCreateDestinationSelectModel) sender;

            // get already selected group pairs
            var valuePairs = SelectAccountDetailsModel.AccountFriendsPair
                .Where(x => x.Key == allAccountDetailsSelectModel.AccountId).ToList();

            var alreadySelectedFriends = valuePairs.Select(x => x.Value).ToList();

            // Get the initial selector details and also passing the action for getting the group details
            var accountDetailsSelector = new AccountDetailsSelector(UpdateSingleAccountFriendsDetails,
                allAccountDetailsSelectModel)
            {
                AccountDetailsSelectorViewModel =
                {
                    Title = "LangKeyFriendsSelect".FromResourceDictionary(),
                    DetailsUrlHeader = "LangKeyFriendUrl".FromResourceDictionary(),
                    DetailsNameHeader = "LangKeyFriendName".FromResourceDictionary(),
                    AlreadySelectedList = alreadySelectedFriends
                }
            };

            var dialog = new Dialog();

            // display the dialog window
            var window = dialog.GetMetroWindow(accountDetailsSelector, "LangKeyFriendsSelect".FromResourceDictionary());

            accountDetailsSelector.btnSave.Click += (senderDetails, events) =>
            {
                // Remove already saved group pairs 
                valuePairs.ForEach(x =>
                {
                    SelectAccountDetailsModel.AccountFriendsPair.Remove(x);
                    SelectAccountDetailsModel.DestinationDetailsModels.RemoveAll(y =>
                        x.Key == y.AccountId && y.DestinationType == ConstantVariable.Group);
                });

                // get currectly selected groups from UI objects
                var keyValuePairs = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItems().ToList();

                // get the full destination details
                var destinationDetails = accountDetailsSelector.AccountDetailsSelectorViewModel
                    .GetSelectedItemsDestinations(ConstantVariable.Group).ToList();

                // Append with destination details of the accounts
                SelectAccountDetailsModel.DestinationDetailsModels.AddRange(destinationDetails);

                // Add to account's group pair
                SelectAccountDetailsModel.AccountFriendsPair.AddRange(keyValuePairs);

                alreadySelectedFriends = SelectAccountDetailsModel.AccountFriendsPair
                    .Where(x => x.Key == allAccountDetailsSelectModel.AccountId).Select(x => x.Value).ToList();

                var createDestinationSelectModel =
                    SelectAccountDetailsModel.ListSelectDestination.FirstOrDefault(x =>
                        x.AccountId == allAccountDetailsSelectModel.AccountId);

                // Get the group selector details
                if (createDestinationSelectModel != null)
                    createDestinationSelectModel.FriendsSelectorText =
                        $"{alreadySelectedFriends.Count}/{createDestinationSelectModel.TotalFriends}";

                window.Close();
            };

            accountDetailsSelector.btnCancel.Click += (senderDetails, events) => { window.Close(); };

            window.Show();

            // Trigger the action
            accountDetailsSelector.UpdateUiSingleData();
        }

        private async Task UpdateSingleAccountFriendsDetails(AccountDetailsSelector accountDetailsSelector,
            PublisherCreateDestinationSelectModel allAccountDetailsSelectModel)
        {
            var valuePairs = SelectAccountDetailsModel.AccountFriendsPair
                .Where(x => x.Key == allAccountDetailsSelectModel.AccountId).ToList();
            ;

            // Get already selected groups
            var alreadySelectedGroups = valuePairs.Select(x => x.Value).ToList();

            if (GroupsAvailableInNetworks.Contains(allAccountDetailsSelectModel.SocialNetworks.ToString()))
            {
                // Get the factory for account selector for a network
                var accountsDetailsSelector = SocinatorInitialize
                    .GetSocialLibrary(allAccountDetailsSelectModel.SocialNetworks)
                    .GetNetworkCoreFactory().AccountDetailsSelectors;

                // fetch the groups details for particular accounts
                var groups = await accountsDetailsSelector.GetFriendsDetails(allAccountDetailsSelectModel.AccountId,
                    allAccountDetailsSelectModel.AccountName, alreadySelectedGroups);

                groups.ForEach(friend =>
                {
                    friend.CurrentIndex = accountDetailsSelector.AccountDetailsSelectorViewModel
                                              .ListAccountDetailsSelectorModels.Count + 1;
                    friend.Network = allAccountDetailsSelectModel.SocialNetworks;

                    // Add the group details to Ui's view model 
                    if (!Application.Current.Dispatcher.CheckAccess())
                        Application.Current.Dispatcher.Invoke(() =>
                            accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(
                                friend));
                    else
                        accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(
                            friend);
                });
            }

            // Update the status of details selector
            UpdateStatus(accountDetailsSelector);
        }

        #endregion

        #region Open Context

        private bool OpenContextMenuCanExecute(object sender)
        {
            return true;
        }

        private void OpenContextMenuExecute(object sender)
        {
            try
            {
                var contextMenu = ((Button) sender).ContextMenu;
                if (contextMenu == null) return;
                contextMenu.DataContext = ((Button) sender).DataContext;
                contextMenu.IsOpen = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

        #region Selection execution for all destination to select

        private bool SelectionCanExecute(object sender)
        {
            return true;
        }

        private void SelectionExecute(object sender)
        {
            var moduleName = sender.ToString();
            switch (moduleName)
            {
                case "MenuSelectNone":
                    IsAllDestinationSelected = false;
                    break;

                case "MenuSelectAll":
                    IsAllDestinationSelected = true;
                    break;
                case "SelectManually":

                    if (SelectAccountDetailsModel.ListSelectDestination.All(x => x.IsAccountSelected))
                    {
                        IsAllDestinationSelected = true;
                    }
                    else
                    {
                        if (IsAllDestinationSelected)
                            _isUncheckedFromList = true;
                        IsAllDestinationSelected = false;
                    }

                    break;
            }
        }

        private bool _isUncheckedFromList { get; set; }

        #endregion

        #region Select Destination fucntionality , and also selection menu options

        public void SelectAllDestination(bool isChecked)
        {
            if (_isUncheckedFromList)
                return;
            SelectAccountDetailsModel.ListSelectDestination.Select(x =>
            {
                x.IsAccountSelected = isChecked;
                return x;
            }).ToList();
        }

        private bool SelectAccountDetailsCanExecute(object sender)
        {
            return true;
        }

        private void SelectAccountDetailsExecute(object sender)
        {
            var moduleName = sender.ToString();
            switch (moduleName)
            {
                case "OwnProfile":
                    LoadAllAccountsFriends();
                    break;
                case "Groups":
                    LoadAllAccountsGroup();
                    break;
                case "Pages":
                    LoadAllAccountsPagesOrBoards();
                    break;
            }
        }

        #endregion

        #region Get all account's pages, groups and Friends details

        public void LoadAllAccountsGroup()
        {
            var valuePairs = SelectAccountDetailsModel.AccountGroupPair.ToList();

            var alreadySelectedGroups = valuePairs.Select(x => x.Value).ToList();

            var accountDetailsSelector = new AccountDetailsSelector(UpdateAllGroupsDetails, "Group")
            {
                AccountDetailsSelectorViewModel =
                {
                    Title = "LangKeySelectGroup".FromResourceDictionary(),
                    DetailsUrlHeader = "LangKeyGroupUrl".FromResourceDictionary(),
                    DetailsNameHeader = "LangKeyGroupName".FromResourceDictionary(),
                    AlreadySelectedList = alreadySelectedGroups
                }
            };

            var dialog = new Dialog();

            var window = dialog.GetMetroWindow(accountDetailsSelector, "LangKeySelectGroup".FromResourceDictionary());

            accountDetailsSelector.btnSave.Click += (sender, events) =>
            {
                valuePairs.ForEach(x =>
                {
                    SelectAccountDetailsModel.AccountGroupPair.Remove(x);
                    SelectAccountDetailsModel.DestinationDetailsModels.RemoveAll(y =>
                        x.Key == y.AccountId && y.DestinationType == ConstantVariable.Group);
                });

                var keyValuePairs = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItems().ToList();

                SelectAccountDetailsModel.AccountGroupPair.AddRange(keyValuePairs);

                var destinationDetails = accountDetailsSelector.AccountDetailsSelectorViewModel
                    .GetSelectedItemsDestinations(ConstantVariable.Group).ToList();

                SelectAccountDetailsModel.DestinationDetailsModels.AddRange(destinationDetails);

                keyValuePairs.ForEach(selectedItems =>
                {
                    alreadySelectedGroups = SelectAccountDetailsModel.AccountGroupPair
                        .Where(x => x.Key == selectedItems.Key).Select(x => x.Value).ToList();

                    var createDestinationSelectModel =
                        SelectAccountDetailsModel.ListSelectDestination.FirstOrDefault(x =>
                            x.AccountId == selectedItems.Key);

                    if (createDestinationSelectModel != null)
                        createDestinationSelectModel.GroupSelectorText =
                            $"{alreadySelectedGroups.Count}/{createDestinationSelectModel.TotalGroups}";
                });

                window.Close();
            };

            accountDetailsSelector.btnCancel.Click += (sender, events) => { window.Close(); };

            window.Show();

            accountDetailsSelector.UpdateUiAllData();
        }

        private void UpdateAllGroupsDetails(AccountDetailsSelector accountDetailsSelector)
        {
            var valuePairs = SelectAccountDetailsModel.AccountGroupPair.ToList();

            var alreadySelectedGroups = valuePairs.Select(x => x.Value).ToList();

            var count = SelectAccountDetailsModel.ListSelectDestination.Count;

            SelectAccountDetailsModel.ListSelectDestination.ForEach(async x =>
            {
                if (GroupsAvailableInNetworks.Contains(x.SocialNetworks.ToString()))
                {
                    var accountsDetailsSelector = SocinatorInitialize
                        .GetSocialLibrary(x.SocialNetworks)
                        .GetNetworkCoreFactory().AccountDetailsSelectors;

                    var groups =
                        await accountsDetailsSelector.GetGroupsDetails(x.AccountId, x.AccountName,
                            alreadySelectedGroups);

                    groups.ForEach(group =>
                    {
                        group.CurrentIndex = accountDetailsSelector.AccountDetailsSelectorViewModel
                                                 .ListAccountDetailsSelectorModels.Count + 1;
                        group.Network = x.SocialNetworks;
                        if (!Application.Current.Dispatcher.CheckAccess())
                            Application.Current.Dispatcher.Invoke(() =>
                                accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels
                                    .Add(group));
                        else
                            accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(
                                group);
                    });
                }

                count--;

                if (count <= 0)
                    UpdateStatus(accountDetailsSelector);
            });
        }

        public void LoadAllAccountsPagesOrBoards()
        {
            var valuePairs = SelectAccountDetailsModel.AccountPagesBoardsPair.ToList();

            var alreadySelectedPages = valuePairs.Select(x => x.Value).ToList();

            var accountDetailsSelector = new AccountDetailsSelector(UpdatePagesDetails, "Page")
            {
                AccountDetailsSelectorViewModel =
                {
                    Title = "LangKeySelectPagesBoards".FromResourceDictionary(),
                    DetailsUrlHeader = "LangKeyPagesBoardsUrl".FromResourceDictionary(),
                    DetailsNameHeader = "LangKeyPagesBoardsName".FromResourceDictionary(),
                    AlreadySelectedList = alreadySelectedPages
                }
            };

            var dialog = new Dialog();

            var window = dialog.GetMetroWindow(accountDetailsSelector,
                "LangKeySelectPagesBoards".FromResourceDictionary());

            accountDetailsSelector.btnSave.Click += (sender, events) =>
            {
                valuePairs.ForEach(x =>
                {
                    SelectAccountDetailsModel.AccountPagesBoardsPair.Remove(x);
                    SelectAccountDetailsModel.DestinationDetailsModels.RemoveAll(y =>
                        x.Key == y.AccountId && y.DestinationType == ConstantVariable.PageOrBoard);
                });

                var keyValuePairs = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItems().ToList();

                SelectAccountDetailsModel.AccountPagesBoardsPair.AddRange(keyValuePairs);

                var destinationDetails = accountDetailsSelector.AccountDetailsSelectorViewModel
                    .GetSelectedItemsDestinations(ConstantVariable.PageOrBoard).ToList();

                SelectAccountDetailsModel.DestinationDetailsModels.AddRange(destinationDetails);

                keyValuePairs.ForEach(selectedItems =>
                {
                    alreadySelectedPages = SelectAccountDetailsModel.AccountPagesBoardsPair
                        .Where(x => x.Key == selectedItems.Key).Select(x => x.Value).ToList();

                    var createDestinationSelectModel =
                        SelectAccountDetailsModel.ListSelectDestination.FirstOrDefault(x =>
                            x.AccountId == selectedItems.Key);

                    if (createDestinationSelectModel != null)
                        createDestinationSelectModel.PagesOrBoardsSelectorText =
                            $"{alreadySelectedPages.Count}/{createDestinationSelectModel.TotalPagesOrBoards}";
                });
                window.Close();
            };

            accountDetailsSelector.btnCancel.Click += (sender, events) => { window.Close(); };

            window.Show();

            accountDetailsSelector.UpdateUiAllData();
        }

        private void UpdatePagesDetails(AccountDetailsSelector accountDetailsSelector)
        {
            var valuePairs = SelectAccountDetailsModel.AccountPagesBoardsPair.ToList();

            var alreadySelectedPages = valuePairs.Select(x => x.Value).ToList();

            var count = SelectAccountDetailsModel.ListSelectDestination.Count;

            SelectAccountDetailsModel.ListSelectDestination.ForEach(async x =>
            {
                try
                {
                    if (BoardsOrPagesAvailableInNetworks.Contains(x.SocialNetworks.ToString()))
                    {
                        var accountsDetailsSelector = SocinatorInitialize
                            .GetSocialLibrary(x.SocialNetworks)
                            .GetNetworkCoreFactory().AccountDetailsSelectors;

                        var pages = await accountsDetailsSelector.GetPagesDetails(x.AccountId, x.AccountName,
                            alreadySelectedPages);

                        pages.ForEach(group =>
                        {
                            group.CurrentIndex = accountDetailsSelector.AccountDetailsSelectorViewModel
                                                     .ListAccountDetailsSelectorModels.Count + 1;
                            group.Network = x.SocialNetworks;
                            if (!Application.Current.Dispatcher.CheckAccess())
                                Application.Current.Dispatcher.Invoke(() =>
                                    accountDetailsSelector.AccountDetailsSelectorViewModel
                                        .ListAccountDetailsSelectorModels.Add(group));
                            else
                                accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels
                                    .Add(group);
                        });
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                count--;

                if (count <= 0)
                    UpdateStatus(accountDetailsSelector);
            });
        }


        public void LoadAllAccountsFriends()
        {
            var valuePairs = SelectAccountDetailsModel.AccountFriendsPair.ToList();

            var alreadySelectedFriends = valuePairs.Select(x => x.Value).ToList();

            var accountDetailsSelector = new AccountDetailsSelector(UpdateAllFriendsDetails)
            {
                AccountDetailsSelectorViewModel =
                {
                    Title = "LangKeyFriendsSelect".FromResourceDictionary(),
                    DetailsUrlHeader = "LangKeyFriendUrl".FromResourceDictionary(),
                    DetailsNameHeader = "LangKeyFriendName".FromResourceDictionary(),
                    AlreadySelectedList = alreadySelectedFriends
                }
            };


            var dialog = new Dialog();

            var window = dialog.GetMetroWindow(accountDetailsSelector, "LangKeyFriendsSelect".FromResourceDictionary());

            accountDetailsSelector.btnSave.Click += (sender, events) =>
            {
                valuePairs.ForEach(x =>
                {
                    SelectAccountDetailsModel.AccountFriendsPair.Remove(x);
                    SelectAccountDetailsModel.DestinationDetailsModels.RemoveAll(y =>
                        x.Key == y.AccountId && y.DestinationType == ConstantVariable.Group);
                });

                var keyValuePairs = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItems().ToList();

                SelectAccountDetailsModel.AccountFriendsPair.AddRange(keyValuePairs);

                var destinationDetails = accountDetailsSelector.AccountDetailsSelectorViewModel
                    .GetSelectedItemsDestinations("Friends").ToList();

                SelectAccountDetailsModel.DestinationDetailsModels.AddRange(destinationDetails);

                keyValuePairs.ForEach(selectedItems =>
                {
                    alreadySelectedFriends = SelectAccountDetailsModel.AccountFriendsPair
                        .Where(x => x.Key == selectedItems.Key).Select(x => x.Value).ToList();

                    var createDestinationSelectModel =
                        SelectAccountDetailsModel.ListSelectDestination.FirstOrDefault(x =>
                            x.AccountId == selectedItems.Key);

                    if (createDestinationSelectModel != null)
                        createDestinationSelectModel.FriendsSelectorText =
                            $"{alreadySelectedFriends.Count}/{createDestinationSelectModel.TotalFriends}";
                });

                window.Close();
            };

            accountDetailsSelector.btnCancel.Click += (sender, events) => { window.Close(); };

            window.Show();

            accountDetailsSelector.UpdateUiAllData();
        }

        private void UpdateAllFriendsDetails(AccountDetailsSelector accountDetailsSelector)
        {
            var valuePairs = SelectAccountDetailsModel.AccountFriendsPair.ToList();

            var alreadySelectedGroups = valuePairs.Select(x => x.Value).ToList();

            var count = SelectAccountDetailsModel.ListSelectDestination.Count;

            SelectAccountDetailsModel.ListSelectDestination.ForEach(async x =>
            {
                if (GroupsAvailableInNetworks.Contains(x.SocialNetworks.ToString()))
                {
                    var accountsDetailsSelector = SocinatorInitialize
                        .GetSocialLibrary(x.SocialNetworks)
                        .GetNetworkCoreFactory().AccountDetailsSelectors;

                    var groups =
                        await accountsDetailsSelector.GetFriendsDetails(x.AccountId, x.AccountName,
                            alreadySelectedGroups);

                    groups.ForEach(friend =>
                    {
                        friend.CurrentIndex = accountDetailsSelector.AccountDetailsSelectorViewModel
                                                  .ListAccountDetailsSelectorModels.Count + 1;
                        friend.Network = x.SocialNetworks;
                        if (!Application.Current.Dispatcher.CheckAccess())
                            Application.Current.Dispatcher.Invoke(() =>
                                accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels
                                    .Add(friend));
                        else
                            accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(
                                friend);
                    });
                }

                count--;

                if (count <= 0)
                    UpdateStatus(accountDetailsSelector);
            });
        }


        private static void UpdateStatus(AccountDetailsSelector accountDetailsSelector)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    accountDetailsSelector.AccountDetailsSelectorViewModel.IsProgressRingActive = false;
                    accountDetailsSelector.AccountDetailsSelectorViewModel.StatusText =
                        accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Count >
                        0
                            ? $"{accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Count} {"LangKeyRowsFound".FromResourceDictionary()}"
                            : "LangKeyNoRowsFound".FromResourceDictionary();
                });
            }
            else
            {
                accountDetailsSelector.AccountDetailsSelectorViewModel.IsProgressRingActive = false;
                accountDetailsSelector.AccountDetailsSelectorViewModel.StatusText =
                    accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Count > 0
                        ? $"{accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Count} {"LangKeyRowsFound".FromResourceDictionary()}"
                        : "LangKeyNoRowsFound".FromResourceDictionary();
            }
        }

        #endregion

        #region Initialize Updates

        public void InitializeDestinationList()
        {
            var accounts = _accountsFileManager.GetAll().Where(x => x.AccountBaseModel.Status == AccountStatus.Success);

            if (SelectAccountDetailsModel.IsDisplaySingleAccount)
                accounts = accounts.Where
                    (x => x.AccountBaseModel.UserName == SelectAccountDetailsModel.DisplayAccount).ToList();

            if (!Application.Current.CheckAccess())
                Application.Current.Dispatcher.Invoke(
                    () => { SelectAccountDetailsModel.ListSelectDestination.Clear(); });
            else
                SelectAccountDetailsModel.ListSelectDestination.Clear();

            accounts.ForEach(x =>
            {
                var allAccountDetailsSelectModel = new PublisherCreateDestinationSelectModel
                {
                    AccountId = x.AccountBaseModel.AccountId,
                    AccountName = x.AccountBaseModel.UserName,
                    SocialNetworks = x.AccountBaseModel.AccountNetwork,
                    AccountGroupName = x.AccountBaseModel.AccountGroup.Content,
                    IsOwnWallAvailable =
                        !WallAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork
                            .ToString()), //x.AccountBaseModel.AccountNetwork != SocialNetworks.Pinterest,
                    IsGroupsAvailable =
                        GroupsAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                    IsPagesOrBoardsAvailable =
                        BoardsOrPagesAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                    IsFriendsAvailable =
                        FriendsAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                    IsScrapingAvailableInNetworks =
                        ScrapingAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                    IsAccountSelected = x.IsAccountSelected,
                    PublishonOwnWall = false,
                    SelectedGroups = 0,
                    TotalFriends = x.DisplayColumnValue1 ?? 0,
                    TotalGroups = x.DisplayColumnValue2 ?? 0,
                    TotalPagesOrBoards = x.DisplayColumnValue3 ?? 0
                };

                allAccountDetailsSelectModel.GroupSelectorText =
                    GroupsAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString())
                        ? "0" + "/" + allAccountDetailsSelectModel.TotalGroups
                        : "LangKeyNA".FromResourceDictionary();

                allAccountDetailsSelectModel.PagesOrBoardsSelectorText =
                    BoardsOrPagesAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString())
                        ? "0" + "/" + allAccountDetailsSelectModel.TotalPagesOrBoards
                        : "LangKeyNA".FromResourceDictionary();

                allAccountDetailsSelectModel.FriendsSelectorText =
                    FriendsAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString())
                        ? "0" + "/" + allAccountDetailsSelectModel.TotalFriends
                        : "LangKeyNA".FromResourceDictionary();

                if (x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook)
                    SelectAccountDetailsModel.ListSelectDestination.Add(allAccountDetailsSelectModel);
            });

            DestinationCollectionView =
                CollectionViewSource.GetDefaultView(SelectAccountDetailsModel.ListSelectDestination);
        }

        public void RemoveUnnecessaryDestinationList()
        {
            var accounts = _accountsFileManager.GetAll();


            accounts.ForEach(x =>
            {
                if (x.AccountBaseModel.UserName != SelectAccountDetailsModel.DisplayAccount)
                {
                    if (!Application.Current.CheckAccess())
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var account = SelectAccountDetailsModel.ListSelectDestination.FirstOrDefault(y =>
                                y.AccountName == SelectAccountDetailsModel.DisplayAccount);
                            SelectAccountDetailsModel.ListSelectDestination.Remove(account);
                        });
                    }
                    else
                    {
                        var account =
                            SelectAccountDetailsModel.ListSelectDestination.FirstOrDefault(y =>
                                y.AccountName == x.AccountBaseModel.UserName);
                        SelectAccountDetailsModel.ListSelectDestination.Remove(account);
                    }
                }
            });
        }

        #endregion

        #region Save Destinations

        private bool SaveDestinationCanExecute(object sender)
        {
            return true;
        }

        private void SaveDestinationExecute(object sender)
        {
            SelectAccountDetailsModel.SelectedAccountIds.Clear();
            SelectAccountDetailsModel.PublishOwnWallAccount.Clear();
            SelectAccountDetailsModel.AccountsWithNetwork.Clear();

            var selectedAccountsCount =
                SelectAccountDetailsModel.ListSelectDestination.Count(x => x.IsAccountSelected);

            if (selectedAccountsCount == 0)
            {
                Dialog.ShowDialog(Application.Current.MainWindow,
                    "LangKeyWarning".FromResourceDictionary(),
                    "LangKeyPleaseSelectAccountsSelectedOnlyDestinations".FromResourceDictionary());
                return;
            }

            SelectAccountDetailsModel.ListSelectDestination.ForEach(x =>
            {
                // Check the account has been selected or not
                if (x.IsAccountSelected)
                {
                    SelectAccountDetailsModel.SelectedAccountIds.Add(x.AccountId);
                    SelectAccountDetailsModel.AccountsWithNetwork.Add(
                        new KeyValuePair<SocialNetworks, string>(x.SocialNetworks, x.AccountId));
                }
                else
                {
                    // If account has selected, remove from selected lists
                    var unwantedGroups = SelectAccountDetailsModel.AccountGroupPair.Where(y => y.Key == x.AccountId)
                        .Select(y => y.Key);
                    SelectAccountDetailsModel.AccountGroupPair.RemoveAll(z => unwantedGroups.Contains(z.Key));

                    var unwantedPages = SelectAccountDetailsModel.AccountPagesBoardsPair
                        .Where(y => y.Key == x.AccountId).Select(y => y.Key);
                    SelectAccountDetailsModel.AccountPagesBoardsPair.RemoveAll(z => unwantedPages.Contains(z.Key));

                    SelectAccountDetailsModel.DestinationDetailsModels.RemoveAll(z =>
                        z.AccountId == x.AccountId);
                }
            });

            SelectAccountDetailsModel.AccountGroupPair =
                SelectAccountDetailsModel.AccountGroupPair.Distinct().ToList();


            SelectAccountDetailsModel.SelectedAccountIds =
                SelectAccountDetailsModel.SelectedAccountIds.Distinct().ToList();

            SelectAccountDetailsModel.AccountPagesBoardsPair =
                SelectAccountDetailsModel.AccountPagesBoardsPair.Distinct().ToList();

            SelectAccountDetailsModel.AccountFriendsPair =
                SelectAccountDetailsModel.AccountFriendsPair.Distinct().ToList();

            if (SelectAccountDetailsModel.AccountGroupPair.Count == 0 &&
                SelectAccountDetailsModel.AccountPagesBoardsPair.Count == 0 &&
                SelectAccountDetailsModel.AccountFriendsPair.Count == 0 &&
                SelectAccountDetailsModel.PublishOwnWallAccount.Count == 0
            )
            {
                Dialog.ShowDialog(Application.Current.MainWindow,
                    "LangKeyWarning".FromResourceDictionary(),
                    "LangKeyPleaseSelectDestination".FromResourceDictionary());
                return;
            }

            InitializeProperties();

            IsSavedDestination = true;
        }

        #endregion

        #region Clear Destination

        private bool ClearCanExecute(object sender)
        {
            return true;
        }

        private void ClearExecute(object sender)
        {
            ClearCurrentDestination();
        }

        public void ClearCurrentDestination()
        {
            try
            {
                InitializeProperties();
                InitializeDestinationList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

        #region Sync Destination

        private bool SyncCanExecute(object sender)
        {
            return true;
        }

        private void SyncExecute(object sender)
        {
            try
            {
                var selectedSyncAccount = sender as PublisherCreateDestinationSelectModel;
                new Action(async () => { await UpdateSyncStatusAsync(selectedSyncAccount); }).Invoke();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public List<PublisherCreateDestinationSelectModel> GetSyncUpdateDestinations()
        {
            return SelectAccountDetailsModel.ListSelectDestination
                .Where(x => x.StatusSyncContent == ConstantVariable.NeedUpdateStatusSync).ToList();
        }


        public async Task UpdateSyncStatusAsync(PublisherCreateDestinationSelectModel selectedSyncAccount)
        {
            var currentAccountDetails =
                SelectAccountDetailsModel.ListSelectDestination.FirstOrDefault(x =>
                    x.AccountId == selectedSyncAccount.AccountId);

            if (currentAccountDetails == null)
                return;

            var accountsDetailsSelector = SocinatorInitialize
                .GetSocialLibrary(selectedSyncAccount.SocialNetworks)
                .GetNetworkCoreFactory().AccountDetailsSelectors;

            var currentGroups =
                await accountsDetailsSelector.GetGroupsUrls(selectedSyncAccount.AccountId,
                    selectedSyncAccount.AccountName);

            var currentPages =
                await accountsDetailsSelector.GetPageOrBoardUrls(selectedSyncAccount.AccountId,
                    selectedSyncAccount.AccountName);

            SelectAccountDetailsModel.AccountGroupPair.RemoveAll(x =>
                x.Key == selectedSyncAccount.AccountId && !currentGroups.Contains(x.Value));

            SelectAccountDetailsModel.AccountPagesBoardsPair.RemoveAll(x =>
                x.Key == selectedSyncAccount.AccountId && !currentPages.Contains(x.Value));

            currentAccountDetails.TotalGroups = currentGroups.Count;

            currentAccountDetails.TotalPagesOrBoards = currentPages.Count;

            currentAccountDetails.SelectedGroups =
                SelectAccountDetailsModel.AccountGroupPair.Count(x => x.Key == selectedSyncAccount.AccountId);

            currentAccountDetails.SelectedPagesOrBoards =
                SelectAccountDetailsModel.AccountPagesBoardsPair.Count(x => x.Key == selectedSyncAccount.AccountId);

            currentAccountDetails.UpdatePagesOrBoardsText();

            currentAccountDetails.UpdateGroupText();

            currentAccountDetails.StatusSyncContent = ConstantVariable.FineStatusSync;
        }

        #endregion

        #region Add fresh accounts

        private bool AddFreshAccountCanExecute(object sender)
        {
            return true;
        }

        private void AddFreshAccountExecute(object sender)
        {
            try
            {
                var accounts = _accountsFileManager.GetAll()
                    .Where(x => x.AccountBaseModel.Status == AccountStatus.Success);

                if (SelectAccountDetailsModel.IsDisplaySingleAccount)
                    accounts = accounts.Where
                        (x => x.AccountBaseModel.UserName == SelectAccountDetailsModel.DisplayAccount).ToList();
                accounts.ForEach(x =>
                {
                    if (SelectAccountDetailsModel.ListSelectDestination.All(y => y.AccountId != x.AccountId))
                    {
                        var allAccountDetailsSelectModel = new PublisherCreateDestinationSelectModel
                        {
                            AccountId = x.AccountBaseModel.AccountId,
                            AccountName = x.AccountBaseModel.UserName,
                            AccountGroupName = x.AccountBaseModel.AccountGroup.Content,
                            SocialNetworks = x.AccountBaseModel.AccountNetwork,
                            IsOwnWallAvailable =
                                !WallAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork
                                    .ToString()), // x.AccountBaseModel.AccountNetwork != SocialNetworks.Pinterest,
                            IsGroupsAvailable =
                                GroupsAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                            IsScrapingAvailableInNetworks =
                                ScrapingAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                            IsPagesOrBoardsAvailable =
                                BoardsOrPagesAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                            PublishonOwnWall = false,
                            SelectedGroups = 0,
                            TotalGroups = x.DisplayColumnValue2 ?? 0,
                            TotalPagesOrBoards = x.DisplayColumnValue3 ?? 0
                        };
                        allAccountDetailsSelectModel.GroupSelectorText =
                            GroupsAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString())
                                ? "0" + "/" + allAccountDetailsSelectModel.TotalGroups
                                : "LangKeyNA".FromResourceDictionary();

                        allAccountDetailsSelectModel.PagesOrBoardsSelectorText =
                            BoardsOrPagesAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString())
                                ? "0" + "/" + allAccountDetailsSelectModel.TotalPagesOrBoards
                                : "LangKeyNA".FromResourceDictionary();
                        if (x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook)
                            SelectAccountDetailsModel.ListSelectDestination.Add(
                                allAccountDetailsSelectModel);
                    }
                });
                DestinationCollectionView =
                    CollectionViewSource.GetDefaultView(SelectAccountDetailsModel.ListSelectDestination);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion


        #region Add Custom Destination Command

        private bool AddCustomDestinationCanExecute(object sender)
        {
            return true;
        }

        private void AddCustomDestinationExecute(object sender)
        {
            var allAccountDetailsSelectModel = sender as PublisherCreateDestinationSelectModel;

            if (allAccountDetailsSelectModel == null)
                return;

            var valuePairs = SelectAccountDetailsModel.CustomDestinations
                .Where(x => x.Key == allAccountDetailsSelectModel.AccountId).ToList();

            var alreadySavedCustomDestination = new ObservableCollection<PublisherCustomDestinationModel>();

            valuePairs.ForEach(x =>
            {
                var publisherCustomDestinationModel = new PublisherCustomDestinationModel
                {
                    DestinationType = x.Value.DestinationType,
                    DestinationValue = x.Value.DestinationValue
                };
                alreadySavedCustomDestination.Add(publisherCustomDestinationModel);
            });

            var publisherAddCustomDestination =
                PublisherAddCustomDestination.GetPublisherAddCustomDestination(alreadySavedCustomDestination);

            publisherAddCustomDestination.PublisherCustomDestinationViewModel.InputDestination.DestinationType =
                FbEntityTypes.Friend.ToString();
            var dialog = new Dialog();
            var window = dialog.GetMetroWindow(publisherAddCustomDestination,
                "LangKeyAddCustomDestination".FromResourceDictionary());

            publisherAddCustomDestination.ButtonSave.Click += (senders, args) =>
            {
                var savedNewCustomDestination = publisherAddCustomDestination.GetSavedCustomDestination();
                var createDestinationSelectModel =
                    SelectAccountDetailsModel.ListSelectDestination.FirstOrDefault(x =>
                        x.AccountId == allAccountDetailsSelectModel.AccountId);

                SelectAccountDetailsModel.CustomDestinations.RemoveAll(x =>
                    x.Key == allAccountDetailsSelectModel.AccountId);

                SelectAccountDetailsModel.DestinationDetailsModels.RemoveAll(x =>
                    x.AccountId == allAccountDetailsSelectModel.AccountId && x.IsCustomDestintions);

                savedNewCustomDestination.ForEach(x =>
                {
                    SelectAccountDetailsModel.CustomDestinations.Add(
                        new KeyValuePair<string, PublisherCustomDestinationModel>(
                            allAccountDetailsSelectModel.AccountId, x));

                    SelectAccountDetailsModel.DestinationDetailsModels.Add(new PublisherDestinationDetailsModel
                    {
                        AccountId = allAccountDetailsSelectModel.AccountId,
                        DestinationType = x.DestinationType,
                        DestinationUrl = x.DestinationValue,
                        SocialNetworks = allAccountDetailsSelectModel.SocialNetworks,
                        AccountGroupName = allAccountDetailsSelectModel.AccountGroupName,
                        PublisherPostlistModel = new PublisherPostlistModel(),
                        IsCustomDestintions = true,
                        DestinationGuid = Utilities.GetGuid(),
                        AccountName = allAccountDetailsSelectModel.AccountName
                    });
                });

                publisherAddCustomDestination.ResetCurrectObject();

                if (createDestinationSelectModel != null)
                    createDestinationSelectModel.CustomDestinationSelectorText =
                        $"{SelectAccountDetailsModel.CustomDestinations.Where(x => x.Key == allAccountDetailsSelectModel.AccountId).ToList().Count}";
                window.Close();
            };
            publisherAddCustomDestination.ButtonCancel.Click += (senders, args) => { window.Close(); };


            window.Show();
        }

        #endregion
    }
}
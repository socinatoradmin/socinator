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
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.Views.SocioPublisher;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherCreateDestinationsViewModel : BindableBase
    {
        //ConstructorS
        private readonly IAccountsFileManager _accountsFileManager;

        public PublisherCreateDestinationsViewModel()
        {
            _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            NavigationCommand = new BaseCommand<object>(NavigationCanExecute, NavigationExecute);
            GetSingleAccountGroupsCommand =
                new BaseCommand<object>(GetSingleAccountGroupsCanExecute, GetSingleAccountGroupsExecute);
            GetSingleAccountPagesOrBoardsCommand = new BaseCommand<object>(GetSingleAccountPagesOrBoardsCanExecute,
                GetSingleAccountPagesOrBoardsExecute);
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
            NetworkSelectionChangedCommand = new BaseCommand<object>(sender => true, NetworkSelectionChangedExecute);
            AccountSelectionCommand = new BaseCommand<object>(sender => true, AccountSelectionChangedExecute);
            InitializeProperties();
            InitializeDestinationList();
            IsSavedDestination = false;
        }

        public bool IsNeedToNavigate { get; set; }

        private void AccountSelectionChangedExecute(object sender)
        {
            try
            {
                var model = (PublisherCreateDestinationSelectModel)sender;
                if (model.IsAccountSelected)
                    model.IsScrapeFromAccount = model.IsAccountSelected;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void NetworkSelectionChangedExecute(object sender)
        {
            try
            {
                if (sender != null)
                {
                    SelectedNetworks = (SocialNetworks)sender;
                    FilterByNetwork();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void FilterByNetwork()
        {
            try
            {
                if (SelectedNetworks == SocialNetworks.Social)
                {
                    if (!string.IsNullOrEmpty(FilterText))
                        if (_filterByUserName)
                            DestinationCollectionView.Filter = x =>
                                ((PublisherCreateDestinationSelectModel)x).AccountName.IndexOf(FilterText,
                                    StringComparison.CurrentCultureIgnoreCase) >= 0;
                        else
                            DestinationCollectionView.Filter = x =>
                                ((PublisherCreateDestinationSelectModel)x).AccountGroupName.IndexOf(FilterText,
                                    StringComparison.CurrentCultureIgnoreCase) >= 0;
                    else DestinationCollectionView.Filter = x => true;
                }
                else
                {
                    if (!string.IsNullOrEmpty(FilterText))
                        if (_filterByUserName)
                            DestinationCollectionView.Filter =
                                x => ((PublisherCreateDestinationSelectModel)x).SocialNetworks == SelectedNetworks &&
                                     ((PublisherCreateDestinationSelectModel)x).AccountName.IndexOf(_filterText,
                                         StringComparison.CurrentCultureIgnoreCase) >= 0;
                        else
                            DestinationCollectionView.Filter =
                                x => ((PublisherCreateDestinationSelectModel)x).SocialNetworks == SelectedNetworks &&
                                     ((PublisherCreateDestinationSelectModel)x).AccountGroupName.IndexOf(_filterText,
                                         StringComparison.CurrentCultureIgnoreCase) >= 0;

                    else
                        DestinationCollectionView.Filter = x =>
                            ((PublisherCreateDestinationSelectModel)x).SocialNetworks == SelectedNetworks;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void InitializeProperties()
        {
            Title = "LangKeyCreateDestination".FromResourceDictionary();
            IsAllDestinationSelected = false;
            EditDestinationId = string.Empty;
            IsSavedDestination = false;
            PublisherCreateDestinationModel = PublisherCreateDestinationModel.DestinationDefaultBuilder();
        }

        #region Initialize Updates

        public void InitializeDestinationList()
        {
            var accountList = InstanceProvider.GetInstance<IAccountCollectionViewModel>().GetCopySync();
            var accounts = accountList.Where(x =>
                x.AccountBaseModel.Status == AccountStatus.Success &&
                x.AccountBaseModel.AccountNetwork != SocialNetworks.TikTok);

            if (!Application.Current.CheckAccess())
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PublisherCreateDestinationModel.ListSelectDestination.Clear();
                });
            else
                PublisherCreateDestinationModel.ListSelectDestination.Clear();

            accounts.ForEach(x =>
            {
                var pageCount = x.AccountBaseModel.AccountNetwork == SocialNetworks.LinkedIn
                    ? x.DisplayColumnValue4 ?? 0
                    : x.AccountBaseModel.AccountNetwork == SocialNetworks.YouTube
                    ? x.DisplayColumnValue1 ?? 0
                    : x.DisplayColumnValue3 ?? 0;


                var publisherCreateDestinationSelectModel = new PublisherCreateDestinationSelectModel
                {
                    AccountId = x.AccountBaseModel.AccountId,
                    AccountName = x.AccountBaseModel.UserName,
                    SocialNetworks = x.AccountBaseModel.AccountNetwork,
                    AccountGroupName = x.AccountBaseModel.AccountGroup.Content,
                    IsOwnWallAvailable =
                        !WallAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                    IsScrapingAvailableInNetworks =
                        ScrapingAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                    IsCustomDestinationInNetworks =
                        IsCustomDestinationInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                    IsGroupsAvailable =
                        GroupsAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                    IsPagesOrBoardsAvailable =
                        BoardsOrPagesAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                    PublishonOwnWall = false,
                    SelectedGroups = 0,
                    TotalGroups = x.DisplayColumnValue2 ?? 0,
                    TotalPagesOrBoards = pageCount
                };

                publisherCreateDestinationSelectModel.GroupSelectorText =
                    GroupsAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString())
                        ? "0" + "/" + publisherCreateDestinationSelectModel.TotalGroups
                        : "LangKeyNA".FromResourceDictionary();

                publisherCreateDestinationSelectModel.PagesOrBoardsSelectorText =
                    BoardsOrPagesAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString())
                        ? "0" + "/" + publisherCreateDestinationSelectModel.TotalPagesOrBoards
                        : "LangKeyNA".FromResourceDictionary();

                if (SocinatorInitialize.IsNetworkAvailable(x.AccountBaseModel.AccountNetwork))
                    PublisherCreateDestinationModel.ListSelectDestination.Add(publisherCreateDestinationSelectModel);
            });

            DestinationCollectionView =
                CollectionViewSource.GetDefaultView(PublisherCreateDestinationModel.ListSelectDestination);
        }

        #endregion

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
                if (x.DestinationName == PublisherCreateDestinationModel.DestinationName)
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

            InitializeDestinationList();

            var saveDestination = PublisherCreateDestinationModel.GetDestination(EditDestinationId);

            var currentlyAvailableAccounts =
                PublisherCreateDestinationModel.ListSelectDestination.Select(x => x.AccountId).ToList();

            foreach (var savedDestination in saveDestination.ListSelectDestination)
            {
                if (!currentlyAvailableAccounts.Contains(savedDestination.AccountId))
                {
                    savedDestination.StatusSyncContent = ConstantVariable.NotAvailableAccountSync;
                    continue;
                }

                var currentAccountDetails =
                    PublisherCreateDestinationModel.ListSelectDestination.FirstOrDefault(x =>
                        x.AccountId == savedDestination.AccountId);

                if (currentAccountDetails == null)
                    return;

                if (savedDestination.TotalGroups != currentAccountDetails.TotalGroups ||
                    savedDestination.TotalPagesOrBoards != currentAccountDetails.TotalPagesOrBoards)
                    savedDestination.StatusSyncContent = ConstantVariable.NeedUpdateStatusSync;
            }

            PublisherCreateDestinationModel = saveDestination;

            PublisherCreateDestinationModel.UpdateDestination(saveDestination);

            DestinationCollectionView =
                CollectionViewSource.GetDefaultView(PublisherCreateDestinationModel.ListSelectDestination);
        }

        #endregion

        #region Commands

        public ICommand AddFreshAccounts { get; set; }

        public ICommand StatusSyncCommand { get; set; }

        public ICommand ClearCommand { get; set; }

        public ICommand NavigationCommand { get; set; }

        public ICommand GetSingleAccountGroupsCommand { get; set; }

        public ICommand SelectAllAccountDetailsCommand { get; set; }

        public ICommand OpenContextMenuCommand { get; set; }

        public ICommand SelectionCommand { get; set; }

        public ICommand GetSingleAccountPagesOrBoardsCommand { get; set; }

        public ICommand SaveDestinationCommand { get; set; }

        public ICommand AddCustomDestinationCommand { get; set; }
        public ICommand NetworkSelectionChangedCommand { get; set; }

        public ICommand AccountSelectionCommand { get; set; }

        #endregion

        #region Properties

        private SocialNetworks _selectedNetworks = SocialNetworks.Social;

        public static AccountDetailsSelector accountDetailsSelector = new AccountDetailsSelector();
        public SocialNetworks SelectedNetworks
        {
            get => _selectedNetworks;
            set
            {
                if (_selectedNetworks == value)
                    return;
                SetProperty(ref _selectedNetworks, value);
            }
        }

        private string _filterText;

        public string FilterText
        {
            get => _filterText;
            set
            {
                if (_filterText == value)
                    return;
                SetProperty(ref _filterText, value);
                FilterByNetwork();
                ChangeSelectionAfterFilter();
            }
        }

        public void ChangeSelectionAfterFilter()
        {
            CompareModelAndSelectionList();
            var list = DestinationCollectionView.Cast<PublisherCreateDestinationSelectModel>();
            if (list.Count().Equals(0) || !list.All(x => x.IsAccountSelected))
            {
                if (IsAllDestinationSelected)
                {
                    _isUncheckedFromList = true;
                    IsAllDestinationSelected = false;
                }
            }
            else
            {
                if (!IsAllDestinationSelected)
                {
                    _isUncheckedFromList = true;
                    IsAllDestinationSelected = true;
                }
            }
        }

        private bool _filterByGroupName;

        public bool FilterByGroupName
        {
            get => _filterByGroupName;
            set
            {
                SetProperty(ref _filterByGroupName, value);
                if (!string.IsNullOrWhiteSpace(FilterText))
                    FilterByNetwork();
            }
        }

        private bool _filterByUserName = true;

        public bool FilterByUserName
        {
            get => _filterByUserName;
            set
            {
                SetProperty(ref _filterByUserName, value);
                if (!string.IsNullOrWhiteSpace(FilterText))
                    FilterByNetwork();
            }
        }

        private HashSet<SocialNetworks> _availableNetworks = SocinatorInitialize.AvailableNetworks;

        public HashSet<SocialNetworks> AvailableNetworks
        {
            get
            {
                if (_availableNetworks.Contains(SocialNetworks.TikTok))
                    _availableNetworks.Remove(SocialNetworks.TikTok);
                return _availableNetworks;
            }
            set
            {
                _availableNetworks = value;
                OnPropertyChanged(nameof(AvailableNetworks));
            }
        }

        private PublisherCreateDestinationModel _publisherCreateDestinationModel =
            PublisherCreateDestinationModel.DestinationDefaultBuilder();

        public PublisherCreateDestinationModel PublisherCreateDestinationModel
        {
            get => _publisherCreateDestinationModel;
            set
            {
                if (_publisherCreateDestinationModel == value)
                    return;
                _publisherCreateDestinationModel = value;
                OnPropertyChanged(nameof(PublisherCreateDestinationModel));
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

        //   public bool IsSavedDestination { get; set; }
        private bool _isSavedDestination;

        public bool IsSavedDestination
        {
            get => _isSavedDestination;
            set => SetProperty(ref _isSavedDestination, value);
        }

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


        private bool _isAllWallsSelected;

        public bool IsAllWallsSelected
        {
            get => _isAllWallsSelected;
            set
            {
                if (_isAllWallsSelected == value)
                    return;
                SetProperty(ref _isAllWallsSelected, value);
                SelectAllWalls(_isAllWallsSelected);
                //_isUncheckedFromList = false;
            }
        }

        private bool _isAllScrapeFromAccount;

        public bool IsAllScrapeFromAccount
        {
            get => _isAllScrapeFromAccount;
            set
            {
                if (_isAllScrapeFromAccount == value)
                    return;
                SetProperty(ref _isAllScrapeFromAccount, value);
                CheckAllScrapeFromAccount(_isAllScrapeFromAccount);
            }
        }


        private List<string> _needToUpdateAccounts = new List<string>();


        public List<string> GroupsAvailableInNetworks { get; set; } =
            new List<string> { "Facebook", "LinkedIn", "Reddit" };

        public List<string> WallAvailableInNetworks { get; set; } = new List<string> { "Pinterest", "Tumblr" };

        public List<string> ScrapingAvailableInNetworks { get; set; } =
            new List<string> { "Facebook", "Pinterest", "Twitter", "Reddit","Quora", "LinkedIn" };

        public List<string> IsCustomDestinationInNetworks { get; set; } = new List<string> { "Facebook", "Reddit" };

        public List<string> BoardsOrPagesAvailableInNetworks { get; set; } = new List<string>
            {"Facebook", "Pinterest", "YouTube", "Tumblr", "LinkedIn"};

        private string _selectPageBoard = "LangKeySelectPagesBoardsAll".FromResourceDictionary();

        public string SelectPageBoard
        {
            get => _selectPageBoard;
            set
            {
                _selectPageBoard = value;
                OnPropertyChanged(nameof(SelectPageBoard));
            }
        }

        private string _selectGroups = "LangKeySelectGroupsAll".FromResourceDictionary();

        public string SelectGroups
        {
            get => _selectGroups;
            set
            {
                _selectGroups = value;
                OnPropertyChanged(nameof(SelectGroups));
            }
        }


        private string _selectWall = "LangKeySelectWallsProfilesAll".FromResourceDictionary();

        public string SelectWall
        {
            get => _selectWall;
            set
            {
                _selectWall = value;
                OnPropertyChanged(nameof(SelectWall));
            }
        }

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
            var publisherCreateDestinationSelectModel = (PublisherCreateDestinationSelectModel)sender;

            // get already selected group pairs
            var valuePairs = PublisherCreateDestinationModel.AccountGroupPair
                .Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).ToList();

            var alreadySelectedGroups = valuePairs.Select(x => x.Value).ToList();

            // Get the initial selector details and also passing the action for getting the group details
            var accountDetailsSelector =
                new AccountDetailsSelector(UpdateSingleAccountGroupsDetails, publisherCreateDestinationSelectModel)
                {
                    AccountDetailsSelectorViewModel =
                    {
                        Title = "LangKeySelectGroups".FromResourceDictionary(),
                        DetailsUrlHeader = "LangKeyGroupUrl".FromResourceDictionary(),
                        DetailsNameHeader = "LangKeyGroupName".FromResourceDictionary(),
                        AlreadySelectedList = alreadySelectedGroups
                    }
                };

            var dialog = new Dialog();

            // display the dialog window
            var window = dialog.GetMetroWindow(accountDetailsSelector, "LangKeySelectGroups".FromResourceDictionary());

            accountDetailsSelector.btnSave.Click += (senderDetails, events) =>
            {
                // Remove already saved group pairs 
                valuePairs.ForEach(x =>
                {
                    PublisherCreateDestinationModel.AccountGroupPair.Remove(x);
                    PublisherCreateDestinationModel.DestinationDetailsModels.RemoveAll(y =>
                        x.Key == y.AccountId && y.DestinationType == ConstantVariable.Group);
                });

                // get currectly selected groups from UI objects
                var keyValuePairs = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItems().ToList();

                // get the full destination details
                var destinationDetails = accountDetailsSelector.AccountDetailsSelectorViewModel
                    .GetSelectedItemsDestinations(ConstantVariable.Group).ToList();

                // Append with destination details of the accounts
                PublisherCreateDestinationModel.DestinationDetailsModels.AddRange(destinationDetails);

                // Add to account's group pair
                PublisherCreateDestinationModel.AccountGroupPair.AddRange(keyValuePairs);

                alreadySelectedGroups = PublisherCreateDestinationModel.AccountGroupPair
                    .Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).Select(x => x.Value).ToList();

                var createDestinationSelectModel =
                    PublisherCreateDestinationModel.ListSelectDestination.FirstOrDefault(x =>
                        x.AccountId == publisherCreateDestinationSelectModel.AccountId);

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
            PublisherCreateDestinationSelectModel publisherCreateDestinationSelectModel)
        {
            // Get the account group pair
            // var valuePairs = PublisherCreateDestinationModel.AccountGroupPair.Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).ToList(); ;

            // Get already selected groups
            // var alreadySelectedGroups = valuePairs.Select(x => x.Value).ToList();

            var alreadySelectedGroups = accountDetailsSelector.AccountDetailsSelectorViewModel.AlreadySelectedList;

            if (GroupsAvailableInNetworks.Contains(publisherCreateDestinationSelectModel.SocialNetworks.ToString()))
            {
                // Get the factory for account selector for a network
                var accountsDetailsSelector = SocinatorInitialize
                    .GetSocialLibrary(publisherCreateDestinationSelectModel.SocialNetworks)
                    .GetNetworkCoreFactory().AccountDetailsSelectors;

                // fetch the groups details for particular accounts
                var groups = await accountsDetailsSelector.GetGroupsDetails(
                    publisherCreateDestinationSelectModel.AccountId, publisherCreateDestinationSelectModel.AccountName,
                    alreadySelectedGroups);

                groups.ForEach(group =>
                {
                    group.CurrentIndex = accountDetailsSelector.AccountDetailsSelectorViewModel
                                             .ListAccountDetailsSelectorModels.Count + 1;
                    group.Network = publisherCreateDestinationSelectModel.SocialNetworks;
                    group.IsSelected = alreadySelectedGroups.Contains(group.DetailUrl);
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
            var publisherCreateDestinationSelectModel = (PublisherCreateDestinationSelectModel)sender;

            // get the page or board pair from collection with the account Id
            var valuePairs = PublisherCreateDestinationModel.AccountPagesBoardsPair
                .Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).ToList();

            // get the factory pattern for the network of an account
            var accountsDetailsSelector = SocinatorInitialize
                .GetSocialLibrary(publisherCreateDestinationSelectModel.SocialNetworks)
                .GetNetworkCoreFactory().AccountDetailsSelectors;

            // Fetch the page details only
            var alreadySelectedPages = valuePairs.Select(x => x.Value).ToList();

            // Pass the fetching activity functions as action to UI
            var IsValidNetworkForSection = publisherCreateDestinationSelectModel.SocialNetworks.ToString().Contains("Pinterest");
            accountDetailsSelector =
                new AccountDetailsSelector(UpdateSingleAccountPagesDetails, publisherCreateDestinationSelectModel)
                {
                    // Find whether page or board, its vary based on each network
                    AccountDetailsSelectorViewModel =
                    {
                        Title =
                            $"{"LangKeySelect".FromResourceDictionary()} {accountsDetailsSelector.DisplayAsPageOrBoards}",
                        DetailsUrlHeader =
                            $"{accountsDetailsSelector.DisplayAsPageOrBoards} {"LangKeyUrl".FromResourceDictionary()}",
                        DetailsNameHeader =
                            $"{accountsDetailsSelector.DisplayAsPageOrBoards} {"LangKeyName".FromResourceDictionary()}",
                        DetailsUrlSection=IsValidNetworkForSection ?$"{accountsDetailsSelector.DisplayAsPageOrBoards} {"LangKeySection".FromResourceDictionary()}":string.Empty,
                        AlreadySelectedList = alreadySelectedPages
                    }
                };

            var dialog = new Dialog();

            var window = dialog.GetMetroWindow(accountDetailsSelector,
                $"{"LangKeySelect".FromResourceDictionary()} {accountsDetailsSelector.DisplayAsPageOrBoards}");

            accountDetailsSelector.SectionHeader.Width = IsValidNetworkForSection ? 100 : 0;
            // Defining the save buttons click events
            accountDetailsSelector.btnSave.Click += (senderDetails, events) =>
            {
                // Remove all the saved accounts page or boards pair
                valuePairs.ForEach(x =>
                {
                    PublisherCreateDestinationModel.AccountPagesBoardsPair.Remove(x);
                    PublisherCreateDestinationModel.DestinationDetailsModels.RemoveAll(y =>
                        x.Key == y.AccountId && y.DestinationType == ConstantVariable.PageOrBoard);
                });

                // Get the selected pairs
                var keyValuePairs = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItems().ToList();

                // Add to key value pair
                PublisherCreateDestinationModel.AccountPagesBoardsPair.AddRange(keyValuePairs);

                // Get the destination full details of a page or board
                var destinationDetails = accountDetailsSelector.AccountDetailsSelectorViewModel
                    .GetSelectedItemsDestinations(ConstantVariable.PageOrBoard).ToList();

                // Update with destination details
                PublisherCreateDestinationModel.DestinationDetailsModels.AddRange(destinationDetails);

                // Get the already selected page details 
                alreadySelectedPages = PublisherCreateDestinationModel.AccountPagesBoardsPair
                    .Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).Select(x => x.Value).ToList();

                //
                var createDestinationSelectModel =
                    PublisherCreateDestinationModel.ListSelectDestination.FirstOrDefault(x =>
                        x.AccountId == publisherCreateDestinationSelectModel.AccountId);

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
            PublisherCreateDestinationSelectModel publisherCreateDestinationSelectModel)
        {
            //var valuePairs = PublisherCreateDestinationModel.AccountGroupPair.Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).ToList(); ;

            //var alreadySelectedPages = valuePairs.Select(x => x.Value).ToList();

            var alreadySelectedPages = accountDetailsSelector.AccountDetailsSelectorViewModel.AlreadySelectedList;
            if (BoardsOrPagesAvailableInNetworks.Contains(
                publisherCreateDestinationSelectModel.SocialNetworks.ToString()))
            {
                var accountsDetailsSelector = SocinatorInitialize
                    .GetSocialLibrary(publisherCreateDestinationSelectModel.SocialNetworks)
                    .GetNetworkCoreFactory().AccountDetailsSelectors;

                var pagesOrBoards = await accountsDetailsSelector.GetPagesDetails(
                    publisherCreateDestinationSelectModel.AccountId, publisherCreateDestinationSelectModel.AccountName,
                    alreadySelectedPages);

                pagesOrBoards.ForEach(page =>
                {
                    page.CurrentIndex = accountDetailsSelector.AccountDetailsSelectorViewModel
                                            .ListAccountDetailsSelectorModels.Count + 1;
                    page.Network = publisherCreateDestinationSelectModel.SocialNetworks;
                    page.IsSelected = alreadySelectedPages.Contains(page.DetailUrl);
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

        #region Open Context

        private bool OpenContextMenuCanExecute(object sender)
        {
            return true;
        }

        private void OpenContextMenuExecute(object sender)
        {
            try
            {
                var contextMenu = ((Button)sender).ContextMenu;
                if (contextMenu == null) return;
                contextMenu.DataContext = ((Button)sender).DataContext;
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
            var moduleName = string.Empty;
            var model = new PublisherCreateDestinationSelectModel();

            if (sender.GetType() == typeof(PublisherCreateDestinationSelectModel))
                model = (PublisherCreateDestinationSelectModel)sender;
            else
                moduleName = sender.ToString();


            switch (moduleName)
            {
                case "MenuSelectNone":
                    IsAllDestinationSelected = false;
                    SelectAllDestination(false);
                    break;

                case "MenuSelectAll":
                    IsAllDestinationSelected = true;
                    break;
            }

            if (string.IsNullOrEmpty(moduleName))
            {
                if (DestinationCollectionView.Cast<PublisherCreateDestinationSelectModel>()
                    .All(x => x.IsAccountSelected))
                {
                    IsAllDestinationSelected = true;
                }
                else
                {
                    if (IsAllDestinationSelected)
                    {
                        _isUncheckedFromList = true;
                        IsAllDestinationSelected = false;
                    }
                }

                model.IsScrapeFromAccount = model.IsAccountSelected;
            }
        }

        private bool _isUncheckedFromList { get; set; }

        #endregion

        #region Select Destination fucntionality , and also selection menu options

        public void SelectAllDestination(bool isChecked)
        {
            if (_isUncheckedFromList)
                return;

            var list = DestinationCollectionView.Cast<PublisherCreateDestinationSelectModel>();
            var selectFromlist = PublisherCreateDestinationModel.ListSelectDestination.Count == list.Count()
                ? PublisherCreateDestinationModel.ListSelectDestination.ToList()
                : PublisherCreateDestinationModel.ListSelectDestination.Intersect(list).ToList();
            selectFromlist.Select(x =>
            {
                x.IsAccountSelected = isChecked;
                x.IsScrapeFromAccount = isChecked;
                return x;
            }).ToList();
        }

        private void CompareModelAndSelectionList()
        {
            var collectionList = DestinationCollectionView.Cast<PublisherCreateDestinationSelectModel>();
            if (collectionList.Count().Equals(0))
                return;
            var list = PublisherCreateDestinationModel.ListSelectDestination.Except(collectionList);
            list.Where(x => x.IsAccountSelected).ForEach(x => x.IsAccountSelected = false);
        }

        public void SelectAllWalls(bool isChecked)
        {
            //    if (_isUncheckedFromList)
            //        return;
            PublisherCreateDestinationModel.ListSelectDestination.Where(y => y.IsAccountSelected).Select(x =>
            {
                x.PublishonOwnWall = isChecked;
                return x;
            }).ToList();
        }

        public void CheckAllScrapeFromAccount(bool isChecked)
        {
            PublisherCreateDestinationModel.ListSelectDestination.Where(y => y.IsAccountSelected).Select(x =>
            {
                x.IsScrapeFromAccount = isChecked;
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
                    var IsAllselect = SelectWall == "LangKeySelectWallsProfilesAll".FromResourceDictionary();
                    PublisherCreateDestinationModel.ListSelectDestination.Select(x =>
                    {
                        x.PublishonOwnWall = IsAllselect;
                        // x.PublishonOwnWall = true;
                        return x;
                    }).ToList();
                    if (IsAllselect)
                        SelectWall = "LangKeyDeSelectWallsProfilesAll".FromResourceDictionary();
                    else
                        SelectWall = "LangKeySelectWallsProfilesAll".FromResourceDictionary();
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

        #region Get all account's pages and groups details

        public void LoadAllAccountsGroup()
        {
            var valuePairs = PublisherCreateDestinationModel.AccountGroupPair.ToList();

            var alreadySelectedGroups = valuePairs.Select(x => x.Value).ToList();

            var accountDetailsSelector = new AccountDetailsSelector(UpdateAllGroupsDetails)
            {
                AccountDetailsSelectorViewModel =
                {
                    Title = "LangKeySelectGroups".FromResourceDictionary(),
                    DetailsUrlHeader = "LangKeyGroupUrl".FromResourceDictionary(),
                    DetailsNameHeader = "LangKeyGroupName".FromResourceDictionary(),
                    AlreadySelectedList = alreadySelectedGroups
                }
            };
            UpdateAllGroupsDetails(accountDetailsSelector);
            if (SelectGroups == "LangKeySelectGroupsAll".FromResourceDictionary())
            {
                accountDetailsSelector.AccountDetailsSelectorViewModel.SelectAllCampaign();
                SelectGroups = "LangKeyDeSelectGroupsAll".FromResourceDictionary();
            }
            else
            {
                accountDetailsSelector.AccountDetailsSelectorViewModel.SelectNoneCampaign();
                SelectGroups = "LangKeySelectGroupsAll".FromResourceDictionary();
            }
            //var dialog = new Dialog();

            //var window = dialog.GetMetroWindow(accountDetailsSelector, "Select Groups");

            //accountDetailsSelector.btnSave.Click += (sender, events) =>
            //{
            valuePairs.ForEach(x =>
            {
                PublisherCreateDestinationModel.AccountGroupPair.Remove(x);
                PublisherCreateDestinationModel.DestinationDetailsModels.RemoveAll(y =>
                    x.Key == y.AccountId && y.DestinationType == ConstantVariable.Group);
            });

            var keyValuePairs = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItems().ToList();

            PublisherCreateDestinationModel.AccountGroupPair.AddRange(keyValuePairs);

            var destinationDetails = accountDetailsSelector.AccountDetailsSelectorViewModel
                .GetSelectedItemsDestinations(ConstantVariable.Group).ToList();

            PublisherCreateDestinationModel.DestinationDetailsModels.AddRange(destinationDetails);

            keyValuePairs.ForEach(selectedItems =>
            {
                alreadySelectedGroups = PublisherCreateDestinationModel.AccountGroupPair
                    .Where(x => x.Key == selectedItems.Key).Select(x => x.Value).ToList();

                var createDestinationSelectModel =
                    PublisherCreateDestinationModel.ListSelectDestination.FirstOrDefault(x =>
                        x.AccountId == selectedItems.Key);

                if (createDestinationSelectModel != null)
                    createDestinationSelectModel.GroupSelectorText =
                        $"{alreadySelectedGroups.Count}/{createDestinationSelectModel.TotalGroups}";
            });
            if (keyValuePairs.Count == 0)
                PublisherCreateDestinationModel.ListSelectDestination.Select(x =>
                {
                    x.GroupSelectorText = $"0/{x.TotalPagesOrBoards}";
                    return x;
                }).ToList();
            //    window.Close();
            //};

            //accountDetailsSelector.btnCancel.Click += (sender, events) => { window.Close(); };

            //window.Show();

            //accountDetailsSelector.UpdateUiAllData();
        }

        private void UpdateAllGroupsDetails(AccountDetailsSelector accountDetailsSelector)
        {
            var valuePairs = PublisherCreateDestinationModel.AccountGroupPair.ToList();

            var alreadySelectedGroups = valuePairs.Select(x => x.Value).ToList();

            var count = PublisherCreateDestinationModel.ListSelectDestination.Count;

            PublisherCreateDestinationModel.ListSelectDestination.ForEach(async x =>
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
                        group.IsSelected = alreadySelectedGroups.Contains(group.DetailUrl);
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
            var valuePairs = PublisherCreateDestinationModel.AccountPagesBoardsPair.ToList();

            var alreadySelectedPages = valuePairs.Select(x => x.Value).ToList();

            var accountDetailsSelector = new AccountDetailsSelector(UpdatePagesDetails)
            {
                AccountDetailsSelectorViewModel =
                {
                    Title = "LangKeySelectPagesBoards".FromResourceDictionary(),
                    DetailsUrlHeader = "LangKeyPagesBoardsUrl".FromResourceDictionary(),
                    DetailsNameHeader = "LangKeyPagesBoardsName".FromResourceDictionary(),
                    AlreadySelectedList = alreadySelectedPages
                }
            };
            UpdatePagesDetails(accountDetailsSelector);
            if (SelectPageBoard == "LangKeySelectPagesBoardsAll".FromResourceDictionary())
            {
                accountDetailsSelector.AccountDetailsSelectorViewModel.SelectAllCampaign();
                SelectPageBoard = "LangKeyDeSelectPagesBoardsAll".FromResourceDictionary();
            }
            else
            {
                accountDetailsSelector.AccountDetailsSelectorViewModel.SelectNoneCampaign();
                SelectPageBoard = "LangKeySelectPagesBoardsAll".FromResourceDictionary();
            }

            //var dialog = new Dialog();

            //var window = dialog.GetMetroWindow(accountDetailsSelector, "Select Pages/Boards");

            //accountDetailsSelector.btnSave.Click += (sender, events) =>
            //{
            valuePairs.ForEach(x =>
            {
                PublisherCreateDestinationModel.AccountPagesBoardsPair.Remove(x);
                PublisherCreateDestinationModel.DestinationDetailsModels.RemoveAll(y =>
                    x.Key == y.AccountId && y.DestinationType == ConstantVariable.PageOrBoard);
            });

            var keyValuePairs = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItems().ToList();

            PublisherCreateDestinationModel.AccountPagesBoardsPair.AddRange(keyValuePairs);

            var destinationDetails = accountDetailsSelector.AccountDetailsSelectorViewModel
                .GetSelectedItemsDestinations(ConstantVariable.PageOrBoard).ToList();

            PublisherCreateDestinationModel.DestinationDetailsModels.AddRange(destinationDetails);

            keyValuePairs.ForEach(selectedItems =>
            {
                alreadySelectedPages = PublisherCreateDestinationModel.AccountPagesBoardsPair
                    .Where(x => x.Key == selectedItems.Key).Select(x => x.Value).ToList();

                var createDestinationSelectModel =
                    PublisherCreateDestinationModel.ListSelectDestination.FirstOrDefault(x =>
                        x.AccountId == selectedItems.Key);

                createDestinationSelectModel.PagesOrBoardsSelectorText =
                    $"{alreadySelectedPages.Count}/{createDestinationSelectModel.TotalPagesOrBoards}";
            });
            if (keyValuePairs.Count == 0)
                PublisherCreateDestinationModel.ListSelectDestination.Select(x =>
                {
                    x.PagesOrBoardsSelectorText = $"0/{x.TotalPagesOrBoards}";
                    return x;
                }).ToList();
            //    window.Close();
            //};

            //accountDetailsSelector.btnCancel.Click += (sender, events) => { window.Close(); };

            //window.Show();

            //accountDetailsSelector.UpdateUiAllData();
        }

        private void UpdatePagesDetails(AccountDetailsSelector accountDetailsSelector)
        {
            var valuePairs = PublisherCreateDestinationModel.AccountPagesBoardsPair.ToList();

            var alreadySelectedPages = valuePairs.Select(x => x.Value).ToList();

            var count = PublisherCreateDestinationModel.ListSelectDestination.Count;

            PublisherCreateDestinationModel.ListSelectDestination.ForEach(async x =>
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
                            group.IsSelected = alreadySelectedPages.Contains(group.DetailUrl);
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


        public static void UpdateStatus(AccountDetailsSelector accountDetailsSelector)
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

        #region Save Destinations

        private bool SaveDestinationCanExecute(object sender)
        {
            return true;
        }

        private void SaveDestinationExecute(object sender)
        {
            var selectedAccountsCount =
                PublisherCreateDestinationModel.ListSelectDestination.Count(x => x.IsAccountSelected);

            if (selectedAccountsCount == 0)
            {
                Dialog.ShowDialog("LangKeyWarning".FromResourceDictionary(),
                    "LangKeyPleaseSelectAccountsSelectedOnlyDestinations".FromResourceDictionary());
                return;
            }

            // Check whether destination name is already present or not 
            if (!IsDuplicate())
            {
                // Clear all pre saved selected accounts Id and own wall profile
                PublisherCreateDestinationModel.SelectedAccountIds.Clear();
                PublisherCreateDestinationModel.PublishOwnWallAccount.Clear();
                PublisherCreateDestinationModel.AccountsWithNetwork.Clear();


                PublisherCreateDestinationModel.ListSelectDestination.ForEach(x =>
                {
                    // Check the account has been selected or not
                    if (x.IsAccountSelected)
                    {
                        PublisherCreateDestinationModel.SelectedAccountIds.Add(x.AccountId);
                        PublisherCreateDestinationModel.AccountsWithNetwork.Add(
                            new KeyValuePair<SocialNetworks, string>(x.SocialNetworks, x.AccountId));

                        if (x.PublishonOwnWall)
                        {
                            PublisherCreateDestinationModel.PublishOwnWallAccount.Add(x.AccountId);
                            PublisherCreateDestinationModel.DestinationDetailsModels.Add(
                                new PublisherDestinationDetailsModel
                                {
                                    AccountId = x.AccountId,
                                    SocialNetworks = x.SocialNetworks,
                                    DestinationType = ConstantVariable.OwnWall,
                                    DestinationUrl = x.AccountId,
                                    PublisherPostlistModel = new PublisherPostlistModel(),
                                    DestinationGuid = Utilities.GetGuid(),
                                    AccountName = x.AccountName
                                });
                        }
                        else
                        {
                            PublisherCreateDestinationModel.DestinationDetailsModels.RemoveAll(z =>
                                z.DestinationType == ConstantVariable.OwnWall && z.AccountId == x.AccountId);
                        }
                    }
                    else
                    {
                        // If account has selected, remove from selected lists
                        var unwantedGroups = PublisherCreateDestinationModel.AccountGroupPair
                            .Where(y => y.Key == x.AccountId).Select(y => y.Key);
                        PublisherCreateDestinationModel.AccountGroupPair.RemoveAll(z => unwantedGroups.Contains(z.Key));

                        var unwantedPages = PublisherCreateDestinationModel.AccountPagesBoardsPair
                            .Where(y => y.Key == x.AccountId).Select(y => y.Key);
                        PublisherCreateDestinationModel.AccountPagesBoardsPair.RemoveAll(z =>
                            unwantedPages.Contains(z.Key));

                        PublisherCreateDestinationModel.DestinationDetailsModels.RemoveAll(z =>
                            z.AccountId == x.AccountId);

                        PublisherCreateDestinationModel.CustomDestinations.RemoveAll(z => z.Key == x.AccountId);
                    }
                });

                PublisherCreateDestinationModel.AccountGroupPair =
                    PublisherCreateDestinationModel.AccountGroupPair.Distinct().ToList();

                PublisherCreateDestinationModel.PublishOwnWallAccount =
                    PublisherCreateDestinationModel.PublishOwnWallAccount.Distinct().ToList();

                PublisherCreateDestinationModel.SelectedAccountIds =
                    PublisherCreateDestinationModel.SelectedAccountIds.Distinct().ToList();

                PublisherCreateDestinationModel.AccountPagesBoardsPair =
                    PublisherCreateDestinationModel.AccountPagesBoardsPair.Distinct().ToList();

                if (PublisherCreateDestinationModel.AccountGroupPair.Count == 0 &&
                    PublisherCreateDestinationModel.AccountPagesBoardsPair.Count == 0 &&
                    PublisherCreateDestinationModel.PublishOwnWallAccount.Count == 0 &&
                    PublisherCreateDestinationModel.CustomDestinations.Count == 0)
                {
                    Dialog.ShowDialog(Application.Current.MainWindow,
                        "LangKeyWarning".FromResourceDictionary(),
                        "LangKeyPleaseSelectDestination".FromResourceDictionary());
                    return;
                }

                // New Destination
                if (string.IsNullOrEmpty(EditDestinationId))
                {
                    PublisherCreateDestinationModel.AddDestination(PublisherCreateDestinationModel);

                    var publisherManageDestinationModel = new PublisherManageDestinationModel
                    {
                        AccountCount = PublisherCreateDestinationModel.SelectedAccountIds.Count,
                        CampaignsCount = !IsNeedToNavigate ? 0 : 1,
                        CreatedDate = DateTime.Now,
                        DestinationId = PublisherCreateDestinationModel.DestinationId,
                        DestinationName = PublisherCreateDestinationModel.DestinationName,
                        GroupsCount = PublisherCreateDestinationModel.AccountGroupPair.Count,
                        IsSelected = false,
                        PagesOrBoardsCount = PublisherCreateDestinationModel.AccountPagesBoardsPair.Count,
                        WallsOrProfilesCount = PublisherCreateDestinationModel.PublishOwnWallAccount.Count,
                        CustomDestinationsCount = PublisherCreateDestinationModel.CustomDestinations.Count,
                        IsAddNewGroups = PublisherCreateDestinationModel.IsAddedNewGroups,
                        IsRemoveGroupsRequiresValidation =
                            PublisherCreateDestinationModel.IsRemoveGroupsRequiresApproval
                    };

                    PublisherManageDestinations.Instance().PublisherManageDestinationViewModel.AddDestinations(
                        publisherManageDestinationModel, true);
                }
                // Edit Destination
                else
                {
                    PublisherCreateDestinationModel.UpdateDestination(PublisherCreateDestinationModel);

                    UpdateManageDestination();
                }

                InitializeProperties();

                IsSavedDestination = true;
                if (!IsNeedToNavigate)
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl
                        = PublisherManageDestinations.Instance();
                Dialog.CloseDialog(sender);
            }
            else
            {
                GlobusLogHelper.log.Info("LangKeyValidationFailed".FromResourceDictionary());
            }
        }

        private void UpdateManageDestination()
        {
            var publisherManageDestinationModel = PublisherManageDestinations.Instance()
                .PublisherManageDestinationViewModel.GetManageDestination(EditDestinationId);

            // To update the destination name
            publisherManageDestinationModel.DestinationName
                = PublisherCreateDestinationModel.DestinationName;

            // To update the selected account count
            publisherManageDestinationModel.AccountCount =
                PublisherCreateDestinationModel.SelectedAccountIds.Count;

            // To update the group count
            publisherManageDestinationModel.GroupsCount =
                PublisherCreateDestinationModel.AccountGroupPair.Count;

            // To update the page or boards counts
            publisherManageDestinationModel.PagesOrBoardsCount =
                PublisherCreateDestinationModel.AccountPagesBoardsPair.Count;

            // To update the wall count 
            publisherManageDestinationModel.WallsOrProfilesCount =
                PublisherCreateDestinationModel.PublishOwnWallAccount.Count;

            publisherManageDestinationModel.CustomDestinationsCount =
                PublisherCreateDestinationModel.CustomDestinations.Count;

            publisherManageDestinationModel.IsAddNewGroups
                = PublisherCreateDestinationModel.IsAddedNewGroups;

            publisherManageDestinationModel.IsRemoveGroupsRequiresValidation =
                PublisherCreateDestinationModel.IsRemoveGroupsRequiresApproval;

            if (!IsNeedToNavigate)
                // To call a method to update the manage destination user interface
                PublisherManageDestinations.Instance().PublisherManageDestinationViewModel.UpdateDestinations(
                    publisherManageDestinationModel);
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
            SelectPageBoard = "LangKeySelectPagesBoardsAll".FromResourceDictionary();
            SelectGroups = "LangKeySelectGroupsAll".FromResourceDictionary();
            SelectWall = "LangKeySelectWallsProfilesAll".FromResourceDictionary();
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
            return PublisherCreateDestinationModel.ListSelectDestination
                .Where(x => x.StatusSyncContent == ConstantVariable.NeedUpdateStatusSync).ToList();
        }


        public async Task UpdateSyncStatusAsync(PublisherCreateDestinationSelectModel selectedSyncAccount)
        {
            var currentAccountDetails =
                PublisherCreateDestinationModel.ListSelectDestination.FirstOrDefault(x =>
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

            PublisherCreateDestinationModel.AccountGroupPair.RemoveAll(x =>
                x.Key == selectedSyncAccount.AccountId && !currentGroups.Contains(x.Value));

            PublisherCreateDestinationModel.AccountPagesBoardsPair.RemoveAll(x =>
                x.Key == selectedSyncAccount.AccountId && !currentPages.Contains(x.Value));

            currentAccountDetails.TotalGroups = currentGroups.Count;

            currentAccountDetails.TotalPagesOrBoards = currentPages.Count;

            currentAccountDetails.SelectedGroups =
                PublisherCreateDestinationModel.AccountGroupPair.Count(x => x.Key == selectedSyncAccount.AccountId);

            currentAccountDetails.SelectedPagesOrBoards =
                PublisherCreateDestinationModel.AccountPagesBoardsPair.Count(
                    x => x.Key == selectedSyncAccount.AccountId);

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
                var isNeedToUpdate = false;
                var accountList = InstanceProvider.GetInstance<IDominatorAccountViewModel>()
                    .LstDominatorAccountModel;
                var accounts = accountList.Where(x => x.AccountBaseModel.Status == AccountStatus.Success);
                var availableAccountId = accounts.Select(x => x.AccountId);
                PublisherCreateDestinationModel.ListSelectDestination.ToList().ForEach(x =>
                {
                    if (!availableAccountId.Contains(x.AccountId))
                    {
                        isNeedToUpdate = true;
                        PublisherCreateDestinationModel.ListSelectDestination.Remove(x);
                        PublisherCreateDestinationModel.AccountGroupPair.RemoveAll(g => g.Key == x.AccountId);
                        PublisherCreateDestinationModel.AccountPagesBoardsPair.RemoveAll(g => g.Key == x.AccountId);
                        PublisherCreateDestinationModel.AccountsWithNetwork.RemoveAll(g => g.Value == x.AccountId);
                        PublisherCreateDestinationModel.CustomDestinations.RemoveAll(g => g.Key == x.AccountId);
                        PublisherCreateDestinationModel.DestinationDetailsModels.RemoveAll(g =>
                            g.AccountId == x.AccountId);
                        PublisherCreateDestinationModel.PublishOwnWallAccount.RemoveAll(g => g == x.AccountId);
                        PublisherCreateDestinationModel.SelectedAccountIds.Remove(x.AccountId);
                    }
                });

                #region Update Destination

                if (isNeedToUpdate)
                {
                    PublisherCreateDestinationModel.UpdateDestination(PublisherCreateDestinationModel);
                    EditDestinationId = PublisherCreateDestinationModel.DestinationId;
                    UpdateManageDestination();
                }

                #endregion

                accounts.ForEach(x =>
                {
                    if (PublisherCreateDestinationModel.ListSelectDestination.All(y => y.AccountId != x.AccountId))
                    {
                        var pageCount = x.AccountBaseModel.AccountNetwork == SocialNetworks.LinkedIn
                            ? x.DisplayColumnValue4 ?? 0
                            : x.DisplayColumnValue3 ?? 0;

                        var publisherCreateDestinationSelectModel = new PublisherCreateDestinationSelectModel
                        {
                            AccountId = x.AccountBaseModel.AccountId,
                            AccountName = x.AccountBaseModel.UserName,
                            SocialNetworks = x.AccountBaseModel.AccountNetwork,
                            IsOwnWallAvailable =
                                !WallAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork
                                    .ToString()), // x.AccountBaseModel.AccountNetwork != SocialNetworks.Pinterest,
                            IsScrapingAvailableInNetworks =
                                ScrapingAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                            IsCustomDestinationInNetworks =
                                IsCustomDestinationInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                            IsGroupsAvailable =
                                GroupsAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                            IsPagesOrBoardsAvailable =
                                BoardsOrPagesAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                            PublishonOwnWall = false,
                            SelectedGroups = 0,
                            TotalGroups = x.DisplayColumnValue2 ?? 0,
                            TotalPagesOrBoards = pageCount
                        };
                        publisherCreateDestinationSelectModel.GroupSelectorText =
                            GroupsAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString())
                                ? "0" + "/" + publisherCreateDestinationSelectModel.TotalGroups
                                : "LangKeyNA".FromResourceDictionary();

                        publisherCreateDestinationSelectModel.PagesOrBoardsSelectorText =
                            BoardsOrPagesAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString())
                                ? "0" + "/" + publisherCreateDestinationSelectModel.TotalPagesOrBoards
                                : "LangKeyNA".FromResourceDictionary();
                        if (SocinatorInitialize.IsNetworkAvailable(x.AccountBaseModel.AccountNetwork))
                            PublisherCreateDestinationModel.ListSelectDestination.Add(
                                publisherCreateDestinationSelectModel);
                    }
                });
                DestinationCollectionView =
                    CollectionViewSource.GetDefaultView(PublisherCreateDestinationModel.ListSelectDestination);
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
            var publisherCreateDestinationSelectModel = sender as PublisherCreateDestinationSelectModel;

            if (publisherCreateDestinationSelectModel == null)
                return;

            var valuePairs = PublisherCreateDestinationModel.CustomDestinations
                .Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).ToList();

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
            var dialog = new Dialog();
            var window = dialog.GetMetroWindow(publisherAddCustomDestination,
                "LangKeyAddCustomDestination".FromResourceDictionary());

            publisherAddCustomDestination.ButtonSave.Click += (senders, args) =>
            {
                var savedNewCustomDestination = publisherAddCustomDestination.GetSavedCustomDestination();
                var createDestinationSelectModel =
                    PublisherCreateDestinationModel.ListSelectDestination.FirstOrDefault(x =>
                        x.AccountId == publisherCreateDestinationSelectModel.AccountId);

                PublisherCreateDestinationModel.CustomDestinations.RemoveAll(x =>
                    x.Key == publisherCreateDestinationSelectModel.AccountId);

                PublisherCreateDestinationModel.DestinationDetailsModels.RemoveAll(x =>
                    x.AccountId == publisherCreateDestinationSelectModel.AccountId && x.IsCustomDestintions);

                savedNewCustomDestination.ForEach(x =>
                {
                    PublisherCreateDestinationModel.CustomDestinations.Add(
                        new KeyValuePair<string, PublisherCustomDestinationModel>(
                            publisherCreateDestinationSelectModel.AccountId, x));

                    PublisherCreateDestinationModel.DestinationDetailsModels.Add(new PublisherDestinationDetailsModel
                    {
                        AccountId = publisherCreateDestinationSelectModel.AccountId,
                        DestinationType = x.DestinationType,
                        DestinationUrl = x.DestinationValue,
                        SocialNetworks = publisherCreateDestinationSelectModel.SocialNetworks,
                        AccountGroupName = publisherCreateDestinationSelectModel.AccountGroupName,
                        PublisherPostlistModel = new PublisherPostlistModel(),
                        IsCustomDestintions = true,
                        DestinationGuid = Utilities.GetGuid(),
                        AccountName = publisherCreateDestinationSelectModel.AccountName
                    });
                });

                publisherAddCustomDestination.ResetCurrectObject();

                if (createDestinationSelectModel != null)
                    createDestinationSelectModel.CustomDestinationSelectorText =
                        $"{PublisherCreateDestinationModel.CustomDestinations.Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).ToList().Count}";
                window.Close();
            };
            publisherAddCustomDestination.ButtonCancel.Click += (senders, args) => { window.Close(); };


            window.Show();
        }

        #endregion
    }
}
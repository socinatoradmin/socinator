using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PinDominator.CustomControl;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
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
using AccountDetailsSelectorModel = PinDominatorCore.PDModel.AccountDetailsSelectorModel;

namespace PinDominator.ViewModel
{
    public class BoardCreateDestinationsViewModel : BindableBase
    {
        //ConstructorS
        public static AccountDetailsSelector accountDetailsSelector = new AccountDetailsSelector();
        public BoardCreateDestinationsViewModel()
        {
            //NavigationCommand = new BaseCommand<object>(NavigationCanExecute, NavigationExecute);
            GetSingleAccountPagesOrBoardsCommand = new BaseCommand<object>(GetSingleAccountPagesOrBoardsCanExecute,
                GetSingleAccountPagesOrBoardsExecute);
            CheckSelectAllAccountsAndBoardsCommand = new BaseCommand<object>(SelectAllAccountsAndBoardsCanExecute,
                SelectAllAccountsAndBoardsExecute);
            SelectionCommand = new BaseCommand<object>(SelectionCanExecute, SelectionExecute);
            OpenContextMenuCommand = new BaseCommand<object>(OpenContextMenuCanExecute, OpenContextMenuExecute);
            SelectAllAccountDetailsCommand =
                new BaseCommand<object>(SelectAccountDetailsCanExecute, SelectAccountDetailsExecute);
            ClearCommand = new BaseCommand<object>(ClearCanExecute, ClearExecute);
            StatusSyncCommand = new BaseCommand<object>(SyncCanExecute, SyncExecute);
            AddFreshAccounts = new BaseCommand<object>(AddFreshAccountCanExecute, AddFreshAccountExecute);
            SaveCommand = new BaseCommand<object>(SaveCanExecute, SaveExecute);
            InitializeDestinationList();
            IsSavedDestination = false;
        }

        private void FilterByNetwork()
        {
            try
            {
                if (SelectedNetworks == SocialNetworks.Social)
                {
                    if (!string.IsNullOrEmpty(FilterText))
                        DestinationCollectionView.Filter = x =>
                            ((PublisherCreateDestinationSelectModel) x).AccountName.IndexOf(FilterText,
                                StringComparison.CurrentCultureIgnoreCase) >= 0;
                    else DestinationCollectionView.Filter = x => true;
                }
                else
                {
                    if (!string.IsNullOrEmpty(FilterText))
                        DestinationCollectionView.Filter =
                            x => ((PublisherCreateDestinationSelectModel) x).SocialNetworks == SelectedNetworks &&
                                 ((PublisherCreateDestinationSelectModel) x).AccountName.IndexOf(_filterText,
                                     StringComparison.CurrentCultureIgnoreCase) >= 0;
                    else
                        DestinationCollectionView.Filter = x =>
                            ((PublisherCreateDestinationSelectModel) x).SocialNetworks == SelectedNetworks;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region Initialize Updates

        public void InitializeDestinationList()
        {
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var accounts = accountsFileManager.GetAll();

            if (!Application.Current.CheckAccess())
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PublisherCreateDestinationModel.ListSelectDestination.Clear();
                });
            else
                PublisherCreateDestinationModel.ListSelectDestination.Clear();

            accounts.ForEach(x =>
            {
                var publisherCreateDestinationSelectModel = new RepinCreateDestinationSelectModel
                {
                    AccountId = x.AccountBaseModel.AccountId,
                    GroupName=x.AccountBaseModel.AccountGroup.Content,
                    AccountName = x.AccountBaseModel.UserName,
                    SocialNetworks = x.AccountBaseModel.AccountNetwork,
                    //IsOwnWallAvailable = x.AccountBaseModel.AccountNetwork != SocialNetworks.Pinterest,
                    IsGroupsAvailable =
                        GroupsAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                    IsPagesOrBoardsAvailable =
                        BoardsOrPagesAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                    PublishonOwnWall = false,
                    SelectedGroups = 0,
                    TotalGroups = x.DisplayColumnValue2 ?? 0,
                    TotalPagesOrBoards = x.DisplayColumnValue3 ?? 0
                };

                publisherCreateDestinationSelectModel.GroupSelectorText =
                    GroupsAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString())
                        ? "0" + "/" + publisherCreateDestinationSelectModel.TotalGroups
                        : "NA";

                publisherCreateDestinationSelectModel.PagesOrBoardsSelectorText =
                    BoardsOrPagesAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString())
                        ? "0" + "/" + publisherCreateDestinationSelectModel.TotalPagesOrBoards
                        : "NA";

                if (x.AccountBaseModel.AccountNetwork.Equals(SocialNetworks.Pinterest) &&
                    x.AccountBaseModel.Status.Equals(AccountStatus.Success))
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

        #region Commands

        public ICommand AddFreshAccounts { get; set; }

        public ICommand StatusSyncCommand { get; set; }

        public ICommand ClearCommand { get; set; }

        public ICommand NavigationCommand { get; set; }

        public ICommand SelectAllAccountDetailsCommand { get; set; }

        public ICommand OpenContextMenuCommand { get; set; }

        public ICommand SelectionCommand { get; set; }

        public ICommand GetSingleAccountPagesOrBoardsCommand { get; set; }

        public ICommand SaveDestinationCommand { get; set; }

        public ICommand CheckSelectAllAccountsAndBoardsCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        #endregion

        #region Properties

        private SocialNetworks _selectedNetworks = SocialNetworks.Social;

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
            }
        }

        private HashSet<SocialNetworks> _availableNetworks = SocinatorInitialize.AvailableNetworks;

        public HashSet<SocialNetworks> AvailableNetworks
        {
            get => _availableNetworks;
            set
            {
                _availableNetworks = value;
                OnPropertyChanged(nameof(AvailableNetworks));
            }
        }

        private BoardCreateDestinationModel _publisherCreateDestinationModel =
            BoardCreateDestinationModel.DestinationDefaultBuilder();

        public BoardCreateDestinationModel PublisherCreateDestinationModel
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
                IsUncheckedFromList = false;
            }
        }

        private bool _isSelectAllAccountsAndBoards;

        public bool IsSelectAllAccountsAndBoards
        {
            get => _isSelectAllAccountsAndBoards;
            set
            {
                if (_isSelectAllAccountsAndBoards == value)
                    return;
                SetProperty(ref _isSelectAllAccountsAndBoards, value);
            }
        }

        private List<RepinQueryContent> _lstOfKeywords = new List<RepinQueryContent>();

        public List<RepinQueryContent> LstOfKeywords
        {
            get => _lstOfKeywords;
            set
            {
                if (_lstOfKeywords == value)
                    return;
                SetProperty(ref _lstOfKeywords, value);
            }
        }

        private string _boardToSelect;

        public string BoardToSelect
        {
            get => _boardToSelect;
            set
            {
                if (_boardToSelect == value)
                    return;
                SetProperty(ref _boardToSelect, value);
            }
        }

        public List<string> GroupsAvailableInNetworks { get; set; } =
            new List<string> {"Facebook", "LinkedIn", "Reddit"};

        public List<string> BoardsOrPagesAvailableInNetworks { get; set; } = new List<string>
            {"Facebook", "YouTube", "Pinterest", "LinkedIn", "Gplus", "Tumblr"};

        #endregion

        #region Get Single Account Page Details

        private bool GetSingleAccountPagesOrBoardsCanExecute(object sender)
        {
            return true;
        }

        private bool SelectAllAccountsAndBoardsCanExecute(object sender)
        {
            return true;
        }

        private void GetSingleAccountPagesOrBoardsExecute(object sender)
        {
            var publisherCreateDestinationSelectModel = (RepinCreateDestinationSelectModel) sender;

            // get the page or board pair from collection with the account Id
            var valuePairs =
                PublisherCreateDestinationModel.AccountPagesBoardsPair
                    .Where(x => x.AccountId == publisherCreateDestinationSelectModel.AccountId).ToList();

            // get the factory pattern for the network of an account
            var accountsDetailsSelector = SocinatorInitialize
                .GetSocialLibrary(publisherCreateDestinationSelectModel.SocialNetworks)
                .GetNetworkCoreFactory().AccountDetailsSelectors;

            // Fetch the page details only
            var alreadySelectedPages = valuePairs.Select(x => x.LstofPinsToRepin.Key).ToList();

            // Pass the fetching activity functions as action to UI
            accountDetailsSelector =
                new AccountDetailsSelector(UpdateSingleAccountPagesDetails, publisherCreateDestinationSelectModel)
                {
                    // Find whether page or board, its vary based on each network
                    AccountDetailsSelectorViewModel =
                    {
                        Title = $"Select {accountsDetailsSelector.DisplayAsPageOrBoards}",
                        DetailsUrlHeader = $"{accountsDetailsSelector.DisplayAsPageOrBoards} Url",
                        DetailsNameHeader = $"{accountsDetailsSelector.DisplayAsPageOrBoards} Name",
                        DetailsQueryTypeHeader = $"{accountsDetailsSelector.DisplayAsPageOrBoards} Query Type",
                        DetailsLabelHeader = $"{accountsDetailsSelector.DisplayAsPageOrBoards} Label",
                        DetailsUrlSection=$"{accountsDetailsSelector.DisplayAsPageOrBoards} Section",
                        AlreadySelectedList = alreadySelectedPages
                    }
                };

            var dialog = new Dialog();

            var window = dialog.GetMetroWindow(accountDetailsSelector,
                $"Select {accountsDetailsSelector.DisplayAsPageOrBoards}");

            // Defining the save buttons click events
            accountDetailsSelector.BtnSave.Click += (senderDetails, events) =>
            {
                var keyValuePairs =
                    accountDetailsSelector.AccountDetailsSelectorViewModel
                        .GetSelectedItems(publisherCreateDestinationSelectModel.IsAccountSelected)
                        .ToList();

                if (keyValuePairs.Count == 0)
                {
                    Dialog.ShowDialog("Error", "please select atleast one board");
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                        publisherCreateDestinationSelectModel.AccountName, ActivityType.Repin,
                        "please select atleast one board");
                }

                else
                {
                    // here we taking only list of group selected but not selected any query inside it.
                    //var unselectedList = keyValuePairs.Where(x => x.Item2.Value.Any(y => !y.IsContentSelected)).ToList();
                    var check = keyValuePairs.Count != 0 &&
                                keyValuePairs.All(x => x.LstofPinsToRepin.Value.Any(y => y.IsContentSelected));
                    if (check /*unselectedList.Count == 0*/)
                    {
                        // Remove all the saved accounts page or boards pair
                        valuePairs.ForEach(x =>
                        {
                            //PublisherCreateDestinationModel.AccountPagesBoardsPair.Remove(x);
                            PublisherCreateDestinationModel.DestinationDetailsModels.RemoveAll(y =>
                                x.AccountId == y.AccountId && y.DestinationType == ConstantVariable.PageOrBoard);
                        });

                        //PublisherCreateDestinationModel.AccountPagesBoardsPair.Clear();

                        PublisherCreateDestinationModel.AccountPagesBoardsPair.RemoveAll(x =>
                            keyValuePairs.Any(y => y.AccountId == x.AccountId));

                        // Get the selected pairs

                        //keyValuePairs.RemoveAll(x =>
                        //    PublisherCreateDestinationModel.AccountPagesBoardsPair.Any(y =>
                        //        x.AccountId == y.AccountId && x.LstofPinsToRepin.Key == y.LstofPinsToRepin.Key && x.IsSelected == y.IsSelected));

                        // Add to key value pair
                        PublisherCreateDestinationModel.AccountPagesBoardsPair.AddRange(keyValuePairs);

                        // Get the destination full details of a page or board
                        var destinationDetails = accountDetailsSelector.AccountDetailsSelectorViewModel
                            .GetSelectedItemsDestinations(ConstantVariable.PageOrBoard).ToList();

                        // Update with destination details
                        PublisherCreateDestinationModel.DestinationDetailsModels.AddRange(destinationDetails);

                        // Get the already selected page details 
                        alreadySelectedPages = PublisherCreateDestinationModel.AccountPagesBoardsPair
                            .Where(x => x.AccountId == publisherCreateDestinationSelectModel.AccountId)
                            .Select(x => x.LstofPinsToRepin.Key)
                            .ToList();

                        //
                        var createDestinationSelectModel =
                            PublisherCreateDestinationModel.ListSelectDestination.FirstOrDefault(x =>
                                x.AccountId == publisherCreateDestinationSelectModel.AccountId);

                        if (createDestinationSelectModel != null)
                            createDestinationSelectModel.PagesOrBoardsSelectorText =
                                $"{alreadySelectedPages.Count}/{createDestinationSelectModel.TotalPagesOrBoards}";

                        window.Close();
                    }
                    else
                    {
                        Dialog.ShowDialog("Error", "please select atleast one query type");
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                            publisherCreateDestinationSelectModel.AccountName, ActivityType.Repin,
                            "please select atleast one query type");
                    }
                }
            };

            accountDetailsSelector.BtnCancel.Click += (senderDetails, events) => { window.Close(); };

            window.Show();

            accountDetailsSelector.UpdateUiSingleData();
        }

        private void SelectAllAccountsAndBoardsExecute(object sender)
        {
            if (BoardCreateDestination.ListQueries.Count == 0)
                return;
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var accounts = accountsFileManager.GetAll();
            if (IsSelectAllAccountsAndBoards)
            {
                // Pass the fetching activity functions as action to UI
                var AccountDetailsSelectorViewModel = new AccountDetailsSelectorViewModel();

                PublisherCreateDestinationModel.ListSelectDestination.ForEach(x => x.IsAccountSelected = true);

                accounts.Where(x =>
                    x.AccountBaseModel.AccountNetwork == SocialNetworks.Pinterest &&
                    x.AccountBaseModel.Status == AccountStatus.Success).ForEach(async m =>
                {
                    var pagesOrBoards = await GetPagesDetails(m.AccountId, m.UserName);

                    AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Clear();
                    pagesOrBoards.ForEach(page =>
                    {
                        page.Network = SocialNetworks.Pinterest;
                        page.IsSelected = true;
                        page.QueryType.ForEach(x => x.IsContentSelected = true);
                        if (!Application.Current.Dispatcher.CheckAccess())
                            Application.Current.Dispatcher.Invoke(() =>
                                AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(page));
                        else
                            AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(page);
                    });

                    var keyValuePairs = AccountDetailsSelectorViewModel.SelectAllItems().ToList();

                    //PublisherCreateDestinationModel.AccountPagesBoardsPair.Clear();
                    PublisherCreateDestinationModel.AccountPagesBoardsPair.RemoveAll(x => x.AccountId == m.AccountId);
                    // Add to key value pair
                    PublisherCreateDestinationModel.AccountPagesBoardsPair.AddRange(keyValuePairs);

                    var createDestinationSelectModel =
                        PublisherCreateDestinationModel.ListSelectDestination.FirstOrDefault(x =>
                            x.AccountId == m.AccountId);

                    // Get the already selected page details 
                    var alreadySelectedPages = PublisherCreateDestinationModel.AccountPagesBoardsPair
                        .Where(x => x.AccountId == m.AccountId).Select(x => x.LstofPinsToRepin.Key)
                        .ToList();

                    if (createDestinationSelectModel != null)
                        createDestinationSelectModel.PagesOrBoardsSelectorText =
                            $"{alreadySelectedPages.Count}/{createDestinationSelectModel.TotalPagesOrBoards}";
                });
            }
            else
            {
                PublisherCreateDestinationModel.AccountPagesBoardsPair.Clear();

                PublisherCreateDestinationModel.ListSelectDestination.ForEach(x => x.IsAccountSelected = false);
                accounts.Where(x =>
                    x.AccountBaseModel.AccountNetwork == SocialNetworks.Pinterest &&
                    x.AccountBaseModel.Status == AccountStatus.Success).ForEach(y =>
                {
                    var createDestinationSelectModel =
                        PublisherCreateDestinationModel.ListSelectDestination.FirstOrDefault(x =>
                            x.AccountId == y.AccountId);

                    var alreadySelectedPages = PublisherCreateDestinationModel.AccountPagesBoardsPair
                        .Where(x => x.AccountId == y.AccountId).Select(x => x.LstofPinsToRepin.Key)
                        .ToList();

                    if (createDestinationSelectModel != null)
                        createDestinationSelectModel.PagesOrBoardsSelectorText =
                            $"{alreadySelectedPages.Count}/{createDestinationSelectModel.TotalPagesOrBoards}";
                });
            }
        }

        private async Task UpdateSingleAccountPagesDetails(AccountDetailsSelector accountDetailsSelector,
            RepinCreateDestinationSelectModel publisherCreateDestinationSelectModel)
        {
            //var valuePairs = PublisherCreateDestinationModel.AccountGroupPair.Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).ToList(); ;

            //var alreadySelectedPages = valuePairs.Select(x => x.Value).ToList();

            var alreadySelectedPages = accountDetailsSelector.AccountDetailsSelectorViewModel.AlreadySelectedList;
            var pagesOrBoards = await GetPagesDetails(publisherCreateDestinationSelectModel.AccountId,
                publisherCreateDestinationSelectModel.AccountName, alreadySelectedPages);

            pagesOrBoards.ForEach(page =>
            {
                page.Network = publisherCreateDestinationSelectModel.SocialNetworks;
                //page.IsSelected = alreadySelectedPages.Contains(page.DetailUrl);
                page.IsSelected = page.QueryType.Any(x => x.IsContentSelected);
                if (!Application.Current.Dispatcher.CheckAccess())
                    Application.Current.Dispatcher.Invoke(() =>
                        accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels
                            .Add(page));
                else
                    accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(page);
            });

            UpdateStatus(accountDetailsSelector);
        }

        public async Task<List<AccountDetailsSelectorModel>> GetPagesDetails(string accountId, string accountName,
            List<string> alreadySelectedList = null)
        {
            if (alreadySelectedList == null) alreadySelectedList = new List<string>();
            var listBoardUrl = new List<AccountDetailsSelectorModel>();

            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var account = accountsFileManager.GetAll().FirstOrDefault(x => x.AccountId == accountId);
            var dbAccountService = InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(account);
            var boards = await dbAccountService.GetAsync<OwnBoards>();
            boards.ForEach(x =>
            {
                var listQueriesSelected = PublisherCreateDestinationModel.AccountPagesBoardsPair
                    .FirstOrDefault(y => y.LstofPinsToRepin.Key == x.BoardUrl)?.LstofPinsToRepin.Value;
                var listQueriesFinal = new List<RepinQueryContent>();
                BoardCreateDestination.ListQueries.ForEach(i =>
                {
                    var isContentSelected = listQueriesSelected?.FirstOrDefault(y => y.Content == i.Content)
                        ?.IsContentSelected;
                    listQueriesFinal.Add(new RepinQueryContent
                    {
                        BoardUrl = x.BoardUrl,
                        Content = i.Content,
                        IsContentSelected = isContentSelected ?? i.IsContentSelected
                    });
                });
                if (listQueriesFinal.Count == 1 || listQueriesFinal.Skip(1).Any(y => y.IsContentSelected != true))
                    listQueriesFinal.FirstOrDefault(y => y.Content == "All").IsContentSelected = false;
                else if (listQueriesFinal.Count != 1 &&
                         listQueriesFinal.FirstOrDefault(y => y.Content == "All")?.IsContentSelected == false &&
                         listQueriesFinal.Skip(1).All(y => y.IsContentSelected))
                    listQueriesFinal.FirstOrDefault(y => y.Content == "All").IsContentSelected = true;

                var sections = JsonConvert.DeserializeObject<JArray>(x.BoardSections);
                var accountDetailsSelectorModel = new AccountDetailsSelectorModel
                {
                    AccountId = accountId,
                    AccountName = accountName,
                    DetailName = x.BoardName,
                    DetailUrl = x.BoardUrl,
                    Label = PublisherCreateDestinationModel.AccountPagesBoardsPair.Count != 0
                        ? PublisherCreateDestinationModel.AccountPagesBoardsPair
                            .FirstOrDefault(y => y.LstofPinsToRepin.Key == x.BoardUrl)?.Label
                        : "",
                    IsSectionAvailable = sections.Count > 0,
                    DetailSection = sections.Count > 0? $"0/{sections.Count}":"N/A",
                    SectionValue=JsonConvert.SerializeObject(sections),
                    QueryType = new ObservableCollection<RepinQueryContent>(listQueriesFinal),
                    IsSelected = alreadySelectedList.Contains(x.BoardUrl)
                };
                listBoardUrl.Add(accountDetailsSelectorModel);
            });

            return listBoardUrl;
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

                    if (PublisherCreateDestinationModel.ListSelectDestination.All(x => x.IsAccountSelected))
                    {
                        IsAllDestinationSelected = true;
                    }
                    else
                    {
                        if (IsAllDestinationSelected)
                            IsUncheckedFromList = true;
                        IsAllDestinationSelected = false;
                    }

                    break;
            }
        }

        private bool IsUncheckedFromList { get; set; }

        #endregion

        #region Select Destination fucntionality , and also selection menu options

        public void SelectAllDestination(bool isChecked)
        {
            if (IsUncheckedFromList)
                return;
            PublisherCreateDestinationModel.ListSelectDestination.Select(x =>
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
                case "Pages":
                    LoadAllAccountsPagesOrBoards();
                    break;
            }
        }

        #endregion

        #region Get all account's pages and groups details

        public void LoadAllAccountsPagesOrBoards()
        {
            var valuePairs = PublisherCreateDestinationModel.AccountPagesBoardsPair.ToList();

            var alreadySelectedPages = valuePairs.Select(x => x.LstofPinsToRepin.Key).ToList();

            var accountDetailsSelector = new AccountDetailsSelector(UpdatePagesDetails)
            {
                AccountDetailsSelectorViewModel =
                {
                    Title = "Select Pages/Boards",
                    DetailsUrlHeader = "Pages/Boards Url",
                    DetailsNameHeader = "Pages/Boards Name",
                    AlreadySelectedList = alreadySelectedPages
                }
            };

            var dialog = new Dialog();

            var window = dialog.GetMetroWindow(accountDetailsSelector, "Select Pages/Boards");

            accountDetailsSelector.BtnSave.Click += (sender, events) =>
            {
                valuePairs.ForEach(x =>
                {
                    PublisherCreateDestinationModel.AccountPagesBoardsPair.Remove(x);
                    PublisherCreateDestinationModel.DestinationDetailsModels.RemoveAll(y =>
                        x.AccountId == y.AccountId && y.DestinationType == ConstantVariable.PageOrBoard);
                });

                var keyValuePairs = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItems().ToList();

                PublisherCreateDestinationModel.AccountPagesBoardsPair.AddRange(keyValuePairs);

                var destinationDetails = accountDetailsSelector.AccountDetailsSelectorViewModel
                    .GetSelectedItemsDestinations(ConstantVariable.PageOrBoard).ToList();

                PublisherCreateDestinationModel.DestinationDetailsModels.AddRange(destinationDetails);

                keyValuePairs.ForEach(selectedItems =>
                {
                    alreadySelectedPages = PublisherCreateDestinationModel.AccountPagesBoardsPair
                        .Where(x => x.AccountId == selectedItems.AccountId).Select(x => x.LstofPinsToRepin.Key)
                        .ToList();

                    var createDestinationSelectModel =
                        PublisherCreateDestinationModel.ListSelectDestination.FirstOrDefault(x =>
                            x.AccountId == selectedItems.AccountId);

                    if (createDestinationSelectModel != null)
                        createDestinationSelectModel.PagesOrBoardsSelectorText =
                            $"{alreadySelectedPages.Count}/{createDestinationSelectModel.TotalPagesOrBoards}";
                });
                window.Close();
            };

            accountDetailsSelector.BtnCancel.Click += (sender, events) => { window.Close(); };

            window.Show();

            accountDetailsSelector.UpdateUiAllData();
        }

        private void UpdatePagesDetails(AccountDetailsSelector accountDetailsSelector)
        {
            var valuePairs = PublisherCreateDestinationModel.AccountPagesBoardsPair.ToList();

            var alreadySelectedPages = valuePairs.Select(x => x.LstofPinsToRepin.Key).ToList();

            var count = PublisherCreateDestinationModel.ListSelectDestination.Count;

            PublisherCreateDestinationModel.ListSelectDestination.ForEach(async x =>
            {
                try
                {
                    if (BoardsOrPagesAvailableInNetworks.Contains(x.SocialNetworks.ToString()))
                    {
                        var pages = await GetPagesDetails(x.AccountId, x.AccountName, alreadySelectedPages);

                        pages.ForEach(group =>
                        {
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
                            ? $"{accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Count} row(s) found !"
                            : "No row(s) found !";
                });
            }
            else
            {
                accountDetailsSelector.AccountDetailsSelectorViewModel.IsProgressRingActive = false;
                accountDetailsSelector.AccountDetailsSelectorViewModel.StatusText =
                    accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Count > 0
                        ? $"{accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Count} row(s) found !"
                        : "No row(s) found !";
            }
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
                //InitializeProperties();
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


        //public List<PublisherCreateDestinationSelectModel> GetSyncUpdateDestinations()
        //    => PublisherCreateDestinationModel.ListSelectDestination.Where(x => x.StatusSyncContent == ConstantVariable.NeedUpdateStatusSync).ToList();


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
                x.AccountId == selectedSyncAccount.AccountId && !currentPages.Contains(x.LstofPinsToRepin.Key));

            currentAccountDetails.TotalGroups = currentGroups.Count;

            currentAccountDetails.TotalPagesOrBoards = currentPages.Count;

            currentAccountDetails.SelectedGroups =
                PublisherCreateDestinationModel.AccountGroupPair.Count(x => x.Key == selectedSyncAccount.AccountId);

            currentAccountDetails.SelectedPagesOrBoards =
                PublisherCreateDestinationModel.AccountPagesBoardsPair.Count(x =>
                    x.AccountId == selectedSyncAccount.AccountId);

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
                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var accounts = accountsFileManager.GetAll();
                accounts.ForEach(x =>
                {
                    if (PublisherCreateDestinationModel.ListSelectDestination.All(y => y.AccountId != x.AccountId))
                    {
                        var publisherCreateDestinationSelectModel = new RepinCreateDestinationSelectModel
                        {
                            AccountId = x.AccountBaseModel.AccountId,
                            AccountName = x.AccountBaseModel.UserName,
                            SocialNetworks = x.AccountBaseModel.AccountNetwork,
                            //IsOwnWallAvailable = x.AccountBaseModel.AccountNetwork != SocialNetworks.Pinterest,
                            IsGroupsAvailable =
                                GroupsAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                            IsPagesOrBoardsAvailable =
                                BoardsOrPagesAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                            PublishonOwnWall = false,
                            SelectedGroups = 0,
                            TotalGroups = x.DisplayColumnValue2 ?? 0,
                            TotalPagesOrBoards = x.DisplayColumnValue3 ?? 0
                        };
                        publisherCreateDestinationSelectModel.GroupSelectorText =
                            GroupsAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString())
                                ? "0" + "/" + publisherCreateDestinationSelectModel.TotalGroups
                                : "NA";

                        publisherCreateDestinationSelectModel.PagesOrBoardsSelectorText =
                            BoardsOrPagesAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString())
                                ? "0" + "/" + publisherCreateDestinationSelectModel.TotalPagesOrBoards
                                : "NA";
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

        #region save

        private bool SaveCanExecute(object sender)
        {
            return true;
        }

        private void SaveExecute(object sender)
        {
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var accounts = accountsFileManager.GetAll();
            var accountDetailsSelectorViewModel = new AccountDetailsSelectorViewModel();

            accounts.Where(x =>
                x.AccountBaseModel.AccountNetwork == SocialNetworks.Pinterest &&
                x.AccountBaseModel.Status == AccountStatus.Success).ForEach(async m =>
            {
                // get the page or board pair from collection with the account Id
                var valuePairs = PublisherCreateDestinationModel.AccountPagesBoardsPair
                    .Where(x => x.AccountId == m.AccountId).ToList();

                var alreadySelectedPages = valuePairs.Select(x => x.LstofPinsToRepin.Key).ToList();
                //var pagesOrBoards = await GetPagesDetails(m.AccountId, m.UserName);
                var pagesOrBoards =
                    await GetPagesDetails(m.AccountId, m.AccountBaseModel.ProfileId, alreadySelectedPages);

                pagesOrBoards.ForEach(page =>
                {
                    //page.IsSelected = alreadySelectedPages.Contains(page.DetailUrl);
                    page.IsSelected = page.QueryType.Any(x => x.IsContentSelected);
                    if (!Application.Current.Dispatcher.CheckAccess())
                        Application.Current.Dispatcher.Invoke(() =>
                            accountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(page));
                    else
                        accountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(page);
                });

                pagesOrBoards.ForEach(page =>
                {
                    page.Network = SocialNetworks.Pinterest;
                    if (page.DetailName.ToLower().Contains(BoardToSelect))
                    {
                        page.IsSelected = true;
                        if (LstOfKeywords.Any(x => x.Content == "All" && x.IsContentSelected))
                            page.QueryType.ForEach(x => x.IsContentSelected = true);
                        else
                            LstOfKeywords.ForEach(x =>
                            {
                                page.QueryType.ForEach(y =>
                                {
                                    if (y.Content.Contains(x.Content))
                                        y.IsContentSelected = x.IsContentSelected;
                                });
                            });
                    }

                    if (accountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.All(x =>
                        x.DetailUrl != page.DetailUrl))
                    {
                        if (!Application.Current.Dispatcher.CheckAccess())
                            Application.Current.Dispatcher.Invoke(() =>
                                accountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(page));
                        else
                            accountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(page);
                    }
                });
            });

            var keyValuePairs = accountDetailsSelectorViewModel.SelectAllItems().ToList();
            PublisherCreateDestinationModel.AccountPagesBoardsPair = new List<RepinSelectDestination>(keyValuePairs);

            accounts.Where(x =>
                x.AccountBaseModel.AccountNetwork == SocialNetworks.Pinterest &&
                x.AccountBaseModel.Status == AccountStatus.Success).ForEach(m =>
            {
                var createDestinationSelectModel =
                    PublisherCreateDestinationModel.ListSelectDestination.FirstOrDefault(x =>
                        x.AccountId == m.AccountId);

                // Get the already selected page details 
                var alreadySelectedPages = PublisherCreateDestinationModel.AccountPagesBoardsPair
                    .Where(x => x.AccountId == m.AccountId && x.LstofPinsToRepin.Value.Any(y => y.IsContentSelected))
                    .Select(x => x.LstofPinsToRepin.Key)
                    .ToList();

                if (alreadySelectedPages.Count > 0)
                    createDestinationSelectModel.IsAccountSelected = true;
                else
                    createDestinationSelectModel.IsAccountSelected = false;

                if (createDestinationSelectModel != null)
                    createDestinationSelectModel.PagesOrBoardsSelectorText =
                        $"{alreadySelectedPages.Count}/{createDestinationSelectModel.TotalPagesOrBoards}";
            });
        }

        #endregion
    }
}
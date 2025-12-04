using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;
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
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.YoutubeViewModel
{
    public class SelectChannelViewModel : BindableBase
    {
        //ConstructorS
        public SelectChannelViewModel()
        {
            SelectionCommand = new BaseCommand<object>(SelectionCanExecute, SelectionExecute);
            OpenContextMenuCommand = new BaseCommand<object>(OpenContextMenuCanExecute, OpenContextMenuExecute);
            ClearCommand = new BaseCommand<object>(ClearCanExecute, ClearExecute);
            StatusSyncCommand = new BaseCommand<object>(SyncCanExecute, SyncExecute);
            AddFreshAccounts = new BaseCommand<object>(AddFreshAccountCanExecute, AddFreshAccountExecute);
            ChannelsDropDownCommand =
                new BaseCommand<object>(ChannelListDropDownCanExecute, ChannelListDropDownExecute);
        }

        #region Select Destination fucntionality , and also selection menu options

        public void SelectAllDestination(bool isChecked)
        {
            if (IsUncheckedFromList)
                return;
            ChannelSelectModel.ListSelectDestination.Select(x =>
            {
                x.IsAccountSelected = isChecked;
                return x;
            }).ToList();
        }

        #endregion

        #region Commands

        public ICommand AddFreshAccounts { get; set; }

        public ICommand StatusSyncCommand { get; set; }

        public ICommand ClearCommand { get; set; }

        public ICommand OpenContextMenuCommand { get; set; }

        public ICommand SelectionCommand { get; set; }

        public ICommand ChannelsDropDownCommand { get; set; }

        #endregion

        #region Properties

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

        private ChannelSelectModel _channelSelectModel = ChannelSelectModel.DestinationDefaultBuilder();

        public ChannelSelectModel ChannelSelectModel
        {
            get => _channelSelectModel;
            set
            {
                if (_channelSelectModel == value)
                    return;
                _channelSelectModel = value;
                OnPropertyChanged(nameof(ChannelSelectModel));
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

        private ObservableCollection<ContentSelectGroup> _groups = new ObservableCollection<ContentSelectGroup>();

        public ObservableCollection<ContentSelectGroup> Groups
        {
            get => _groups;
            set
            {
                if (_groups == value)
                    return;
                SetProperty(ref _groups, value);
            }
        }


        public List<string> ChannelsAvailableInNetworks { get; set; } = new List<string> { "YouTube" };

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

                    if (ChannelSelectModel.ListSelectDestination.All(x => x.IsAccountSelected))
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

        #region Initialize Updates

        public void CheckDeletedAccounts(List<DominatorAccountModel> accounts)
        {
            if (ChannelSelectModel?.ListSelectDestination == null) return;

            var temp = new List<ChannelDestinationSelectModel>();
            temp.AddRange(ChannelSelectModel?.ListSelectDestination);

            if (ChannelSelectModel == null) return;
            if (accounts.Count == 0)
            {
                ChannelSelectModel.ListSelectDestination = new ObservableCollection<ChannelDestinationSelectModel>();
                return;
            }

            foreach (var item in temp)
            {
                var account =
                    accounts.FirstOrDefault(x => x.AccountId == item.AccountId && x.UserName == item.AccountName);
                var shouldDelete = account == null; /* || account.DisplayColumnValue3 != item.TotalChannels;*/

                if (shouldDelete)
                {
                    var delIt = ChannelSelectModel?.ListSelectDestination?.FirstOrDefault(y =>
                        y.AccountId == item.AccountId);
                    ChannelSelectModel?.ListSelectDestination?.Remove(delIt);
                }

                if (ChannelSelectModel?.ListSelectDestination?.Count == 0)
                    break;
            }
        }

        public void CheckDeletedAccounts(List<DominatorAccountModel> accounts,
            ObservableCollection<ChannelDestinationSelectModel> listSelectDestination)
        {
            if (ChannelSelectModel?.ListSelectDestination == null) return;

            var temp = new List<ChannelDestinationSelectModel>();

            temp.AddRange(ChannelSelectModel?.ListSelectDestination);

            if (accounts.Count == 0)
            {
                listSelectDestination.Clear();
                return;
            }

            foreach (var item in temp)
            {
                var account =
                    accounts.FirstOrDefault(x => x.AccountId == item.AccountId && x.UserName == item.AccountName);
                var shouldDelete = account == null; /* || account.DisplayColumnValue3 != item.TotalChannels;*/

                if (shouldDelete)
                {
                    var delIt = listSelectDestination?.FirstOrDefault(y => y.AccountId == item.AccountId);
                    listSelectDestination?.Remove(delIt);
                }

                if (listSelectDestination?.Count == 0)
                    break;
            }
        }

        public IAccountCollectionViewModel GetGlobalAccountsList => InstanceProvider
            .GetInstance<IDominatorAccountViewModel>().LstDominatorAccountModel;

        public List<DominatorAccountModel> GetYdSuccessAccounts => GetGlobalAccountsList.Where(x =>
            (x.AccountBaseModel.Status == AccountStatus.Success ||
             x.AccountBaseModel.Status == AccountStatus.UpdatingDetails) &&
            x.AccountBaseModel.AccountNetwork == SocialNetworks.YouTube).ToList();

        public void InitializeDestinationList()
        {
            try
            {
                var accounts = GetYdSuccessAccounts;

                CheckDeletedAccounts(accounts);

                accounts.ForEach(x =>
                {
                    try
                    {
                        if (ChannelSelectModel.ListSelectDestination.All(y => y.AccountId != x.AccountId))
                        {
                            var channelDestinationSelectModel = new ChannelDestinationSelectModel
                            {
                                AccountId = x.AccountBaseModel.AccountId,
                                GroupName = x.AccountBaseModel.AccountGroup.Content,
                                AccountName = x.AccountBaseModel.UserName,
                                AccountStatus = x.AccountBaseModel.Status,
                                SelectedChannel = $" [{YdStatic.DefaultChannel}]", //selectedChannel,
                                ListOfChannel = new List<string>(), //listChannels,
                                TotalChannels = x.DisplayColumnValue1 ?? 0,
                                ChannelSelectorText = "NA",
                                AccountNickName = x.AccountBaseModel.AccountName
                            };

                            ChannelSelectModel.ListSelectDestination.Add(channelDestinationSelectModel);
                        }
                    }
                    catch (Exception)
                    {
                        //Ignored
                    }
                });

                DestinationCollectionView =
                    CollectionViewSource.GetDefaultView(ChannelSelectModel.ListSelectDestination);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

        #region Validate Destinations

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

        public void InitializeProperties()
        {
            Title = "Select Channel";
            IsAllDestinationSelected = false;
            ChannelSelectModel = ChannelSelectModel.DestinationDefaultBuilder();
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

        public async Task UpdateSyncStatusAsync(PublisherCreateDestinationSelectModel selectedSyncAccount)
        {
            var currentAccountDetails =
                ChannelSelectModel.ListSelectDestination.FirstOrDefault(x =>
                    x.AccountId == selectedSyncAccount.AccountId);

            if (currentAccountDetails == null)
                return;

            var accountsDetailsSelector = SocinatorInitialize
                .GetSocialLibrary(selectedSyncAccount.SocialNetworks)
                .GetNetworkCoreFactory().AccountDetailsSelectors;

            var currentGroups =
                await accountsDetailsSelector.GetGroupsUrls(selectedSyncAccount.AccountId,
                    selectedSyncAccount.AccountName);

            await accountsDetailsSelector.GetPageOrBoardUrls(selectedSyncAccount.AccountId,
                selectedSyncAccount.AccountName);

            currentAccountDetails.TotalChannels = currentGroups.Count;

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
            InitializeDestinationList();
        }

        #endregion

        #region CHannel list drop Down

        private bool ChannelListDropDownCanExecute(object sender)
        {
            return true;
        }

        private void ChannelListDropDownExecute(object sender)
        {
            try
            {
                var senderAccount = (ChannelDestinationSelectModel)((FrameworkElement)sender).DataContext;

                if (senderAccount.ListOfChannel.Count == senderAccount.TotalChannels)
                    return;

                var index = ChannelSelectModel.ListSelectDestination.FindIndex(x =>
                    x.AccountId == senderAccount.AccountId);
                var accountsDetailsSelector = SocinatorInitialize
                    .GetSocialLibrary(SocialNetworks.YouTube)
                    .GetNetworkCoreFactory().AccountDetailsSelectors;

                var account = GetYdSuccessAccounts.FirstOrDefault(x => x.AccountId == senderAccount.AccountId);
                if (account == null) return;

                var channels = accountsDetailsSelector
                    .GetPagesDetails(senderAccount.AccountId, senderAccount.AccountName, new List<string>()).Result;

                var selectedChannelPageId = account.AccountBaseModel.ProfileId;
                var listChannels = new List<string>();
                var selectedChannel = "";
                foreach (var accountDetailsSelectorModel in channels)
                {
                    if (string.IsNullOrEmpty(accountDetailsSelectorModel.DetailUrl))
                        accountDetailsSelectorModel.DetailUrl = YdStatic.DefaultChannel;
                    var channelNow =
                        $"{accountDetailsSelectorModel.DetailName} [{accountDetailsSelectorModel.DetailUrl}]";
                    if (listChannels.All(str => str != channelNow))
                        listChannels.Add(channelNow);

                    if (string.IsNullOrEmpty(selectedChannelPageId) ||
                        selectedChannelPageId == accountDetailsSelectorModel.DetailUrl)
                    {
                        selectedChannelPageId = accountDetailsSelectorModel.DetailUrl;
                        selectedChannel =
                            $"{accountDetailsSelectorModel.DetailName} [{accountDetailsSelectorModel.DetailUrl}]";
                    }
                }

                ChannelSelectModel.ListSelectDestination[index].SelectedChannel = selectedChannel;
                ChannelSelectModel.ListSelectDestination[index].ListOfChannel = listChannels;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels;
using YoutubeDominatorCore.YoutubeViewModel;

namespace YoutubeDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for SelectChannel.xaml
    /// </summary>
    public partial class SelectChannel
    {
        private static SelectChannel _instance;
        private SelectChannelViewModel _selectChannelViewModel = new SelectChannelViewModel();

        public SelectChannel()
        {
            InitializeComponent();
            ChannelSelect.DataContext = SelectChannelViewModel;
            //var savedAccounts = CommonServiceLocator.InstanceProvider.GetInstance<DominatorUIUtility.ViewModel.IAccountCollectionViewModel>()
            //    .BySocialNetwork(SocinatorInitialize.ActiveSocialNetwork);
            var savedAccounts = _selectChannelViewModel.GetYdSuccessAccounts;
            if (_selectChannelViewModel.ChannelSelectModel.ListSelectDestination.Count == 0)
                _selectChannelViewModel.InitializeDestinationList();
            savedAccounts.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x.AccountBaseModel.AccountGroup.Content) &&
                    !SelectChannelViewModel.Groups.Any(
                        group => group.Content == x.AccountBaseModel.AccountGroup.Content))
                    SelectChannelViewModel.Groups.Add(
                        new ContentSelectGroup
                        {
                            Content = x.AccountBaseModel.AccountGroup.Content
                        });
            });
        }

        public static SelectChannel Instance { get; set; } = _instance ?? (_instance = new SelectChannel());

        public SelectChannelViewModel SelectChannelViewModel
        {
            get => _selectChannelViewModel;
            set
            {
                _selectChannelViewModel = value;
                OnPropertyChanged(nameof(SelectChannelViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SelectChannel_OnLoaded(object sender, RoutedEventArgs e)
        {
            SelectChannelViewModel.Title = "LangKeySelectChannels".FromResourceDictionary();
            ChannelSelect.DataContext = SelectChannelViewModel;
        }

        private void SwitchChannel_OnDropDownOpened(object sender, EventArgs e)
        {
            try
            {
                var senderAccount = (ChannelDestinationSelectModel)((FrameworkElement)sender).DataContext;

                if (senderAccount.ListOfChannel.Count == senderAccount.TotalChannels)
                    return;

                var index = SelectChannelViewModel.ChannelSelectModel.ListSelectDestination.FindIndex(x =>
                    x.AccountId == senderAccount.AccountId);
                var accountsDetailsSelector = SocinatorInitialize
                    .GetSocialLibrary(SocialNetworks.YouTube)
                    .GetNetworkCoreFactory().AccountDetailsSelectors;

                var account =
                    SelectChannelViewModel.GetYdSuccessAccounts.FirstOrDefault(x =>
                        x.AccountId == senderAccount.AccountId);
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

                SelectChannelViewModel.ChannelSelectModel.ListSelectDestination[index].SelectedChannel =
                    selectedChannel;
                SelectChannelViewModel.ChannelSelectModel.ListSelectDestination[index].ListOfChannel = listChannels;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void chkgroup_Checked(object sender, RoutedEventArgs e)
        {
            var groups = SelectChannelViewModel.Groups.Where(x => x.IsContentSelected).ToList();
            SelectChannelViewModel.ChannelSelectModel.ListSelectDestination.ForEach(x =>
            {
                groups.ForEach(y =>
                {
                    if (x.GroupName == y.Content) x.IsAccountSelected = y.IsContentSelected;
                });
            });
        }

        private void chkgroup_Unchecked(object sender, RoutedEventArgs e)
        {
            var groups = SelectChannelViewModel.Groups.Where(x => !x.IsContentSelected).ToList();
            SelectChannelViewModel.ChannelSelectModel.ListSelectDestination.ForEach(x =>
            {
                groups.ForEach(y =>
                {
                    if (x.GroupName == y.Content) x.IsAccountSelected = y.IsContentSelected;
                });
            });
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterAccount();
        }

        private void FilterAccount()
        {
            try
            {
                if (SelectChannelViewModel.DestinationCollectionView != null)
                    switch (ComboBoxSearchFilter.SelectedIndex)
                {
                    case 0:
                        SelectChannelViewModel.DestinationCollectionView.Filter = FilterByGroupName;
                        break;
                    case 1:
                        SelectChannelViewModel.DestinationCollectionView.Filter = FilterByAccounts;
                        break;
                    case 2:
                        SelectChannelViewModel.DestinationCollectionView.Filter = FilterByAccountsNickName;
                        break;
                    default:
                        if (!string.IsNullOrEmpty(TextSearch.Text))
                            SelectChannelViewModel.DestinationCollectionView.Filter = FilterByAccounts;
                        else
                            SelectChannelViewModel.DestinationCollectionView.Filter = null;
                        break;
                }
            }
            catch
            {
            }
        }

        private bool FilterByGroupName(object groupName)
        {
            try
            {
                var objAccountViewModel = groupName as ChannelDestinationSelectModel;

                return objAccountViewModel != null &&
                       objAccountViewModel.GroupName.IndexOf(TextSearch.Text,
                           StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private bool FilterByAccounts(object accountName)
        {
            try
            {
                var objAccountViewModel = accountName as ChannelDestinationSelectModel;
                return objAccountViewModel != null &&
                       objAccountViewModel.AccountName.IndexOf(TextSearch.Text,
                           StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private bool FilterByAccountsNickName(object model)
        {
            try
            {
                var objAccountViewModel = model as ChannelDestinationSelectModel;
                return objAccountViewModel != null &&
                       objAccountViewModel.AccountNickName.IndexOf(TextSearch.Text,
                           StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private void Filter(object sender, SelectionChangedEventArgs e)
        {
            FilterAccount();
        }

        private void CmbAllGroups_OnDropDownOpened(object sender, EventArgs e)
        {
            var savedAccounts = _selectChannelViewModel.ChannelSelectModel.ListSelectDestination;
            var lastGroups = new ObservableCollection<ContentSelectGroup>();
            lastGroups.AddRange(SelectChannelViewModel.Groups);
            SelectChannelViewModel.Groups.Clear();
            savedAccounts.ForEach(x =>
            {
                //add only account status should be success

                if (!string.IsNullOrEmpty(x.GroupName) &&
                    !SelectChannelViewModel.Groups.Any(group => group.Content == x.GroupName))
                    SelectChannelViewModel.Groups.Add(
                        new ContentSelectGroup
                        {
                            IsContentSelected =
                                lastGroups.FirstOrDefault(y => y.Content == x.GroupName)?.IsContentSelected ?? false,
                            Content = x.GroupName
                        });
            });
        }
    }
}
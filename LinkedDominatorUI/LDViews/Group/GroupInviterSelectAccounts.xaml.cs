using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDViewModel.Group;
using LinkedDominatorUI.Utility;

namespace LinkedDominatorUI.LDViews.Group
{
    /// <summary>
    ///     Interaction logic for GroupInviterSelectAccounts.xaml
    /// </summary>
    public partial class GroupInviterSelectAccounts : UserControl, INotifyPropertyChanged
    {
        private static GroupInviterSelectAccounts _indexPage;

        private GroupInviterSelectAccountsViewModel _groupInviterSelectAccountsViewModel =
            new GroupInviterSelectAccountsViewModel();

        private GroupInviterViewModel _groupInviterViewModel;

        public GroupInviterSelectAccounts(GroupInviterViewModel groupInviterViewModel, string account = "")
        {
            InitializeComponent();
            CreateDestination.DataContext = GroupInviterSelectAccountsViewModel;
            GroupInviterViewModel = groupInviterViewModel;
            SelectedAccount = account;
        }

        public string SelectedAccount { get; set; }
        public static List<GroupQueryContent> ListQueries { get; set; } = new List<GroupQueryContent>();

        public GroupInviterSelectAccountsViewModel GroupInviterSelectAccountsViewModel
        {
            get => _groupInviterSelectAccountsViewModel;
            set
            {
                _groupInviterSelectAccountsViewModel = value;
                OnPropertyChanged(nameof(GroupInviterSelectAccountsViewModel));
            }
        }

        public GroupInviterViewModel GroupInviterViewModel
        {
            get => _groupInviterViewModel;
            set
            {
                _groupInviterViewModel = value;
                OnPropertyChanged(nameof(GroupInviterViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PublisherCreateDestination_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(GroupInviterSelectAccountsViewModel.EditDestinationId))
            {
                //PublisherCreateDestinationsViewModel.EditDestination();
                CreateDestination.DataContext = GroupInviterSelectAccountsViewModel;
            }
            else
            {
                GroupInviterSelectAccountsViewModel = new GroupInviterSelectAccountsViewModel();
                GroupInviterSelectAccountsViewModel.Title = "Select Group";
                CreateDestination.DataContext = GroupInviterSelectAccountsViewModel;
            }

            ListQueries = new List<GroupQueryContent>();
            if (ListQueries.All(x => x.Content != "All") && GroupInviterViewModel.Model.SavedQueries.Count > 0)
            {
                ListQueries.Add(new GroupQueryContent {Content = "All"});
                GroupInviterSelectAccountsViewModel.LstOfKeywords.Add(new GroupQueryContent {Content = "All"});
            }

            GroupInviterViewModel.Model.SavedQueries.ForEach(x =>
            {
                ListQueries.Add(new GroupQueryContent {Content = x.QueryType + " [" + x.QueryValue + "]"});
                GroupInviterSelectAccountsViewModel.LstOfKeywords.Add(new GroupQueryContent
                    {Content = x.QueryType + " [" + x.QueryValue + "]"});
            });

            if (GroupInviterViewModel.Model.AccountPagesBoardsPair != null)
            {
                GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel.AccountPagesBoardsPair =
                    new List<GroupSelectDestination>(GroupInviterViewModel.Model.AccountPagesBoardsPair);
                GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel
                    .ListSelectDestination = new ObservableCollection<GroupCreateDestinationSelectModel>
                    (GroupInviterViewModel.Model.ListSelectDestination);
            }

            if (GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel
                    .ListSelectDestination == null || GroupInviterSelectAccountsViewModel
                    .PublisherCreateDestinationModel
                    .ListSelectDestination.Count == 0)
                GetDestinationsForSelectAccounts();

            if (GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel
                    .ListSelectDestination != null)
                GroupInviterSelectAccountsViewModel.DestinationCollectionView =
                    CollectionViewSource.GetDefaultView(new ObservableCollection<GroupCreateDestinationSelectModel>(
                        GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel
                            .ListSelectDestination).Where(x =>
                        x.SocialNetworks == SocialNetworks.LinkedIn && x.TotalGroups > 0 &&
                        x.AccountStatus == AccountStatus.Success));

            if (GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel.AccountPagesBoardsPair != null
                && GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel.AccountPagesBoardsPair.Count !=
                0)
                GroupInviterSelectAccountsViewModel.DestinationCollectionView =
                    CollectionViewSource.GetDefaultView(GroupInviterViewModel.Model.ListSelectDestination.Where(x =>
                        x.SocialNetworks == SocialNetworks.LinkedIn && x.TotalGroups > 0 &&
                        x.AccountStatus == AccountStatus.Success));

            GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel
                .ListSelectDestination.ForEach(x =>
                {
                    x.PagesOrBoardsSelectorText =
                        GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel.AccountPagesBoardsPair
                            .Where(y => y.AccountId == x.AccountId).ToList().Count + "/" + x.TotalGroups;
                });

            GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel
                .ListSelectDestination.ForEach(x =>
                {
                    if (x.PagesOrBoardsSelectorText.Split('/')[0].Equals("0"))
                        x.IsAccountSelected = false;
                });

            if (GroupInviterViewModel.Model.IsSelectGroup && !string.IsNullOrEmpty(SelectedAccount))
            {
                GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel
                        .ListSelectDestination =
                    new ObservableCollection<GroupCreateDestinationSelectModel>(GroupInviterSelectAccountsViewModel
                        .PublisherCreateDestinationModel
                        .ListSelectDestination
                        .Where(x => x.AccountName.Equals(SelectedAccount) && x.TotalPagesOrBoards > 0));
                GroupInviterSelectAccountsViewModel.DestinationCollectionView =
                    CollectionViewSource.GetDefaultView(new ObservableCollection<GroupCreateDestinationSelectModel>(
                        GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel
                            .ListSelectDestination));
            }
        }

        public void GetDestinationsForSelectAccounts()
        {
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            //GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel.ListSelectDestination.Clear();
            var accounts = accountsFileManager.GetAll();
            accounts.ForEach(x =>
            {
                if (GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel.ListSelectDestination
                    .All(y => y.AccountId != x.AccountId))
                {
                    var publisherCreateDestinationSelectModel = new GroupCreateDestinationSelectModel
                    {
                        AccountId = x.AccountBaseModel.AccountId,
                        AccountName = x.AccountBaseModel.UserName,
                        SocialNetworks = x.AccountBaseModel.AccountNetwork,
                        AccountStatus = x.AccountBaseModel.Status,
                        IsGroupsAvailable = true,
                        IsPagesOrBoardsAvailable = true,
                        PublishonOwnWall = false,
                        SelectedGroups = 0,
                        TotalGroups = x.DisplayColumnValue2 ?? 0,
                        TotalPagesOrBoards = x.DisplayColumnValue3 ?? 0,
                        GroupSelectorText = "NA"
                    };

                    publisherCreateDestinationSelectModel.PagesOrBoardsSelectorText =
                        "0" + "/" + publisherCreateDestinationSelectModel.TotalPagesOrBoards;
                    if (SocinatorInitialize.IsNetworkAvailable(x.AccountBaseModel.AccountNetwork) &&
                        x.AccountBaseModel.Status == AccountStatus.Success)
                        GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel.ListSelectDestination.Add(
                            publisherCreateDestinationSelectModel);
                }
            });
        }

        public void InitializeDestinationList()
        {
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var accounts = accountsFileManager.GetAll();

            if (!Application.Current.CheckAccess())
                Application.Current.Dispatcher.Invoke(() =>
                {
                    GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel.ListSelectDestination
                        .Clear();
                });
            else
                GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel.ListSelectDestination.Clear();

            accounts.ForEach(x =>
            {
                var publisherCreateDestinationSelectModel = new GroupCreateDestinationSelectModel
                {
                    AccountId = x.AccountBaseModel.AccountId,
                    AccountName = x.AccountBaseModel.UserName,
                    SocialNetworks = x.AccountBaseModel.AccountNetwork,
                    AccountStatus = x.AccountBaseModel.Status,

                    IsGroupsAvailable = true,
                    IsPagesOrBoardsAvailable = true,
                    PublishonOwnWall = false,
                    SelectedGroups = 0,
                    TotalGroups = x.DisplayColumnValue2 ?? 0,
                    TotalPagesOrBoards = x.DisplayColumnValue3 ?? 0,
                    GroupSelectorText = "NA"
                };


                publisherCreateDestinationSelectModel.PagesOrBoardsSelectorText =
                    "0" + "/" + publisherCreateDestinationSelectModel.TotalPagesOrBoards;

                if (SocinatorInitialize.IsNetworkAvailable(x.AccountBaseModel.AccountNetwork))
                    GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel.ListSelectDestination.Add(
                        publisherCreateDestinationSelectModel);
            });

            if (string.IsNullOrEmpty(SelectedAccount))
                GroupInviterSelectAccountsViewModel.DestinationCollectionView =
                    CollectionViewSource.GetDefaultView(
                        GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel.ListSelectDestination.Where(
                            x => x.SocialNetworks == SocialNetworks.LinkedIn && x.TotalGroups > 0 &&
                                 x.AccountStatus == AccountStatus.Success));
            else
                GroupInviterSelectAccountsViewModel.DestinationCollectionView =
                    CollectionViewSource.GetDefaultView(
                        GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel.ListSelectDestination.Where(
                            x => x.SocialNetworks == SocialNetworks.LinkedIn && x.TotalGroups > 0 &&
                                 x.AccountStatus == AccountStatus.Success && x.AccountName.Equals(SelectedAccount)));
        }


        public void InitializeProperties()
        {
            GroupInviterSelectAccountsViewModel.Title = "Select Group";
            GroupInviterSelectAccountsViewModel.IsAllDestinationSelected = false;
            GroupInviterSelectAccountsViewModel.EditDestinationId = string.Empty;
            GroupInviterSelectAccountsViewModel.IsSavedDestination = false;
            GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel =
                GroupInviterSelectAccountsModel.DestinationDefaultBuilder();
        }


        public void BtnSaveEvent(object sender, EventArgs e)
        {
            try
            {
                GroupInviterViewModel.GroupInviterModel.ListOfGroupUrl.Clear();

                GroupInviterViewModel.GroupInviterModel.ListSelectDestination =
                    new ObservableCollection<GroupCreateDestinationSelectModel>
                    (GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel
                        .ListSelectDestination.Where(x => x.SocialNetworks == SocialNetworks.LinkedIn).ToList());

                GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel.AccountPagesBoardsPair.RemoveAll(
                    x =>
                        !GroupInviterViewModel.GroupInviterModel.ListSelectDestination
                            .Any(y => y.AccountId == x.AccountId && y.IsAccountSelected));

                GroupInviterViewModel.GroupInviterModel.AccountPagesBoardsPair = new List<GroupSelectDestination>(
                    GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel.AccountPagesBoardsPair);


                GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel
                    .AccountPagesBoardsPair
                    ?.ForEach(x =>
                        GroupInviterViewModel.GroupInviterModel.ListOfGroupUrl.Add(x.ListofGroupsofAccounts.Key));

                GroupInviterViewModel.GroupInviterModel.AccountPagesBoardsPair.ForEach(x =>
                {
                    x.IsSelected = GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel
                        .ListSelectDestination.FirstOrDefault(y => y.AccountId == x.AccountId).IsAccountSelected;
                });

                GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel
                    .ListSelectDestination.ForEach(x =>
                    {
                        if (x.PagesOrBoardsSelectorText.Split('/')[0].Equals("0"))
                            x.IsAccountSelected = false;
                    });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void BtnClearEvent(object sender, RoutedEventArgs e)
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

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckUnCheckQuery(sender, true);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckUnCheckQuery(sender, false);
        }

        private void ButtonAddFreshAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetDestinationsForSelectAccounts();
                if (!string.IsNullOrEmpty(SelectedAccount))
                    GroupInviterSelectAccountsViewModel.DestinationCollectionView =
                        CollectionViewSource.GetDefaultView(
                            GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel.ListSelectDestination
                                .Where(x => x.SocialNetworks == SocialNetworks.LinkedIn && x.TotalGroups > 0 &&
                                            x.AccountStatus == AccountStatus.Success &&
                                            x.AccountName.Equals(SelectedAccount)));
                else
                    GroupInviterSelectAccountsViewModel.DestinationCollectionView =
                        CollectionViewSource.GetDefaultView(
                            GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel.ListSelectDestination
                                .Where(x => x.SocialNetworks == SocialNetworks.LinkedIn && x.TotalGroups > 0 &&
                                            x.AccountStatus == AccountStatus.Success));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void CheckUnCheckQuery(object sender, bool isSelect)
        {
            var dataContext = (GroupQueryContent) ((FrameworkElement) sender).DataContext;

            try
            {
                if (isSelect)
                {
                    var noSExceptAll =
                        GroupInviterSelectAccountsViewModel.LstOfKeywords.Any(x =>
                            x.Content == "All" && x.IsContentSelected) &&
                        GroupInviterSelectAccountsViewModel.LstOfKeywords.Any(x => x.Content != "All" &&
                                                                                   !x.IsContentSelected);
                    var allSExceptAll =
                        GroupInviterSelectAccountsViewModel.LstOfKeywords
                            .Where(x => x.Content != "All" && x.IsContentSelected).Select(y => true).Count() ==
                        GroupInviterSelectAccountsViewModel.LstOfKeywords.Count - 1;

                    if (noSExceptAll)
                        GroupInviterSelectAccountsViewModel.LstOfKeywords.ForEach(x => { x.IsContentSelected = true; });
                    else if (allSExceptAll)
                        GroupInviterSelectAccountsViewModel.LstOfKeywords.FirstOrDefault(x => x.Content == "All")
                            .IsContentSelected = true;
                }
                else
                {
                    var dall = GroupInviterSelectAccountsViewModel.LstOfKeywords.Any(x =>
                                   x.Content == "All" && !x.IsContentSelected)
                               && GroupInviterSelectAccountsViewModel.LstOfKeywords.Any(x =>
                                   x.Content != "All" && x.IsContentSelected);
                    var allSExpOne = GroupInviterSelectAccountsViewModel.LstOfKeywords
                                         .Where(x => x.Content != "All" && !x.IsContentSelected)
                                         .Select(y => true).Count() == 1;
                    var allSExpAll = GroupInviterSelectAccountsViewModel.LstOfKeywords.Where(x => !x.IsContentSelected)
                                         .Select(y => true).Count() == 1;

                    if (dall && allSExpAll)
                    {
                        GroupInviterSelectAccountsViewModel.LstOfKeywords.ForEach(x =>
                        {
                            x.IsContentSelected = false;
                        });
                    }
                    else if (allSExpOne)
                    {
                        var groupInviterQueryContent =
                            GroupInviterSelectAccountsViewModel.LstOfKeywords.FirstOrDefault(x => x.Content == "All");
                        if (groupInviterQueryContent != null) groupInviterQueryContent.IsContentSelected = false;
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
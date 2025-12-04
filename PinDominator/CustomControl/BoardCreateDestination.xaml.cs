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
using PinDominator.ViewModel;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDViewModel.PinPoster;

namespace PinDominator.CustomControl
{
    /// <summary>
    ///     Interaction logic for PublisherCreateDestination.xaml
    /// </summary>
    public partial class BoardCreateDestination : INotifyPropertyChanged
    {
        private BoardCreateDestinationsViewModel _boardCreateDestinationsViewModel =
            new BoardCreateDestinationsViewModel();

        private RePinViewModel _rePinViewModel;

        public BoardCreateDestination(RePinViewModel repinViewModel, string account = "")
        {
            InitializeComponent();
            RePinViewModel = repinViewModel;
            CreateDestination.DataContext = BoardCreateDestinationsViewModel;
            SelectedAccount = account;
        }

        public static List<RepinQueryContent> ListQueries { get; set; } = new List<RepinQueryContent>();
        public string SelectedAccount { get; set; }

        public RePinViewModel RePinViewModel
        {
            get => _rePinViewModel;
            set
            {
                _rePinViewModel = value;
                OnPropertyChanged(nameof(RePinViewModel));
            }
        }

        public BoardCreateDestinationsViewModel BoardCreateDestinationsViewModel
        {
            get => _boardCreateDestinationsViewModel;
            set
            {
                _boardCreateDestinationsViewModel = value;
                OnPropertyChanged(nameof(BoardCreateDestinationsViewModel));
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
            //if (!BoardCreateDestinationsViewModel.IsSavedDestination)
            //    return;

            if (!string.IsNullOrEmpty(BoardCreateDestinationsViewModel.EditDestinationId))
            {
                //PublisherCreateDestinationsViewModel.EditDestination();
                CreateDestination.DataContext = BoardCreateDestinationsViewModel;
            }
            else
            {
                BoardCreateDestinationsViewModel = new BoardCreateDestinationsViewModel();
                BoardCreateDestinationsViewModel.Title = "Select Board";
                CreateDestination.DataContext = BoardCreateDestinationsViewModel;
            }

            ListQueries = new List<RepinQueryContent>();
            if (ListQueries.All(x => x.Content != "All") && RePinViewModel.Model.SavedQueries.Count > 0)
            {
                ListQueries.Add(new RepinQueryContent {Content = "All"});
                BoardCreateDestinationsViewModel.LstOfKeywords.Add(new RepinQueryContent {Content = "All"});
            }

            RePinViewModel.Model.SavedQueries.ForEach(x =>
            {
                ListQueries.Add(new RepinQueryContent {Content = x.QueryType + " [" + x.QueryValue + "]"});
                BoardCreateDestinationsViewModel.LstOfKeywords.Add(new RepinQueryContent
                    {Content = x.QueryType + " [" + x.QueryValue + "]"});
            });

            //To update account status and board count while selecting account in campaign
            if (RePinViewModel.Model.ListSelectDestination.Count != 0)
                BoardCreateDestinationsViewModel.PublisherCreateDestinationModel
                    .ListSelectDestination.ForEach(x =>
                    {
                        var accountFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                        var lstAccount = accountFileManager.GetAll(SocialNetworks.Pinterest);

                        RePinViewModel.Model.ListSelectDestination.FirstOrDefault(y => x.AccountId == y.AccountId)
                            .AccountStatus = x.AccountStatus;
                        RePinViewModel.Model.ListSelectDestination.FirstOrDefault(y => x.AccountId == y.AccountId)
                            .TotalPagesOrBoards = x.TotalPagesOrBoards;

                        RePinViewModel.Model.ListSelectDestination.Remove(
                            RePinViewModel.Model.ListSelectDestination.FirstOrDefault(y =>
                                !lstAccount.Any(z => z.AccountId == y.AccountId)));
                    });
            if (RePinViewModel.Model.AccountPagesBoardsPair != null)
            {
                BoardCreateDestinationsViewModel.PublisherCreateDestinationModel.AccountPagesBoardsPair =
                    new List<RepinSelectDestination>(RePinViewModel.Model.AccountPagesBoardsPair);
                BoardCreateDestinationsViewModel.PublisherCreateDestinationModel
                    .ListSelectDestination = new ObservableCollection<RepinCreateDestinationSelectModel>
                    (RePinViewModel.Model.ListSelectDestination);
            }

            if (BoardCreateDestinationsViewModel.PublisherCreateDestinationModel
                    .ListSelectDestination == null || BoardCreateDestinationsViewModel.PublisherCreateDestinationModel
                    .ListSelectDestination.Count == 0)
                GetDestinationsForSelectAccounts();

            if (BoardCreateDestinationsViewModel.PublisherCreateDestinationModel
                    .ListSelectDestination != null)
                BoardCreateDestinationsViewModel.DestinationCollectionView =
                    CollectionViewSource.GetDefaultView(new ObservableCollection<RepinCreateDestinationSelectModel>(
                        BoardCreateDestinationsViewModel.PublisherCreateDestinationModel
                            .ListSelectDestination).Where(x =>
                        x.SocialNetworks == SocialNetworks.Pinterest && x.TotalPagesOrBoards > 0 &&
                        x.AccountStatus == AccountStatus.Success));

            if (BoardCreateDestinationsViewModel.PublisherCreateDestinationModel.AccountPagesBoardsPair != null
                && BoardCreateDestinationsViewModel.PublisherCreateDestinationModel.AccountPagesBoardsPair.Count != 0)
                BoardCreateDestinationsViewModel.DestinationCollectionView =
                    CollectionViewSource.GetDefaultView(RePinViewModel.Model.ListSelectDestination.Where(x =>
                        x.SocialNetworks == SocialNetworks.Pinterest && x.TotalPagesOrBoards > 0 &&
                        x.AccountStatus == AccountStatus.Success));

            BoardCreateDestinationsViewModel.PublisherCreateDestinationModel
                .ListSelectDestination.ForEach(x =>
                {
                    x.PagesOrBoardsSelectorText =
                        BoardCreateDestinationsViewModel.PublisherCreateDestinationModel.AccountPagesBoardsPair
                            .Where(y => y.AccountId == x.AccountId).ToList().Count + "/" + x.TotalPagesOrBoards;
                });

            BoardCreateDestinationsViewModel.PublisherCreateDestinationModel
                .ListSelectDestination.ForEach(x =>
                {
                    if (x.PagesOrBoardsSelectorText.Split('/')[0].Equals("0"))
                        x.IsAccountSelected = false;
                });

            if (RePinViewModel.Model.IsSelectPinsFromBoard && !string.IsNullOrEmpty(SelectedAccount))
            {
                BoardCreateDestinationsViewModel.PublisherCreateDestinationModel
                        .ListSelectDestination =
                    new ObservableCollection<RepinCreateDestinationSelectModel>(BoardCreateDestinationsViewModel
                        .PublisherCreateDestinationModel
                        .ListSelectDestination
                        .Where(x => x.AccountName.Equals(SelectedAccount) && x.TotalPagesOrBoards > 0));
                BoardCreateDestinationsViewModel.DestinationCollectionView =
                    CollectionViewSource.GetDefaultView(new ObservableCollection<RepinCreateDestinationSelectModel>(
                        BoardCreateDestinationsViewModel.PublisherCreateDestinationModel
                            .ListSelectDestination));
            }
        }

        public void GetDestinationsForSelectAccounts()
        {
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            //BoardCreateDestinationsViewModel.PublisherCreateDestinationModel.ListSelectDestination.Clear();
            var accounts = accountsFileManager.GetAll(SocialNetworks.Pinterest);
            accounts.ForEach(x =>
            {
                if (BoardCreateDestinationsViewModel.PublisherCreateDestinationModel.ListSelectDestination
                    .All(y => y.AccountId != x.AccountId))
                {
                    var publisherCreateDestinationSelectModel = new RepinCreateDestinationSelectModel
                    {
                        AccountId = x.AccountBaseModel.AccountId,
                        AccountName = x.AccountBaseModel.UserName,
                        SocialNetworks = x.AccountBaseModel.AccountNetwork,
                        AccountStatus = x.AccountBaseModel.Status,
                        IsGroupsAvailable = false,
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
                        BoardCreateDestinationsViewModel.PublisherCreateDestinationModel.ListSelectDestination.Add(
                            publisherCreateDestinationSelectModel);
                }
            });
        }

        public void BtnSaveEvent(object sender, EventArgs e)
        {
            try
            {
                RePinViewModel.RePinModel.ListOfBoardUrl.Clear();

                RePinViewModel.RePinModel.ListSelectDestination =
                    new ObservableCollection<RepinCreateDestinationSelectModel>
                    (BoardCreateDestinationsViewModel.PublisherCreateDestinationModel
                        .ListSelectDestination.Where(x => x.SocialNetworks == SocialNetworks.Pinterest).ToList());

                BoardCreateDestinationsViewModel.PublisherCreateDestinationModel.AccountPagesBoardsPair.RemoveAll(x =>
                    !RePinViewModel.RePinModel.ListSelectDestination
                        .Any(y => y.AccountId == x.AccountId && y.IsAccountSelected));

                RePinViewModel.RePinModel.AccountPagesBoardsPair = new List<RepinSelectDestination>(
                    BoardCreateDestinationsViewModel.PublisherCreateDestinationModel.AccountPagesBoardsPair);


                BoardCreateDestinationsViewModel.PublisherCreateDestinationModel
                    .AccountPagesBoardsPair
                    ?.ForEach(x => RePinViewModel.RePinModel.ListOfBoardUrl.Add(x.LstofPinsToRepin.Key));

                RePinViewModel.RePinModel.AccountPagesBoardsPair.ForEach(x =>
                {
                    x.IsSelected = BoardCreateDestinationsViewModel.PublisherCreateDestinationModel
                        .ListSelectDestination.FirstOrDefault(y => y.AccountId == x.AccountId).IsAccountSelected;
                });

                BoardCreateDestinationsViewModel.PublisherCreateDestinationModel
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


        public void InitializeDestinationList()
        {
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var accounts = accountsFileManager.GetAll();

            if (!Application.Current.CheckAccess())
                Application.Current.Dispatcher.Invoke(() =>
                {
                    BoardCreateDestinationsViewModel.PublisherCreateDestinationModel.ListSelectDestination.Clear();
                });
            else
                BoardCreateDestinationsViewModel.PublisherCreateDestinationModel.ListSelectDestination.Clear();

            accounts.ForEach(x =>
            {
                var publisherCreateDestinationSelectModel = new RepinCreateDestinationSelectModel
                {
                    AccountId = x.AccountBaseModel.AccountId,
                    AccountName = x.AccountBaseModel.UserName,
                    SocialNetworks = x.AccountBaseModel.AccountNetwork,
                    AccountStatus = x.AccountBaseModel.Status,
                    //IsOwnWallAvailable = x.AccountBaseModel.AccountNetwork != SocialNetworks.Pinterest,
                    IsGroupsAvailable = false,
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
                    BoardCreateDestinationsViewModel.PublisherCreateDestinationModel.ListSelectDestination.Add(
                        publisherCreateDestinationSelectModel);
            });

            if (string.IsNullOrEmpty(SelectedAccount))
                BoardCreateDestinationsViewModel.DestinationCollectionView =
                    CollectionViewSource.GetDefaultView(
                        BoardCreateDestinationsViewModel.PublisherCreateDestinationModel.ListSelectDestination.Where(
                            x => x.SocialNetworks == SocialNetworks.Pinterest && x.TotalPagesOrBoards > 0 &&
                                 x.AccountStatus == AccountStatus.Success));
            else
                BoardCreateDestinationsViewModel.DestinationCollectionView =
                    CollectionViewSource.GetDefaultView(
                        BoardCreateDestinationsViewModel.PublisherCreateDestinationModel.ListSelectDestination.Where(
                            x => x.SocialNetworks == SocialNetworks.Pinterest && x.TotalPagesOrBoards > 0 &&
                                 x.AccountStatus == AccountStatus.Success && x.AccountName.Equals(SelectedAccount)));
        }

        public void InitializeProperties()
        {
            BoardCreateDestinationsViewModel.Title = "Select Board";
            BoardCreateDestinationsViewModel.IsAllDestinationSelected = false;
            BoardCreateDestinationsViewModel.EditDestinationId = string.Empty;
            BoardCreateDestinationsViewModel.IsSavedDestination = false;
            BoardCreateDestinationsViewModel.PublisherCreateDestinationModel =
                BoardCreateDestinationModel.DestinationDefaultBuilder();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckUnCheckQuery(sender, true);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckUnCheckQuery(sender, false);
        }

        private void CheckUnCheckQuery(object sender, bool isSelect)
        {
            try
            {
                if (isSelect)
                {
                    var noSExceptAll =
                        BoardCreateDestinationsViewModel.LstOfKeywords.Any(x =>
                            x.Content == "All" && x.IsContentSelected) &&
                        BoardCreateDestinationsViewModel.LstOfKeywords.Any(x => x.Content != "All" &&
                                                                                !x.IsContentSelected);
                    var allSExceptAll =
                        BoardCreateDestinationsViewModel.LstOfKeywords
                            .Where(x => x.Content != "All" && x.IsContentSelected).Select(y => true).Count() ==
                        BoardCreateDestinationsViewModel.LstOfKeywords.Count - 1;

                    if (noSExceptAll)
                        BoardCreateDestinationsViewModel.LstOfKeywords.ForEach(x => { x.IsContentSelected = true; });
                    else if (allSExceptAll)
                        BoardCreateDestinationsViewModel.LstOfKeywords.FirstOrDefault(x => x.Content == "All")
                            .IsContentSelected = true;
                }
                else
                {
                    var dall = BoardCreateDestinationsViewModel.LstOfKeywords.Any(x =>
                                   x.Content == "All" && !x.IsContentSelected)
                               && BoardCreateDestinationsViewModel.LstOfKeywords.Any(x =>
                                   x.Content != "All" && x.IsContentSelected);
                    var allSExpOne = BoardCreateDestinationsViewModel.LstOfKeywords
                                         .Where(x => x.Content != "All" && !x.IsContentSelected)
                                         .Select(y => true).Count() == 1;
                    var allSExpAll = BoardCreateDestinationsViewModel.LstOfKeywords.Where(x => !x.IsContentSelected)
                                         .Select(y => true).Count() == 1;

                    if (dall && allSExpAll)
                    {
                        BoardCreateDestinationsViewModel.LstOfKeywords.ForEach(x => { x.IsContentSelected = false; });
                    }
                    else if (allSExpOne)
                    {
                        var repinQueryContent =
                            BoardCreateDestinationsViewModel.LstOfKeywords.FirstOrDefault(x => x.Content == "All");
                        if (repinQueryContent != null) repinQueryContent.IsContentSelected = false;
                    }
                }
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

        private void ButtonAddFreshAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetDestinationsForSelectAccounts();
                if (!string.IsNullOrEmpty(SelectedAccount))
                    BoardCreateDestinationsViewModel.DestinationCollectionView =
                        CollectionViewSource.GetDefaultView(
                            BoardCreateDestinationsViewModel.PublisherCreateDestinationModel.ListSelectDestination
                                .Where(x => x.SocialNetworks == SocialNetworks.Pinterest && x.TotalPagesOrBoards > 0 &&
                                            x.AccountStatus == AccountStatus.Success &&
                                            x.AccountName.Equals(SelectedAccount)));
                else
                    BoardCreateDestinationsViewModel.DestinationCollectionView =
                        CollectionViewSource.GetDefaultView(
                            BoardCreateDestinationsViewModel.PublisherCreateDestinationModel.ListSelectDestination
                                .Where(x => x.SocialNetworks == SocialNetworks.Pinterest && x.TotalPagesOrBoards > 0 &&
                                            x.AccountStatus == AccountStatus.Success));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
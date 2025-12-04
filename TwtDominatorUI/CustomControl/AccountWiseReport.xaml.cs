using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.Helper;
using TwtDominatorCore.Report;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for AccountWiseReport.xaml
    /// </summary>
    public partial class AccountWiseReport : UserControl
    {
        public AccountWiseReport(ActivityType activityType)
        {
            _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            InitializeComponent();
            try
            {
                ActivityType = activityType;

                var accounts = new ObservableCollectionBase<string>(_accountsFileManager.GetAll().Where(x =>
                    x.AccountBaseModel.AccountNetwork == SocialNetworks.Twitter
                    && x.AccountBaseModel.Status == AccountStatus.Success).Select(x => x.UserName));
                CmbAccounts.SelectedIndex = -1;
                CmbAccounts.ItemsSource = accounts;

                var currentAccount = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Twitter)
                    .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;

                CmbAccounts.SelectedItem = string.IsNullOrEmpty(currentAccount) ? accounts[0] : currentAccount;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void OnRefresh_Button_Clicked(object sender, RoutedEventArgs e)
        {
            //  endDate = DateTime.Now.AddDays(5000);
            GenerateReport();
        }


        private void CmbAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SelectedUserName =
                    SocinatorInitialize.GetSocialLibrary(SocialNetworks.Twitter).GetNetworkCoreFactory()
                        .AccountUserControlTools.RecentlySelectedAccount = CmbAccounts.SelectedItem.ToString();

                var account = _accountsFileManager.GetAccount(SelectedUserName, SocialNetworks.Twitter);
                var accountId = account.AccountId;
                var dbAccountService = InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(account);

                IList reportDetails = null;
                // dboperation = new DbOperations(AccountIdSelected, SocialNetworks.Twitter, ConstantVariable.GetAccountDb);
                reportDetails = TDInitialise.GetModuleLibrary(ActivityType).TDUtilityFactory().TDReportFactory
                    .GetsAccountReport(dbAccountService);
                ReportGrid.ItemsSource = reportDetails;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void RefreshDatagrid()
        {
            AccountsInteractedUsers.Clear();
            FilteredTweet.Clear();
            FilteredUnfollowList.Clear();
        }

        private void StartDate_SelectionChanged(object sender, TimePickerBaseSelectionChangedEventArgs<DateTime?> e)
        {
            startDate = (DateTime) e.NewValue;
            RefreshDatagrid();
        }

        private void EndDate_SelectionChanged(object sender, TimePickerBaseSelectionChangedEventArgs<DateTime?> e)
        {
            endDate = (DateTime) e.NewValue;
            RefreshDatagrid();
        }

        private void GenerateColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = Regex.Replace(e.Column.Header.ToString(), "(\\B[A-Z])", " $1");
            e.Column.IsReadOnly = false;
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //FileUtilities.GetExportPath();
                var SelectedFolder = FileUtilities.GetExportPath();
                if (!string.IsNullOrEmpty(SelectedFolder))
                {
                    var ExportFileName =
                        $"{SelectedFolder}\\{ActivityType}Report-{SelectedUserName}[{DateTimeUtilities.GetEpochTime()}].csv";
                    TDInitialise.GetModuleLibrary(ActivityType).TDUtilityFactory().TDReportFactory
                        .ExportReports(ExportFileName, ReportType.Account);
                    GlobusLogHelper.log.Info(
                        $"TwtDominator: [Account : {SelectedUserName}] => Reports exported successfully !!!");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void Button_ClickDeleteAll(object sender, RoutedEventArgs e)
        {
            if (ReportGrid.Items.Count == 0 && FilteredUnfollowList.Count == 0 && FilteredTweet.Count == 0)
            {
                Dialog.ShowDialog("Error",
                    "Empty records can not be deleted");
                return;
            }

            var dialogResult = Dialog.ShowCustomDialog(
                "Confirmation", "It will delete all records from database permanently \nAre you sure ?",
                "Delete Anyways", "Don't delete");
            if (dialogResult != MessageDialogResult.Affirmative)
                return;
            Task.Factory.StartNew(DeleteAll);
            GlobusLogHelper.log.Info(Log.CustomMessage, "Twitter", SelectedUserName, "",
                "Please wait it will take some time");
        }

        private void DeleteAll()
        {
            var account = _accountsFileManager.GetAccount(SelectedUserName, SocialNetworks.Twitter);
            var dbAccountService = InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(account);
            var activity = ActivityType.ToString();
            if (ActivityType == ActivityType.Unfollow)
                dbAccountService.RemoveMatch<UnfollowedUsers>(x => true);
            else if (TwtStringHelper.IsUserProcessorAccountReport(ActivityType))
                dbAccountService.RemoveMatch<InteractedUsers>(x => x.ActivityType == activity);
            else
                dbAccountService.RemoveMatch<InteractedPosts>(x => x.ActivityType == activity);


            Dispatcher.Invoke(() => GenerateReport());

            GlobusLogHelper.log.Info(Log.CustomMessage, "Twitter", SelectedUserName, "", "Selected records deleted");
        }

        private void AccountWiseReport_Loaded(object sender, RoutedEventArgs e)
        {
            CmbAccounts.SelectedItem = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Twitter)
                .GetNetworkCoreFactory().AccountUserControlTools
                .RecentlySelectedAccount; // SelectedDominatorAccounts.TdAccounts;
        }

        /// <summary>
        /// </summary>
        private async void GenerateReport()
        {
            try
            {
                var account = _accountsFileManager.GetAccount(SelectedUserName, SocialNetworks.Twitter);
                var accountId = account.AccountId;
                var dbAccountService = InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(account);

                IList reportDetails = null;
                reportDetails = TDInitialise.GetModuleLibrary(ActivityType).TDUtilityFactory().TDReportFactory
                    .GetsAccountReport(dbAccountService);
                await Task.Run(() => { Dispatcher.Invoke(() => { ReportGrid.ItemsSource = reportDetails; }); },
                    new CancellationToken());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region  Class fields

        private readonly IAccountsFileManager _accountsFileManager;
        private DbOperations dboperation;
        private List<InteractedUserReport> InteractedUserList = new List<InteractedUserReport>();

        private readonly ObservableCollection<InteractedUserReport> AccountsInteractedUsers =
            new ObservableCollection<InteractedUserReport>();

        private List<InteractedTweetReport> InteractedTweetList = new List<InteractedTweetReport>();

        private readonly ObservableCollection<InteractedTweetReport> FilteredTweet =
            new ObservableCollection<InteractedTweetReport>();

        private List<UnfollowReport> UnfollowedList = new List<UnfollowReport>();

        private readonly ObservableCollection<UnfollowReport> FilteredUnfollowList =
            new ObservableCollection<UnfollowReport>();

        private string AccountIdSelected = string.Empty;
        private string SelectedUserName;
        private DateTime startDate = new DateTime(2000, 01, 01);
        private DateTime endDate = DateTime.Now.AddDays(5000);
        private readonly ActivityType ActivityType;
        private string CsvHeader;

        #endregion
    }
}
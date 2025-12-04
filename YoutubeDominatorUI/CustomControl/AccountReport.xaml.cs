using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.DatabaseHandler.YdTables.Accounts;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.DAL;

namespace YoutubeDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for AccountReport.xaml
    /// </summary>
    public partial class AccountReport
    {
        public AccountReport()
        {
            InitializeComponent();
        }

        public AccountReport(ActivityType activityType) : this()
        {
            ActivityType = activityType;

            AccountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var accounts = new ObservableCollectionBase<string>(AccountsFileManager.GetAll()
                .Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.YouTube &&
                            x.AccountBaseModel.Status == AccountStatus.Success).Select(x => x.UserName));
            CmbAccounts.SelectedIndex = -1;
            CmbAccounts.ItemsSource = accounts;
            CmbAccounts.SelectedItem =
                string.IsNullOrEmpty(SocinatorInitialize.GetSocialLibrary(SocialNetworks.YouTube)
                    .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount)
                    ? !string.IsNullOrEmpty(accounts[0]) ? accounts[0] : ""
                    : SocinatorInitialize.GetSocialLibrary(SocialNetworks.YouTube).GetNetworkCoreFactory()
                        .AccountUserControlTools.RecentlySelectedAccount;
            SocinatorInitialize.GetSocialLibrary(SocialNetworks.YouTube).GetNetworkCoreFactory().AccountUserControlTools
                    .RecentlySelectedAccount =
                string.IsNullOrEmpty(SocinatorInitialize.GetSocialLibrary(SocialNetworks.YouTube)
                    .GetNetworkCoreFactory()
                    .AccountUserControlTools.RecentlySelectedAccount)
                    ? CmbAccounts.SelectedItem.ToString()
                    : SocinatorInitialize.GetSocialLibrary(SocialNetworks.YouTube).GetNetworkCoreFactory()
                        .AccountUserControlTools.RecentlySelectedAccount;
        }

        private ActivityType ActivityType { get; }
        private IAccountsFileManager AccountsFileManager { get; }

        private IList ReportDetails { get; set; }

        private void BtnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var activity = ActivityType.ToString().ToLower();
                var selectedAccount = CmbAccounts.SelectedItem.ToString();
                var dataCount = ReportDetails.Count;
                if (dataCount == 0)
                {
                    ToasterNotification.ShowInfomation("Found no data to delete.");
                    return;
                }

                ToasterNotification.ShowInfomation("Deleting data from report");

                var accountId = InstanceProvider.GetInstance<IAccountsFileManager>().GetAll()
                    .FirstOrDefault(x => x.AccountBaseModel.UserName == CmbAccounts.SelectedItem.ToString())
                    ?.AccountId;
                var dataBase = new DbOperations(accountId, SocialNetworks.YouTube, ConstantVariable.GetAccountDb);

                if (ActivityType == ActivityType.Comment || ActivityType == ActivityType.Dislike ||
                    ActivityType == ActivityType.LikeComment
                    || ActivityType == ActivityType.Like || ActivityType == ActivityType.PostScraper ||
                    ActivityType == ActivityType.ViewIncreaser)
                    dataBase.RemoveMatch<InteractedPosts>(x =>
                        x.ActivityType.ToLower() == activity && x.AccountUsername == selectedAccount);
                else if (ActivityType == ActivityType.ChannelScraper || ActivityType == ActivityType.Subscribe
                                                                     || ActivityType == ActivityType.UnSubscribe)
                    dataBase.RemoveMatch<InteractedChannels>(x =>
                        x.ActivityType.ToLower() == activity && x.AccountUsername == selectedAccount);
                ReportDetails.Clear();
                ReportGrid.ItemsSource = null;
                ReportGrid.ItemsSource = ReportDetails;

                ToasterNotification.ShowSuccess($"Deleted {dataCount} data.");
            }
            catch (Exception ex)
            {
                ToasterNotification.ShowError("Oops! An error occured on deleting report.");
                ex.DebugLog();
            }
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var exportPath = FileUtilities.GetExportPath();

                if (string.IsNullOrEmpty(exportPath))
                    return;

                var filename = Regex.Replace(
                    $"{CmbAccounts.SelectedItem}_{ActivityType}-Reports[{DateTime.Now:ddMMyyyyHmmss}]", //ConstantVariable.DateasFileName
                    "[\\/:*?<>|\"]",
                    "-");

                filename = $"{exportPath}\\{filename}.csv";

                YoutubeInitialize.GetModuleLibrary(ActivityType).YdUtilityFactory().YdReportFactory
                    .ExportReports(ReportType.Account, filename);

                Dialog.ShowDialog(Application.Current.MainWindow, "Sucess",
                    "Sucessfully Exported to " + filename);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                Dialog.ShowDialog(Application.Current.MainWindow, "Fail",
                    "Export fail !!");
            }
        }

        private void CmbAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SocinatorInitialize.GetSocialLibrary(SocialNetworks.YouTube).GetNetworkCoreFactory().AccountUserControlTools
                .RecentlySelectedAccount = CmbAccounts.SelectedItem.ToString();

            GenerateReports();
        }

        private void GenerateReports(bool isRefresh = false)
        {
            var account = AccountsFileManager
                .GetAccount(CmbAccounts.SelectedItem.ToString(), SocialNetworks.YouTube);

            var dbGrowthOperations = InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(account);

            if (isRefresh)
                ToasterNotification.ShowInfomation("Refreshing the report");

            Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (ReportDetails != null)
                        ReportGrid.ItemsSource = null;

                    ReportDetails = YoutubeInitialize.GetModuleLibrary(ActivityType).YdUtilityFactory().YdReportFactory
                        .GetsAccountReport(dbGrowthOperations);

                    ReportGrid.ItemsSource = ReportDetails;
                    if (isRefresh)
                        ToasterNotification.ShowSuccess($"Refreshed the report\nGot {ReportDetails.Count} Data. ");
                });
            });
        }

        private void ReportGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = Regex.Replace(e.Column.Header.ToString(), "(\\B[A-Z])", " $1");
        }

        private void AccountReport_Loaded(object sender, RoutedEventArgs e)
        {
            var accounts = new ObservableCollectionBase<string>(AccountsFileManager.GetAll()
                .Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.YouTube &&
                            x.AccountBaseModel.Status == AccountStatus.Success).Select(x => x.UserName));
            CmbAccounts.SelectedItem =
                string.IsNullOrEmpty(SocinatorInitialize.GetSocialLibrary(SocialNetworks.YouTube)
                    .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount)
                    ? !string.IsNullOrEmpty(accounts[0]) ? accounts[0] : ""
                    : SocinatorInitialize.GetSocialLibrary(SocialNetworks.YouTube).GetNetworkCoreFactory()
                        .AccountUserControlTools.RecentlySelectedAccount;
        }

        private void OnRefresh_Button_Clicked(object sender, RoutedEventArgs e)
        {
            GenerateReports(true);
        }
    }
}
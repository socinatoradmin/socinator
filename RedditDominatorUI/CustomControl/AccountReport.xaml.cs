using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.Utility;
using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RedditDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for AccountReport.xaml
    /// </summary>
    public partial class AccountReport
    {
        private readonly IAccountsFileManager _accountsFileManager;

        //private List<UnfollowedUsersReport> LstUnfollowedUser { get; set; }        
        //private List<InteractedPostReportDetails> LstInteractedPost { get; set; }        
        //private List<InteractedUsersReportDetails> LstInteractedUser { get; set; }        
        //private string _csvHeader = string.Empty;        
        //private List<string> CsvData { get; set; }

        public AccountReport()
        {
            InitializeComponent();
        }

        public AccountReport(ActivityType activityType) : this()
        {
            ActivityType = activityType;

            _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var accounts = new ObservableCollectionBase<string>(_accountsFileManager.GetAll()
                .Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Reddit &&
                            x.AccountBaseModel.Status == AccountStatus.Success).Select(x => x.UserName));
            CmbAccounts.SelectedIndex = -1;
            CmbAccounts.ItemsSource = accounts;
            var selectedaccount = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Reddit).GetNetworkCoreFactory()
                .AccountUserControlTools.RecentlySelectedAccount;
            CmbAccounts.SelectedItem = string.IsNullOrEmpty(selectedaccount)
                ? !string.IsNullOrEmpty(accounts[0]) ? accounts[0] : ""
                : selectedaccount;
            SocinatorInitialize.GetSocialLibrary(SocialNetworks.Reddit).GetNetworkCoreFactory().AccountUserControlTools
                .RecentlySelectedAccount = string.IsNullOrEmpty(selectedaccount)
                ? CmbAccounts.SelectedItem.ToString()
                : selectedaccount;
        }

        private ActivityType ActivityType { get; }

        private void BtnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var account = _accountsFileManager.GetAll().FirstOrDefault(x =>
                    x.AccountBaseModel.UserName == CmbAccounts.SelectedItem.ToString());
                var dbAccountService = InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(account);
                var reportDetails = RedditInitialize.GetModuleLibrary(ActivityType).RdUtilityFactory().RdReportFactory
                    .GetAccountReport(dbAccountService);

                var selectedAccount = CmbAccounts.SelectedItem.ToString();
                var activityTypeString = ActivityType.ToString();
                var accountId = _accountsFileManager.GetAll()
                    .FirstOrDefault(x => x.AccountBaseModel.UserName == CmbAccounts.SelectedItem.ToString())
                    ?.AccountId;
                if (reportDetails.Count != 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                        account.AccountBaseModel.UserName, ActivityType,
                        "Please wait data is deleting ...");
                    ToasterNotification.ShowSuccess("Please wait data is deleting ...");

                    Task.Factory.StartNew(() =>
                    {
                        if (ActivityType == ActivityType.Comment || ActivityType == ActivityType.CommentScraper ||
                            ActivityType == ActivityType.Downvote || ActivityType == ActivityType.RemoveVote ||
                            ActivityType == ActivityType.Reply || ActivityType == ActivityType.Upvote ||
                            ActivityType == ActivityType.UrlScraper || ActivityType == ActivityType.DownvoteComment ||
                            ActivityType == ActivityType.RemoveVoteComment ||
                            ActivityType == ActivityType.UpvoteComment)
                            dbAccountService.RemoveMatch<InteractedPost>(x => x.ActivityType == activityTypeString);

                        else if (ActivityType == ActivityType.ChannelScraper ||
                                 ActivityType == ActivityType.Subscribe ||
                                 ActivityType == ActivityType.UnSubscribe)
                            dbAccountService.RemoveMatch<InteractedSubreddit>(x =>
                                x.ActivityType == activityTypeString);

                        else if (ActivityType == ActivityType.Follow ||
                                 ActivityType == ActivityType.BroadcastMessages ||
                                 ActivityType == ActivityType.UserScraper)
                            dbAccountService.RemoveMatch<InteractedUsers>(x => x.ActivityType == activityTypeString);

                        else if (ActivityType == ActivityType.Unfollow)
                            dbAccountService.RemoveAll<UnfollowedUsers>();

                        GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                            account.AccountBaseModel.UserName, ActivityType,
                            "All data is deleted successfully in this Report");

                        GenerateReports();
                    });
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                        account.AccountBaseModel.UserName, ActivityType,
                        "Data is not available to delete");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            var exportPath = FileUtilities.GetExportPath();

            if (string.IsNullOrEmpty(exportPath))
                return;

            var filename = Regex.Replace(
                $"{CmbAccounts.SelectedItem}_{ActivityType}-Reports[{DateTime.Now:ddMMyyyyHmmss}]", //ConstantVariable.DateasFileName
                "[\\/:*?<>|\"]",
                "-");

            filename = $"{exportPath}\\{filename}.csv";
            RedditInitialize.GetModuleLibrary(ActivityType).RdUtilityFactory().RdReportFactory
                .ExportReports(ActivityType, filename, ReportType.Account);
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            //var openFileDialog = new Microsoft.Win32.OpenFileDialog
            //{
            //    Multiselect = false,
            //    Filter = "Database file (.db)|*.db|CSV file (.csv)|*.csv"
            //};
            //if (openFileDialog.ShowDialog() == true)
            //{
            //    string filePath = openFileDialog.FileName;

            //    //if (Path.GetExtension(filePath) == ".db")
            //    //{
            //    //    var CampaignId = Utilities.GetBetween(filePath, "DB\\", ".db");
            //    //    // GetReport(CampaignId);
            //    //}
            //    //else//if csv file
            //    //{
            //        var reportContent = FileUtilities.GetFileContent(filePath);
            //        reportContent = reportContent?.Skip(1).ToList();
            //    ObservableCollection<FollowReportDetails> FollowerReports
            //        ReportGrid.ItemsSource = new ObservableCollection<FollowReportDetails>( reportContent);
            //  //  }

            //}
        }

        private void CmbAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SocinatorInitialize.GetSocialLibrary(SocialNetworks.Reddit).GetNetworkCoreFactory().AccountUserControlTools
                .RecentlySelectedAccount = CmbAccounts.SelectedItem.ToString();
            GenerateReports();
        }

        private void GenerateReports()
        {
            var selectedAccount = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Reddit).GetNetworkCoreFactory()
                .AccountUserControlTools.RecentlySelectedAccount;
            var account = _accountsFileManager.GetAll()
                .FirstOrDefault(x => x.AccountBaseModel.UserName == selectedAccount);
            var dbAccountService = InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(account);
            IList reportDetails;
            Task.Run(() =>
            {
                reportDetails = RedditInitialize.GetModuleLibrary(ActivityType).RdUtilityFactory().RdReportFactory
                    .GetAccountReport(dbAccountService);
                Dispatcher.Invoke(() =>
                {
                    ReportGrid.ItemsSource = null;
                    ReportGrid.ItemsSource = reportDetails;
                });
            });
        }

        private void ReportGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = Regex.Replace(e.Column.Header.ToString(), "(\\B[A-Z])", " $1");
        }

        private void AccountReport_OnLoaded(object sender, RoutedEventArgs e)
        {
            CmbAccounts.SelectedItem = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Reddit)
                .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;
        }

        private void Refresh_OnMouseDown(object sender, RoutedEventArgs e)
        {
            GenerateReports();
        }
    }
}
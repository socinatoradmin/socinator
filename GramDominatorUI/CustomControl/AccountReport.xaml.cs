using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.Report;
using GramDominatorCore.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace GramDominatorUI.CustomControl
{
    public partial class AccountReport : UserControl
    {
        private string _csvHeader = string.Empty;

        public AccountReport()
        {
            InitializeComponent();
        }

        public AccountReport(ActivityType activityType) : this()
        {
            ActivityType = activityType;
            var instance = InstanceProvider.GetInstance<IAccountsFileManager>();
            var accounts = new ObservableCollectionBase<string>(instance.GetAll().Where(x =>
                x.AccountBaseModel.AccountNetwork == SocialNetworks.Instagram &&
                (x.AccountBaseModel.Status == AccountStatus.Success ||
                 x.AccountBaseModel.Status == AccountStatus.UpdatingDetails)).Select(x => x.UserName));
            CmbAccounts.SelectedIndex = -1;
            CmbAccounts.ItemsSource = accounts;

            var recentAccount = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Instagram).GetNetworkCoreFactory()
                .AccountUserControlTools.RecentlySelectedAccount;

            CmbAccounts.SelectedItem = string.IsNullOrEmpty(recentAccount)
                ? !string.IsNullOrEmpty(accounts[0]) ? accounts[0] : ""
                : recentAccount;
            recentAccount = string.IsNullOrEmpty(recentAccount) ? CmbAccounts.SelectedItem.ToString() : recentAccount;
        }

        private ActivityType ActivityType { get; }
        private List<FollowReportDetails> LstFollowReportModel { get; set; }

        private List<UnfollowReportDetails> LstUnfollowReportModel { get; set; }

        private List<LikeReportDetails> LstLikeReportModel { get; set; }

        private List<CommentReportDetails> LstCommentReportModel { get; set; }

        private List<FollowBackReportDetails> LstFollowBackReportModel { get; set; }

        private List<BlockFollowerReportDetails> LstBlockFollowerReportModel { get; set; }

        private List<ReposterReportDetails> LstReposterReportModel { get; set; }

        private List<DeletePostReportDetails> LstDeletePostReportModel { get; set; }

        private List<HashtagScrapeReportDetails> LstHashtagScrapeReportModel { get; set; }

        private List<BroadcastMessageReportDetails> LstBroadcastMessageReportModel { get; set; }

        private List<UserScrapeReportDetails> LstUserScraperReportModel { get; set; }

        private List<DownloadPhotoReportDetails> LstDownloadPhotoReportModel { get; set; }


        private List<string> CsvData { get; set; }

        private void BtnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            var selectItem = CmbAccounts.SelectedItem.ToString();
            var instance = InstanceProvider.GetInstance<IAccountsFileManager>();
            var accountmodel = instance.GetAll()
                .FirstOrDefault(x => x.AccountBaseModel.UserName == selectItem);
            IDbAccountService _dbAccountService = new DbAccountService(accountmodel);
            var activity = ActivityType.ToString();
            if (ActivityType == ActivityType.Follow || ActivityType == ActivityType.UserScraper ||
                ActivityType == ActivityType.BroadcastMessages ||
                ActivityType == ActivityType.BlockFollower || ActivityType == ActivityType.FollowBack ||
                ActivityType == ActivityType.AutoReplyToNewMessage)
                Task.Factory.StartNew(() =>
                {
                    _dbAccountService.RemoveMatch<InteractedUsers>(x =>
                        x.ActivityType == activity && x.Username == selectItem);
                });

            else if (ActivityType == ActivityType.HashtagsScraper)
                _dbAccountService.RemoveMatch<HashtagScrape>(x =>
                    x.ActivityType == ActivityType && x.AccountUsername == selectItem);
            else if (ActivityType == ActivityType.Unfollow)
                _dbAccountService.RemoveMatch<UnfollowedUsers>(x => x.AccountUsername == selectItem);
            else
                _dbAccountService.RemoveMatch<InteractedPosts>(x =>
                    x.ActivityType == ActivityType && x.Username == selectItem);
            ReportGrid.ItemsSource = null;
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var win = Application.Current.MainWindow;

                var exportPath = FileUtilities.GetExportPath(win);

                if (string.IsNullOrEmpty(exportPath))
                    return;

                var filename = Regex.Replace(
                    $"{CmbAccounts.SelectedItem}_{ActivityType}-Reports[{DateTime.Now:ddMMyyyyHmmss}]", //ConstantVariable.DateasFileName
                    "[\\/:*?<>|\"]",
                    "-");

                filename = $"{exportPath}\\{filename}.csv";

                InstagramInitialize.GetModuleLibrary(ActivityType).GdUtilityFactory().GdReportFactory
                    .ExportReports(ActivityType, filename, ReportType.Account);
                Dialog.ShowDialog(Application.Current.MainWindow, "Sucess", "Sucessfully Exported to " + filename);
            }
            catch (Exception)
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Fail", "Export fail !!");
            }
        }

        private void CmbAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetGDReport();
        }

        private void ReportGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = Regex.Replace(e.Column.Header.ToString(), "(\\B[A-Z])", " $1");
        }

        private void OnRefresh_Button_Clicked(object sender, RoutedEventArgs e)
        {
            GetGDReport();
        }

        public void GetGDReport()
        {
            var selectedItem = CmbAccounts.SelectedItem.ToString();
            SocinatorInitialize.GetSocialLibrary(SocialNetworks.Instagram).GetNetworkCoreFactory()
                .AccountUserControlTools.RecentlySelectedAccount = selectedItem;
            var instance = InstanceProvider.GetInstance<IAccountsFileManager>();
            var accountModleuserId = instance.GetAccount(selectedItem, SocialNetworks.Instagram);
            // var dboperation = new DbOperations(accountUserId, SocialNetworks.Instagram, ConstantVariable.GetAccountDb);
            var dboperation = new DbAccountService(accountModleuserId);
            var reportDetails = InstagramInitialize.GetModuleLibrary(ActivityType).GdUtilityFactory().GdReportFactory
                .GetsAccountReport(dboperation);
            ReportGrid.ItemsSource = null;
            ReportGrid.ItemsSource = reportDetails;
        }
    }
}
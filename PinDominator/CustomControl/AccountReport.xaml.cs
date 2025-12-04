using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.Report;
using PinDominatorCore.Utility;

namespace PinDominator.CustomControl
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
                .Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Pinterest &&
                            x.AccountBaseModel.Status == AccountStatus.Success).Select(x => x.UserName));
            CmbAccounts.SelectedIndex = -1;
            CmbAccounts.ItemsSource = accounts;
            CmbAccounts.SelectedItem =
                string.IsNullOrEmpty(SocinatorInitialize.GetSocialLibrary(SocialNetworks.Pinterest)
                    .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount)
                    ? !string.IsNullOrEmpty(accounts[0]) ? accounts[0] : ""
                    : SocinatorInitialize.GetSocialLibrary(SocialNetworks.Pinterest).GetNetworkCoreFactory()
                        .AccountUserControlTools.RecentlySelectedAccount;
            SocinatorInitialize.GetSocialLibrary(SocialNetworks.Pinterest).GetNetworkCoreFactory()
                    .AccountUserControlTools.RecentlySelectedAccount =
                string.IsNullOrEmpty(SocinatorInitialize.GetSocialLibrary(SocialNetworks.Pinterest)
                    .GetNetworkCoreFactory()
                    .AccountUserControlTools.RecentlySelectedAccount)
                    ? CmbAccounts.SelectedItem.ToString()
                    : SocinatorInitialize.GetSocialLibrary(SocialNetworks.Pinterest).GetNetworkCoreFactory()
                        .AccountUserControlTools.RecentlySelectedAccount;
        }

        private ActivityType ActivityType { get; }
        private IAccountsFileManager AccountsFileManager { get; }
        private IList ReportDetails { get; set; }

        private void BtnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedAccount = AccountsFileManager
                    .GetAccount(CmbAccounts.SelectedItem.ToString(), SocialNetworks.Pinterest).AccountBaseModel
                    .AccountId;

                var activityTypeString = ActivityType.ToString();
                //var accountId = AccountsFileManager.GetAll()
                //    .FirstOrDefault(x => x.AccountBaseModel.UserName == CmbAccounts.SelectedItem.ToString() && x.)?.AccountId;
                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var account = accountsFileManager.GetAll().FirstOrDefault(x => x.AccountId == selectedAccount);
                var dbAccountService = InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(account);
                if (ActivityType == ActivityType.Follow || ActivityType == ActivityType.BroadcastMessages ||
                    ActivityType == ActivityType.UserScraper
                    || ActivityType == ActivityType.SendMessageToFollower ||
                    ActivityType == ActivityType.AutoReplyToNewMessage
                    || ActivityType == ActivityType.FollowBack)
                    dbAccountService.RemoveMatch<InteractedUsers>(x => x.ActivityType == activityTypeString);

                else if (ActivityType == ActivityType.Try || ActivityType == ActivityType.Comment ||
                         ActivityType == ActivityType.PinScraper
                         || ActivityType == ActivityType.DeletePin || ActivityType == ActivityType.Repin)
                    dbAccountService.RemoveMatch<InteractedPosts>(x => x.OperationType == activityTypeString);

                else if (ActivityType == ActivityType.BoardScraper || ActivityType == ActivityType.CreateBoard ||
                         ActivityType == ActivityType.EditPin)
                    dbAccountService.RemoveMatch<InteractedBoards>(x => x.OperationType == ActivityType);

                else if (ActivityType == ActivityType.Unfollow)
                    dbAccountService.RemoveMatch<UnfollowedUsers>(x => x.OperationType == ActivityType);

                ReportDetails.Clear();
                ReportGrid.ItemsSource = null;
                ReportGrid.ItemsSource = ReportDetails;
            }
            catch (Exception ex)
            {
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

                PinterestInitialize.GetModuleLibrary(ActivityType).PdUtilityFactory().PdReportFactory
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
            SocinatorInitialize.GetSocialLibrary(SocialNetworks.Pinterest).GetNetworkCoreFactory()
                .AccountUserControlTools.RecentlySelectedAccount = CmbAccounts.SelectedItem.ToString();
            //string AccountId = AccountsFileManager.GetAll().FirstOrDefault(x => x.UserName == CmbAccounts.SelectedItem.ToString()).AccountId;
            //var dataBase = new DbOperations(AccountId, SocialNetworks.Pinterest, ConstantVariable.GetAccountDb);

            GenerateReports();
        }

        private void ReportGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = Regex.Replace(e.Column.Header.ToString(), "(\\B[A-Z])", " $1");
        }

        private void AccountReport_Loaded(object sender, RoutedEventArgs e)
        {
            var accounts = new ObservableCollectionBase<string>(AccountsFileManager.GetAll()
                .Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Pinterest &&
                            x.AccountBaseModel.Status == AccountStatus.Success).Select(x => x.UserName));
            CmbAccounts.SelectedItem =
                string.IsNullOrEmpty(SocinatorInitialize.GetSocialLibrary(SocialNetworks.Pinterest)
                    .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount)
                    ? !string.IsNullOrEmpty(accounts[0]) ? accounts[0] : ""
                    : SocinatorInitialize.GetSocialLibrary(SocialNetworks.Pinterest).GetNetworkCoreFactory()
                        .AccountUserControlTools.RecentlySelectedAccount;
        }

        private void GenerateReports()
        {
            var accountUserId = AccountsFileManager
                .GetAccount(CmbAccounts.SelectedItem.ToString(), SocialNetworks.Pinterest).AccountBaseModel.AccountId;

            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var account = accountsFileManager.GetAll().FirstOrDefault(x => x.AccountId == accountUserId);
            var dataBase = InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(account);

            Task.Run(() =>
            {
                ReportDetails?.Clear();
                ReportDetails = PinterestInitialize.GetModuleLibrary(ActivityType).PdUtilityFactory().PdReportFactory
                    .GetsAccountReport(dataBase);
                Dispatcher.Invoke(() =>
                {
                    ReportGrid.ItemsSource = null;
                    ReportGrid.ItemsSource = ReportDetails;
                });
            });
        }

        private void OnRefresh_Button_Clicked(object sender, RoutedEventArgs e)
        {
            GenerateReports();
        }
    }
}
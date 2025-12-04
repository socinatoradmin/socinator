using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorUI.CustomControl
{
    public partial class AccountReport
    {
        public AccountReport()
        {
            InitializeComponent();
            accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
        }

        public AccountReport(ActivityType activityType) : this()
        {
            try
            {
                ActivityType = activityType;

                var accounts = new ObservableCollectionBase<string>(accountsFileManager.GetAll()
                    .Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Quora &&
                                x.AccountBaseModel.Status == AccountStatus.Success).Select(x => x.UserName));
                CmbAccounts.SelectedIndex = -1;
                CmbAccounts.ItemsSource = accounts;
                var selectedaccount = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Quora).GetNetworkCoreFactory()
                    .AccountUserControlTools.RecentlySelectedAccount;
                CmbAccounts.SelectedItem = string.IsNullOrEmpty(selectedaccount)
                    ? !string.IsNullOrEmpty(accounts[0]) ? accounts[0] : ""
                    : selectedaccount;
                SocinatorInitialize.GetSocialLibrary(SocialNetworks.Quora).GetNetworkCoreFactory()
                    .AccountUserControlTools.RecentlySelectedAccount = string.IsNullOrEmpty(selectedaccount)
                    ? CmbAccounts.SelectedItem.ToString()
                    : selectedaccount;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private ActivityType ActivityType { get; }
        private IList ReportDetails { get; set; }
        private IAccountsFileManager accountsFileManager { get; }

        private void BtnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedAccount = CmbAccounts.SelectedItem.ToString();
                var account = accountsFileManager.GetAll()
                    .FirstOrDefault(x => x.AccountBaseModel.UserName == selectedAccount);
                var dbAccountService =
                    InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(account);

                if (ActivityType == ActivityType.Follow || ActivityType == ActivityType.ReportUsers ||
                    ActivityType == ActivityType.UserScraper)
                    dbAccountService.DeleteInteractedUsers(ActivityType, selectedAccount);
                else if (ActivityType == ActivityType.ReportAnswers || ActivityType == ActivityType.UpvoteAnswers ||
                         ActivityType == ActivityType.DownvoteAnswers ||
                         ActivityType == ActivityType.AnswersScraper || ActivityType == ActivityType.AnswerOnQuestions)
                    dbAccountService.DeleteInteractedAnswers(ActivityType, selectedAccount);
                else if (ActivityType == ActivityType.QuestionsScraper ||
                         ActivityType == ActivityType.DownvoteQuestions)
                    dbAccountService.DeleteInteractedQuestion(ActivityType, selectedAccount);
                else
                    dbAccountService.DeleteInteractedMessage(ActivityType, selectedAccount);


                ReportDetails.Clear();
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
                QuoraInitialize.GetModuleLibrary(ActivityType).QdUtilityFactory().QdReportFactory
                    .ExportReports(ActivityType, filename, ReportType.Account);
            }

            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        //private void BtnImport_Click(object sender, RoutedEventArgs e)
        //{
        //    //var openFileDialog = new Microsoft.Win32.OpenFileDialog
        //    //{
        //    //    Multiselect = false,
        //    //    Filter = "Database file (.db)|*.db|CSV file (.csv)|*.csv"
        //    //};


        //    //if (openFileDialog.ShowDialog() == true)
        //    //{
        //    //    string filePath = openFileDialog.FileName;

        //    //    //if (Path.GetExtension(filePath) == ".db")
        //    //    //{
        //    //    //    var CampaignId = Utilities.GetBetween(filePath, "DB\\", ".db");
        //    //    //    // GetReport(CampaignId);
        //    //    //}
        //    //    //else//if csv file
        //    //    //{
        //    //        var reportContent = FileUtilities.GetFileContent(filePath);
        //    //        reportContent = reportContent?.Skip(1).ToList();
        //    //    ObservableCollection<FollowReportDetails> FollowerReports
        //    //        ReportGrid.ItemsSource = new ObservableCollection<FollowReportDetails>( reportContent);
        //    //  //  }

        //    //}
        //}

        private void CmbAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SocinatorInitialize.GetSocialLibrary(SocialNetworks.Quora).GetNetworkCoreFactory()
                    .AccountUserControlTools.RecentlySelectedAccount = CmbAccounts.SelectedItem.ToString();
                GenerateReports();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void GenerateReports()
        {
            try
            {
                var account = accountsFileManager.GetAll()
                    .FirstOrDefault(x => x.AccountBaseModel.UserName == CmbAccounts.SelectedItem.ToString());
                var dbAccountService =
                    InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(account);
                //  IDbAccountService dbAccountService = new DbAccountService(account);
                

                Task.Run(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        ReportDetails = QuoraInitialize.GetModuleLibrary(ActivityType).QdUtilityFactory().QdReportFactory
                        .GetAccountReport(dbAccountService);
                        ReportGrid.ItemsSource = null;
                        ReportGrid.ItemsSource = ReportDetails;
                    });
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ReportGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = Regex.Replace(e.Column.Header.ToString(), "(\\B[A-Z])", " $1");
        }


        private void Refresh_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            GenerateReports();
        }

        private void AccountReport_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CmbAccounts.SelectedItem = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Quora)
                    .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;
                //ReportGrid.ItemsSource = ReportDetails;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
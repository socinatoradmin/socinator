using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.CustomControl
{
    public partial class AccountReport : UserControl
    {
        private readonly IAccountsFileManager _accountsFileManager;

        private readonly string _csvHeader = string.Empty;
        private readonly string _selectedUserName;

        public AccountReport(ActivityType activityType)
        {
            InitializeComponent();
            try
            {
                try
                {
                    _selectedUserName = SocinatorInitialize.GetSocialLibrary(SocialNetworks.LinkedIn)
                        .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;
                }
                catch (Exception)
                {
                    //
                }

                ActivityType = activityType;
                _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var accounts = new ObservableCollectionBase<string>(_accountsFileManager.GetAll()
                    .Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.LinkedIn &&
                                x.AccountBaseModel.Status == AccountStatus.Success).Select(x => x.UserName));
                CmbAccounts.SelectedIndex = -1;
                CmbAccounts.ItemsSource = accounts;
                CmbAccounts.SelectedItem = string.IsNullOrEmpty(_selectedUserName)
                    ? !string.IsNullOrEmpty(accounts[0]) ? accounts[0] : ""
                    : _selectedUserName;
                //GenerateReports();
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
                //}
            }
        }

        private ActivityType ActivityType { get; }

        private List<InteractedUsers> LstInteractedUsers { get; set; }

        private List<InteractedGroups> LstInteractedGroups { get; set; }

        private List<InteractedJobs> LstInteractedJobs { get; set; }

        private List<InteractedCompanies> LstInteractedCompanies { get; set; }

        private List<string> CsvData { get; set; }

        private void BtnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            if (ReportGrid.Items.Count == 0)
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Error",
                    "Empty records can not be deleted");
                return;
            }

            var dialogResult = Dialog.ShowCustomDialog(
                "Confirmation", "It will delete all records from database permanently \nAre you sure ?",
                "Delete Anyways", "Don't delete");
            if (dialogResult != MessageDialogResult.Affirmative)
                return;
            Task.Factory.StartNew(DeleteAll);
            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.LinkedIn, _selectedUserName, ActivityType,
                "Please, wait it will take some time.");
        }

        private void DeleteAll()
        {
            var account = _accountsFileManager.GetAccount(_selectedUserName, SocialNetworks.LinkedIn);
            var dbAccountService = InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(account);

            var activity = ActivityType.ToString();
            if (ActivityType == ActivityType.JobScraper)
                dbAccountService.RemoveMatch<InteractedJobs>(x => true);
            else if (LdDataHelper.IsGroupProcessor(ActivityType))
                dbAccountService.RemoveMatch<InteractedGroups>(x => x.ActivityType == activity);
            else if (LdDataHelper.IsCompanyProcessor(ActivityType))
                dbAccountService.RemoveMatch<InteractedCompanies>(x => x.ActivityType == activity);
            else if (LdDataHelper.IsPostProcessor(ActivityType))
                dbAccountService.RemoveMatch<InteractedPosts>(x => x.ActivityType == activity);
            else if (LdDataHelper.IsPageProcessor(ActivityType))
                dbAccountService.RemoveMatch<InteractedPage>(x => x.ActivityType == activity);
            else
                dbAccountService.RemoveMatch<InteractedUsers>(x => x.ActivityType == activity);


            Dispatcher.Invoke(() => GenerateReports());

            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.LinkedIn, _selectedUserName, ActivityType,
                "All records deleted");
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

            LinkedInInitialize.GetModuleLibrary(ActivityType).LdUtilityFactory().LdReportFactory
                .ExportReports(ActivityType, filename, ReportType.Account);

            using (var streamWriter = new StreamWriter(filename, true))
            {
                streamWriter.WriteLine(_csvHeader);
            }

            try
            {
                foreach (var item in CsvData)
                    using (var streamWriter = new StreamWriter(filename, true))
                    {
                        streamWriter.WriteLine(item);
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
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
            try
            {
                SocinatorInitialize.GetSocialLibrary(SocialNetworks.LinkedIn).GetNetworkCoreFactory()
                    .AccountUserControlTools.RecentlySelectedAccount = CmbAccounts.SelectedItem.ToString();
                GenerateReports();
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            GenerateReports();
        }

        //private IEntityCountersManager entityCountersManager;
        private void GenerateReports()
        {
            var accountModel = _accountsFileManager.GetAll()
                .FirstOrDefault(x => x.AccountBaseModel.UserName == CmbAccounts.SelectedItem.ToString());
            IDbAccountService dbAccountService = new DbAccountService(accountModel);
            IList reportDetails = null;
            Task.Run(() =>
            {
                reportDetails = LinkedInInitialize.GetModuleLibrary(ActivityType).LdUtilityFactory().LdReportFactory
                    .GetsAccountReport(dbAccountService);
                Dispatcher.Invoke(() =>
                {
                    ReportGrid.ItemsSource = null;
                    ReportGrid.ItemsSource = reportDetails;
                });
            });


            SaveAccountNameWithId(accountModel.AccountBaseModel);
        }

        private void ReportGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = Regex.Replace(e.Column.Header.ToString(), "(\\B[A-Z])", " $1");
        }

        /// <summary>
        ///     only record for to save account name with his id to check his db
        /// </summary>
        /// <param name="dominatorAccount"></param>
        public void SaveAccountNameWithId(DominatorAccountBaseModel dominatorAccount)
        {
            try
            {
                if (!ActivityType.Equals(ActivityType.ExportConnection))
                    return;

                var listAccountDetails = new List<string>();
                var folderPath = ConstantVariable.GetIndexAccountDir();
                var fileName = "AccountWithAccountId.csv";
                var fullPath = $"{folderPath}\\{fileName}";
                var currentData =
                    $"{dominatorAccount.UserName},{dominatorAccount.AccountId}";

                if (File.Exists(fullPath))
                {
                    listAccountDetails = File.ReadAllLines(fullPath).ToList();
                    if (!listAccountDetails.Contains(currentData))
                    {
                        listAccountDetails.Add(currentData);
                        File.WriteAllLines(fullPath, listAccountDetails.ToArray());
                    }
                }
                else
                {
                    listAccountDetails.Add(currentData);
                    File.WriteAllLines(fullPath, listAccountDetails.ToArray());
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
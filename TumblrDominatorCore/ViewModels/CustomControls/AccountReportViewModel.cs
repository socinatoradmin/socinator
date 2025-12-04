using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Command;
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
using System.Windows;
using System.Windows.Input;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrLibrary.DAL;

namespace TumblrDominatorCore.ViewModels.CustomControls
{
    public class AccountReportViewModel : BindableBase
    {
        private IList _reportDetailscount;

        private string _selectedAccount = string.Empty;

        public AccountReportViewModel()
        {
            RefreshCommand = new BaseCommand<object>(sender => true, GenerateReports);
            CmdAccSelectionChanged = new BaseCommand<object>(sender => true, Account_SelectionChanged);
            LoadedCommand = new BaseCommand<object>(sender => true, OnAccount_Load);
            ExportCommand = new BaseCommand<object>(sender => true, ExportAccountReport);
            DeleteaAllCommand = new BaseCommand<object>(sender => true, DeleteAllReports);
        }

        public ICommand ClearCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand DeleteaAllCommand { get; set; }
        public ICommand CmdAccSelectionChanged { get; set; }
        public ICommand LoadedCommand { get; set; }
        public ActivityType ActivityType { get; set; }

        public string SelectedAccount
        {
            get => _selectedAccount;

            set
            {
                if (value == _selectedAccount)
                    return;
                SetProperty(ref _selectedAccount, value);
            }
        }

        public IList ReportDetailscount
        {
            get => _reportDetailscount;

            set => SetProperty(ref _reportDetailscount, value);
        }

        public void GenerateReports(object sender)
        {
            try
            {
                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var accounts = accountsFileManager.GetAll().FirstOrDefault(x =>
                    x.AccountBaseModel.UserName == SelectedAccount &&
                    x.AccountBaseModel.AccountNetwork == SocialNetworks.Tumblr);
                // IQueryProcessor processor = _unityContainer.Resolve<HashtagPostProcessor>();
                // processor.Start(queryInfo);
                var dbAccountService =
                    InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(accounts);
                ReportDetailscount?.Clear();
                ReportDetailscount = TumblrInitialize.GetModuleLibrary(ActivityType).TumblrUtilityFactory()
                    .TumblrReportFactory.GetAccountReport(dbAccountService);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void Account_SelectionChanged(object sender)
        {
            try
            {
                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var accounts = accountsFileManager.GetAll()
                    .FirstOrDefault(x => x.AccountBaseModel.UserName == SelectedAccount);

                var dbAccountService = InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(accounts);

                ReportDetailscount = null;
                ReportDetailscount = TumblrInitialize.GetModuleLibrary(ActivityType).TumblrUtilityFactory()
                    .TumblrReportFactory.GetAccountReport(dbAccountService);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void OnAccount_Load(object sender)
        {
            try
            {
                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                SelectedAccount = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Tumblr)
                    .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;
                var accounts = accountsFileManager.GetAll().FirstOrDefault(x =>
                    x.AccountBaseModel.UserName == SelectedAccount &&
                    x.AccountBaseModel.AccountNetwork == SocialNetworks.Tumblr);
                var dbAccountService = InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(accounts);
                //var list =  dbAccountService.GetInteractedUsers(ActivityType.Follow);
                ReportDetailscount?.Clear();
                ReportDetailscount = TumblrInitialize.GetModuleLibrary(ActivityType).TumblrUtilityFactory()
                    .TumblrReportFactory.GetAccountReport(dbAccountService);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void ExportAccountReport(object sender)
        {
            try
            {
                var exportPath = FileUtilities.GetExportPath();

                if (string.IsNullOrEmpty(exportPath))
                    return;

                var filename = Regex.Replace(
                    $"{SelectedAccount}_{ActivityType}-Reports[{DateTime.Now:ddMMyyyyHmmss}]", //ConstantVariable.DateasFileName
                    "[\\/:*?<>|\"]",
                    "-");

                filename = $"{exportPath}\\{filename}.csv";

                //  var selectedAccount = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Tumblr).GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;

                TumblrInitialize.GetModuleLibrary(ActivityType).TumblrUtilityFactory().TumblrReportFactory
                    .ExportReports(ReportType.Account, filename);

                Dialog.ShowDialog(Application.Current.MainWindow, "Sucess",
                    "Sucessfully Exported to " + filename);
            }
            catch (Exception)
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Fail",
                    "Export fail !!");
            }
        }

        public void DeleteAllReports(object sender)
        {
            //string activity = ActivityType.ToString();
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var accounts = accountsFileManager.GetAll().FirstOrDefault(x =>
                x.AccountBaseModel.UserName == SelectedAccount &&
                x.AccountBaseModel.AccountNetwork == SocialNetworks.Tumblr);
            var dbAccountService =
                InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(accounts);

            if (ActivityType.Equals(ActivityType.Follow) || ActivityType.Equals(ActivityType.UserScraper) ||
                ActivityType.Equals(ActivityType.BroadcastMessages))
            {
                var lstUsers = dbAccountService.GetInteractedUsers(ActivityType);
                lstUsers.ForEach(x => { dbAccountService.Remove(x); });
            }
            else if (ActivityType.Equals(ActivityType.Unlike))
            {
                var lstUsers = dbAccountService.GetUnLikedUsers(ActivityType);
                lstUsers.ForEach(x => { dbAccountService.Remove(x); });
            }
            else if (ActivityType.Equals(ActivityType.Unfollow))
            {
                var lstUsers = dbAccountService.GetUnfollowedUsers(ActivityType);
                lstUsers.ForEach(x => { dbAccountService.Remove(x); });
            }
            else
            {
                var lstPosts = dbAccountService.GetInteractedPosts(ActivityType);
                lstPosts.ForEach(x => { dbAccountService.Remove(x); });
            }

            ReportDetailscount.Clear();
            ReportDetailscount = null;
        }
    }
}
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace FaceDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for AccountReports.xaml
    /// </summary>
    public partial class AccountReports
    {
        public AccountReports()
        {
            InitializeComponent();
        }


        public AccountReports(ActivityType activityType) : this()
        {
            ActivityType = activityType;
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var accounts = new ObservableCollectionBase<string>(accountsFileManager.GetAll()
                .Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook).Select(x => x.UserName));
            CmbAccounts.SelectedIndex = -1;
            CmbAccounts.ItemsSource = accounts;
            var selctedAccount = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Facebook).GetNetworkCoreFactory()
                .AccountUserControlTools.RecentlySelectedAccount;
            var accountUserId = accountsFileManager.GetAccount(selctedAccount, SocialNetworks.Facebook).AccountBaseModel
                .AccountId;

            AccountModel = accountsFileManager.GetAccountById(accountUserId);

            CmbAccounts.SelectedItem = string.IsNullOrEmpty(selctedAccount)
                ? !string.IsNullOrEmpty(accounts[0]) ? accounts[0] : ""
                : selctedAccount;

            ObjDbAccountService = new DbAccountService(AccountModel);
        }

        private ActivityType ActivityType { get; }


        private DominatorAccountModel AccountModel { get; } = new DominatorAccountModel();

        private DbAccountService ObjDbAccountService { get; set; }


        private void CmbAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SocinatorInitialize.GetSocialLibrary(SocialNetworks.Facebook).GetNetworkCoreFactory()
                .AccountUserControlTools.RecentlySelectedAccount = CmbAccounts.SelectedItem.ToString();

            GenereateReports();
        }

        private void GenereateReports()
        {
            var selectedAccount = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Facebook).GetNetworkCoreFactory()
                .AccountUserControlTools.RecentlySelectedAccount;
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var accountUserId = accountsFileManager.GetAccount(selectedAccount, SocialNetworks.Facebook)
                .AccountBaseModel.AccountId;

            var accountModel = accountsFileManager.GetAccountById(accountUserId);

            var dataBase = new DbAccountService(accountModel);

            ObjDbAccountService = dataBase;


            var reportDetails = FacebookInitialize.GetModuleLibrary(ActivityType).FdUtilityFactory().FdReportFactory
                .GetsAccountReport(dataBase);
            ReportGrid.ItemsSource = null;
            ReportGrid.ItemsSource = reportDetails;
        }


        private void Report_Onload(object sender, RoutedEventArgs e)
        {
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var accounts = new ObservableCollectionBase<string>(accountsFileManager.GetAll()
                .Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook).Select(x => x.UserName));
            var selctedAccount = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Facebook).GetNetworkCoreFactory()
                .AccountUserControlTools.RecentlySelectedAccount;
            CmbAccounts.SelectedItem = string.IsNullOrEmpty(selctedAccount)
                ? !string.IsNullOrEmpty(accounts[0]) ? accounts[0] : ""
                : selctedAccount;
        }

        private void Button_ClickDeleteAll(object sender, RoutedEventArgs e)
        {
            var isClose =
                Dialog.ShowCustomDialog("LangKeyConfirmation".FromResourceDictionary(),
                    "LangKeyDeleteAllRelatedData".FromResourceDictionary(), "Yes", "No") ==
                MessageDialogResult.Affirmative;

            if (isClose)
                switch (ActivityType)
                {
                    case ActivityType.SendFriendRequest:
                        DeleteUserData();
                        break;

                    case ActivityType.Unfriend:
                        DeleteUserData();
                        break;

                    case ActivityType.GroupJoiner:
                        DeleteGroupData();
                        break;

                    case ActivityType.FanpageLiker:
                        DeleteFanpageData();
                        break;

                    case ActivityType.PostLikerCommentor:
                        DeletePostData();
                        break;

                    case ActivityType.ProfileScraper:
                        DeleteUserData();
                        break;

                    case ActivityType.GroupScraper:
                        DeleteGroupData();
                        break;

                    case ActivityType.FanpageScraper:
                        DeleteFanpageData();
                        break;

                    case ActivityType.CommentScraper:
                        DeleteCommentData();
                        break;

                    case ActivityType.BroadcastMessages:
                        DeleteUserData();
                        break;

                    case ActivityType.AutoReplyToNewMessage:
                        DeleteUserData();
                        break;

                    case ActivityType.GroupUnJoiner:
                        DeleteGroupData();
                        break;

                    case ActivityType.PostScraper:
                        DeletePostData();
                        break;

                    case ActivityType.IncommingFriendRequest:
                        DeleteUserData();
                        break;

                    case ActivityType.WithdrawSentRequest:
                        DeleteUserData();
                        break;

                    case ActivityType.PostLiker:
                        DeletePostData();
                        break;

                    case ActivityType.PostCommentor:
                        DeletePostData();
                        break;

                    case ActivityType.GroupInviter:
                        DeleteUserData();
                        break;

                    case ActivityType.PageInviter:
                        DeleteUserData();
                        break;

                    case ActivityType.EventInviter:
                        DeleteUserData();
                        break;

                    case ActivityType.LikeComment:
                        DeleteCommentData();
                        break;

                    case ActivityType.ReplyToComment:
                        DeleteCommentData();
                        break;

                    case ActivityType.DownloadScraper:
                        DeletePostData();
                        break;

                    case ActivityType.EventCreator:
                        DeleteEventData();
                        break;

                    case ActivityType.SendGreetingsToFriends:
                        DeleteUserData();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }

        private void DeleteUserData()
        {
            ThreadFactory.Instance.Start(() =>
            {
                try
                {
                    var objActivityType = ActivityType.ToString();
                    ObjDbAccountService.RemoveAllMatches<InteractedUsers>(x => x.ActivityType == objActivityType);
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, ActivityType, "Succesfully deleted data");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });
        }

        private void DeleteFanpageData()
        {
            ThreadFactory.Instance.Start(() =>
            {
                try
                {
                    var objActivityType = ActivityType.ToString();
                    ObjDbAccountService.RemoveAllMatches<InteractedPages>(x => x.ActivityType == objActivityType);
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, ActivityType, "Succesfully deleted data");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });
        }

        private void DeleteGroupData()
        {
            ThreadFactory.Instance.Start(() =>
            {
                try
                {
                    var objActivityType = ActivityType.ToString();
                    ObjDbAccountService.RemoveAllMatches<InteractedGroups>(x => x.ActivityType == objActivityType);
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, ActivityType, "Succesfully deleted data");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });
        }

        private void DeletePostData()
        {
            ThreadFactory.Instance.Start(() =>
            {
                try
                {
                    var objActivityType = ActivityType.ToString();
                    ObjDbAccountService.RemoveAllMatches<InteractedPosts>(x => x.ActivityType == objActivityType);
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, ActivityType, "Succesfully deleted data");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });
        }

        private void DeleteCommentData()
        {
            ThreadFactory.Instance.Start(() =>
            {
                try
                {
                    var objActivityType = ActivityType.ToString();
                    ObjDbAccountService.RemoveAllMatches<InteractedComments>(x => x.ActivityType == objActivityType);
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, ActivityType, "Succesfully deleted data");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });
        }


        private void DeleteEventData()
        {
            ThreadFactory.Instance.Start(() =>
            {
                try
                {
                    var objActivityType = ActivityType.ToString();
                    ObjDbAccountService.RemoveAllMatches<InteractedEvents>(x => x.ActivityType == objActivityType);
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, ActivityType, "Succesfully deleted data");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
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


                FacebookInitialize.GetModuleLibrary(ActivityType).FdUtilityFactory().FdReportFactory
                    .ExportReports(ActivityType, filename, ReportType.Account);

                Dialog.ShowDialog(Application.Current.MainWindow, "Sucess",
                    "Sucessfully Exported to " + filename);
            }
            catch (Exception ex)
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Fail",
                    "Export fail !!");
                ex.DebugLog();
            }
        }

        private void OnRefresh_Button_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                SocinatorInitialize.GetSocialLibrary(SocialNetworks.Facebook).GetNetworkCoreFactory()
                    .AccountUserControlTools.RecentlySelectedAccount = CmbAccounts.SelectedItem.ToString();

                GenereateReports();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void GenerateColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = Regex.Replace(e.Column.Header.ToString(), "(\\B[A-Z])", " $1");
        }
    }
}
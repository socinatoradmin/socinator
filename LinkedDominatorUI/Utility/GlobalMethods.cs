
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler;
using DominatorHouseCore.DatabaseHandler.LdTables.Campaign;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.LDModel.Messenger;
using LinkedDominatorCore.LDModel;
using LinkedDominatorUI.LDViews.Group;
using LinkedDominatorUI.LDViews.GrowConnection;
using LinkedDominatorUI.LDViews.Scraper;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LinkedDominatorCore.LDModel.ReportModel;
using System.Windows.Data;
using LinkedDominatorCore.DetailedInfo;
using LinkedDominatorUI.LDViews.Profilling;
using LinkedDominatorUI.LDViews.Messenger;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDModel.Profilling;
using DominatorHouseCore.DatabaseHandler.Utility;
using LinkedDominatorUI.LDViews.Engage;
using LinkedDominatorCore.LDModel.Engage;

namespace LinkedDominatorUI.LDUtility
{
    public class GlobalMethods
    {

        /// <summary>
        /// SaveDetails method take two argument first is list of select account to be add for campaign and second is moduleType
        /// and it will update accountdetail (account bin file )and campaign (campaign bin file)
        /// </summary>
        /// <param name="lstSelectedAccounts"></param>
        /// <param name="moduleType"></param>
        public static void SaveDetails(List<string> lstSelectedAccounts, ActivityType moduleType)
        {
            try
            {
                var AccountDetails = AccountsFileManager.GetAll();
                var OldAccountDetails = AccountsFileManager.GetAll();

                // this list contain detail of accounts which are already having setting

                ErrorModelControl objErrorModelControl = new ErrorModelControl();

                List<DominatorAccountModel> accountHavingSetting =
                            AccountDetails.Where(
                                x => x.ActivityManager.LstModuleConfiguration.FirstOrDefault(y => y.ActivityType == moduleType)
                                                         ?.TemplateId != null)?.ToList() ?? new List<DominatorAccountModel>();

                #region Setting  WarningText according to Module type

                switch (moduleType)
                {
                    case ActivityType.ConnectionRequest:
                        objErrorModelControl.WarningText = Application.Current.FindResource("LDlangConnectionRequestWarning").ToString();
                        break;
                    case ActivityType.GroupJoiner:
                        objErrorModelControl.WarningText = Application.Current.FindResource("LDlangGroupJoinerWarning").ToString();
                        break;
                    case ActivityType.GroupInviter:
                        objErrorModelControl.WarningText = Application.Current.FindResource("LDlangGroupInviterWarning").ToString();
                        break;
                    case ActivityType.UserScraper:
                        objErrorModelControl.WarningText = Application.Current.FindResource("LDlangUserScraperWarning").ToString();
                        break;
                    case ActivityType.JobScraper:
                        objErrorModelControl.WarningText = Application.Current.FindResource("LDlangJobScraperWarning").ToString();
                        break;
                    case ActivityType.CompanyScraper:
                        //LDlangCompanyScraperScraperWarning
                        objErrorModelControl.WarningText = Application.Current.FindResource("LDlangCompanyScraperWarning").ToString();
                        break;
                    case ActivityType.GroupMemberScraper:
                        objErrorModelControl.WarningText = Application.Current.FindResource("LDlangGroupMemberScraperWarning").ToString();
                        break;
                    case ActivityType.SalesNavigatorUserScraper:
                        objErrorModelControl.WarningText = Application.Current.FindResource("LDlangSalesNavigator_UserScraperWarning").ToString();
                        break;
                }

                #endregion


                //this list contains list of all account which are not selected for modification
                List<string> UnSelectedAccountForModification = new List<string>();

                //it will check if any account having setting or not 
                if (accountHavingSetting.Count > 0)
                {
                    try
                    {
                        try
                        {
                            lstSelectedAccounts.ForEach(account => objErrorModelControl.Accounts.Add(new ErrorModelControl()
                            {
                                UserName = accountHavingSetting.FirstOrDefault(x => x.AccountBaseModel.UserName == account).AccountBaseModel.UserName
                            })
                            );
                        }
                        catch (Exception Ex)
                        {
                            GlobusLogHelper.log.Error(Ex.StackTrace);
                        }

                        //Check if account is running with campaign or not if any account running with campaign then it will show ErrorModel
                        //there you can update campaign
                        if (objErrorModelControl.Accounts.Count > 0)
                        {
                            Dialog objDialog = new Dialog();
                            var WarningWindow = objDialog.GetMetroWindow(objErrorModelControl, "Warning");

                            //if we want to replace prvious setting we need to click save button
                            objErrorModelControl.BtnSave.Click += (senders, Events) =>
                            {
                                try
                                {
                                    //Getting account that are not required to update
                                    UnSelectedAccountForModification = objErrorModelControl.Accounts.Where(x => x.IsChecked == false).Select(x => x.UserName).ToList();

                                    //To remove the account which we don't want to update with new Configuration
                                    UnSelectedAccountForModification.ForEach(item => lstSelectedAccounts.Remove(item));

                                    //it will check if account is updated or not if updated then will delete account and save that updated details
                                    UpdateSelectedAccountDetails(AccountDetails, lstSelectedAccounts, moduleType);

                                    //To update campaign file calling UpdateCampaignBinFile() method
                                    UpdateCampaignBinFile(OldAccountDetails, lstSelectedAccounts, moduleType);
                                    WarningWindow.Visibility = Visibility.Hidden;
                                    WarningWindow.Close();
                                }
                                catch (Exception Ex)
                                {
                                    Ex.DebugLog();
                                }
                            };
                            objErrorModelControl.BtnCancel.Click += (senders, Events) =>
                            {
                                WarningWindow.Close();
                            };
                            WarningWindow.ShowDialog();
                        }

                        else // if account is not running with any campaign then it will save 

                        {
                            //it will check if account is update or not if updated then will delete account and save that updated details
                            UpdateSelectedAccountDetails(AccountDetails, lstSelectedAccounts, moduleType);

                            //To update campaign file calling UpdateCampaignBinFile() method
                            UpdateCampaignBinFile(OldAccountDetails, lstSelectedAccounts, moduleType);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                }
                else // if account is not running with any campaign then it will save 

                {
                    //it will check if account is update or not if updated then will delete account and save that updated details
                    UpdateSelectedAccountDetails(AccountDetails, lstSelectedAccounts, moduleType);

                    //To update campaign file calling UpdateCampaignBinFile() method
                    UpdateCampaignBinFile(OldAccountDetails, lstSelectedAccounts, moduleType);
                }
            }
            catch (Exception Ex)
            {
                Ex.ErrorLog();
            }
        }

        internal static void ShowUserFilterControl(SearchQueryControl _queryControl)
        {
            //UserFiltersControl objUserFiltersControl = new UserFiltersControl();
            //Dialog objDialog = new Dialog();
            //objUserFiltersControl.UserFilter.SaveCloseButtonVisible = true;
            //var FilterWindow = objDialog.GetMetroWindow(objUserFiltersControl, "Filter");

            //objUserFiltersControl.SaveButton.Click += (senders, Events) =>
            //{
            //    _queryControl.CurrentQuery.CustomFilters = JsonConvert.SerializeObject(objUserFiltersControl.UserFilter);
            //    FilterWindow.Close();
            //};

            //FilterWindow.ShowDialog();
        }

        /// <summary>
        /// UpdateSelectedAccountDetails method will take AccountDetails and list of selected account to modify as  argument and
        /// will update that account with setting and return status
        /// </summary>
        /// <param name="allAccountDetails"></param>
        /// <param name="listSelectedAccounts"></param>
        /// <returns></returns>
        public static bool UpdateSelectedAccountDetails(IEnumerable<DominatorAccountModel> allAccountDetails, List<string> listSelectedAccounts, ActivityType moduleType)
        {
            bool isAccountDetailsUpdated = false;
            try
            {
                List<DominatorAccountModel> selectedAccounts = new List<DominatorAccountModel>(listSelectedAccounts.Count);
                foreach (var account in allAccountDetails)
                {
                    if (!listSelectedAccounts.Contains(account.AccountBaseModel.UserName))
                        continue;


                    isAccountDetailsUpdated = true;
                    try
                    {
                        if (account.ActivityManager.RunningTime == null)
                            account.ActivityManager.RunningTime = RunningTimes.DayWiseRunningTimes;

                        var moduleConfiguration = account.ActivityManager.LstModuleConfiguration?.FirstOrDefault(y => y.ActivityType == moduleType);
                        if (moduleConfiguration == null)
                        {
                            moduleConfiguration = new ModuleConfiguration() { ActivityType = moduleType };
                            account.ActivityManager.LstModuleConfiguration.Add(moduleConfiguration);
                        }


                        moduleConfiguration.LastUpdatedDate = DateTimeUtilities.GetEpochTime();
                        moduleConfiguration.IsEnabled = true;
                        moduleConfiguration.Status = "Active";

                        #region Update TemplateId and Module for RunningTime for required Model

                        switch (moduleType)
                        {
                            case ActivityType.ConnectionRequest:
                                ConnectionRequest objConnectionRequest = ConnectionRequest.GetSingeltonObjectConnectionRequest();
                                moduleConfiguration.TemplateId = objConnectionRequest.TemplateId;
                                objConnectionRequest.ObjViewModel.ConnectionRequestModel.JobConfiguration.RunningTime.ForEach(x =>
                                {
                                    foreach (var timingRange in x.Timings)
                                    {
                                        timingRange.Module = ActivityType.ConnectionRequest.ToString();
                                    }
                                });
                                account.ActivityManager.RunningTime = objConnectionRequest.ObjViewModel.ConnectionRequestModel
                                    .JobConfiguration.RunningTime;
                                break;

                            case ActivityType.GroupJoiner:
                                GroupJoiner objGroupJoiner = GroupJoiner.GetSingeltonObjectGroupJoiner();
                                moduleConfiguration.TemplateId = objGroupJoiner.TemplateId;
                                objGroupJoiner.ObjViewModel.GroupJoinerModel.JobConfiguration.RunningTime.ForEach(x =>
                                {
                                    foreach (var timingRange in x.Timings)
                                    {
                                        timingRange.Module = ActivityType.GroupJoiner.ToString();
                                    }
                                });
                                account.ActivityManager.RunningTime = objGroupJoiner.ObjViewModel.GroupJoinerModel
                                    .JobConfiguration.RunningTime;
                                break;

                            case ActivityType.GroupInviter:
                                GroupInviter objGroupInviter = GroupInviter.GetSingeltonObjectGroupInviter();
                                moduleConfiguration.TemplateId = objGroupInviter.TemplateId;
                                objGroupInviter.ObjViewModel.GroupInviterModel.JobConfiguration.RunningTime.ForEach(x =>
                                {
                                    foreach (var timingRange in x.Timings)
                                    {
                                        timingRange.Module = ActivityType.GroupInviter.ToString();
                                    }
                                });
                                account.ActivityManager.RunningTime = objGroupInviter.ObjViewModel.GroupInviterModel
                                    .JobConfiguration.RunningTime;
                                break;

                            case ActivityType.UserScraper:

                                UserScraper objUserScraper = UserScraper.GetSingeltonObjectUserScraper();
                                moduleConfiguration.TemplateId = objUserScraper.TemplateId;
                                objUserScraper.ObjViewModel.UserScraperModel.JobConfiguration.RunningTime.ForEach(x =>
                                {
                                    foreach (var timingRange in x.Timings)
                                    {
                                        timingRange.Module = ActivityType.UserScraper.ToString();
                                    }
                                });
                                account.ActivityManager.RunningTime = objUserScraper.ObjViewModel.UserScraperModel
                                    .JobConfiguration.RunningTime;
                                break;

                            case ActivityType.JobScraper:

                                JobScraper objJobScraper = JobScraper.GetSingeltonObjectJobScraper();
                                moduleConfiguration.TemplateId = objJobScraper.TemplateId;
                                objJobScraper.ObjViewModel.JobScraperModel.JobConfiguration.RunningTime.ForEach(x =>
                                {
                                    foreach (var timingRange in x.Timings)
                                    {
                                        timingRange.Module = ActivityType.JobScraper.ToString();
                                    }
                                });
                                account.ActivityManager.RunningTime = objJobScraper.ObjViewModel.JobScraperModel
                                    .JobConfiguration.RunningTime;
                                break;

                            case ActivityType.CompanyScraper:

                                CompanyScraper objCompanyScraper = CompanyScraper.GetSingeltonObjectCompanyScraper();
                                moduleConfiguration.TemplateId = objCompanyScraper.TemplateId;
                                objCompanyScraper.ObjViewModel.CompanyScraperModel.JobConfiguration.RunningTime.ForEach(x =>
                                {
                                    foreach (var timingRange in x.Timings)
                                    {
                                        timingRange.Module = ActivityType.CompanyScraper.ToString();
                                    }
                                });
                                account.ActivityManager.RunningTime = objCompanyScraper.ObjViewModel.CompanyScraperModel
                                    .JobConfiguration.RunningTime;
                                break;

                            case ActivityType.GroupMemberScraper:

                                GroupMemberScraper objGroupMemberScraper = GroupMemberScraper.GetSingeltonObjectGroupMemberScraper();
                                moduleConfiguration.TemplateId = objGroupMemberScraper.TemplateId;
                                objGroupMemberScraper.ObjViewModel.GroupMemberScraperModel.JobConfiguration.RunningTime.ForEach(x =>
                                {
                                    foreach (var timingRange in x.Timings)
                                    {
                                        timingRange.Module = ActivityType.GroupMemberScraper.ToString();
                                    }
                                });
                                account.ActivityManager.RunningTime = objGroupMemberScraper.ObjViewModel.GroupMemberScraperModel
                                    .JobConfiguration.RunningTime;
                                break;

                            case ActivityType.SalesNavigatorUserScraper:

                                break;

                        }
                        #endregion


                        //account.IsCretedFromNormalMode = true;
                        selectedAccounts.Add(account);

                        AccountsFileManager.Edit(account);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                // save all accounts and schedule actitvities of selected accounts            
                foreach (var account in selectedAccounts)
                {
                    DominatorScheduler.ScheduleTodayJobs(account, SocialNetworks.LinkedIn, moduleType);
                    DominatorScheduler.ScheduleForEachModule(moduleToIgnore: moduleType, account: account, network: SocialNetworks.LinkedIn);
                }
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
            return isAccountDetailsUpdated;
        }



        /// <summary>
        /// UpdateCampaign method take AccountDetails as argument and will delete account from campaign account list 
        /// if account exist in any campaign
        /// </summary>
        /// <param name="allAccountDetails"></param>
        /// 
        public static void UpdateCampaignBinFile(IEnumerable<DominatorAccountModel> allAccountDetails, List<string> lstSelectedAccounts, ActivityType moduleType)
        {
            var campaignsList = CampaignsFileManager.Get();

            if (campaignsList.Count == 0)
                return;

            try
            {
                foreach (var selectedAccount in lstSelectedAccounts)
                {
                    var SelectedAccount = allAccountDetails.FirstOrDefault(x => x.AccountBaseModel.UserName == selectedAccount);
                    if (SelectedAccount == null) continue;

                    var TemplateId = SelectedAccount.ActivityManager.LstModuleConfiguration
                        .FirstOrDefault(y => y.ActivityType == moduleType)?.TemplateId;

                    foreach (var campaign in campaignsList)
                    {
                        if (campaign.TemplateId == TemplateId)
                        {
                            campaign.SelectedAccountList.Remove(selectedAccount);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }

            // Save campaigns back
            CampaignsFileManager.Save(campaignsList);
        }

        /// <summary>
        /// AddNewCampaign take two argument first is list of select account to be add for campaign and second is moduleType
        /// and it will save add new campaign for given moduleType
        /// </summary>
        /// <param name="listSelectedAccounts"></param>
        /// <param name="moduleType"></param>
        public static void AddNewCampaign(List<string> listSelectedAccounts, ActivityType moduleType)
        {
            try
            {
                #region Variables Initializations
                string CampaignName = String.Empty;
                string TemplateId = String.Empty;
                string MainModule = string.Empty;
                string SubModule = string.Empty;
                #endregion

                #region Update CampaignName,TemplateId,MainModule and SubModule for Respective Module

                switch (moduleType)
                {
                    case ActivityType.ConnectionRequest:

                        ConnectionRequest objConnectionRequest = ConnectionRequest.GetSingeltonObjectConnectionRequest();
                        CampaignName = objConnectionRequest.CampaignName;
                        TemplateId = objConnectionRequest.TemplateId;
                        MainModule = LinkedDominatorCore.Enums.LdMainModules.GrowConnection.ToString();
                        SubModule = ActivityType.ConnectionRequest.ToString();
                        break;

                    case ActivityType.GroupJoiner:

                        GroupJoiner objGroupJoiner = GroupJoiner.GetSingeltonObjectGroupJoiner();
                        CampaignName = objGroupJoiner.CampaignName;
                        TemplateId = objGroupJoiner.TemplateId;
                        MainModule = LinkedDominatorCore.Enums.LdMainModules.Group.ToString();
                        SubModule = ActivityType.GroupJoiner.ToString();
                        break;

                    case ActivityType.UserScraper:

                        UserScraper objUserScraper = UserScraper.GetSingeltonObjectUserScraper();
                        CampaignName = objUserScraper.CampaignName;
                        TemplateId = objUserScraper.TemplateId;
                        MainModule = LinkedDominatorCore.Enums.LdMainModules.Scraper.ToString();
                        SubModule = ActivityType.UserScraper.ToString();
                        break;

                    case ActivityType.JobScraper:

                        JobScraper objJobScraper = JobScraper.GetSingeltonObjectJobScraper();
                        CampaignName = objJobScraper.CampaignName;
                        TemplateId = objJobScraper.TemplateId;
                        MainModule = LinkedDominatorCore.Enums.LdMainModules.Scraper.ToString();
                        SubModule = ActivityType.JobScraper.ToString();
                        break;

                    case ActivityType.CompanyScraper:

                        CompanyScraper objCompanyScraper = CompanyScraper.GetSingeltonObjectCompanyScraper();
                        CampaignName = objCompanyScraper.CampaignName;
                        TemplateId = objCompanyScraper.TemplateId;
                        MainModule = LinkedDominatorCore.Enums.LdMainModules.Scraper.ToString();
                        SubModule = ActivityType.CompanyScraper.ToString();
                        break;

                    case ActivityType.GroupMemberScraper:

                        GroupMemberScraper objGroupMemberScraper = GroupMemberScraper.GetSingeltonObjectGroupMemberScraper();
                        CampaignName = objGroupMemberScraper.CampaignName;
                        TemplateId = objGroupMemberScraper.TemplateId;
                        MainModule = LinkedDominatorCore.Enums.LdMainModules.Scraper.ToString();
                        SubModule = ActivityType.GroupMemberScraper.ToString();
                        break;

                    case ActivityType.SalesNavigatorUserScraper:
                        break;

                }
                #endregion

                var existingCampaign = CampaignsFileManager.Get()
                                                       .Where(x => x.CampaignName == CampaignName)
                                                       .FirstOrDefault();

                // If campaign with such name already exists
                if (existingCampaign != null)
                {
                    string warningMessege = "This account is already running with " + moduleType + " configuration from another campaign. Saving this settings will override previous settings and remove this account from the campaign.\r\nWould you still like to proceed?";

                    var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Warning",
                            warningMessege, MessageDialogStyle.AffirmativeAndNegative, Dialog.SetMetroDialogButton());


                    if (dialogResult == MessageDialogResult.Negative)
                        return;

                    // Update campaign
                    existingCampaign.CampaignName = CampaignName;
                    existingCampaign.MainModule = MainModule;
                    existingCampaign.SubModule = SubModule;
                    existingCampaign.SocialNetworks = SocialNetworks.LinkedIn;
                    existingCampaign.SelectedAccountList = listSelectedAccounts;
                    existingCampaign.TemplateId = TemplateId;
                    existingCampaign.CreationDate = DateTimeUtilities.GetEpochTime();
                    existingCampaign.Status = "Active";
                    existingCampaign.LastEditedDate = DateTimeUtilities.GetEpochTime();

                    // update exitsting Campaign 
                    CampaignsFileManager.Edit(existingCampaign);
                }

                // add new campaign
                else
                {
                    var newCampaign = new CampaignDetails()
                    {
                        CampaignName = CampaignName,
                        MainModule = MainModule,
                        SubModule = SubModule,
                        SocialNetworks = SocialNetworks.LinkedIn,
                        SelectedAccountList = listSelectedAccounts,
                        TemplateId = TemplateId,
                        CreationDate = DateTimeUtilities.GetEpochTime(),
                        Status = "Active",
                        LastEditedDate = DateTimeUtilities.GetEpochTime(),
                    };

                    // create new database for campaign
                    //DataBaseHandler.CreateDataBase(newCampaign.CampaignId, SocialNetworks.LinkedIn, DatabaseType.CampaignType);
                    CampaignsFileManager.Add(newCampaign);
                }
            }
            catch (Exception Ex)
            {
                Ex.ErrorLog();
            }
        }
    }
    public static class CampaignHelper
    {
        /// <summary>
        /// Callback on Edit/Duplicate campaign from Campaigns screen
        /// </summary>
        /// <param name="templateDetails"></param>
        /// <param name="campaignDetails"></param>
        /// <param name="IsEditCampaignName"></param>
        /// <param name="CancelEditVisibility"></param>
        /// <param name="CampaignButtonContent"></param>
        /// <param name="TemplateID"></param>
        public static void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails, bool IsEditCampaignName, Visibility CancelEditVisibility, string CampaignButtonContent, string TemplateID)
        {
            try
            {
                var ActivityType = (ActivityType)Enum.Parse(typeof(ActivityType), templateDetails.ActivityType);

                switch (ActivityType)
                {
                    case ActivityType.ConnectionRequest:

                        ConnectionRequest objConnectionRequest = ConnectionRequest.GetSingeltonObjectConnectionRequest();
                        objConnectionRequest.IsEditCampaignName = IsEditCampaignName;
                        objConnectionRequest.CancelEditVisibility = CancelEditVisibility;
                        objConnectionRequest.TemplateId = TemplateID;
                        objConnectionRequest.CampaignName = campaignDetails.CampaignName;
                        objConnectionRequest.CampaignButtonContent = CampaignButtonContent;
                        objConnectionRequest.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objConnectionRequest.ConnectionFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objConnectionRequest.ObjViewModel.ConnectionRequestModel =
                            JsonConvert.DeserializeObject<ConnectionRequestModel>(templateDetails.ActivitySettings);

                        objConnectionRequest.MainGrid.DataContext = objConnectionRequest.ObjViewModel.ConnectionRequestModel;

                        TabSwitcher.ChangeTabIndex(1, 0);
                        break;
                    case ActivityType.AcceptConnectionRequest:

                        LinkedDominatorUI.LDViews.GrowConnection.AcceptConnectionRequest objAcceptConnectionRequest = LinkedDominatorUI.LDViews.GrowConnection.AcceptConnectionRequest.GetSingeltonObjectAcceptConnectionRequest();
                        objAcceptConnectionRequest.IsEditCampaignName = IsEditCampaignName;
                        objAcceptConnectionRequest.CancelEditVisibility = CancelEditVisibility;
                        objAcceptConnectionRequest.TemplateId = TemplateID;
                        objAcceptConnectionRequest.CampaignName = campaignDetails.CampaignName;
                        objAcceptConnectionRequest.CampaignButtonContent = CampaignButtonContent;
                        objAcceptConnectionRequest.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objAcceptConnectionRequest.AcceptConnectionFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objAcceptConnectionRequest.ObjViewModel.AcceptConnectionRequestModel =
                            JsonConvert.DeserializeObject<AcceptConnectionRequestModel>(templateDetails.ActivitySettings);

                        objAcceptConnectionRequest.MainGrid.DataContext = objAcceptConnectionRequest.ObjViewModel.AcceptConnectionRequestModel;

                        TabSwitcher.ChangeTabIndex(1, 1);
                        break;

                    case ActivityType.RemoveOrWithdrawConnections:

                        RemoveOrWithdrawConnections objRemoveConnections = RemoveOrWithdrawConnections.GetSingeltonObjectRemoveOrWithdrawConnections();
                        objRemoveConnections.IsEditCampaignName = IsEditCampaignName;
                        objRemoveConnections.CancelEditVisibility = CancelEditVisibility;
                        objRemoveConnections.TemplateId = TemplateID;
                        objRemoveConnections.CampaignName = campaignDetails.CampaignName;
                        objRemoveConnections.CampaignButtonContent = CampaignButtonContent;
                        objRemoveConnections.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objRemoveConnections.RemoveConnectionsFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objRemoveConnections.ObjViewModel.RemoveOrWithdrawConnectionsModel =
                            JsonConvert.DeserializeObject<RemoveOrWithdrawConnectionsModel>(templateDetails.ActivitySettings);

                        objRemoveConnections.MainGrid.DataContext = objRemoveConnections.ObjViewModel.RemoveOrWithdrawConnectionsModel;

                        TabSwitcher.ChangeTabIndex(1, 2);
                        break;
                    case ActivityType.BroadcastMessages:

                        BroadcastMessages objBroadcastMessages = BroadcastMessages.GetSingeltonBroadcastMessages();
                        objBroadcastMessages.IsEditCampaignName = IsEditCampaignName;
                        objBroadcastMessages.CancelEditVisibility = CancelEditVisibility;
                        objBroadcastMessages.TemplateId = TemplateID;
                        objBroadcastMessages.CampaignName = campaignDetails.CampaignName;
                        objBroadcastMessages.CampaignButtonContent = CampaignButtonContent;
                        objBroadcastMessages.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objBroadcastMessages.BrodCastMessageFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objBroadcastMessages.ObjViewModel.BroadcastMessagesModel =
                            JsonConvert.DeserializeObject<BroadcastMessagesModel>(templateDetails.ActivitySettings);

                        objBroadcastMessages.MainGrid.DataContext = objBroadcastMessages.ObjViewModel.BroadcastMessagesModel;

                        TabSwitcher.ChangeTabIndex(2, 0);
                        break;

                    case ActivityType.AutoReplyToNewMessage:

                        AutoReplyToNewMessage objAutoReplyToNewMessage = AutoReplyToNewMessage.GetSingeltonAutoReplyToNewMessage();
                        objAutoReplyToNewMessage.IsEditCampaignName = IsEditCampaignName;
                        objAutoReplyToNewMessage.CancelEditVisibility = CancelEditVisibility;
                        objAutoReplyToNewMessage.TemplateId = TemplateID;
                        objAutoReplyToNewMessage.CampaignName = campaignDetails.CampaignName;
                        objAutoReplyToNewMessage.CampaignButtonContent = CampaignButtonContent;
                        objAutoReplyToNewMessage.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objAutoReplyToNewMessage.AutoReplyToNewMessageFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objAutoReplyToNewMessage.ObjViewModel.AutoReplyToNewMessageModel =
                            JsonConvert.DeserializeObject<AutoReplyToNewMessageModel>(templateDetails.ActivitySettings);

                        objAutoReplyToNewMessage.MainGrid.DataContext = objAutoReplyToNewMessage.ObjViewModel.AutoReplyToNewMessageModel;

                        TabSwitcher.ChangeTabIndex(2, 1);
                        break;

                    case ActivityType.SendMessageToNewConnection:

                        SendMessageToNewConnection objSendMessageToNewConnection = SendMessageToNewConnection.GetSingeltonSendMessageToNewConnection();
                        objSendMessageToNewConnection.IsEditCampaignName = IsEditCampaignName;
                        objSendMessageToNewConnection.CancelEditVisibility = CancelEditVisibility;
                        objSendMessageToNewConnection.TemplateId = TemplateID;
                        objSendMessageToNewConnection.CampaignName = campaignDetails.CampaignName;
                        objSendMessageToNewConnection.CampaignButtonContent = CampaignButtonContent;
                        objSendMessageToNewConnection.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objSendMessageToNewConnection.SendMessageToNewConnectionFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objSendMessageToNewConnection.ObjViewModel.SendMessageToNewConnectionModel =
                            JsonConvert.DeserializeObject<SendMessageToNewConnectionModel>(templateDetails.ActivitySettings);

                        objSendMessageToNewConnection.MainGrid.DataContext = objSendMessageToNewConnection.ObjViewModel.SendMessageToNewConnectionModel;

                        TabSwitcher.ChangeTabIndex(2, 2);
                        break;

                    case ActivityType.SendGreetingsToConnections:

                        SendGreetingsToConnections objSendGreetingsToConnections = SendGreetingsToConnections.GetSingeltonSendGreetingsToConnections();
                        objSendGreetingsToConnections.IsEditCampaignName = IsEditCampaignName;
                        objSendGreetingsToConnections.CancelEditVisibility = CancelEditVisibility;
                        objSendGreetingsToConnections.TemplateId = TemplateID;
                        objSendGreetingsToConnections.CampaignName = campaignDetails.CampaignName;
                        objSendGreetingsToConnections.CampaignButtonContent = CampaignButtonContent;
                        objSendGreetingsToConnections.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objSendGreetingsToConnections.SendGreetingsToConnectionsFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objSendGreetingsToConnections.ObjViewModel.SendGreetingsToConnectionsModel =
                            JsonConvert.DeserializeObject<SendGreetingsToConnectionsModel>(templateDetails.ActivitySettings);

                        objSendGreetingsToConnections.MainGrid.DataContext = objSendGreetingsToConnections.ObjViewModel.SendGreetingsToConnectionsModel;

                        TabSwitcher.ChangeTabIndex(2, 3);
                        break;

                    case ActivityType.Like:

                        Like objLike = Like.GetSingeltonObjectLike();
                        objLike.IsEditCampaignName = IsEditCampaignName;
                        objLike.CancelEditVisibility = CancelEditVisibility;
                        objLike.TemplateId = TemplateID;
                        objLike.CampaignName = campaignDetails.CampaignName;
                        objLike.CampaignButtonContent = CampaignButtonContent;
                        objLike.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objLike.LikeFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objLike.ObjViewModel.LikeModel =
                            JsonConvert.DeserializeObject<LikeModel>(templateDetails.ActivitySettings);

                        objLike.MainGrid.DataContext = objLike.ObjViewModel.LikeModel;

                        TabSwitcher.ChangeTabIndex(3, 0);
                        break;

                    case ActivityType.Comment:

                        Comment objComment = Comment.GetSingeltonObjectComment();
                        objComment.IsEditCampaignName = IsEditCampaignName;
                        objComment.CancelEditVisibility = CancelEditVisibility;
                        objComment.TemplateId = TemplateID;
                        objComment.CampaignName = campaignDetails.CampaignName;
                        objComment.CampaignButtonContent = CampaignButtonContent;
                        objComment.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objComment.CommentFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objComment.ObjViewModel.CommentModel =
                            JsonConvert.DeserializeObject<CommentModel>(templateDetails.ActivitySettings);

                        objComment.MainGrid.DataContext = objComment.ObjViewModel.CommentModel;

                        TabSwitcher.ChangeTabIndex(3, 1);
                        break;

                    case ActivityType.GroupJoiner:
                        GroupJoiner objGroupJoiner = GroupJoiner.GetSingeltonObjectGroupJoiner();
                        objGroupJoiner.IsEditCampaignName = IsEditCampaignName;
                        objGroupJoiner.CancelEditVisibility = CancelEditVisibility;
                        objGroupJoiner.TemplateId = TemplateID;
                        objGroupJoiner.CampaignName = campaignDetails.CampaignName;
                        objGroupJoiner.CampaignButtonContent = CampaignButtonContent;
                        objGroupJoiner.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objGroupJoiner.GroupJoinerFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objGroupJoiner.ObjViewModel.GroupJoinerModel =
                            JsonConvert.DeserializeObject<GroupJoinerModel>(templateDetails.ActivitySettings);

                        objGroupJoiner.MainGrid.DataContext = objGroupJoiner.ObjViewModel.GroupJoinerModel;

                        TabSwitcher.ChangeTabIndex(4, 0);
                        break;

                    case ActivityType.GroupUnJoiner:
                        GroupUnJoiner objGroupUnJoiner = GroupUnJoiner.GetSingeltonObjectGroupUnJoiner();
                        objGroupUnJoiner.IsEditCampaignName = IsEditCampaignName;
                        objGroupUnJoiner.CancelEditVisibility = CancelEditVisibility;
                        objGroupUnJoiner.TemplateId = TemplateID;
                        objGroupUnJoiner.CampaignName = campaignDetails.CampaignName;
                        objGroupUnJoiner.CampaignButtonContent = CampaignButtonContent;
                        objGroupUnJoiner.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objGroupUnJoiner.GroupUnJoinerFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objGroupUnJoiner.ObjViewModel.GroupUnJoinerModel =
                            JsonConvert.DeserializeObject<GroupUnJoinerModel>(templateDetails.ActivitySettings);

                        objGroupUnJoiner.MainGrid.DataContext = objGroupUnJoiner.ObjViewModel.GroupUnJoinerModel;

                        TabSwitcher.ChangeTabIndex(4, 1);
                        break;
                    case ActivityType.UserScraper:
                        UserScraper objUserScraper = UserScraper.GetSingeltonObjectUserScraper();
                        objUserScraper.IsEditCampaignName = IsEditCampaignName;
                        objUserScraper.CancelEditVisibility = CancelEditVisibility;
                        objUserScraper.TemplateId = TemplateID;
                        objUserScraper.CampaignName = campaignDetails.CampaignName;
                        objUserScraper.CampaignButtonContent = CampaignButtonContent;
                        objUserScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objUserScraper.UserScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objUserScraper.ObjViewModel.UserScraperModel =
                            JsonConvert.DeserializeObject<UserScraperModel>(templateDetails.ActivitySettings);

                        objUserScraper.MainGrid.DataContext = objUserScraper.ObjViewModel.UserScraperModel;

                        TabSwitcher.ChangeTabIndex(5, 0);
                        break;
                    case ActivityType.JobScraper:
                        JobScraper objJobScraper = JobScraper.GetSingeltonObjectJobScraper();
                        objJobScraper.IsEditCampaignName = IsEditCampaignName;
                        objJobScraper.CancelEditVisibility = CancelEditVisibility;
                        objJobScraper.TemplateId = TemplateID;
                        objJobScraper.CampaignName = campaignDetails.CampaignName;
                        objJobScraper.CampaignButtonContent = CampaignButtonContent;
                        objJobScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objJobScraper.JobScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objJobScraper.ObjViewModel.JobScraperModel =
                            JsonConvert.DeserializeObject<JobScraperModel>(templateDetails.ActivitySettings);

                        objJobScraper.MainGrid.DataContext = objJobScraper.ObjViewModel.JobScraperModel;

                        TabSwitcher.ChangeTabIndex(5, 1);
                        break;
                    case ActivityType.CompanyScraper:
                        CompanyScraper objCompanyScraper = CompanyScraper.GetSingeltonObjectCompanyScraper();
                        objCompanyScraper.IsEditCampaignName = IsEditCampaignName;
                        objCompanyScraper.CancelEditVisibility = CancelEditVisibility;
                        objCompanyScraper.TemplateId = TemplateID;
                        objCompanyScraper.CampaignName = campaignDetails.CampaignName;
                        objCompanyScraper.CampaignButtonContent = CampaignButtonContent;
                        objCompanyScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objCompanyScraper.CompanyScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objCompanyScraper.ObjViewModel.CompanyScraperModel =
                            JsonConvert.DeserializeObject<CompanyScraperModel>(templateDetails.ActivitySettings);

                        objCompanyScraper.MainGrid.DataContext = objCompanyScraper.ObjViewModel.CompanyScraperModel;

                        TabSwitcher.ChangeTabIndex(5, 2);
                        break;
                    case ActivityType.GroupMemberScraper:
                        GroupMemberScraper objGroupMemberScraper = GroupMemberScraper.GetSingeltonObjectGroupMemberScraper();
                        objGroupMemberScraper.IsEditCampaignName = IsEditCampaignName;
                        objGroupMemberScraper.CancelEditVisibility = CancelEditVisibility;
                        objGroupMemberScraper.TemplateId = TemplateID;
                        objGroupMemberScraper.CampaignName = campaignDetails.CampaignName;
                        objGroupMemberScraper.CampaignButtonContent = CampaignButtonContent;
                        objGroupMemberScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objGroupMemberScraper.GroupMemberScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objGroupMemberScraper.ObjViewModel.GroupMemberScraperModel =
                            JsonConvert.DeserializeObject<GroupMemberScraperModel>(templateDetails.ActivitySettings);

                        objGroupMemberScraper.MainGrid.DataContext = objGroupMemberScraper.ObjViewModel.GroupMemberScraperModel;

                        TabSwitcher.ChangeTabIndex(5, 3);
                        break;
                    case ActivityType.ProfileEndorsement:
                        ProfileEndorsement objProfileEndorsement = ProfileEndorsement.GetSingeltonObjectProfileEndorsement();
                        objProfileEndorsement.IsEditCampaignName = IsEditCampaignName;
                        objProfileEndorsement.CancelEditVisibility = CancelEditVisibility;
                        objProfileEndorsement.TemplateId = TemplateID;
                        objProfileEndorsement.CampaignName = campaignDetails.CampaignName;
                        objProfileEndorsement.CampaignButtonContent = CampaignButtonContent;
                        objProfileEndorsement.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
                        objProfileEndorsement.ProfileEndorsementFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
                        objProfileEndorsement.ObjViewModel.ProfileEndorsementModel =
                            JsonConvert.DeserializeObject<ProfileEndorsementModel>(templateDetails.ActivitySettings);

                        objProfileEndorsement.MainGrid.DataContext = objProfileEndorsement.ObjViewModel.ProfileEndorsementModel;

                        TabSwitcher.ChangeTabIndex(6, 0);
                        break;

                }
            }
            catch (Exception ex)
            {
                ex.TraceLog();
            }
        }
    }
    public static class ReportHelper
    {
        static ObservableCollection<InteractedUsersReportModel> objInteractedUsersReportModel = new ObservableCollection<InteractedUsersReportModel>();
        static ObservableCollection<InteractedGroupReportModel> objGroupJoinerReportModel = new ObservableCollection<InteractedGroupReportModel>();
        static ObservableCollection<UserScraperReportModel> objUserScraperReportModel = new ObservableCollection<UserScraperReportModel>();
        static ObservableCollection<InteractedJobsReportModel> objJobScraperReportModel = new ObservableCollection<InteractedJobsReportModel>();
        static ObservableCollection<InteractedCompanyReportModel> objCompanyScraperReportModel = new ObservableCollection<InteractedCompanyReportModel>();



        static string Header = string.Empty;
        /// <summary>
        /// this method will return header for csv file 
        /// </summary>
        /// <returns></returns>
        internal static string GetHeader()
        {
            return Header;
        }

        /// <summary>
        /// ObjReports=>this is Report UI object
        /// lstCurrentQueries=>this is dictionary of current campaign QueryValue(key) and QueryType(value)
        /// DbCampaignOperations=>this is DataBaseConnectionCodeFirst.DataBaseConnection Object
        /// campaign=>this is CampaignDetails Object
        /// this method return report details
        /// </summary>
        /// <returns></returns>
        internal static int GetReportDetail(Reports objReports, List<KeyValuePair<string, string>> lstCurrentQueries, DbOperations DbCampaignOperations, CampaignDetails campaign)
        {
            DbCampaignOperations = new DbOperations(campaign.CampaignId, SocialNetworks.LinkedIn, ConstantVariable.GetCampaignDb);

            if (campaign.SubModule == ActivityType.ConnectionRequest.ToString())
            {
                GenerateConnectionRequestReport(objReports, lstCurrentQueries, DbCampaignOperations, campaign);
            }
            else if (campaign.SubModule == ActivityType.AcceptConnectionRequest.ToString())
            {
                GenerateAcceptConnectionRequestReport(objReports, lstCurrentQueries, DbCampaignOperations, campaign);
            }
            else if (campaign.SubModule == ActivityType.RemoveOrWithdrawConnections.ToString())
            {
                GenerateRemoveOrWithdrawConnectionsReport(objReports, lstCurrentQueries, DbCampaignOperations, campaign);
            }
            else if (campaign.SubModule == ActivityType.BroadcastMessages.ToString() || campaign.SubModule == ActivityType.AutoReplyToNewMessage.ToString() || campaign.SubModule == ActivityType.SendMessageToNewConnection.ToString())
            {
                MessengerReport(objReports, lstCurrentQueries, DbCampaignOperations, campaign);
            }
            else if (campaign.SubModule == ActivityType.GroupJoiner.ToString())
            {
                GenerateGroupJoinerReport(objReports, lstCurrentQueries, DbCampaignOperations, campaign);
            }
            else if (campaign.SubModule == ActivityType.GroupUnJoiner.ToString())
            {
                GenerateGroupUnJoinerReport(objReports, lstCurrentQueries, DbCampaignOperations, campaign);
            }
            else if (campaign.SubModule == ActivityType.UserScraper.ToString() || campaign.SubModule == ActivityType.GroupMemberScraper.ToString())
            {
                GenerateUserScraperReport(objReports, lstCurrentQueries, DbCampaignOperations, campaign);
            }
            else if (campaign.SubModule == ActivityType.JobScraper.ToString())
            {
                GenerateJobScraperReport(objReports, lstCurrentQueries, DbCampaignOperations, campaign);
            }

            else if (campaign.SubModule == ActivityType.CompanyScraper.ToString())
            {
                GenerateCompanyScraperReport(objReports, lstCurrentQueries, DbCampaignOperations, campaign);
            }
            else if (campaign.SubModule == ActivityType.ProfileEndorsement.ToString())
            {
                GeneratProfileEndorsementReport(objReports, lstCurrentQueries, DbCampaignOperations, campaign);
            }
            return objReports.ReportModel.ReportCollection.Cast<object>().Count();
        }

        private static void GenerateConnectionRequestReport(Reports objReports, List<KeyValuePair<string, string>> lstCurrentQueries, DbOperations DbCampaignOperations, CampaignDetails campaign)
        {
            try
            {
                Header = "AccountEmail,QueryType,QueryValue,ActivityType,UserFullName,UserProfileUrl,Personal Note,RequestedDate";

                objInteractedUsersReportModel.Clear();

                #region get data from InteractedUsers table and add to ConnectionRequestReportModel

                DbCampaignOperations.Get<InteractedUsers>().ForEach(
                    ReportItem =>
                    {
                        var queryDetails = lstCurrentQueries.FirstOrDefault(x => x.Key == ReportItem.QueryValue);
                        if (queryDetails.Key == ReportItem.QueryValue && queryDetails.Value == ReportItem.QueryType)
                        {
                            objInteractedUsersReportModel.Add(new InteractedUsersReportModel()
                            {
                                Id = ReportItem.Id,
                                AccountEmail = ReportItem.AccountEmail,
                                QueryType = ReportItem.QueryType,
                                QueryValue = ReportItem.QueryValue,
                                ActivityType = ReportItem.ActivityType,
                                UserFullName = ReportItem.UserFullName,
                                UserProfileUrl = ReportItem.UserProfileUrl,
                                DetailedUserInfo = ReportItem.DetailedUserInfo,
                                InteractionDateTime = ReportItem.InteractionTimeStamp.EpochToDateTimeUtc().ToLocalTime()
                            });
                        }
                    });

                objReports.ReportModel.ReportCollection =
                    CollectionViewSource.GetDefaultView(objInteractedUsersReportModel);

                #endregion

                #region Generate Reports column with data

                //campaign.SelectedAccountList.ToList().ForEach(x =>
                //{
                objReports.ReportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "AccountEmail"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "User FullName",ColumnBindingText="UserFullName" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "User ProfileUrl",ColumnBindingText="UserProfileUrl" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "Requested Date",ColumnBindingText="InteractionDateTime" }
                    };
                //});

                #endregion
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        private static void GenerateAcceptConnectionRequestReport(Reports objReports, List<KeyValuePair<string, string>> lstCurrentQueries, DbOperations DbCampaignOperations, CampaignDetails campaign)
        {
            try
            {
                Header = "AccountEmail,QueryType,QueryValue,ActivityType,UserFullName,UserProfileUrl,AcceptedDate";

                objInteractedUsersReportModel.Clear();

                #region get data from InteractedUsers table and add to ConnectionRequestReportModel

                DbCampaignOperations.Get<InteractedUsers>().ForEach(
                    ReportItem =>
                    {
                        //if (lstCurrentQueries.ContainsKey(ReportItem.QueryValue) && lstCurrentQueries.ContainsValue(ReportItem.QueryType))
                        //{
                        objInteractedUsersReportModel.Add(new InteractedUsersReportModel()
                        {
                            Id = ReportItem.Id,
                            AccountEmail = ReportItem.AccountEmail,
                            QueryType = ReportItem.QueryType,
                            QueryValue = ReportItem.QueryValue,
                            ActivityType = ReportItem.ActivityType,
                            UserFullName = ReportItem.UserFullName,
                            UserProfileUrl = ReportItem.UserProfileUrl,
                            DetailedUserInfo = string.Empty,
                            InteractionDateTime = ReportItem.InteractionTimeStamp.EpochToDateTimeUtc().ToLocalTime()
                        });
                        //}
                    });

                objReports.ReportModel.ReportCollection =
                    CollectionViewSource.GetDefaultView(objInteractedUsersReportModel);

                #endregion

                #region Generate Reports column with data

                //campaign.SelectedAccountList.ToList().ForEach(x =>
                //{
                objReports.ReportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "AccountEmail"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "User FullName",ColumnBindingText="UserFullName" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "User ProfileUrl",ColumnBindingText="UserProfileUrl" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "Accepted Date",ColumnBindingText="InteractionDateTime" }
                    };
                //});

                #endregion
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        private static void GenerateRemoveOrWithdrawConnectionsReport(Reports objReports, List<KeyValuePair<string, string>> lstCurrentQueries,DbOperations DbCampaignOperations, CampaignDetails campaign)
        {
            try
            {
                Header = "AccountEmail,QueryType,QueryValue,ActivityType,UserFullName,UserProfileUrl,InteractionDateTime";

                objInteractedUsersReportModel.Clear();

                #region get data from InteractedUsers table and add to ConnectionRequestReportModel

                DbCampaignOperations.Get<InteractedUsers>().ForEach(
                    ReportItem =>
                    {
                        //Note : ReportItem.DetailedUserInfo is used to differentiate Remove from Withdraw
                        objInteractedUsersReportModel.Add(new InteractedUsersReportModel()
                        {
                            Id = ReportItem.Id,
                            AccountEmail = ReportItem.AccountEmail,
                            QueryType = ReportItem.QueryType = "N/A",
                            QueryValue = ReportItem.QueryValue = "N/A",
                            ActivityType = ReportItem.DetailedUserInfo,
                            UserFullName = ReportItem.UserFullName,
                            UserProfileUrl = ReportItem.UserProfileUrl,
                            InteractionDateTime = ReportItem.InteractionTimeStamp.EpochToDateTimeUtc().ToLocalTime()
                        });
                    });

                objReports.ReportModel.ReportCollection =
                    CollectionViewSource.GetDefaultView(objInteractedUsersReportModel);

                #endregion

                #region Generate Reports column with data

                //campaign.SelectedAccountList.ToList().ForEach(x =>
                //{
                objReports.ReportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "AccountEmail"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "User FullName",ColumnBindingText="UserFullName" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "User ProfileUrl",ColumnBindingText="UserProfileUrl" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "Interacted Date",ColumnBindingText="InteractionDateTime" }
                    };
                //});

                #endregion
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        private static void MessengerReport(Reports objReports, List<KeyValuePair<string, string>> lstCurrentQueries, DbOperations DbCampaignOperations, CampaignDetails campaign)
        {
            try
            {
                Header = "AccountEmail,QueryType,QueryValue,ActivityType,UserFullName,UserProfileUrl,Message,Messaged Date";

                objInteractedUsersReportModel.Clear();

                #region get data from InteractedUsers table and add to ConnectionRequestReportModel

                DbCampaignOperations.Get<InteractedUsers>().ForEach(
                    ReportItem =>
                    {
                        objInteractedUsersReportModel.Add(new InteractedUsersReportModel()
                        {
                            Id = ReportItem.Id,
                            AccountEmail = ReportItem.AccountEmail,
                            QueryType = ReportItem.QueryType = "N/A",
                            QueryValue = ReportItem.QueryValue = "N/A",
                            ActivityType = ReportItem.ActivityType,
                            UserFullName = ReportItem.UserFullName,
                            UserProfileUrl = ReportItem.UserProfileUrl,
                            DetailedUserInfo = ReportItem.DetailedUserInfo,
                            InteractionDateTime = ReportItem.InteractionTimeStamp.EpochToDateTimeUtc().ToLocalTime()
                        });
                    });

                objReports.ReportModel.ReportCollection =
                    CollectionViewSource.GetDefaultView(objInteractedUsersReportModel);

                #endregion

                #region Generate Reports column with data

                //campaign.SelectedAccountList.ToList().ForEach(x =>
                //{
                objReports.ReportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "AccountEmail"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "User FullName",ColumnBindingText="UserFullName" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "User ProfileUrl",ColumnBindingText="UserProfileUrl" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "Messaged Date",ColumnBindingText="InteractionDateTime" }
                    };
                //});

                #endregion
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        private static void GenerateGroupJoinerReport(Reports objReports, List<KeyValuePair<string, string>> lstCurrentQueries, DbOperations DbCampaignOperations, CampaignDetails campaign)
        {
            try
            {
                Header = "AccountEmail,QueryType,QueryValue,ActivityType,GroupName,GroupUrl,TotalMembers,JoinRequestDate";

                objGroupJoinerReportModel.Clear();

                #region get data from InteractedGroups table and add to GroupJoinerReportMode

                DbCampaignOperations.Get<InteractedGroups>().ForEach(
                    ReportItem =>
                    {
                        var queryDetails = lstCurrentQueries.FirstOrDefault(x => x.Key == ReportItem.QueryValue);
                        if (queryDetails.Key == ReportItem.QueryValue && queryDetails.Value == ReportItem.QueryType)
                        {
                            objGroupJoinerReportModel.Add(new InteractedGroupReportModel()
                            {
                                Id = ReportItem.Id,
                                AccountEmail = ReportItem.AccountEmail,
                                QueryType = ReportItem.QueryType,
                                QueryValue = ReportItem.QueryValue,
                                ActivityType = ReportItem.ActivityType,
                                GroupName = ReportItem.GroupName,
                                GroupUrl = ReportItem.GroupUrl,
                                TotalMembers = ReportItem.TotalMembers,
                                CommunityType = ReportItem.CommunityType,
                                InteractionDateTime = ReportItem.InteractionTimeStamp.EpochToDateTimeUtc().ToLocalTime()
                            });
                        }
                    });

                objReports.ReportModel.ReportCollection =
                    CollectionViewSource.GetDefaultView(objGroupJoinerReportModel);

                #endregion

                #region Generate Reports column with data

                //campaign.SelectedAccountList.ToList().ForEach(x =>
                //{
                objReports.ReportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "AccountEmail"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Group Name",ColumnBindingText="GroupName" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "Total Members",ColumnBindingText="TotalMembers" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "Community Type",ColumnBindingText="CommunityType" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "JoinRequest Date",ColumnBindingText="InteractionDateTime" }
                    };
                //});

                #endregion
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        private static void GenerateGroupUnJoinerReport(Reports objReports, List<KeyValuePair<string, string>> lstCurrentQueries, DbOperations DbCampaignOperations, CampaignDetails campaign)
        {
            try
            {
                Header = "AccountEmail,QueryType,QueryValue,ActivityType,GroupName,GroupUrl,TotalMembers,UnJoinedDate";

                objGroupJoinerReportModel.Clear();

                #region get data from InteractedGroups table and add to GroupJoinerReportMode

                DbCampaignOperations.Get<InteractedGroups>().ForEach(
                    ReportItem =>
                    {
                        objGroupJoinerReportModel.Add(new InteractedGroupReportModel()
                        {
                            Id = ReportItem.Id,
                            AccountEmail = ReportItem.AccountEmail,
                            QueryType = ReportItem.QueryType = "N/A",
                            QueryValue = ReportItem.QueryValue = "N/A",
                            ActivityType = ReportItem.ActivityType,
                            GroupName = ReportItem.GroupName,
                            GroupUrl = ReportItem.GroupUrl,
                            TotalMembers = ReportItem.TotalMembers,
                            CommunityType = ReportItem.CommunityType,
                            InteractionDateTime = ReportItem.InteractionTimeStamp.EpochToDateTimeUtc().ToLocalTime()
                        });
                    });

                objReports.ReportModel.ReportCollection =
                    CollectionViewSource.GetDefaultView(objGroupJoinerReportModel);

                #endregion

                #region Generate Reports column with data

                //campaign.SelectedAccountList.ToList().ForEach(x =>
                //{
                objReports.ReportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "AccountEmail"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Group Name",ColumnBindingText="GroupName" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "Total Members",ColumnBindingText="TotalMembers" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "Community Type",ColumnBindingText="CommunityType" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "UnJoined Date",ColumnBindingText="InteractionDateTime" }
                    };
                //});

                #endregion
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        private static void GenerateUserScraperReport(Reports objReports, List<KeyValuePair<string, string>> lstCurrentQueries, DbOperations DbCampaignOperations, CampaignDetails campaign)
        {
            try
            {
                Header = "AccountEmail,AccountUserFullName,AccountUserProfileUrl,QueryType,QueryValue,ActivityType,UserFullName,UserProfileUrl,HeadlineTitle,EmailId,PersonalPhoneNumber,PersonalWebsites,Birthdate,TwitterUrl,Titlecurrent,Companycurrent,CurrentCompanyUrl,CompanyDescription,CurrentCompanyWebsite,Location,Industry,Country,ConnectionCount,Recomandation,Skill,Experience,EducationCollection,PastTitles,PastCompany,ScrapedDate";

                objUserScraperReportModel.Clear();

                #region get data from InteractedUsers table and add to UserScraperReportModel
                int Count = 1;
                DbCampaignOperations.Get<InteractedUsers>().ForEach(
                    ReportItem =>
                    {
                        var queryDetails = lstCurrentQueries.FirstOrDefault(x => x.Key == ReportItem.QueryValue);
                        if (queryDetails.Key == ReportItem.QueryValue && queryDetails.Value == ReportItem.QueryType)
                        {
                            objUserScraperReportModel.Add(new UserScraperReportModel()
                            {
                                Id = Count++,
                                AccountEmail = ReportItem.AccountEmail,
                                QueryType = ReportItem.QueryType,
                                QueryValue = ReportItem.QueryValue,
                                ActivityType = ReportItem.ActivityType,
                                UserFullName = ReportItem.UserFullName,
                                UserProfileUrl = ReportItem.UserProfileUrl,
                                DetailedUserInfo = ReportItem.DetailedUserInfo,
                                ScrapedDate = ReportItem.InteractionTimeStamp.EpochToDateTimeUtc().ToLocalTime()
                            });
                        }
                    });

                objReports.ReportModel.ReportCollection =
                    CollectionViewSource.GetDefaultView(objUserScraperReportModel);

                #endregion

                #region Generate Reports column with data

                //campaign.SelectedAccountList.ToList().ForEach(x =>
                //{
                objReports.ReportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "AccountEmail"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "User FullName",ColumnBindingText="UserFullName" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "User ProfileUrl",ColumnBindingText="UserProfileUrl" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "Scraped Date",ColumnBindingText="ScrapedDate" }
                    };
                //});

                #endregion
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        private static void GenerateJobScraperReport(Reports objReports, List<KeyValuePair<string, string>> lstCurrentQueries, DbOperations DbCampaignOperations, CampaignDetails campaign)
        {
            try
            {
                Header = "AccountEmail,AccountUserFullName,AccountUserProfileUrl,QueryType,QueryValue,ActivityType,JobPostUrl,CompanyName,CompanyWebsite,JobTitle,JobLocation,PostedOn,NumberOfApplicants,Industry,EmploymentType,SeniorityLevel,JobFunction,JobPosterProfileUrl,JobPosterFullName,DegreeOfConnection,ScrapedDate";

                objJobScraperReportModel.Clear();

                #region get data from InteractedJobs table and add to JobScraperReportModel
                int Count = 1;
                DbCampaignOperations.Get<InteractedJobs>().ForEach(
                    ReportItem =>
                    {
                        var queryDetails = lstCurrentQueries.FirstOrDefault(x => x.Key == ReportItem.QueryValue);
                        if (queryDetails.Key == ReportItem.QueryValue && queryDetails.Value == ReportItem.QueryType)
                        {
                            objJobScraperReportModel.Add(new InteractedJobsReportModel()
                            {
                                Id = Count++,
                                AccountEmail = ReportItem.AccountEmail,
                                QueryType = ReportItem.QueryType,
                                QueryValue = ReportItem.QueryValue,
                                ActivityType = ReportItem.ActivityType,
                                JobTitle = ReportItem.JobTitle,
                                JobPostUrl = ReportItem.JobPostUrl,
                                DetailedInfo = ReportItem.DetailedInfo,
                                InteractedDateTime = ReportItem.InteractionTimeStamp.EpochToDateTimeUtc().ToLocalTime()
                            });
                        }
                    });

                objReports.ReportModel.ReportCollection =
                    CollectionViewSource.GetDefaultView(objJobScraperReportModel);

                #endregion

                #region Generate Reports column with data

                //campaign.SelectedAccountList.ToList().ForEach(x =>
                //{
                objReports.ReportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "AccountEmail"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Job Title",ColumnBindingText="JobTitle" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "Job Post Url",ColumnBindingText="JobPostUrl" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "Scraped Date",ColumnBindingText="ScrapedDate" }
                    };
                //});

                #endregion
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        private static void GenerateCompanyScraperReport(Reports objReports, List<KeyValuePair<string, string>> lstCurrentQueries, DbOperations DbCampaignOperations, CampaignDetails campaign)
        {
            try
            {
                Header = "AccountEmail,AccountUserFullName,AccountUserProfileUrl,QueryType,QueryValue,ActivityType,CompanyName,CompanyUrl,Industry,IsFollowed,CompanyDesciption,Specialties,CompanySize,Website,FoundationDate,Headquater,ScrapedDate";

                objCompanyScraperReportModel.Clear();

                #region get data from InteractedCompanies table and add to objCompanyScraperReportModel
                int Count = 1;
                DbCampaignOperations.Get<InteractedCompanies>().ForEach(
                    ReportItem =>
                    {
                        var queryDetails = lstCurrentQueries.FirstOrDefault(x => x.Key == ReportItem.QueryValue);
                        if (queryDetails.Key == ReportItem.QueryValue && queryDetails.Value == ReportItem.QueryType)
                        {
                            objCompanyScraperReportModel.Add(new InteractedCompanyReportModel()
                            {
                                Id = Count++,
                                AccountEmail = ReportItem.AccountEmail,
                                QueryType = ReportItem.QueryType,
                                QueryValue = ReportItem.QueryValue,
                                ActivityType = ReportItem.ActivityType,
                                CompanyName = ReportItem.CompanyName,
                                CompanyUrl = ReportItem.CompanyUrl,
                                TotalEmployees = ReportItem.TotalEmployees,
                                Industry = ReportItem.Industry,
                                IsFollowed = ReportItem.IsFollowed,
                                DetailedCompanyScraperInfo = ReportItem.DetailedInfo,
                                InteractionDateTime = ReportItem.InteractionTimeStamp.EpochToDateTimeUtc().ToLocalTime()
                            });
                        }
                    });

                objReports.ReportModel.ReportCollection =
                    CollectionViewSource.GetDefaultView(objCompanyScraperReportModel);

                #endregion

                #region Generate Reports column with data

                //campaign.SelectedAccountList.ToList().ForEach(x =>
                //{
                objReports.ReportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "AccountEmail"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Company Name",ColumnBindingText="CompanyName" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "Company Url",ColumnBindingText="CompanyUrl" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "Industry",ColumnBindingText="Industry" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "Is Followed",ColumnBindingText="IsFollowed" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "Scraped Date",ColumnBindingText="ScrapedDate" }
                    };
                //});

                #endregion
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        private static void GeneratProfileEndorsementReport(Reports objReports, List<KeyValuePair<string, string>> lstCurrentQueries, DbOperations DbCampaignOperations, CampaignDetails campaign)
        {
            try
            {
                Header = "AccountEmail,QueryType,QueryValue,ActivityType,Connection FullName,Connection ProfileUrl,Endorsed Skills,EndorsedDate";

                objUserScraperReportModel.Clear();

                #region get data from InteractedUsers table and add to UserScraperReportModel
                int Count = 1;
                DbCampaignOperations.Get<InteractedUsers>().ForEach(
                    ReportItem =>
                    {
                        objUserScraperReportModel.Add(new UserScraperReportModel()
                        {
                            Id = Count++,
                            AccountEmail = ReportItem.AccountEmail,
                            ActivityType = ReportItem.ActivityType,
                            UserFullName = ReportItem.UserFullName,
                            UserProfileUrl = ReportItem.UserProfileUrl,
                            DetailedUserInfo = ReportItem.DetailedUserInfo,
                            ScrapedDate = ReportItem.InteractionTimeStamp.EpochToDateTimeUtc().ToLocalTime()
                        });
                    });

                objReports.ReportModel.ReportCollection =
                    CollectionViewSource.GetDefaultView(objUserScraperReportModel);

                #endregion

                #region Generate Reports column with data

                //campaign.SelectedAccountList.ToList().ForEach(x =>
                //{
                objReports.ReportModel.GridViewColumn =
                    new ObservableCollection<GridViewColumnDescriptor>
                    {
                        new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Account", ColumnBindingText = "AccountEmail"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Type", ColumnBindingText = "QueryType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Query Value", ColumnBindingText = "QueryValue"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Activity Type", ColumnBindingText = "ActivityType"},
                        new GridViewColumnDescriptor {ColumnHeaderText = "Connection FullName",ColumnBindingText="UserFullName" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "Connection ProfileUrl",ColumnBindingText="UserProfileUrl" },
                        new GridViewColumnDescriptor {ColumnHeaderText = "Endorsed Date",ColumnBindingText="ScrapedDate" }
                    };
                //});

                #endregion
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }



        internal static ObservableCollection<QueryInfo> GetSavedQuery(string moduleType, string activitySettings)
        {
            ObservableCollection<QueryInfo> lstSavedQuery = null;

            #region getting list of Saved Query according to Module type

            switch (moduleType)
            {
                case "ConnectionRequest":
                    lstSavedQuery = JsonConvert.DeserializeObject<ConnectionRequestModel>(activitySettings).SavedQueries;
                    break;
                case "AcceptConnectionRequest":
                    lstSavedQuery = JsonConvert.DeserializeObject<AcceptConnectionRequestModel>(activitySettings).SavedQueries;
                    break;
                case "RemoveConnections":
                    lstSavedQuery = JsonConvert.DeserializeObject<RemoveOrWithdrawConnectionsModel>(activitySettings).SavedQueries;
                    break;
                case "GroupJoiner":
                    lstSavedQuery = JsonConvert.DeserializeObject<GroupJoinerModel>(activitySettings).SavedQueries;
                    break;
                case "GroupUnJoiner":
                    lstSavedQuery = JsonConvert.DeserializeObject<GroupUnJoinerModel>(activitySettings).SavedQueries;
                    break;
                case "UserScraper":
                    lstSavedQuery = JsonConvert.DeserializeObject<UserScraperModel>(activitySettings).SavedQueries;
                    break;
                case "JobScraper":
                    lstSavedQuery = JsonConvert.DeserializeObject<JobScraperModel>(activitySettings).SavedQueries;
                    break;
                case "CompanyScraper":
                    lstSavedQuery = JsonConvert.DeserializeObject<CompanyScraperModel>(activitySettings).SavedQueries;
                    break;
                case "GroupMemberScraper":
                    lstSavedQuery = JsonConvert.DeserializeObject<GroupMemberScraperModel>(activitySettings).SavedQueries;
                    break;
                case "ProfileEndorsement":
                    lstSavedQuery = JsonConvert.DeserializeObject<ProfileEndorsementModel>(activitySettings).SavedQueries;
                    break;
            }

            #endregion

            return lstSavedQuery;
        }


        internal static void ExportReports(string moduleType, string filename)
        {
            if (moduleType == ActivityType.ConnectionRequest.ToString())
            {
                objInteractedUsersReportModel.ToList().ForEach(ReportItem =>
                  {
                      try
                      {
                          #region Replace Sepcial characters
                          ReportItem.UserFullName = ReportItem.UserFullName.Replace(",", "").Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("\\n", "").Replace("\\r", "").Replace("\\t", ""); ;
                          ReportItem.DetailedUserInfo = ReportItem.DetailedUserInfo.Replace(",", "").Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("\\n", "").Replace("\\r", "").Replace("\\t", ""); ;

                          if (ReportItem.DetailedUserInfo == string.Empty) ReportItem.DetailedUserInfo = "N/A";
                          #endregion

                          var csvData = ReportItem.AccountEmail + "," + ReportItem.QueryType + "," + ReportItem.QueryValue + "," + ReportItem.ActivityType + "," + ReportItem.UserFullName + "," + ReportItem.UserProfileUrl + "," + ReportItem.DetailedUserInfo + "," + ReportItem.InteractionDateTime;

                          using (var streamWriter = new StreamWriter(filename, true))
                          {
                              streamWriter.WriteLine(csvData);
                          }
                      }
                      catch (Exception ex)
                      {
                          Console.WriteLine(ex.StackTrace);
                      }
                  });
            }
            else if (moduleType == ActivityType.AcceptConnectionRequest.ToString())
            {
                objInteractedUsersReportModel.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        #region Replace Sepcial characters

                        ReportItem.UserFullName = ReportItem.UserFullName.Replace(",", "").Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("\\n", "").Replace("\\r", "").Replace("\\t", ""); ;
                        #endregion

                        var csvData = ReportItem.AccountEmail + "," + ReportItem.QueryType + "," + ReportItem.QueryValue + "," + ReportItem.ActivityType + "," + ReportItem.UserFullName + "," + ReportItem.UserProfileUrl + "," + ReportItem.InteractionDateTime;

                        using (var streamWriter = new StreamWriter(filename, true))
                        {
                            streamWriter.WriteLine(csvData);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }
            else if (moduleType == ActivityType.RemoveOrWithdrawConnections.ToString())
            {
                objInteractedUsersReportModel.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        #region Replace Sepcial characters

                        ReportItem.UserFullName = ReportItem.UserFullName.Replace(",", "").Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("\\n", "").Replace("\\r", "").Replace("\\t", ""); ;
                        #endregion

                        var csvData = ReportItem.AccountEmail + "," + ReportItem.QueryType + "," + ReportItem.QueryValue + "," + ReportItem.ActivityType + "," + ReportItem.UserFullName + "," + ReportItem.UserProfileUrl + "," + ReportItem.InteractionDateTime;

                        using (var streamWriter = new StreamWriter(filename, true))
                        {
                            streamWriter.WriteLine(csvData);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }
            else if (moduleType == ActivityType.BroadcastMessages.ToString() || moduleType == ActivityType.AutoReplyToNewMessage.ToString() || moduleType == ActivityType.SendMessageToNewConnection.ToString())
            {
                objInteractedUsersReportModel.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        #region Replace Sepcial characters
                        ReportItem.UserFullName = ReportItem.UserFullName.Replace(",", "").Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("\\n", "").Replace("\\r", "").Replace("\\t", ""); ;
                        ReportItem.DetailedUserInfo = ReportItem.DetailedUserInfo.Replace(",", "").Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("\\n", "").Replace("\\r", "").Replace("\\t", ""); ;

                        if (ReportItem.DetailedUserInfo == string.Empty) ReportItem.DetailedUserInfo = "N/A";
                        #endregion

                        var csvData = ReportItem.AccountEmail + "," + ReportItem.QueryType + "," + ReportItem.QueryValue + "," + ReportItem.ActivityType + "," + ReportItem.UserFullName + "," + ReportItem.UserProfileUrl + "," + ReportItem.DetailedUserInfo + "," + ReportItem.InteractionDateTime;

                        using (var streamWriter = new StreamWriter(filename, true))
                        {
                            streamWriter.WriteLine(csvData);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }
            else if (moduleType == ActivityType.GroupJoiner.ToString() || moduleType == ActivityType.GroupUnJoiner.ToString())
            {
                objGroupJoinerReportModel.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        var csvData = ReportItem.AccountEmail + "," + ReportItem.QueryType + "," + ReportItem.QueryValue + "," + ReportItem.ActivityType + "," + ReportItem.GroupName + "," + ReportItem.GroupUrl + "," + ReportItem.TotalMembers + "," + ReportItem.InteractionDateTime;

                        using (var streamWriter = new StreamWriter(filename, true))
                        {
                            streamWriter.WriteLine(csvData);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }
            else if (moduleType == ActivityType.UserScraper.ToString() || moduleType == ActivityType.GroupMemberScraper.ToString())
            {
                ExportUserScraperReport(filename);
            }
            else if (moduleType == ActivityType.JobScraper.ToString())
            {
                JobScraperDetailedInfo objJobInfo = null;
                objJobScraperReportModel.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        try
                        {
                            objJobInfo = JsonConvert.DeserializeObject<JobScraperDetailedInfo>(Uri.UnescapeDataString(ReportItem.DetailedInfo));
                        }
                        catch (Exception ex)
                        { }

                        var csvData = ReportItem.AccountEmail + "," + objJobInfo.AccountUserFullName + "," + objJobInfo.AccountUserProfileUrl + "," + ReportItem.QueryType + "," + ReportItem.QueryValue + "," + ReportItem.ActivityType + "," + ReportItem.JobPostUrl + "," + objJobInfo.CompanyName + "," + objJobInfo.CompanyWebsite + "," + objJobInfo.JobTitle + "," + objJobInfo.JobLocation + "," + objJobInfo.PostedOn + "," + objJobInfo.NumberOfApplicants + "," + objJobInfo.Industry + "," + objJobInfo.EmploymentType + "," + objJobInfo.SeniorityLevel + "," + objJobInfo.JobFunction + "," + objJobInfo.JobPosterProfileUrl + "," + objJobInfo.JobPosterFullName + "," + objJobInfo.DegreeOfConnection + "," + ReportItem.InteractedDateTime;

                        using (var streamWriter = new StreamWriter(filename, true))
                        {
                            streamWriter.WriteLine(csvData);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }
            else if (moduleType == ActivityType.CompanyScraper.ToString())
            {
                CompanyScraperDetailedInfo objCompanyScraperInfo = null;
                objCompanyScraperReportModel.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        try
                        {
                            objCompanyScraperInfo = JsonConvert.DeserializeObject<CompanyScraperDetailedInfo>(Uri.UnescapeDataString(ReportItem.DetailedCompanyScraperInfo));
                        }
                        catch (Exception ex)
                        { }

                        var csvData = ReportItem.AccountEmail + "," + objCompanyScraperInfo.AccountUserFullName + "," + objCompanyScraperInfo.AccountUserProfileUrl + "," + ReportItem.QueryType + "," + ReportItem.QueryValue + "," + ReportItem.ActivityType + "," + ReportItem.CompanyName + "," + ReportItem.CompanyUrl + "," + ReportItem.Industry + "," + ReportItem.IsFollowed + "," + objCompanyScraperInfo.CompanyDesciption + "," + objCompanyScraperInfo.Specialties + "," + objCompanyScraperInfo.CompanySize + "," + objCompanyScraperInfo.Website + "," + objCompanyScraperInfo.FoundationDate + "," + objCompanyScraperInfo.Headquater + "," + ReportItem.InteractionDateTime;

                        using (var streamWriter = new StreamWriter(filename, true))
                        {
                            streamWriter.WriteLine(csvData);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }

            else if (moduleType == ActivityType.ProfileEndorsement.ToString())
            {
                objUserScraperReportModel.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        #region Replace Sepcial characters

                        ReportItem.UserFullName = ReportItem.UserFullName.Replace(",", "").Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("\\n", "").Replace("\\r", "").Replace("\\t", "");


                        #endregion

                        var csvData = ReportItem.AccountEmail + "," + ReportItem.QueryType + "," + ReportItem.QueryValue + "," + ReportItem.ActivityType + "," + ReportItem.UserFullName + "," + ReportItem.UserProfileUrl + "," + ReportItem.DetailedUserInfo + "," + ReportItem.ScrapedDate;

                        using (var streamWriter = new StreamWriter(filename, true))
                        {
                            streamWriter.WriteLine(csvData);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }
        }

        internal static void ExportUserScraperReport(string filename)
        {
            try
            {
                UserScraperDetailedInfo objInfo = null;
                objUserScraperReportModel.ToList().ForEach(ReportItem =>
                {
                    try
                    {
                        try
                        {
                            objInfo = JsonConvert.DeserializeObject<UserScraperDetailedInfo>(Uri.UnescapeDataString(ReportItem.DetailedUserInfo));
                        }
                        catch (Exception ex)
                        { }

                        var csvData = ReportItem.AccountEmail + "," + objInfo.AccountUserFullName + "," + objInfo.AccountUserProfileUrl + "," + ReportItem.QueryType + "," + ReportItem.QueryValue + "," + ReportItem.ActivityType + "," + ReportItem.UserFullName + "," + ReportItem.UserProfileUrl + "," + objInfo.HeadlineTitle + "," + objInfo.EmailId + "," + objInfo.PersonalPhoneNumber + "," + objInfo.PersonalWebsites + "," + objInfo.Birthdate + "," + objInfo.TwitterUrl + "," + objInfo.Titlecurrent + "," + objInfo.Companycurrent + "," + objInfo.CurrentCompanyUrl + "," + objInfo.CompanyDescription + "," + objInfo.CurrentCompanyWebsite + "," + objInfo.Location + "," + objInfo.Industry + "," + objInfo.Country + "," + objInfo.Connection + "," + objInfo.Recomandation + "," + objInfo.Skill + "," + objInfo.Experience + "," + objInfo.EducationCollection + "," + objInfo.PastTitles + "," + objInfo.PastCompany + "," + ReportItem.ScrapedDate;

                        using (var streamWriter = new StreamWriter(filename, true))
                        {
                            streamWriter.WriteLine(csvData);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        internal static bool FilterByQueryType(string queryType, DominatorHouseCore.Models.ReportModel reportModel)
        {
            //switch (reportModel.ModuleType)
            //{
            //    case "Follow":
            //        return (reportModel.ReportCollection.CurrentItem as FollowReportDetails).QueryType.Contains(queryType);

            //    case "Unfollow":

            //        break;
            //    case "Like":

            //        break;
            //    case "Comment":

            //        break;
            //    case "Repost":

            //        break;
            //    case "DownloadScraper":

            //        break;
            //    case "UserScraper":

            //        break;
            //}



            return false;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using Unity;

// ReSharper disable once CheckNamespace
namespace LinkedDominatorCore.LDLibrary.Processor.Users
{
    internal class ConnectionOrUserNonQueryProcessor : BaseLinkedinUserProcessor, IQueryProcessor
    {
        internal IUnityContainer UnityContainer;

        public ConnectionOrUserNonQueryProcessor(ILdJobProcess jobProcess,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory, IDelayService delayService,
            IProcessScopeModel ProcessScopeModel) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, ProcessScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var nonQueryClass = new MapperModel();
                ClassMapper.SetModelClass(ref nonQueryClass, ActivityType, LdJobProcess);
                var Instance = InstanceProvider.GetInstance<ILdAccountUpdateFactory>();
                if (nonQueryClass.IsFollower)
                {
                    queryInfo.QueryType = "Own";
                    queryInfo.QueryValue = "Followers";
                    UnityContainer.Resolve<OwnFollowerProcessor>().Start(queryInfo);
                    jobProcessResult.HasNoResult = true;
                    return;
                }
                if (nonQueryClass.IsGroup)
                {
                    Instance.UpdateGroups(DominatorAccountModel, LdFunctions, DbAccountService)
                        .Wait(LdJobProcess.JobCancellationTokenSource.Token);
                    queryInfo.QueryType = LdConstants.GroupProcessor;
                    UnityContainer.Resolve<GroupMembersProcessor>().Start(queryInfo);
                    jobProcessResult.HasNoResult = true;
                    return;
                }
                Instance.UpdateConnections(DominatorAccountModel, LdFunctions, DbAccountService)
                    .Wait(LdJobProcess.JobCancellationTokenSource.Token);
                if (nonQueryClass.IsCheckedBySoftware || nonQueryClass.IsCheckedOutSideSoftware ||
                    nonQueryClass.IsCheckedRecentConnections)
                    SoftwareOrOutsideSoftware(nonQueryClass);
                if (nonQueryClass.IsCheckedLangKeyCustomUserList)
                    CustomListOperations(nonQueryClass);
                if (nonQueryClass.ListUsersFromSelectedSource.Count > 0)
                    ProcessUsersFromSelectedSource(ref jobProcessResult, nonQueryClass);
                else
                    NoMoreDataLoggerInfo(jobProcessResult, nonQueryClass);
            }
            catch (OperationCanceledException)
            {
                CloseBrowserWhenNoMoreResults();
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                CloseBrowserWhenNoMoreResults();
                ex.DebugLog();
            }
        }


        private void ProcessUsersFromSelectedSource(ref JobProcessResult jobProcessResult, MapperModel modelClass)
        {
            // Filter BlacklistedUsers
            int UsersBeforeFilter = modelClass.ListUsersFromSelectedSource.Count;
            if (modelClass.IsChkSkipBlackListedUser &&
                (modelClass.IsChkPrivateBlackList || modelClass.IsChkGroupBlackList))
            {
                FilterBlacklistedUsers(modelClass.ListUsersFromSelectedSource, modelClass.IsChkPrivateBlackList,
                    modelClass.IsChkGroupBlackList);
                int UsersAfterFilter = modelClass.ListUsersFromSelectedSource.Count;
                if (UsersBeforeFilter > UsersAfterFilter)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "Skipped User Present in Blacklist ");
            }
            //Use WhitelistedUsers
            if (modelClass.IsChkSkipWhiteListedUser &&
                (modelClass.IsChkUsePrivateWhiteList || modelClass.IsChkUseGroupWhiteList))
            {
                UseWhitelistedUsers(modelClass.ListUsersFromSelectedSource, modelClass.IsChkUsePrivateWhiteList,
                    modelClass.IsChkUseGroupWhiteList);
                int UsersAfterFilter = modelClass.ListUsersFromSelectedSource.Count;
                if (UsersBeforeFilter > UsersAfterFilter)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "Skipped User Present in Whitelist ");
            }
            modelClass.ListUsersFromSelectedSource = modelClass.ListUsersFromSelectedSource.Distinct().ToList();

            #region Filter Users with no profile picture

            if (modelClass.IsCheckedFilterProfileImageCheckbox && modelClass.ListUsersFromSelectedSource.RemoveAll(x => x.ProfilePicUrl == null) > 0)
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    "successfully filtered users having no profile picture");
            SkipUserAlreadyRecievedMessageFromSoftware(modelClass);

            SkipUserAlreadyReceivedMessageFromOutSideSoftware(modelClass);
            #endregion

            #region Filter Source 

            if (modelClass.IsCheckedConnectedBefore)
            {
                var beforeTime = DateTime.Now.AddDays(-modelClass.Days).AddHours(-modelClass.Hours)
                    .GetCurrentEpochTimeMilliSeconds();
                modelClass.ListUsersFromSelectedSource.RemoveAll(x => x.ConnectedTimeStamp < beforeTime);
                if (modelClass.ListUsersFromSelectedSource.Count > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"found {modelClass.ListUsersFromSelectedSource.Count} results for connected within {modelClass.Days} days.");
            }

            #endregion

            if (modelClass.ListUsersFromSelectedSource.Count > 0)
            {
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var linkedinConfig =
                    genericFileManager.GetModel<LinkedInModel>(ConstantVariable.GetOtherLinkedInSettingsFile());

                if (ActivityType.Equals(ActivityType.BroadcastMessages) &&
                    linkedinConfig.IsFilterDuplicateMessageByCheckingConversationsHistory)
                    FurtherProcessLinkedinUsersFromUserList(QueryInfo.NoQuery, ref jobProcessResult,
                        modelClass.ListUsersFromSelectedSource);
                else
                    ProcessLinkedinUsersFromUserList(QueryInfo.NoQuery, ref jobProcessResult,
                        modelClass.ListUsersFromSelectedSource);

                if (jobProcessResult.IsProcessSuceessfull)
                    return;
                NoMoreDataLoggerInfo(jobProcessResult, modelClass);
            }
            else
            {
                NoMoreDataLoggerInfo(jobProcessResult, modelClass);
            }
        }

        private void NoMoreDataLoggerInfo(JobProcessResult jobProcessResult, MapperModel model = null)
        {
            if (ActivityType.ExportConnection == ActivityType && model != null && model.IsStopScheduling)
                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, SocialNetworks.LinkedIn,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType);
            jobProcessResult.HasNoResult = true;
            CloseBrowserWhenNoMoreResults();
        }

        private void CustomListOperations(MapperModel modelClass)
        {
            try
            {
                #region Add LstUsersFromCustomUserList To LstUsersFromSelectedSource

                var lstUsersFromCustomUserList = CustomUserList(modelClass.ListCustomUser);

                LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (lstUsersFromCustomUserList != null && lstUsersFromCustomUserList.Count > 0)
                    modelClass.ListUsersFromSelectedSource.AddRange(lstUsersFromCustomUserList);

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SoftwareOrOutsideSoftware(MapperModel modelClass)
        {
            var bySoftware = Application.Current.FindResource("LangKeyBySoftware")?.ToString();
            var outSideSoftware = Application.Current.FindResource("LangKeyOutsideSoftware")?.ToString();

            #region Add Either (LstUsersBySoftware or LstUsersFromSelectedSource) or both To LstUsersFromSelectedSource

            var lstUsersBySoftware = new List<LinkedinUser>();
            var lstUsersOutsideSoftware = new List<LinkedinUser>();

            SetModelAndListOfUsers(modelClass, outSideSoftware, lstUsersOutsideSoftware, bySoftware,
                lstUsersBySoftware);

            if (modelClass.IsCheckedBySoftware && lstUsersBySoftware.Count > 0)
                modelClass.ListUsersFromSelectedSource.AddRange(lstUsersBySoftware);
            if (modelClass.IsCheckedOutSideSoftware && lstUsersOutsideSoftware.Count > 0)
                modelClass.ListUsersFromSelectedSource.AddRange(lstUsersOutsideSoftware);

            #endregion
        }

        private void SetModelAndListOfUsers(MapperModel modelClass, string outSideSoftware,
            List<LinkedinUser> lstUsersOutsideSoftware,
            string bySoftware, List<LinkedinUser> lstUsersBySoftware)
        {
            var lstConnections = new List<Connections>();
            switch (ActivityType)
            {
                case ActivityType.RemoveConnections:
                    {
                        var lstRemovedConnections = DbAccountService.GetRemovedConnections().ToList();
                        lstConnections = DbAccountService.GetConnections().ToList();
                        if (lstConnections is null || lstConnections.Count == 0)
                        {
                            var InteractedUsers = DbAccountService.GetInteractedUsers(ActivityType.ConnectionRequest.ToString());
                            lstConnections = ClassMapper.GetConnectionFromInteractedUsers(InteractedUsers?.ToList());
                        }
                        #region Get LstUsersBySoftware and  LstUsersOutsideSoftware (both contains LstConnections Never Interacted With this CurrentActivityType)

                        try
                        {
                            lstConnections.RemoveAll(x => lstRemovedConnections.Any(y => y.ProfileUrl == x.ProfileUrl));

                            if (lstConnections.Count > 0)
                            {
                                var lstConnectionRequestSentFromSoftware =
                                    DbAccountService.GetConnectionRequestsSendFromSoftware().ToList();
                                if (modelClass.IsCheckedOutSideSoftware)
                                {
                                    var lstConnectionConnectedOutsideSoftware = lstConnections.ToList();
                                    lstConnectionConnectedOutsideSoftware.RemoveAll(x =>
                                        lstConnectionRequestSentFromSoftware.Any(y => y.UserProfileUrl == x.ProfileUrl));
                                    var lstUsersConnectedOutsideSoftware =
                                        GetListUsers(lstConnectionConnectedOutsideSoftware, outSideSoftware);
                                    lstUsersOutsideSoftware.AddRange(lstUsersConnectedOutsideSoftware);
                                }
                                var lstConnectionConnectedBySoftware = ClassMapper.GetConnectionFromInteractedUsers(lstConnectionRequestSentFromSoftware);
                                if(lstConnectionConnectedBySoftware is null || lstConnectionConnectedBySoftware.Count == 0)
                                    lstConnectionConnectedBySoftware = lstConnections.FindAll(x =>
                                        lstConnectionRequestSentFromSoftware.Any(y => y.UserProfileUrl == x.ProfileUrl));
                                var lstUsersConnectedBySoftware =
                                    GetListUsers(lstConnectionConnectedBySoftware, bySoftware);
                                lstUsersBySoftware.AddRange(lstUsersConnectedBySoftware);
                            }

                            LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        #endregion

                        break;
                    }


                case ActivityType.ExportConnection:
                case ActivityType.BroadcastMessages:
                case ActivityType.ProfileEndorsement:

                    #region Get LstUsersBySoftware and  LstUsersOutsideSoftware (both contains LstConnections Never Interacted With this CurrentActivityType)

                    try
                    {
                        // getting all the already interacted users according to account or campaign  basis
                        var lstConnectionsAlreadyInteracted = new List<InteractedUsers>();

                        if (string.IsNullOrEmpty(LdJobProcess.CurrentCampaignId))
                        {
                            lstConnectionsAlreadyInteracted =
                                DbAccountService.GetInteractedUsers(ActivityTypeString).ToList();
                        }
                        else
                        {
                            // mapping 'campaign' InteractedUser to 'account'  InteractedUser
                            var campaignsInteractedUsers =
                                DbCampaignService.GetInteractedUsers(ActivityTypeString).ToList();
                            ClassMapper.MapListOfModelClass(campaignsInteractedUsers,
                                ref lstConnectionsAlreadyInteracted);
                        }

                        // get all connections from db

                        var listConnections = DbAccountService.GetConnections().ToList();
                        lstConnections.AddRange(listConnections);

                        // adding recent connections
                        if (modelClass.IsCheckedRecentConnections)
                            RecentConnections(lstConnections);
                        lstConnections.RemoveAll(x =>
                            lstConnectionsAlreadyInteracted.Any(y => y.UserProfileUrl == x.ProfileUrl));
                        
                        var lstConnectionRequestSentFromSoftware =
                            DbAccountService.GetConnectionRequestsSendFromSoftware()?.ToList();


                        if (modelClass.IsCheckedOutSideSoftware)
                        {
                            // checking if any users is from sent connections from s/w accepted connection request remove it
                            //  clone so that it will not change original non interacted user list therefore we use Enumerable to iterate it

                            var lstConnectionConnectedOutsideSoftware = lstConnections.ToList();
                            if (lstConnectionRequestSentFromSoftware != null)
                                lstConnectionConnectedOutsideSoftware.RemoveAll(x =>
                                   lstConnectionRequestSentFromSoftware.Any(y => y.UserFullName == x.FullName));
                            if (lstConnectionConnectedOutsideSoftware.Count == 0)
                                lstConnectionConnectedOutsideSoftware.RemoveAll(x =>
                                    lstConnectionRequestSentFromSoftware.Any(y => y.ProfileId == x.ProfileId));
                            if (lstConnectionConnectedOutsideSoftware.Count == 0)
                                lstConnectionConnectedOutsideSoftware.RemoveAll(x =>
                                    lstConnectionRequestSentFromSoftware.Any(y => y.UserProfileUrl == x.ProfileUrl));

                            var lstUsersConnectedOutsideSoftware =
                                GetListUsers(lstConnectionConnectedOutsideSoftware, outSideSoftware);
                            lstUsersOutsideSoftware.AddRange(lstUsersConnectedOutsideSoftware);
                        }

                        if (modelClass.IsCheckedBySoftware)
                        {
                            var lstConnectionConnectedBySoftware = new List<Connections>();

                            if (lstConnectionRequestSentFromSoftware != null)
                                lstConnectionConnectedBySoftware = lstConnections.FindAll(x =>
                                    lstConnectionRequestSentFromSoftware.Any(y => y.UserProfileUrl == x.ProfileUrl));
                            if (lstConnectionConnectedBySoftware.Count == 0)
                                lstConnectionConnectedBySoftware = lstConnections.FindAll(x =>
                                    lstConnectionRequestSentFromSoftware.Any(y => y.ProfileId == x.ProfileId));
                            if (lstConnectionConnectedBySoftware.Count == 0)
                                lstConnectionConnectedBySoftware = lstConnections.FindAll(x =>
                                    lstConnectionRequestSentFromSoftware.Any(y => y.UserFullName == x.FullName));
                            var lstUsersConnectedBySoftware =
                                GetListUsers(lstConnectionConnectedBySoftware, bySoftware);
                            lstUsersBySoftware.AddRange(lstUsersConnectedBySoftware);
                        }

                        LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion

                    break;
            }
        }


        private void RecentConnections(List<Connections> lstConnections)
        {
            try
            {
                var connectionResponseHandler = LdFunctions
                    .SearchForLinkedinConnectionsAsync(
                        "https://www.linkedin.com/voyager/api/relationships/connections?sortType=RECENTLY_ADDED&count=100&start=0",
                        DominatorAccountModel.Token).Result;
                var listUsers = lstConnections.Select(x => x.ProfileUrl).ToList();
                // removing already interacted users
                connectionResponseHandler.ConnectionsList.RemoveAll(user => listUsers.Contains(user.ProfileUrl));
                ClassMapper.MapListOfModelClass(connectionResponseHandler.ConnectionsList, ref lstConnections);
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }
    }
}
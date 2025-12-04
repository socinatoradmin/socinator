using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using LinkedDominatorCore.LDLibrary.DAL;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using LinkedDominatorCore.LDModel;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDUtility;
using Unity;
using ThreadUtils;

namespace LinkedDominatorCore.LDLibrary.Processor.Users.NonQueryType
{
    class WithDrawConnectionProcessor : BaseLinkedinUserProcessor, IQueryProcessor
    {

        MapperModel nonQueryClass = new MapperModel();
        public WithDrawConnectionProcessor(ILdJobProcess jobProcess, 
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory, IDelayService delayService,IProcessScopeModel processScopeModel) :
            base(jobProcess, campaignService, ldFunctionFactory,delayService, processScopeModel)
        {

        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {

                ClassMapper.SetModelClass(ref nonQueryClass, ActivityType, LdJobProcess);
                var Instance = InstanceProvider.GetInstance<ILdAccountUpdateFactory>();
                Instance.UpdateInvitationsSent(DominatorAccountModel, LdFunctions, DbAccountService).Wait(LdJobProcess.JobCancellationTokenSource.Token);
                if (nonQueryClass.IsCheckedBySoftware || nonQueryClass.IsCheckedOutSideSoftware)
                    SoftwareOrOutsideSoftware(nonQueryClass);

                if (nonQueryClass.IsCheckedLangKeyCustomUserList)
                    CustomListOperations(nonQueryClass);

                ProcessUsersFromSelectedSource(nonQueryClass);
                jobProcessResult.HasNoResult = true;

            }
            catch (OperationCanceledException)
            {
                Utility.LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel, IsBrowser);
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                Utility.LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel, IsBrowser);
                ex.DebugLog();
            }
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
            SetModelAndListOfUsers(modelClass, lstUsersOutsideSoftware, lstUsersBySoftware);

            #endregion
        }

        private void SetModelAndListOfUsers(MapperModel modelClass, List<LinkedinUser> lstUsersOutsideSoftware, List<LinkedinUser> lstUsersBySoftware)
        {
           
            #region Get LstUsersBySoftware and  LstUsersOutsideSoftware (both contains LstUsers Never Interacted With this CurrentActivityType)

            try
            {
                var lstAllPendingConnectionRequest = new List<LinkedinUser>();
                var isMaxJobCount = false;
                var start = IsBrowser ? 1 : 0;
                var totalFiltered = 0;
                string sentInvitationsApiFirstPage = IsBrowser ? "https://www.linkedin.com/mynetwork/invitation-manager/sent/" :
                   "https://www.linkedin.com/voyager/api/relationships/sentInvitationView?count=100&start=0&type=SINGLES_ALL&q=sent";

                var pendingConnectionRequestFromSoftware =
                    DbAccountService.GetPendingConnectionRequestFromSoftware().ToList();

                if (modelClass.IsCheckedBySoftware)
                    GetSoftwareConnectionsInvitations(lstUsersBySoftware, modelClass);

                var softwareList = lstUsersBySoftware.Select(x => x.PublicIdentifier).ToList();

                if (modelClass.IsCheckedOutSideSoftware)
                {
                    var objAllPendingConnectionRequestResponseHandler =
                        LdFunctions.AllPendingConnectionRequest(sentInvitationsApiFirstPage);

                    while (!isMaxJobCount)
                    {
                        var constructedApi = IsBrowser ? $"https://www.linkedin.com/mynetwork/invitation-manager/sent/?page={start}" :
                            $"https://www.linkedin.com/voyager/api/relationships/sentInvitationView?count=100&type=SINGLES_ALL&q=sent&start={start}";

                        if ((!IsBrowser && start != 0) || start != 1)
                            objAllPendingConnectionRequestResponseHandler = LdFunctions.AllPendingConnectionRequest(constructedApi);

                        objAllPendingConnectionRequestResponseHandler.LstAllPendingConnectionRequest.RemoveAll(x =>
                            softwareList.Contains(x.PublicIdentifier));

                        LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (objAllPendingConnectionRequestResponseHandler.Success)
                        {
                            if (objAllPendingConnectionRequestResponseHandler.LstAllPendingConnectionRequest.Count > 0)
                            {
                                if (modelClass.IsCheckedRequestedBefore) //For Withdraw Activity
                                    totalFiltered += RemoveUsersBeforeByDate(modelClass, objAllPendingConnectionRequestResponseHandler.LstAllPendingConnectionRequest, true);

                                modelClass.ListUsersFromSelectedSource = objAllPendingConnectionRequestResponseHandler
                                    .LstAllPendingConnectionRequest;
                                ProcessUsersFromSelectedSource(modelClass);
                            }

                            if (lstAllPendingConnectionRequest.Count >= LdJobProcess.ModuleSetting.JobConfiguration.ActivitiesPerJob.EndValue + 5)
                                isMaxJobCount = true;
                            else
                                start += IsBrowser ? 1 : 100;

                        }
                        else
                        {
                            isMaxJobCount = true;
                            GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, "withdraw connection request");
                        }
                    }

                    lstAllPendingConnectionRequest.RemoveAll(x =>
                        pendingConnectionRequestFromSoftware.Any(z => z == x.ProfileUrl || z == x.EmailAddress));
                    lstUsersOutsideSoftware.AddRange(lstAllPendingConnectionRequest);
                }

                if (modelClass.IsCheckedRequestedBefore && totalFiltered > 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "outside software : successfully filtered " + totalFiltered +
                        " from results for pending connection request");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

        }

        private void GetSoftwareConnectionsInvitations(List<LinkedinUser> lstUsersBySoftware, MapperModel modelClass)
        {
            try
            {
                // getting all store connections in db since we require it to get invitation id
                // comparing sent connection requests with withdrawns from software
                var listConnectionRequestSent = DbAccountService.Get<InvitationsSent>();
                var interactedUsers =
                    DbAccountService.GetInteractedUsers(ActivityType.ConnectionRequest.ToString()).ToList();
                var withDrawConnections = DbAccountService.GetInteractedUsers(ActivityType.ToString());
                foreach (var interactedUser in interactedUsers)
                {
                    try
                    {
                        if (withDrawConnections.Any(x => x.UserProfileUrl == interactedUser.UserProfileUrl)
                                      || string.IsNullOrEmpty(interactedUser.UserProfileUrl))
                            continue;
                        var tempData = interactedUser;
                        var invitationId =
                            listConnectionRequestSent.Where(x => x.ProfileUrl == tempData.UserProfileUrl).FirstOrDefault();

                        var linkedInUser = new LinkedinUser()
                        {
                            ProfileUrl = interactedUser.UserProfileUrl,
                            ProfileId = interactedUser.ProfileId,
                            RequestedTimeStamp = interactedUser.InteractionDatetime.ConvertToEpoch()
                        };

                        if (invitationId != null)
                        {
                            linkedInUser.InvitationId = invitationId.InvitationId;
                            linkedInUser.FullName = invitationId.FullName;
                        }
                        else
                            continue;

                        lstUsersBySoftware.Add(linkedInUser);
                       
                    }
                    catch (Exception)
                    {
                        //
                    }
                }
                if (modelClass.IsCheckedRequestedBefore) //For Withdraw Activity
                {
                    RemoveUsersBeforeByDate(modelClass, lstUsersBySoftware);
                    if (lstUsersBySoftware.Count > 0)
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"found {lstUsersBySoftware.Count} results for pending connection request before {modelClass.Days} days from Software.");
                }
                nonQueryClass.ListUsersFromSelectedSource.AddRange(lstUsersBySoftware);
                ProcessUsersFromSelectedSource(nonQueryClass);
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }

        private int RemoveUsersBeforeByDate(MapperModel modelClass, List<LinkedinUser> ListUsersFromSelectedSource, bool isOutside = false)
        {
            var beforeTime = isOutside ? DateTime.Now.AddDays(-modelClass.Days).AddHours(-modelClass.Hours)
                .GetCurrentEpochTimeMilliSeconds() :
                DateTime.Now.AddDays(-modelClass.Days).AddHours(-modelClass.Hours).GetCurrentEpochTime();
            return ListUsersFromSelectedSource.RemoveAll(x => x.RequestedTimeStamp > beforeTime);
        }

        private void ProcessUsersFromSelectedSource(MapperModel modelClass)
        {
            var jobProcessResult = new JobProcessResult();
            // Filter BlacklistedUsers
            if (modelClass.IsChkSkipBlackListedUser && (modelClass.IsChkPrivateBlackList || modelClass.IsChkGroupBlackList))
                FilterBlacklistedUsers(modelClass.ListUsersFromSelectedSource, modelClass.IsChkPrivateBlackList,
                    modelClass.IsChkGroupBlackList);

            //Use WhitelistedUsers
            if (modelClass.IsChkSkipWhiteListedUser && (modelClass.IsChkUsePrivateWhiteList || modelClass.IsChkUseGroupWhiteList))
                UseWhitelistedUsers(modelClass.ListUsersFromSelectedSource, modelClass.IsChkUsePrivateWhiteList,
                    modelClass.IsChkUseGroupWhiteList);


            modelClass.ListUsersFromSelectedSource = modelClass.ListUsersFromSelectedSource.Distinct().ToList();

            #region Filter Users with no profile picture

            if (modelClass.IsCheckedFilterProfileImageCheckbox)
            {
                modelClass.ListUsersFromSelectedSource.RemoveAll(x => x.ProfilePicUrl == null);
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    "successfully filtered users having no profile picture");
            }

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
                        "found " + modelClass.ListUsersFromSelectedSource.Count +
                        $" results for connected within {modelClass.Days} days.");
            }

            // here we Withdraw send connection before 'x' days
            if (modelClass.IsCheckedRequestedBefore) //For Withdraw Activity
            {
                var beforeTime = DateTime.Now.AddDays(-modelClass.Days).AddHours(-modelClass.Hours)
                    .GetCurrentEpochTimeMilliSeconds();
                modelClass.ListUsersFromSelectedSource.RemoveAll(x => x.RequestedTimeStamp > beforeTime);
                if (modelClass.ListUsersFromSelectedSource.Count > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "found " + modelClass.ListUsersFromSelectedSource.Count +
                        $" results for pending connection request before {modelClass.Days} days.");
            }

            #endregion

            if (modelClass.ListUsersFromSelectedSource.Count > 0)
            {

                ProcessLinkedinUsersFromUserList(QueryInfo.NoQuery, ref jobProcessResult,
                    modelClass.ListUsersFromSelectedSource);
            }

        }
    }
}

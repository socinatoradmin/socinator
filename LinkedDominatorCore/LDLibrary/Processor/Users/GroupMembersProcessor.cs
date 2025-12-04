using System;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.Messenger;
using LinkedDominatorCore.LDUtility;

namespace LinkedDominatorCore.LDLibrary.Processor.Users
{
    public class GroupMembersProcessor : BaseLinkedinUserProcessor, IQueryProcessor
    {
        private readonly IDelayService _delayService;
        private readonly IProcessScopeModel _processScopeModel;

        public GroupMembersProcessor(ILdJobProcess jobProcess, IDbCampaignService campaignService,
            ILdFunctionFactory ldFunctionFactory, IProcessScopeModel processScopeModel, IDelayService delayService) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
            _processScopeModel = processScopeModel;
            _delayService = delayService;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var modelClass = new MapperModel();
                SetModel(ref modelClass);
                modelClass.SetCustomList();

                //GetDbInteractedUsers
                GroupMembers(queryInfo, ref jobProcessResult, modelClass);
            }
            catch (OperationCanceledException)
            {
                jobProcessResult.HasNoResult = true;
                throw new OperationCanceledException();
            }
            catch (Exception exception)
            {
                jobProcessResult.HasNoResult = true;
                exception.DebugLog();
            }
        }

        public void GroupMembers(QueryInfo queryInfo, ref JobProcessResult jobProcessResult, MapperModel modelClass)
        {
            #region MyRegion

            var groupUrl = string.IsNullOrEmpty(queryInfo.QueryValue)
                ? modelClass.GroupUrlList[0]
                : queryInfo.QueryValue;
            var start = 0;
            var groupId = Utils.GetBetween(groupUrl + "**", "groups/", "**").Trim('/');
            var lastCount = 0;
            var groupMemApi = IsBrowser
                ? $"https://www.linkedin.com/groups/{groupId}/members/"
                : $"https://www.linkedin.com/voyager/api/groups/groups/urn:li:group:{groupId}/members?q=membershipStatus&membershipStatuses=List(OWNER,MANAGER,MEMBER)";

            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                var tempGroupMemApi = IsBrowser
                    ? start == 0 ? groupMemApi : $"{groupMemApi}&start={start}"
                    : $"{groupMemApi}&count=40&start={start}";
                var linkedinGroupMemberResponseHandler = LdFunctions.GetGroupMemberInfo(tempGroupMemApi);
                LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (IsBrowser && lastCount == linkedinGroupMemberResponseHandler.UsersList.Count)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "sorry no results found...");
                    break;
                }

                lastCount = linkedinGroupMemberResponseHandler.UsersList.Count;
                if (linkedinGroupMemberResponseHandler.UsersList.Count == 0)
                {
                    jobProcessResult.HasNoResult = true;
                    break;
                }

                // remove self account from group otherwise it will scrap itself and send for messaging
                if (queryInfo.QueryType != null && queryInfo.QueryType.Equals(LdConstants.GroupProcessor) &&
                    linkedinGroupMemberResponseHandler.UsersList.Count != 0)
                    linkedinGroupMemberResponseHandler.UsersList.RemoveAll(x =>
                        x.ProfileId == DominatorAccountModel.AccountBaseModel.UserId);

                if (linkedinGroupMemberResponseHandler.Success)
                {
                    //Filter Already Interacted
                    RemoveOrSkipAlreadyInteractedLinkedInUsers(linkedinGroupMemberResponseHandler.UsersList);


                    if (linkedinGroupMemberResponseHandler.UsersList.Count > 0)
                    {
                        #region Skip Blacklisted User

                        if (modelClass.IsChkSkipBlackListedUser &&
                            (modelClass.IsChkPrivateBlackList || modelClass.IsChkGroupBlackList))
                            FilterBlacklistedUsers(linkedinGroupMemberResponseHandler.UsersList,
                                modelClass.IsChkPrivateBlackList,
                                modelClass.IsChkGroupBlackList);

                        #endregion

                        modelClass.ListUsersFromSelectedSource = linkedinGroupMemberResponseHandler.UsersList;
                        SkipUserAlreadyRecievedMessageFromSoftware(modelClass);

                        SkipUserAlreadyReceivedMessageFromOutSideSoftware(modelClass);
                        DominatorAccountModel.CancellationSource.Token.ThrowIfCancellationRequested();
                        foreach (var eachUser in linkedinGroupMemberResponseHandler.UsersList)
                        {
                            eachUser.MemberId = groupId;
                            LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            jobProcessResult = LdJobProcess.FinalProcess(new ScrapeResultNew
                            {
                                ResultUser = eachUser,
                                QueryInfo = queryInfo
                            });
                        }
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "sorry no results found..");
                    }

                    start += 40;
                    _delayService.ThreadSleep(new Random().Next(5000, 8000));
                }
                else
                {
                    jobProcessResult.HasNoResult = true;
                }
            }

            #endregion
        }

        public void SetModel(ref MapperModel modelClass)
        {
            try
            {
                switch (ActivityType)
                {
                    case ActivityType.UserScraper:
                    {
                        var userScraperModel =_processScopeModel.GetActivitySettingsAs<UserScraperModel>();
                        ClassMapper.MapModelClass(userScraperModel, ref modelClass);
                    }
                        break;

                    case ActivityType.BroadcastMessages:
                    {
                        var broadcastMessagesModel = _processScopeModel.GetActivitySettingsAs<BroadcastMessagesModel>();
                        ClassMapper.MapModelClass(broadcastMessagesModel, ref modelClass);
                        if (!string.IsNullOrEmpty(broadcastMessagesModel.GroupUrlInput) &&
                            string.IsNullOrEmpty(modelClass.GroupUrlInput))
                            modelClass.GroupUrlInput = broadcastMessagesModel.GroupUrlInput;
                    }
                        break;
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
                throw;
            }
        }
    }
}
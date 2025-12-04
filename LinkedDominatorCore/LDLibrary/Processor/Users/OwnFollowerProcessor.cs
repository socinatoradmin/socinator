using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.Messenger;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Response;
using System;
using ThreadUtils;

namespace LinkedDominatorCore.LDLibrary.Processor.Users
{
    public class OwnFollowerProcessor : BaseLinkedinUserProcessor, IQueryProcessor
    {
        private readonly IDelayService _delayService;
        private readonly IProcessScopeModel _processScopeModel;
        public OwnFollowerProcessor(ILdJobProcess ldJobProcess, IDbCampaignService campaignService, 
            ILdFunctionFactory ldFunctionFactory, IDelayService delayService, IProcessScopeModel ProcessScopeModel) 
            : base(ldJobProcess, campaignService, ldFunctionFactory, delayService, ProcessScopeModel)
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
                var start = 0;
                var lastCount = 0;
                while(!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    var followerAPI = IsBrowser ? $"https://www.linkedin.com/mynetwork/network-manager/people-follow/followers/"
                        :$"https://www.linkedin.com/voyager/api/graphql?variables=(start:{start},count:10,origin:CurationHub,query:(flagshipSearchIntent:MYNETWORK_CURATION_HUB,includeFiltersInResponse:true,queryParameters:List((key:resultType,value:List(FOLLOWERS)))))&queryId=voyagerSearchDashClusters.8832876bc08b96972d2c68331a27ba76";
                    var followersResponse = LdFunctions.GetAccountsFollowers(DominatorAccountModel, followerAPI).Result;
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (IsBrowser && lastCount == followersResponse.FollowersList.Count)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "sorry no results found...");
                        break;
                    }

                    lastCount = followersResponse.FollowersList.Count;
                    if (followersResponse.FollowersList.Count == 0)
                    {
                        jobProcessResult.HasNoResult = true;
                        break;
                    }

                    // remove self account from group otherwise it will scrap itself and send for messaging
                    if (queryInfo.QueryType != null && queryInfo.QueryType.Equals(LdConstants.GroupProcessor) &&
                        followersResponse.FollowersList.Count != 0)
                        followersResponse.FollowersList.RemoveAll(x =>
                            x.ProfileId == DominatorAccountModel.AccountBaseModel.UserId);

                    if (followersResponse.Success)
                    {
                        //Filter Already Interacted
                        RemoveOrSkipAlreadyInteractedLinkedInUsers(followersResponse.FollowersList);

                        start = followersResponse.PaginationCount;
                        if (followersResponse.FollowersList.Count > 0)
                        {
                            #region Skip Blacklisted User

                            if (modelClass.IsChkSkipBlackListedUser &&
                                (modelClass.IsChkPrivateBlackList || modelClass.IsChkGroupBlackList))
                                FilterBlacklistedUsers(followersResponse.FollowersList,
                                    modelClass.IsChkPrivateBlackList,
                                    modelClass.IsChkGroupBlackList);

                            #endregion

                            modelClass.ListUsersFromSelectedSource = followersResponse.FollowersList;
                            SkipUserAlreadyRecievedMessageFromSoftware(modelClass);

                            SkipUserAlreadyReceivedMessageFromOutSideSoftware(modelClass);
                            DominatorAccountModel.CancellationSource.Token.ThrowIfCancellationRequested();
                            foreach (var eachUser in followersResponse.FollowersList)
                            {
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
                        _delayService.ThreadSleep(new Random().Next(5000, 8000));
                    }
                    else
                    {
                        jobProcessResult.HasNoResult = true;
                    }
                }
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
            finally
            {

            }
        }
        public void SetModel(ref MapperModel modelClass)
        {
            try
            {
                switch (ActivityType)
                {
                    case DominatorHouseCore.Enums.ActivityType.UserScraper:
                        {
                            var userScraperModel = _processScopeModel.GetActivitySettingsAs<UserScraperModel>();
                            ClassMapper.MapModelClass(userScraperModel, ref modelClass);
                        }
                        break;

                    case DominatorHouseCore.Enums.ActivityType.BroadcastMessages:
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

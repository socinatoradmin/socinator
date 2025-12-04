
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.QdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Factories;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.Response;
using System;
using System.Linq;

namespace QuoraDominatorCore.QdLibrary
{
    public class DownvotePostProcess : QdJobProcessInteracted<InteractedPosts>
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private IQuoraBrowserManager _browser;
        private readonly IQdHttpHelper _httpHelper;
        private readonly SocialNetworks _networks;
        private readonly DownvotePostModel _downvotePostModel;
        private readonly IQuoraFunctions quoraFunct;
        private readonly IDbGlobalService dbGlobalService;
        private IQDBrowserManagerFactory managerFactory;
        public DownvotePostProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, IQuoraFunctions qdFunc,
            IQdQueryScraperFactory queryScraperFactory, IQdHttpHelper qdHttpHelper, IQdLogInProcess qdLogInProcess)
            : base(processScopeModel, accountServiceScoped, executionLimitsManager, queryScraperFactory, qdHttpHelper,
                qdLogInProcess)
        {
            dbGlobalService = globalService;
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _httpHelper = qdHttpHelper;//_accountScopeFactory[DominatorAccountModel.AccountId].Resolve<IQdHttpHelper>();
            quoraFunct = qdFunc;
            _downvotePostModel = processScopeModel.GetActivitySettingsAs<DownvotePostModel>();
            _networks = SocialNetworks.Quora;
            managerFactory = qdLogInProcess.managerFactory;
        }
        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            _browser = managerFactory.QdBrowserManager();
            DownvoteResponseHandler downvoteResponse = null;
            bool isSuccess=false;
            var jobprocessResult = new JobProcessResult();
            var postDetail = (PostDetails)scrapeResult.ResultUser;
            var IsBrowser = DominatorAccountModel.IsRunProcessThroughBrowser;
            try
            {
                if (IsBrowser)
                     isSuccess = _browser.DownVoteAnswer(postDetail.PostUrl);
                else 
                {   
                    var response=quoraFunct.CustomPostResponse(postDetail.PostUrl).Response;
                    downvoteResponse=quoraFunct.DownvotePost(DominatorAccountModel, response, postDetail).Result;
                }
                if(isSuccess || downvoteResponse.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, postDetail.PostUrl);
                    IncrementCounters();
                    AddUpvotedPostDataToDataBase(scrapeResult);
                    jobprocessResult.IsProcessSuceessfull = true;
                }
                else 
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed,
                      DominatorAccountModel.AccountBaseModel.AccountNetwork,
                      DominatorAccountModel.AccountBaseModel.UserName, ActivityType, postDetail.PostUrl,
                      $"Failed to {ActivityType}");
                    jobprocessResult.IsProcessSuceessfull = false;
                }
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            if (_downvotePostModel != null && _downvotePostModel.IsEnableAdvancedUserMode && _downvotePostModel.EnableDelayBetweenPerformingActionOnSamePost)
                DelayBeforeNextActivity(_downvotePostModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
            else
                DelayBeforeNextActivity();
            return jobprocessResult;
        }

        private void AddUpvotedPostDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var postDetail = (PostDetails)scrapeResult.ResultUser;

                #region Save To CampaignDb

                if (!string.IsNullOrEmpty(CampaignId))
                {
                    IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);
                    dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.QdTables.Campaigns.InteractedPosts
                    {
                        ActivityType = ActivityType.ToString(),
                        InteractionDate = DateTime.Now,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        SinAccUsername = DominatorAccountModel.UserName,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        PostUrl = postDetail.PostUrl,
                        LikeCount = postDetail.UpvoteCount,
                        ShareCount = postDetail.ShareCount,
                        CommentCount = postDetail.CommentCount,
                        ViewsCount = postDetail.ViewsCount,
                        PostOwnerName = postDetail.PostAuthorProfileUrl.Split('/').LastOrDefault(),
                        PostOwnerFollowerCount = postDetail.PostAuthorFollowerCount,
                        PostCreationTime = postDetail.Created
                    });
                }

                #endregion

                #region Save to AccountDb

                var dbAccountService =
                    InstanceProvider.ResolveAccountDbOperations(DominatorAccountModel.AccountId, _networks);

                dbAccountService.Add(new InteractedPosts
                {
                    ActivityType = ActivityType,
                    InteractionDate = DateTime.Now,
                    InteractionDateTimeStamp = DateTimeUtilities.GetEpochTime(),
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    AccountName = DominatorAccountModel.UserName,
                    PostUrl = postDetail.PostUrl,
                    UpvoteCount = postDetail.UpvoteCount,
                    ShareCount = postDetail.ShareCount,
                    CommentCount = postDetail.CommentCount,
                    ViewsCount = postDetail.ViewsCount,
                    PostOwnerName = postDetail.PostAuthorProfileUrl.Split('/').LastOrDefault(),
                    PostOwnerFollowerCount = postDetail.PostAuthorFollowerCount
                });

                #endregion

                #region Save to PrivateBlacklist DB

                if (_downvotePostModel.IsChkDownvotePostPrivateBlacklist)
                    dbAccountService.Add(
                        new PrivateBlacklist
                        {
                            UserName = postDetail.PostAuthorProfileUrl.Split('/').LastOrDefault(),
                            UserId = postDetail.PostId,
                            InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                        });

                #endregion

                #region Save to GroupBlacklist DB

                if (_downvotePostModel.IsChkDownvotePostGroupBlackList)
                    dbGlobalService.Add(new BlackListUser
                    {
                        UserName = postDetail.PostAuthorProfileUrl.Split('/').LastOrDefault(),
                        UserId = postDetail.PostId,
                        AddedDateTime = DateTime.Now
                    });

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}

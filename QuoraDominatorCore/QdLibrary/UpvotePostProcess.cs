using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.QdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Request;
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
    public class UpvotePostProcess : QdJobProcessInteracted<InteractedPosts>
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private IQuoraBrowserManager _browser;
        private readonly IQdHttpHelper _httpHelper;
        private readonly SocialNetworks _networks;
        private readonly UpvotePostsModel _upvotePostModel;
        private readonly IQuoraFunctions quoraFunct;
        private int _followedToday;
        private readonly IDbGlobalService dbGlobalService;
        private IQDBrowserManagerFactory managerFactory;
        public UpvotePostProcess(IProcessScopeModel processScopeModel,
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
            _upvotePostModel = processScopeModel.GetActivitySettingsAs<UpvotePostsModel>();
            _networks = SocialNetworks.Quora;
            managerFactory = qdLogInProcess.managerFactory;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            _browser = managerFactory.QdBrowserManager();
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var jobprocessResult = new JobProcessResult();
            UpvoteResponseHandler upvoteResponse=new UpvoteResponseHandler(new ResponseParameter());
            var postDetails=(PostDetails)scrapeResult.ResultUser;
            var IsBrowser = DominatorAccountModel.IsRunProcessThroughBrowser;
            try
            {
                if (IsBrowser)
                    upvoteResponse.Success= _browser.UpvoteAnswer(DominatorAccountModel, postDetails.PostUrl);
                else 
                {
                    var response = _httpHelper.GetRequest(postDetails.PostUrl).Response;
                    upvoteResponse = quoraFunct.UpvotePost(DominatorAccountModel, response, postDetails).Result;
                }
                if (upvoteResponse.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, postDetails.PostUrl);
                    IncrementCounters();
                    AddUpvotedPostDataToDataBase(scrapeResult);
                    jobprocessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, postDetails.PostUrl,
                        $"Failed to {ActivityType}");
                    jobprocessResult.IsProcessSuceessfull = false;
                }
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
            catch (System.Exception)
            {
            }
            if (_upvotePostModel != null && _upvotePostModel.IsEnableAdvancedUserMode && _upvotePostModel.EnableDelayBetweenPerformingActionOnSamePost)
                DelayBeforeNextActivity(_upvotePostModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
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
                        InteractionTimeStamp=DateTimeUtilities.GetEpochTime(),
                        SinAccUsername=DominatorAccountModel.UserName,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        PostUrl = postDetail.PostUrl,
                        LikeCount = postDetail.UpvoteCount,
                        ShareCount = postDetail.ShareCount,
                        CommentCount = postDetail.CommentCount,
                        ViewsCount = postDetail.ViewsCount,
                        PostOwnerName = postDetail.PostAuthorProfileUrl.Split('/').LastOrDefault(),
                        PostOwnerFollowerCount=postDetail.PostAuthorFollowerCount,
                        PostCreationTime=postDetail.Created
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
                    InteractionDateTimeStamp =DateTimeUtilities.GetEpochTime(),
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    AccountName=DominatorAccountModel.UserName,
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

                if (_upvotePostModel.IsChkUpvotePostPrivateBlacklist)
                    dbAccountService.Add(
                        new PrivateBlacklist
                        {
                            UserName = postDetail.PostAuthorProfileUrl.Split('/').LastOrDefault(),
                            UserId = postDetail.PostId,
                            InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                        });

                #endregion

                #region Save to GroupBlacklist DB

                if (_upvotePostModel.IsChkUpvotePostGroupBlackList)
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

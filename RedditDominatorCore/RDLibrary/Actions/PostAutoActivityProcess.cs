using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDRequest;
using System;

namespace RedditDominatorCore.RDLibrary.Actions
{
    internal class PostAutoActivityProcess : RdJobProcessInteracted<InteractedAutoActivityPost>
    {
        private readonly PostAutoActivityModel autoActivityModel;
        private readonly IRdBrowserManager _browserManager;
        private readonly IDbCampaignService _campaignService;
        private readonly IDbAccountServiceScoped _dbAccountServiceScoped;
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        public PostAutoActivityProcess(IProcessScopeModel processScopeModel, IRdQueryScraperFactory queryScraperFactor,
            IRedditLogInProcess redditLogInProcess, IRdHttpHelper rdHttpHelper, IDbAccountServiceScoped dbAccountService,
            IJobActivityConfigurationManager jobActivityConfiguration, IDbCampaignService dbCampaignService)
            : base(processScopeModel, queryScraperFactor, redditLogInProcess, rdHttpHelper)
        {
            autoActivityModel = processScopeModel.GetActivitySettingsAs<PostAutoActivityModel>();
            _browserManager = redditLogInProcess._browserManager;
            _dbAccountServiceScoped = dbAccountService;
            _jobActivityConfigurationManager = jobActivityConfiguration;
            _campaignService = dbCampaignService;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"Started Process to {ActivityType}");
                while (true)
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var IsSuccess = autoActivityModel.IsCheckVisitAndScroll;
                    var IsOnlyScroll = autoActivityModel.IsCheckVisitAndScroll && !autoActivityModel.IsChkFollowPostOwner && !autoActivityModel.IsChkJoinPostCommunity
                        && !autoActivityModel.IsChkUpvote && !autoActivityModel.IsChkDownvote && !autoActivityModel.IsChkUpvoteDownvoteComment;
                    RedditPost redditPost = new RedditPost();
                    var ResponeHandler = _browserManager.ScrollFeedAndGetResponse(DominatorAccountModel, scrapeResult?.QueryInfo?.QueryValue, IsOnlyScroll).Result;
                    if (ResponeHandler != null && ResponeHandler.Success)
                    {
                        if (!IsOnlyScroll && ResponeHandler.LstRedditPost.Count > 0)
                        {
                            var RandomOperationCount = RandomUtilties.GetRandomNumber(ResponeHandler.LstRedditPost.Count - 1);
                            var currentCount = 0;
                            foreach (var post in ResponeHandler.LstRedditPost)
                            {
                                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                if (autoActivityModel.IsChkFollowPostOwner)
                                {
                                    IsSuccess = _browserManager.Follow(DominatorAccountModel, post.User.Url).Status;
                                    if (IsSuccess)
                                    {
                                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                            DominatorAccountModel.AccountBaseModel.UserName, DominatorHouseCore.Enums.ActivityType.Follow, post?.User?.Url);
                                        post.User.IsFollowing = IsSuccess;
                                    }

                                }
                                if (autoActivityModel.IsChkJoinPostCommunity)
                                {
                                    var res = _browserManager.Subscribe(post.SubReddit.Url, DominatorAccountModel);
                                    IsSuccess = res.Item1;
                                    if (IsSuccess)
                                    {
                                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                            DominatorAccountModel.AccountBaseModel.UserName, DominatorHouseCore.Enums.ActivityType.Subscribe, post?.SubReddit?.Url);
                                        post.SubReddit.UserIsSubscriber = IsSuccess;
                                    }
                                    else
                                        post.SubReddit.UserIsSubscriber = res.Item2;

                                }
                                if (autoActivityModel.IsChkUpvote)
                                {
                                    var res = _browserManager.UpVote(DominatorHouseCore.Enums.ActivityType.Upvote, post.Permalink,id:post.Id);
                                    if (IsSuccess = res.Item1)
                                    {
                                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                            DominatorAccountModel.AccountBaseModel.UserName, DominatorHouseCore.Enums.ActivityType.Upvote, post.Permalink);
                                        post.Upvoted = res.Item1;
                                    }

                                }
                                if (autoActivityModel.IsChkDownvote)
                                {
                                    IsSuccess = _browserManager.DownVote(DominatorHouseCore.Enums.ActivityType.Downvote, post.Permalink,post.Id);
                                    if (IsSuccess)
                                    {
                                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                            DominatorAccountModel.AccountBaseModel.UserName, DominatorHouseCore.Enums.ActivityType.Downvote, post.Permalink);
                                        post.Downvoted = IsSuccess;
                                    }
                                }
                                if (autoActivityModel.IsChkUpvoteDownvoteComment)
                                {
                                    var res = _browserManager.UpVote(DominatorHouseCore.Enums.ActivityType.Upvote, post.Permalink, true, id: post.Id);
                                    if (IsSuccess = res.Item1 || res.Item2)
                                    {
                                        var activity = res.Item1 ? DominatorHouseCore.Enums.ActivityType.UpvoteComment : DominatorHouseCore.Enums.ActivityType.DownvoteComment;
                                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                            DominatorAccountModel.AccountBaseModel.UserName, activity, post.Permalink);
                                    }
                                }
                                if (IsSuccess)
                                {
                                    IncrementCounters();
                                    AddAutoActivityDataToDataBase(scrapeResult, post);
                                    jobProcessResult.IsProcessCompleted = CheckJobProcessLimitsReached();
                                    if (jobProcessResult.IsProcessCompleted)
                                        break;
                                    jobProcessResult.IsProcessSuceessfull = true;
                                    currentCount++;
                                    if (currentCount >= RandomOperationCount)
                                        break;
                                    DelayBeforeNextActivity();
                                }
                                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            }

                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "Auto Scroll And Visit");
                        }
                        if (jobProcessResult.IsProcessCompleted)
                            break;
                    }
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
            }
            catch (Exception)
            {
            }
            return jobProcessResult;
        }

        private void AddAutoActivityDataToDataBase(ScrapeResultNew scrapeResult, RedditPost redditPost)
        {
            try
            {
                var user = (RedditUser)scrapeResult.ResultUser;
                var moduleConfiguration =
                    _jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
                    _campaignService.Add(new DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedAutoActivityPostCampaign
                    {
                        ActivityType = ActivityType.ToString(),
                        PostId = redditPost?.PostId,
                        PostUrl = redditPost?.Permalink,
                        UserName = redditPost?.User?.Username,
                        IsFollowing = redditPost.User.IsFollowing,
                        IsJoined = redditPost.SubReddit.UserIsSubscriber,
                        IsUpvoted = redditPost.Upvoted,
                        IsDownvoted = redditPost.Downvoted,
                        InteractedDate = DateTime.Now,
                        ProfileUrl = redditPost?.User?.Url,
                        Created = redditPost.Created.EpochToDateTimeLocal(),
                        CommunityUrl = redditPost?.SubReddit?.Url
                    });


                _dbAccountServiceScoped.Add(new InteractedAutoActivityPost
                {
                    ActivityType = ActivityType.ToString(),
                    PostId = redditPost?.PostId,
                    PostUrl = redditPost?.Permalink,
                    UserName = redditPost?.User?.Username,
                    IsFollowing = redditPost.User.IsFollowing,
                    IsJoined = redditPost.SubReddit.UserIsSubscriber,
                    IsUpvoted = redditPost.Upvoted,
                    IsDownvoted = redditPost.Downvoted,
                    InteractedDate = DateTime.Now,
                    ProfileUrl = redditPost?.User?.Url,
                    Created = redditPost.Created.EpochToDateTimeLocal(),
                    CommunityUrl = redditPost?.SubReddit?.Url
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}

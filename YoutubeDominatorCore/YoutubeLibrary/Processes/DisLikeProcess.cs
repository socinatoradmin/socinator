using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.YdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using System;
using YoutubeDominatorCore.Request;
using YoutubeDominatorCore.YDFactories;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeLibrary.YdFunctions;
using YoutubeDominatorCore.YoutubeModels;
using YoutubeDominatorCore.YoutubeModels.EngageModel;

namespace YoutubeDominatorCore.YoutubeLibrary.Processes
{
    public class DisLikeProcess : YdJobProcessInteracted<InteractedPosts>
    {
        private readonly IDbAccountServiceScoped _accountServiceScoped;
        private readonly IDbCampaignService _campaignService;

        private readonly IYoutubeFunctionality _youtubeFunctionality;

        public DisLikeProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, IYdQueryScraperFactory queryScraperFactory,
            IYdHttpHelper ydHttpHelper, IYoutubeLogInProcess ydLogInProcess, IDbCampaignService campaignService,
            IYoutubeFunctionality youtubeFunctionality)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ydHttpHelper, ydLogInProcess)
        {
            _campaignService = campaignService;
            _accountServiceScoped = accountServiceScoped;
            _youtubeFunctionality = youtubeFunctionality;
            EngageDislikeModel = processScopeModel.GetActivitySettingsAs<DislikeModel>();
        }

        public DislikeModel EngageDislikeModel { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var youtubePost = (YoutubePost)scrapeResult.ResultPost;

            var jobProcessResult = new JobProcessResult();
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var response = DominatorAccountModel.IsRunProcessThroughBrowser
                    ? _youtubeLogInProcess.BrowserManager.LikeDislikeVideo(ActivityType, youtubePost, 2)
                    : _youtubeFunctionality.LikeDislikeVideo(DominatorAccountModel, ActivityType, ref youtubePost);
                if (response.Success)
                {
                    SuccessLog(youtubePost.PostUrl);

                    IncrementCounters();

                    AddToDataBase(youtubePost, scrapeResult.QueryInfo);
                    jobProcessResult.IsProcessSuceessfull = true;
                    if (IsCustom)
                        DominatorAccountModel.IsNeedToSchedule = false;
                }
                else
                {
                    FailedLog(youtubePost.Reaction == InteractedPosts.LikeStatus.Dislike
                        ? $"- Already disliked {youtubePost.PostUrl}"
                        : youtubePost.PostUrl);
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                if (EngageDislikeModel != null && EngageDislikeModel.EnableDelayBetweenPerformingActionOnSamePost)
                    DelayBeforeNextActivity(EngageDislikeModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
                else
                    DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception e)
            {
                e.DebugLog();
            }

            return jobProcessResult;
        }

        public void AddToDataBase(YoutubePost youtubePost, QueryInfo queryInfo)
        {
            try
            {
                _accountServiceScoped.Add(new InteractedPosts
                {
                    AccountUsername = DominatorAccountModel.AccountBaseModel.UserName ?? "",
                    ActivityType = ActivityType.Dislike.ToString(),
                    ChannelId = youtubePost.ChannelId ?? "",
                    InteractedChannelUsername = youtubePost.ChannelUsername ?? "",
                    ChannelName = youtubePost.ChannelTitle,
                    CommentCount = youtubePost.CommentCount ?? "",
                    DislikeCount = youtubePost.DislikeCount ?? "",
                    InteractionTimeStamp = DateTime.UtcNow.GetCurrentEpochTime(),
                    LikeCount = youtubePost.LikeCount ?? "",
                    ReactionOnPost = InteractedPosts.LikeStatus.Like,
                    PostDescription = youtubePost.Caption ?? "",
                    PublishedDate = youtubePost.PublishedDate ?? "",
                    QueryType = queryInfo.QueryType ?? "",
                    QueryValue = queryInfo.QueryValue ?? "",
                    SubscribeCount = youtubePost.ChannelSubscriberCount ?? "",
                    VideoLength = youtubePost.VideoLength,
                    VideoUrl = youtubePost.PostUrl ?? "",
                    ViewsCount = youtubePost.ViewsCount ?? "",
                    MyChannelId = DominatorAccountModel.AccountBaseModel.UserId,
                    MyChannelPageId = DominatorAccountModel.AccountBaseModel.ProfileId,
                    PostTitle = youtubePost.Title,
                    IsSubscribed = youtubePost.IsChannelSubscribed
                });

                if (!string.IsNullOrEmpty(CampaignId))
                {
                    var existing =
                        _campaignService
                            .GetSingle<DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedPosts>(x =>
                                x.AccountUsername == DominatorAccountModel.AccountBaseModel.UserName &&
                                x.MyChannelId == DominatorAccountModel.AccountBaseModel.UserId &&
                                x.VideoUrl == youtubePost.PostUrl);
                    var existed = existing != null;
                    if (!existed)
                        existing = new DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedPosts();

                    existing.AccountUsername = DominatorAccountModel.AccountBaseModel.UserName ?? "";
                    existing.ActivityType = ActivityType.Dislike.ToString();
                    existing.ChannelId = youtubePost.ChannelId ?? "";
                    existing.InteractedChannelUsername = youtubePost.ChannelUsername ?? "";
                    existing.ChannelName = youtubePost.ChannelTitle;
                    existing.CommentCount = youtubePost.CommentCount ?? "";
                    existing.DislikeCount = youtubePost.DislikeCount ?? "";
                    existing.InteractionTimeStamp = DateTime.UtcNow.GetCurrentEpochTime();
                    existing.LikeCount = youtubePost.LikeCount ?? "";
                    existing.ReactionOnPost = youtubePost.Reaction;
                    existing.PostDescription = youtubePost.Caption ?? "";
                    existing.PublishedDate = youtubePost.PublishedDate ?? "";
                    existing.QueryType = queryInfo.QueryType ?? "";
                    existing.QueryValue = queryInfo.QueryValue ?? "";
                    existing.SubscribeCount = youtubePost.ChannelSubscriberCount ?? "";
                    existing.VideoLength = youtubePost.VideoLength;
                    existing.VideoUrl = youtubePost.PostUrl ?? "";
                    existing.ViewsCount = youtubePost.ViewsCount ?? "";
                    existing.MyChannelId = DominatorAccountModel.AccountBaseModel.UserId;
                    existing.MyChannelPageId = DominatorAccountModel.AccountBaseModel.ProfileId;
                    existing.Status = "Success";
                    existing.PostTitle = youtubePost.Title;
                    existing.IsSubscribed = youtubePost.IsChannelSubscribed;


                    if (existed)
                        _campaignService.Update(existing);
                    else
                        _campaignService.Add(existing);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
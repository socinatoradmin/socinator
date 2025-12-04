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
using YoutubeDominatorCore.YoutubeModels;
using YoutubeDominatorCore.YoutubeModels.ScraperModel;

namespace YoutubeDominatorCore.YoutubeLibrary.Processes
{
    public class ScrapePostProcess : YdJobProcessInteracted<InteractedPosts>
    {
        private readonly IDbAccountServiceScoped _accountServiceScoped;

        private readonly IDbCampaignService _campaignService;

        public ScrapePostProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, IYdQueryScraperFactory queryScraperFactory,
            IYdHttpHelper ydHttpHelper, IYoutubeLogInProcess ydLogInProcess, IDbCampaignService campaignService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ydHttpHelper, ydLogInProcess)
        {
            _campaignService = campaignService;
            _accountServiceScoped = accountServiceScoped;
            ScraperPostScraperModel = processScopeModel.GetActivitySettingsAs<PostScraperModel>();
        }

        public PostScraperModel ScraperPostScraperModel { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var youtubePost = (YoutubePost)scrapeResult.ResultPost;

            var jobProcessResult = new JobProcessResult();

            try
            {
                IncrementCounters();
                jobProcessResult.IsProcessSuceessfull = true;
                AddToDataBase(youtubePost, scrapeResult.QueryInfo);
                SuccessLog(youtubePost.PostUrl);
                if (IsCustom)
                    DominatorAccountModel.IsNeedToSchedule = false;
                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
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
                    ActivityType = ActivityType.PostScraper.ToString(),
                    ChannelId = youtubePost.ChannelId ?? "",
                    InteractedChannelUsername = youtubePost.ChannelUsername ?? "",
                    ChannelName = youtubePost.ChannelTitle,
                    CommentCount = youtubePost.CommentCount ?? "",
                    DislikeCount = youtubePost.DislikeCount ?? "",
                    InteractionTimeStamp = DateTime.UtcNow.GetCurrentEpochTime(),
                    LikeCount = youtubePost.LikeCount ?? "",
                    PostTitle = youtubePost?.Title ?? "",
                    ReactionOnPost = youtubePost.Reaction,
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
                    IsSubscribed = youtubePost.IsChannelSubscribed
                });
                if (!string.IsNullOrEmpty(CampaignId))
                    _campaignService.Add(new DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedPosts
                    {
                        AccountUsername = DominatorAccountModel.AccountBaseModel.UserName ?? "",
                        ActivityType = ActivityType.PostScraper.ToString(),
                        ChannelId = youtubePost.ChannelId ?? "",
                        InteractedChannelUsername = youtubePost.ChannelUsername ?? "",
                        ChannelName = youtubePost.ChannelTitle,
                        CommentCount = youtubePost.CommentCount ?? "",
                        PostTitle = youtubePost?.Title ?? "",
                        DislikeCount = youtubePost.DislikeCount ?? "",
                        InteractionTimeStamp = DateTime.UtcNow.GetCurrentEpochTime(),
                        LikeCount = youtubePost.LikeCount ?? "",
                        ReactionOnPost = youtubePost.Reaction,
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
                        IsSubscribed = youtubePost.IsChannelSubscribed
                    });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
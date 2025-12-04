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
    public class ScrapChannelProcess : YdJobProcessInteracted<InteractedChannels>
    {
        private readonly IDbAccountServiceScoped _accountServiceScoped;

        private readonly IDbCampaignService _campaignService;

        public ScrapChannelProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, IYdQueryScraperFactory queryScraperFactory,
            IYdHttpHelper ydHttpHelper, IYoutubeLogInProcess ydLogInProcess, IDbCampaignService campaignService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ydHttpHelper, ydLogInProcess)
        {
            _campaignService = campaignService;
            _accountServiceScoped = accountServiceScoped;
            ScraperChannelScraperModel = processScopeModel.GetActivitySettingsAs<ChannelScraperModel>();
        }

        public ChannelScraperModel ScraperChannelScraperModel { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var youtubeChannel = (YoutubeChannel)scrapeResult.ResultChannel;

            var jobProcessResult = new JobProcessResult();

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                SuccessLog(" => " + youtubeChannel.ChannelName);
                IncrementCounters();

                jobProcessResult.IsProcessSuceessfull = true;
                AddToDataBase(youtubeChannel, scrapeResult.QueryInfo);
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

        public void AddToDataBase(YoutubeChannel youtubeChannel, QueryInfo queryInfo)
        {
            try
            {
                _accountServiceScoped.Add(new InteractedChannels
                {
                    AccountUsername = DominatorAccountModel.AccountBaseModel.UserName ?? "",
                    ActivityType = ActivityType.ChannelScraper.ToString(),
                    ChannelDescription = youtubeChannel.ChannelDescription ?? "",
                    ExternalLinks = youtubeChannel.ExternalLinks ?? "",
                    InteractedChannelId = youtubeChannel.ChannelId ?? "",
                    InteractedChannelUsername = youtubeChannel.ChannelUsername ?? "",
                    InteractedChannelName = youtubeChannel.ChannelName ?? "",
                    InteractionTimeStamp = DateTime.UtcNow.GetCurrentEpochTime(),
                    QueryType = queryInfo.QueryType ?? "",
                    QueryValue = queryInfo.QueryValue ?? "",
                    SubscriberCount = youtubeChannel.SubscriberCount ?? "",
                    IsSubscribed = youtubeChannel.IsSubscribed,
                    ChannelJoinedDate = youtubeChannel.ChannelJoinedDate ?? "",
                    ChannelLocation = youtubeChannel.ChannelLocation ?? "",
                    ChannelProfilePic = youtubeChannel.ProfilePicUrl ?? "",
                    ChannelUrl = youtubeChannel.ChannelUrl ?? "",
                    VideosCount = youtubeChannel.VideosCount ?? "",
                    ViewsCount = youtubeChannel.ViewsCount ?? "",
                    MyChannelId = DominatorAccountModel.AccountBaseModel.UserId,
                    MyChannelPageId = DominatorAccountModel.AccountBaseModel.ProfileId,
                    InteractedCommentUrl = !string.IsNullOrWhiteSpace(youtubeChannel.InteractedCommentUrl)
                        ? youtubeChannel.InteractedCommentUrl
                        : ""
                });

                if (!string.IsNullOrEmpty(CampaignId))
                    _campaignService.Add(new DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedChannels
                    {
                        AccountUsername = DominatorAccountModel.AccountBaseModel.UserName ?? "",
                        ActivityType = ActivityType.ChannelScraper.ToString(),
                        ChannelDescription = youtubeChannel.ChannelDescription ?? "",
                        ExternalLinks = youtubeChannel.ExternalLinks ?? "",
                        InteractedChannelId = youtubeChannel.ChannelId ?? "",
                        InteractedChannelUsername = youtubeChannel.ChannelUsername ?? "",
                        InteractedChannelName = youtubeChannel.ChannelName ?? "",
                        InteractionTimeStamp = DateTime.UtcNow.GetCurrentEpochTime(),
                        QueryType = queryInfo.QueryType ?? "",
                        QueryValue = queryInfo.QueryValue ?? "",
                        SubscriberCount = youtubeChannel.SubscriberCount ?? "",
                        IsSubscribed = youtubeChannel.IsSubscribed,
                        ChannelJoinedDate = youtubeChannel.ChannelJoinedDate ?? "",
                        ChannelLocation = youtubeChannel.ChannelLocation ?? "",
                        ChannelProfilePic = youtubeChannel.ProfilePicUrl ?? "",
                        ChannelUrl = youtubeChannel.ChannelUrl ?? "",
                        VideosCount = youtubeChannel.VideosCount ?? "",
                        ViewsCount = youtubeChannel.ViewsCount ?? "",
                        MyChannelId = DominatorAccountModel.AccountBaseModel.UserId,
                        MyChannelPageId = DominatorAccountModel.AccountBaseModel.ProfileId,
                        InteractedCommentUrl = !string.IsNullOrWhiteSpace(youtubeChannel.InteractedCommentUrl)
                            ? youtubeChannel.InteractedCommentUrl
                            : ""
                    });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
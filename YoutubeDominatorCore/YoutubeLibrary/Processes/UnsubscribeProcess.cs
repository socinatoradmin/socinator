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
using YoutubeDominatorCore.YoutubeModels.GrowSubscribersModel;

namespace YoutubeDominatorCore.YoutubeLibrary.Processes
{
    public class UnsubscribeProcess : YdJobProcessInteracted<InteractedChannels>
    {
        private readonly IDbAccountServiceScoped _accountServiceScoped;
        private readonly IDbCampaignService _campaignService;

        private readonly IYoutubeFunctionality _youtubeFunctionality;

        public UnsubscribeProcess(IProcessScopeModel processScopeModel,
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
            EngageUnsubscribeModel = processScopeModel.GetActivitySettingsAs<UnsubscribeModel>();
        }

        public UnsubscribeModel EngageUnsubscribeModel { get; set; }


        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var youtubeChannel = (YoutubeChannel)scrapeResult.ResultChannel;

            //for unit Test campaign id is not coming so alternate way
            var youtubePost = (YoutubePost)scrapeResult.ResultPost;
            if (youtubePost != null)
                CampaignId = youtubePost.Id;

            var jobProcessResult = new JobProcessResult();
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var response = DominatorAccountModel.IsRunProcessThroughBrowser
                    ? _youtubeLogInProcess.BrowserManager.UnsubscribeChannel(DominatorAccountModel, youtubeChannel)
                    : _youtubeFunctionality.UnsubscribeChannel(DominatorAccountModel, ref youtubeChannel);

                if (response.Success)
                {
                    SuccessLog(youtubeChannel.ChannelUsername);
                    IncrementCounters();

                    AddToDataBase(youtubeChannel, scrapeResult.QueryInfo);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else if (!youtubeChannel.IsSubscribed)
                {
                    FailedLog($"Already unsubscribed channel -> {youtubeChannel.ChannelUsername}.");
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                else
                {
                    FailedLog(youtubeChannel.ChannelUsername);
                    jobProcessResult.IsProcessSuceessfull = false;
                }

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
                    ActivityType = ActivityType.UnSubscribe.ToString(),
                    ChannelDescription = youtubeChannel.ChannelDescription ?? "",
                    ExternalLinks = youtubeChannel.ExternalLinks ?? "",
                    InteractedChannelId = youtubeChannel.ChannelId ?? "",
                    InteractedChannelUsername = youtubeChannel.ChannelUsername ?? "",
                    InteractedChannelName = youtubeChannel.ChannelName ?? "",
                    InteractionTimeStamp = DateTime.UtcNow.GetCurrentEpochTime(),
                    QueryType = queryInfo.QueryType ?? "",
                    QueryValue = queryInfo.QueryValue ?? "",
                    SubscriberCount = youtubeChannel.SubscriberCount ?? "",
                    IsSubscribed = false,
                    ChannelJoinedDate = youtubeChannel.ChannelJoinedDate ?? "",
                    ChannelLocation = youtubeChannel.ChannelLocation ?? "",
                    ChannelProfilePic = youtubeChannel.ProfilePicUrl ?? "",
                    ChannelUrl = youtubeChannel.ChannelUrl ?? "",
                    VideosCount = youtubeChannel.VideosCount ?? "",
                    ViewsCount = youtubeChannel.ViewsCount ?? "",
                    MyChannelId = DominatorAccountModel.AccountBaseModel.UserId,
                    MyChannelPageId = DominatorAccountModel.AccountBaseModel.ProfileId
                });

                if (!string.IsNullOrEmpty(CampaignId))
                    _campaignService.Add(new DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedChannels
                    {
                        AccountUsername = DominatorAccountModel.AccountBaseModel.UserName ?? "",
                        ActivityType = ActivityType.UnSubscribe.ToString(),
                        ChannelDescription = youtubeChannel.ChannelDescription ?? "",
                        ExternalLinks = youtubeChannel.ExternalLinks ?? "",
                        InteractedChannelId = youtubeChannel.ChannelId ?? "",
                        InteractedChannelUsername = youtubeChannel.ChannelUsername ?? "",
                        InteractedChannelName = youtubeChannel.ChannelName ?? "",
                        InteractionTimeStamp = DateTime.UtcNow.GetCurrentEpochTime(),
                        QueryType = queryInfo.QueryType ?? "",
                        QueryValue = queryInfo.QueryValue ?? "",
                        SubscriberCount = youtubeChannel.SubscriberCount ?? "",
                        IsSubscribed = false,
                        ChannelJoinedDate = youtubeChannel.ChannelJoinedDate ?? "",
                        ChannelLocation = youtubeChannel.ChannelLocation ?? "",
                        ChannelProfilePic = youtubeChannel.ProfilePicUrl ?? "",
                        ChannelUrl = youtubeChannel.ChannelUrl ?? "",
                        VideosCount = youtubeChannel.VideosCount ?? "",
                        ViewsCount = youtubeChannel.ViewsCount ?? "",
                        MyChannelId = DominatorAccountModel.AccountBaseModel.UserId,
                        MyChannelPageId = DominatorAccountModel.AccountBaseModel.ProfileId
                    });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
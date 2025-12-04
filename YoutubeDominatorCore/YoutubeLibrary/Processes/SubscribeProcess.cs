using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.YdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
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
    public class SubscribeProcess : YdJobProcessInteracted<InteractedChannels>
    {
        private readonly IDbAccountServiceScoped _accountServiceScoped;
        private readonly IDbCampaignService _campaignService;

        private readonly IYoutubeFunctionality _youtubeFunctionality;

        public SubscribeProcess(IProcessScopeModel processScopeModel,
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
            EngageSubscribeModel = processScopeModel.GetActivitySettingsAs<SubscribeModel>();
        }

        public SubscribeModel EngageSubscribeModel { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            scrapeResult.ActivityType = ActivityType.Subscribe;
            var jobProcessResult = new JobProcessResult();

            var youtubeChannel = (YoutubeChannel)scrapeResult.ResultChannel;

            //for unit Test campaign id is not coming so alternate way
            var youtubePost = (YoutubePost)scrapeResult.ResultPost;
            if (youtubePost != null)
                CampaignId = youtubePost.Id;

            youtubeChannel.ChannelUrl = "https://www.youtube.com/channel/" + youtubeChannel.ChannelId;

            var campaignInteractionDetails =
                InstanceProvider.GetInstance<ICampaignInteractionDetails>();

            var jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleSettings = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
            //var moduleSettings = DominatorAccountModel.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == ActivityType);

            if (moduleSettings.IsTemplateMadeByCampaignMode && EngageSubscribeModel.UniqueSubscribe)
                try
                {
                    campaignInteractionDetails.AddInteractedData(SocialNetworks.YouTube, CampaignId,
                        youtubeChannel.ChannelId);
                }
                catch (Exception)
                {
                    jobProcessResult.IsProcessSuceessfull = false;
                    return jobProcessResult;
                }


            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var response = DominatorAccountModel.IsRunProcessThroughBrowser
                    ? _youtubeLogInProcess.BrowserManager.SubscribeChannel(DominatorAccountModel, youtubeChannel)
                    : _youtubeFunctionality.SubscribeChannel(DominatorAccountModel, youtubeChannel);

                if (response.Success)
                {
                    SuccessLog(youtubeChannel.ChannelUrl);
                    IncrementCounters();
                    if (IsCustom)
                        DominatorAccountModel.IsNeedToSchedule = false;
                    AddToDataBase(youtubeChannel, scrapeResult.QueryInfo);

                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else if (youtubeChannel.IsSubscribed)
                {
                    FailedLog($"Already subscribed channel -> {youtubeChannel.ChannelId}.");
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                else
                {
                    FailedLog(youtubeChannel.ChannelUrl);

                    if (moduleSettings.IsTemplateMadeByCampaignMode && EngageSubscribeModel.UniqueSubscribe)
                        try
                        {
                            campaignInteractionDetails.AddInteractedData(SocialNetworks.YouTube, CampaignId,
                                youtubeChannel.ChannelId);
                        }
                        catch
                        {
                        }

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
                    ActivityType = ActivityType.Subscribe.ToString(),
                    ChannelDescription = youtubeChannel.ChannelDescription ?? "",
                    ExternalLinks = youtubeChannel.ExternalLinks ?? "",
                    InteractedChannelId = youtubeChannel.ChannelId ?? "",
                    InteractedChannelUsername = youtubeChannel.ChannelUsername ?? "",
                    InteractedChannelName = youtubeChannel.ChannelName ?? "",
                    InteractionTimeStamp = DateTime.Now.GetCurrentEpochTime(),
                    QueryType = queryInfo.QueryType ?? "",
                    QueryValue = queryInfo.QueryValue ?? "",
                    SubscriberCount = youtubeChannel.SubscriberCount ?? "",
                    IsSubscribed = true,
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
                        ActivityType = ActivityType.Subscribe.ToString(),
                        ChannelDescription = youtubeChannel.ChannelDescription ?? "",
                        ExternalLinks = youtubeChannel.ExternalLinks ?? "",
                        InteractedChannelId = youtubeChannel.ChannelId ?? "",
                        InteractedChannelUsername = youtubeChannel.ChannelUsername ?? "",
                        InteractedChannelName = youtubeChannel.ChannelName ?? "",
                        InteractionTimeStamp = DateTime.Now.GetCurrentEpochTime(),
                        QueryType = queryInfo.QueryType ?? "",
                        QueryValue = queryInfo.QueryValue ?? "",
                        SubscriberCount = youtubeChannel.SubscriberCount ?? "",
                        IsSubscribed = true,
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
            catch (Exception Ex)
            {
                Ex.DebugLog();
            }
        }
    }
}
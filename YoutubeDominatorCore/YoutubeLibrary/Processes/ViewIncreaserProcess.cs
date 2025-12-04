using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using System;
using System.Linq;
using YoutubeDominatorCore.Request;
using YoutubeDominatorCore.YDEnums;
using YoutubeDominatorCore.YDFactories;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeLibrary.PuppeteerBrowser;
using YoutubeDominatorCore.YoutubeLibrary.YdFunctions;
using YoutubeDominatorCore.YoutubeModels;
using YoutubeDominatorCore.YoutubeModels.WatchVideoModel;
using AccountInteractedPosts = DominatorHouseCore.DatabaseHandler.YdTables.Accounts.InteractedPosts;
using CampaignInteractedPosts = DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedPosts;

namespace YoutubeDominatorCore.YoutubeLibrary.Processes
{
    public class ViewIncreaserProcess : YdJobProcessInteracted<AccountInteractedPosts>
    {
        private readonly IDbAccountServiceScoped _accountServiceScoped;

        private readonly IDbCampaignService _campaignService;
        PupBrowserRequest pupBrowser = new PupBrowserRequest();
        public ViewIncreaserProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, IYdQueryScraperFactory queryScraperFactory,
            IYdHttpHelper ydHttpHelper, IYoutubeLogInProcess ydLogInProcess, IDbCampaignService campaignService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ydHttpHelper, ydLogInProcess)
        {
            _campaignService = campaignService;
            _accountServiceScoped = accountServiceScoped;
            EngageViewIncreaserModel = processScopeModel.GetActivitySettingsAs<ViewIncreaserModel>();
        }

        public ViewIncreaserModel EngageViewIncreaserModel { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {

            var youtubePost = (YoutubePost)scrapeResult.ResultPost;

            var jobProcessResult = new JobProcessResult();
            try
            {
                int delay;
                var videoDuration = youtubePost.VideoLength > 0 ? youtubePost.VideoLength : 30;

                if (EngageViewIncreaserModel.IsChkWatchVideoBetweentSeconds)
                {
                    delay = RandomUtilties.GetRandomNumber(
                        EngageViewIncreaserModel.StopWatchVideoBetweentSeconds.EndValue,
                        EngageViewIncreaserModel.StopWatchVideoBetweentSeconds.StartValue);
                    if (delay > videoDuration)
                        delay = videoDuration;
                }
                else if (EngageViewIncreaserModel.IsChkWatchVideoBetweenPercentage)
                {
                    var percentage = RandomUtilties.GetRandomNumber(
                        EngageViewIncreaserModel.StopWatchVideoBetweenPercentage.EndValue,
                        EngageViewIncreaserModel.StopWatchVideoBetweenPercentage.StartValue);

                    delay = videoDuration * percentage / 100;
                }
                else
                {
                    delay = videoDuration;
                }

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                // add 5 seconds to load the video
                delay = delay + 5;
                var pageId = EngageViewIncreaserModel.ListSelectDestination
                    .FirstOrDefault(x => x.AccountId == DominatorAccountModel.AccountBaseModel.AccountId)
                    ?.SelectedChannel;

                if (!EngageViewIncreaserModel.IsChkRepeatVideo)
                    EngageViewIncreaserModel.ViewCountForRepeatVideo = 0;
                YdBrowserManager browser = null;
                BrowserManager.BrowserDispatcher(BrowserFuncts.GetPageText, 3.5);
                bool viewed;
                if (DominatorAccountModel.IsRunProcessThroughBrowser && !BrowserManager.CurrentData.Contains("Your browser can't play this video"))
                {
                    viewed = BrowserManager.ViewIncreaserVideo(DominatorAccountModel, youtubePost, delay, pageId,
                        EngageViewIncreaserModel.MozillaSelected, EngageViewIncreaserModel.IsBrowserHidden,
                        EngageViewIncreaserModel.SkipAd).Result;
                }
                else if (DominatorAccountModel.IsRunProcessThroughBrowser && BrowserManager.CurrentData.Contains("Your browser can't play this video"))
                {
                    BrowserManager.CloseBrowser();
                    CustomLog("Not Supported in this Browser Trying to Play in Chrome for this Vedio");
                    pupBrowser = new PupBrowserRequest(DominatorAccountModel, JobCancellationTokenSource);
                    viewed = pupBrowser.ViewIncreaserVideo(DominatorAccountModel, youtubePost, delay, pageId,
                        EngageViewIncreaserModel.IsBrowserHidden,
                       EngageViewIncreaserModel.SkipAd);
                }
                else
                {
                    browser = new YdBrowserManager();
                    browser.IsLoggedIn = true;
                    browser.SetAccount(DominatorAccountModel);
                    browser.SetCancellationToken(JobCancellationTokenSource.Token);
                    viewed = browser.ViewIncreaserVideo(DominatorAccountModel, youtubePost, delay, pageId,
                        EngageViewIncreaserModel.MozillaSelected, EngageViewIncreaserModel.IsBrowserHidden,
                        EngageViewIncreaserModel.SkipAd).Result;
                    browser.CloseBrowser();
                }

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (viewed)
                {
                    var watchCount = youtubePost.TotalWatchingCount + 1;
                    SuccessLog($"{youtubePost.PostUrl} ({watchCount} times viewed)");

                    IncrementCounters();
                    AddToDataBase(youtubePost, scrapeResult.QueryInfo, watchCount);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    if (!JobCancellationTokenSource.Token.IsCancellationRequested)
                        FailedLog(youtubePost.PostUrl);

                    jobProcessResult.IsProcessSuceessfull = false;
                }

                if (browser != null) browser.PlayPauseVideo();
                if (EngageViewIncreaserModel != null && EngageViewIncreaserModel.IsChkRepeatVideo && youtubePost.TotalWatchingCount < EngageViewIncreaserModel.ViewCountForRepeatVideo)
                    DelayBeforeNextActivity(EngageViewIncreaserModel.RepeatWatchAfterMinutes.GetRandom() * 60);
                else
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
            finally
            {
                if (pupBrowser.browserActivity != null)
                    pupBrowser.browserActivity.ClosedBrowser();
            }

            return jobProcessResult;
        }
        public void AddToDataBase(YoutubePost youtubePost, QueryInfo queryInfo, int viewedCount)
        {
            try
            {
                var actType = ActivityType.ViewIncreaser.ToString();
                var channelId = string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.UserId) ? YdStatic.DefaultChannel : DominatorAccountModel.AccountBaseModel.UserId;
                var accData = _accountServiceScoped.GetSingle<AccountInteractedPosts>(x =>
                    x.AccountUsername == DominatorAccountModel.AccountBaseModel.UserName
                    && x.ActivityType == actType && x.QueryType == queryInfo.QueryType &&
                    x.QueryValue == queryInfo.QueryValue
                    && x.VideoUrl == youtubePost.PostUrl &&
                    x.MyChannelId == channelId &&
                    x.MyChannelPageId == DominatorAccountModel.AccountBaseModel.ProfileId);

                var justAddItToAcc = accData == null;
                SetAccountData(ref accData, youtubePost, queryInfo, viewedCount);
                if (justAddItToAcc)
                    _accountServiceScoped.Add(accData);
                else
                    _accountServiceScoped.Update(accData);

                if (!string.IsNullOrEmpty(CampaignId))
                {
                    var campData = _campaignService.GetSingle<CampaignInteractedPosts>(x =>
                        x.AccountUsername == DominatorAccountModel.AccountBaseModel.UserName
                        && x.ActivityType == actType && x.QueryType == queryInfo.QueryType &&
                        x.QueryValue == queryInfo.QueryValue
                        && x.VideoUrl == youtubePost.PostUrl &&
                        x.MyChannelId == channelId &&
                        x.MyChannelPageId == DominatorAccountModel.AccountBaseModel.ProfileId);

                    var justAddItToCamp = campData == null;
                    SetCampData(ref campData, youtubePost, queryInfo, viewedCount);
                    if (justAddItToCamp)
                        _campaignService.Add(campData);
                    else
                        _campaignService.Update(campData);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SetAccountData(ref AccountInteractedPosts interactedPosts, YoutubePost youtubePost,
            QueryInfo queryInfo, int viewedCount)
        {
            interactedPosts = interactedPosts ?? new AccountInteractedPosts();

            interactedPosts.AccountUsername = DominatorAccountModel.AccountBaseModel.UserName ?? "";
            interactedPosts.ActivityType = ActivityType.ViewIncreaser.ToString();
            interactedPosts.ChannelId = youtubePost.ChannelId ?? "";
            interactedPosts.InteractedChannelUsername = youtubePost.ChannelUsername ?? "";
            interactedPosts.ChannelName = youtubePost.ChannelTitle;
            interactedPosts.CommentCount = youtubePost.CommentCount ?? "";
            interactedPosts.DislikeCount = youtubePost.DislikeCount ?? "";
            interactedPosts.InteractionTimeStamp = DateTime.Now.GetCurrentEpochTime();
            interactedPosts.LikeCount = youtubePost.LikeCount ?? "";
            interactedPosts.ReactionOnPost = youtubePost.Reaction;
            interactedPosts.PostDescription = youtubePost.Caption ?? "";
            interactedPosts.PublishedDate = youtubePost.PublishedDate ?? "";
            interactedPosts.QueryType = queryInfo.QueryType ?? "";
            interactedPosts.QueryValue = queryInfo.QueryValue ?? "";
            interactedPosts.SubscribeCount = youtubePost.ChannelSubscriberCount ?? "";
            interactedPosts.VideoLength = youtubePost.VideoLength;
            interactedPosts.VideoUrl = youtubePost.PostUrl ?? "";
            interactedPosts.ViewsCount = youtubePost.ViewsCount ?? "";
            interactedPosts.MyChannelId = string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.UserId) ? YdStatic.DefaultChannel : DominatorAccountModel.AccountBaseModel.UserId;
            interactedPosts.MyChannelPageId = DominatorAccountModel.AccountBaseModel.ProfileId;
            interactedPosts.ViewingCountPerAccount = viewedCount;
            interactedPosts.PostTitle = youtubePost.Title;
            interactedPosts.IsSubscribed = youtubePost.IsChannelSubscribed;
        }

        private void SetCampData(ref CampaignInteractedPosts interactedPosts, YoutubePost youtubePost,
            QueryInfo queryInfo, int viewedCount)
        {
            interactedPosts = interactedPosts ?? new CampaignInteractedPosts();

            interactedPosts.AccountUsername = DominatorAccountModel.AccountBaseModel.UserName ?? "";
            interactedPosts.ActivityType = ActivityType.ViewIncreaser.ToString();
            interactedPosts.ChannelId = youtubePost.ChannelId ?? "";
            interactedPosts.InteractedChannelUsername = youtubePost.ChannelUsername ?? "";
            interactedPosts.ChannelName = youtubePost.ChannelTitle;
            interactedPosts.CommentCount = youtubePost.CommentCount ?? "";
            interactedPosts.DislikeCount = youtubePost.DislikeCount ?? "";
            interactedPosts.InteractionTimeStamp = DateTime.Now.GetCurrentEpochTime();
            interactedPosts.LikeCount = youtubePost.LikeCount ?? "";
            interactedPosts.ReactionOnPost = youtubePost.Reaction;
            interactedPosts.PostDescription = youtubePost.Caption ?? "";
            interactedPosts.PublishedDate = youtubePost.PublishedDate ?? "";
            interactedPosts.QueryType = queryInfo.QueryType ?? "";
            interactedPosts.QueryValue = queryInfo.QueryValue ?? "";
            interactedPosts.SubscribeCount = youtubePost.ChannelSubscriberCount ?? "";
            interactedPosts.VideoLength = youtubePost.VideoLength;
            interactedPosts.VideoUrl = youtubePost.PostUrl ?? "";
            interactedPosts.ViewsCount = youtubePost.ViewsCount ?? "";
            interactedPosts.MyChannelId = string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.UserId) ? YdStatic.DefaultChannel : DominatorAccountModel.AccountBaseModel.UserId;
            interactedPosts.MyChannelPageId = DominatorAccountModel.AccountBaseModel.ProfileId;
            interactedPosts.ViewingCountPerAccount = viewedCount;
            interactedPosts.Status = "Success";
            interactedPosts.PostTitle = youtubePost.Title;
            interactedPosts.IsSubscribed = youtubePost.IsChannelSubscribed;
        }
    }
}
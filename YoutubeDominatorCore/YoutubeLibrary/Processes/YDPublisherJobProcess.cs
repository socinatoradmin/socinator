using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.YdFunctions;

namespace YoutubeDominatorCore.YoutubeLibrary.Processes
{
    public class YdPublisherJobProcess : PublisherJobProcess
    {
        private readonly IYoutubeLogInProcess _logInProcess;

        private readonly IYoutubeFunctionality _youtubeFunction;
        private readonly DominatorAccountModel domAccModel;
        public YdPublisherJobProcess(string campaignId, string accountId, SocialNetworks network,
            List<string> groupDestinationLists, List<string> pageDestinationList,
            List<PublisherCustomDestinationModel> customDestinationModels, bool isPublishOnOwnWall,
            CancellationTokenSource campaignCancellationToken)
            : base(campaignId, accountId, network, groupDestinationLists, pageDestinationList, customDestinationModels,
                isPublishOnOwnWall, campaignCancellationToken)
        {
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            domAccModel = accountsFileManager.GetAccountById(accountId);
            var accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _youtubeFunction = accountScopeFactory[domAccModel.AccountId].Resolve<IYoutubeFunctionality>();
            _logInProcess = accountScopeFactory[domAccModel.AccountId].Resolve<IYoutubeLogInProcess>();
        }


        public YdPublisherJobProcess(string campaignId, string campaignName, string accountId, SocialNetworks network,
            IEnumerable<PublisherDestinationDetailsModel> destinationDetails,
            CancellationTokenSource campaignCancellationToken)
            : base(campaignId, campaignName, accountId, network, destinationDetails, campaignCancellationToken)
        {
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            domAccModel = accountsFileManager.GetAccountById(accountId);
            var accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _youtubeFunction = accountScopeFactory[accountId].Resolve<IYoutubeFunctionality>();
            _logInProcess = accountScopeFactory[accountId].Resolve<IYoutubeLogInProcess>();
        }

        public override bool PublishOnGroups(string accountId, string groupUrl, PublisherPostlistModel postDetails,
            bool isDelayNeed)
        {
            return base.PublishOnGroups(accountId, groupUrl, postDetails);
        }

        public override bool PublishOnOwnWall(string accountId, PublisherPostlistModel postDetails, bool isDelayNeed)
        {
            try
            {
                var channelName = GetDestination();
                if (string.IsNullOrEmpty(channelName) || channelName == domAccModel.AccountId) return false;
                if (!_logInProcess.CheckLogin(domAccModel, domAccModel.Token)) return false;

                var updatesPostListModel = PerformGeneralSettings(postDetails);

                var videoId = domAccModel.IsRunProcessThroughBrowser
                    ? _logInProcess.BrowserManager.PublishVideoAndGetVideoId(updatesPostListModel, channelName, OtherConfiguration, 3)
                    : _youtubeFunction.PublishVideoAndGetVideoId(domAccModel, updatesPostListModel, channelName);

                if (!string.IsNullOrEmpty(videoId) && !videoId.Contains("Invalid media"))
                {
                    _logInProcess.BrowserManager.BrowserDispatcher(YDEnums.BrowserFuncts.GetPageSource);
                    var checkUrl = _logInProcess.BrowserManager.CurrentData.Contains("https://youtu.be/");

                    var videoUrl = !string.IsNullOrEmpty(videoId) && !videoId.Contains("www.youtube.com")
                        ? checkUrl ? "https://www.youtube.com/watch?v=" + videoId : "https://www.youtube.com/shorts/" + videoId:videoId;
                    var message = !string.IsNullOrEmpty(videoUrl) && videoUrl.Contains("/post/") ?
                        "Successfully posted post url => {0}" : "Successfully posted video url => {0}";
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.YouTube,
                        domAccModel.AccountBaseModel.UserName, ActivityType.Post,
                        string.Format(message,videoUrl));
                    UpdatePostWithSuccessful(GetSavedChannelName(updatesPostListModel.PostId), updatesPostListModel, videoUrl);
                    if (isDelayNeed)
                        DelayBeforeNextPublish();
                    return true;
                }

                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.YouTube,
                    domAccModel.AccountBaseModel.UserName, ActivityType.Post, $"failed to post the video ==> {videoId}");
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
            finally
            {
                if (domAccModel.IsRunProcessThroughBrowser)
                    _logInProcess.BrowserManager.CloseBrowser();
            }
        }

        private string GetSavedChannelName(string postId, string channelId = "")
        {
            Thread.Sleep(1000);
            var updatelock = PublishScheduler.UpdatingLock.GetOrAdd(postId, _lock => new object());
            lock (updatelock)
            {
                // get the post details
                var post = PostlistFileManager.GetByPostId(CampaignId, postId);

                // get the post index where current destination present
                var currentPost = post.LstPublishedPostDetailsModels.FirstOrDefault(y =>
                        (y.DestinationUrl == domAccModel.AccountBaseModel.UserName || y.DestinationUrl == channelId)
                        && y.AccountId == AccountModel.AccountId);
                return currentPost?.DestinationUrl;
            }
        }

        public override bool PublishOnPages(string accountId, string channelName, PublisherPostlistModel postDetails,
            bool isDelayNeed)
        {
            try
            {
                var IsDefaultChannel = channelName == YdStatic.DefaultChannel;
                var ChannelName = IsDefaultChannel ? channelName : GetDestination(channelName);
                var channelId = channelName;
                channelName = !string.IsNullOrEmpty(ChannelName) ? ChannelName : channelName;
                if (string.IsNullOrEmpty(channelName))
                    return false;

                if (!_logInProcess.CheckLogin(domAccModel, domAccModel.Token)) return false;

                var updatesPostListModel = PerformGeneralSettings(postDetails);

                var videoId = domAccModel.IsRunProcessThroughBrowser
                    ? _logInProcess.BrowserManager.PublishVideoAndGetVideoId(updatesPostListModel, channelName, OtherConfiguration, 3, !IsDefaultChannel)
                    : _youtubeFunction.PublishVideoAndGetVideoId(domAccModel, updatesPostListModel, channelName);

                if (!string.IsNullOrEmpty(videoId) && !videoId.Contains("Invalid media"))
                {
                    _logInProcess.BrowserManager.BrowserDispatcher(YDEnums.BrowserFuncts.GetPageSource);
                    var checkUrl = _logInProcess.BrowserManager.CurrentData.Contains("https://youtu.be/");

                    var videoUrl = !string.IsNullOrEmpty(videoId) && !videoId.Contains("www.youtube.com") ?
                        checkUrl ? "https://www.youtube.com/watch?v=" + videoId : "https://www.youtube.com/shorts/" + videoId:videoId;
                    var message = !string.IsNullOrEmpty(videoUrl) && videoUrl.Contains("/post/") ?
                        "Successfully posted post url => {0}" : "Successfully posted video url => {0}";
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.YouTube,
                        domAccModel.AccountBaseModel.UserName, ActivityType.Post,
                        string.Format(message,videoUrl));

                    UpdatePostWithSuccessful(IsDefaultChannel ? channelName : GetSavedChannelName(updatesPostListModel.PostId, channelId), updatesPostListModel, videoUrl);
                    if (isDelayNeed)
                        DelayBeforeNextPublish();
                    return true;
                }

                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.YouTube,
                    domAccModel.AccountBaseModel.UserName, ActivityType.Post, $"failed to post the video ==> {videoId}");
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
            finally
            {
                if (domAccModel.IsRunProcessThroughBrowser)
                    _logInProcess.BrowserManager.CloseBrowser();
            }
        }
        public string GetDestination(string ChannelID = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(ChannelID) && ChannelID == YdStatic.DefaultChannel)
                    return ChannelID;
                var accountsDetailsSelector = SocinatorInitialize
                            .GetSocialLibrary(domAccModel.AccountBaseModel.AccountNetwork)
                            .GetNetworkCoreFactory().AccountDetailsSelectors;
                var channels = accountsDetailsSelector.GetPagesDetails(domAccModel.AccountId, domAccModel.AccountBaseModel.AccountName, new List<string>()).Result;
                return !string.IsNullOrEmpty(ChannelID) ? channels?.FirstOrDefault(x => x.DetailUrl == ChannelID)?.DetailName?.ToString()
                    ?? channels?.FirstOrDefault(x => x.DetailUrl == YdStatic.DefaultChannel)?.DetailName?.ToString()
                    ?? channels?.FirstOrDefault()?.DetailName
                    : channels?.FirstOrDefault(x => x.DetailUrl == YdStatic.DefaultChannel)?.DetailName?.ToString()
                    ?? channels?.FirstOrDefault()?.DetailName ?? domAccModel.AccountId;
            }
            catch { }
            return domAccModel.AccountId;
        }
    }
}
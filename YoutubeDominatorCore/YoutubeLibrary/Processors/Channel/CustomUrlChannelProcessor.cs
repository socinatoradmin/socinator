using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using System.Collections.Generic;
using YoutubeDominatorCore.Response;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeLibrary.Processes;
using YoutubeDominatorCore.YoutubeLibrary.YdFunctions;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.YoutubeLibrary.Processors.Channel
{
    internal class CustomUrlChannelProcessor : BaseYoutubeChannelProcessor
    {
        public CustomUrlChannelProcessor(IYdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService, IYoutubeFunctionality youtubeFunctionality) : base(jobProcess,
            blackWhiteListHandler, campaignService, youtubeFunctionality)
        {
        }

        protected override void Process(QueryInfo queryInfo)
        {
            var jobProcessResult = new JobProcessResult();
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            JobProcess.IsCustom = true;
            var url = queryInfo.QueryValue;
            ChannelInfoResponseHandler channelInfo;
            KeyValuePair<string, string> idUsername;
            if (url.StartsWith("https://www.youtube.com") && !url.Contains("youtube.com/watch?v="))
            {
                channelInfo = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser
                ? BrowserManager.GetChannelDetails(!string.IsNullOrEmpty(url) && !url.EndsWith("/about") ? url.EndsWith("/") ? url + "about" : url + "/about" : url, 2.5)
                : YoutubeFunction.GetChannelDetails(JobProcess.DominatorAccountModel, queryInfo.QueryValue,
                    delayBeforeHit: 2);
                idUsername = new KeyValuePair<string, string>(channelInfo.YoutubeChannel.ChannelId, channelInfo.YoutubeChannel.ChannelUsername);
            }
            else
            {
                if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    BrowserManager.LoadPageSource(JobProcess.DominatorAccountModel, url, clearandNeedResource: true);
                var postInfo = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser
                    ? BrowserManager.PostInfo(ActType, url,
                        sortCommentsByNew: ActType == ActivityType.LikeComment || ActType == ActivityType.Comment || ActType == ActivityType.CommentScraper, needChannelDetails: true,
                        pauseVideo: ActType != ActivityType.ViewIncreaser)
                    : YoutubeFunction.GetPostDetails(JobProcess.DominatorAccountModel, url, delayBeforeHit: 2.5);
                //var idUsername = YdStatic.ChannelIdUsernameApart(queryInfo.QueryValue);

                idUsername = new KeyValuePair<string, string>(postInfo.YoutubePost.ChannelId, postInfo.YoutubePost.ChannelUsername);

                url = YdStatic.ChannelUrl(idUsername.Key, idUsername.Value);

                channelInfo = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser
                ? BrowserManager.GetChannelDetails(url, 2.5)
                : YoutubeFunction.GetChannelDetails(JobProcess.DominatorAccountModel, queryInfo.QueryValue,
                    delayBeforeHit: 2);

            }
            if (ActType == ActivityType.ChannelScraper &&
                AlreadyDoneInDB(queryInfo, idUsername.Key, idUsername.Value) ||
                SkipBlackListOrWhiteList(new List<YoutubeChannel>
                    {new YoutubeChannel {ChannelId = idUsername.Key, ChannelUsername = idUsername.Value}}).Count == 0)
            {
                CustomLog($"Skipped(already interacted or filtred) the channel ( {queryInfo.QueryValue} ).");
                return;
            }
            if (channelInfo == null || !channelInfo.Success)
                CustomLog($"Failed to fetch info from custom channel url ( {queryInfo.QueryValue} ).");
            else
                StartCustomChannelProcess(queryInfo, ref jobProcessResult, channelInfo.YoutubeChannel);
        }
    }
}
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using System.Collections.Generic;
using YoutubeDominatorCore.Response;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeLibrary.Processes;
using YoutubeDominatorCore.YoutubeLibrary.YdFunctions;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.YoutubeLibrary.Processors.Post
{
    internal class CustomChannelPostScraper : BaseYoutubePostProcessor
    {
        public CustomChannelPostScraper(IYdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService,
            IYoutubeFunctionality youtubeFunctionality) : base(jobProcess, blackWhiteListHandler, campaignService,
            youtubeFunctionality)
        {
        }

        protected override void Process(QueryInfo queryInfo)
        {
            var idUsername = YdStatic.ChannelIdUsernameApart(queryInfo.QueryValue);
            var channelUrl = YdStatic.ChannelUrl(idUsername.Key, idUsername.Value).Replace("/about", "");

            if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                UsingBrowser(queryInfo, channelUrl.TrimEnd('/'));
            else
                UsingHttp(queryInfo, channelUrl);
        }

        private void UsingHttp(QueryInfo queryInfo, string channelUrl)
        {
            var jobProcessResult = new JobProcessResult();

            var channelVideos = new ScrapPostFromChannelResponseHandler();

            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                channelVideos = YoutubeFunction.ScrapPostsFromChannel(JobProcess.DominatorAccountModel, channelUrl,
                    channelVideos.PostDataElements, channelVideos.HeadersElements, 2);

                if (channelVideos.ListYoutubePost.Count == 0)
                {
                    NoData(ref jobProcessResult);
                }
                else
                {
                    jobProcessResult.maxId = channelVideos.PostDataElements?.ContinuationToken;
                    StartProcessForListOfPosts(queryInfo, ref jobProcessResult, channelVideos.ListYoutubePost);
                    jobProcessResult.HasNoResult =
                        string.IsNullOrEmpty(channelVideos.PostDataElements?.ContinuationToken);
                }

                if (SkipTheUniqueOne(queryInfo, "", channelVideos.ListYoutubePost[0].ChannelId))
                    break;
            }
        }

        private void UsingBrowser(QueryInfo queryInfo, string channelUrl)
        {
            var jobProcessResult = new JobProcessResult();

            List<YoutubePost> posts = null;
            var channelId = "";
            var channelUsername = "";
            var index = 0;
            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                posts = BrowserManager.ScrapPostsFromChannel(channelUrl, ActType, ref index, ref channelId,
                    ref channelUsername, posts == null, 2);

                if (posts.Count == 0)
                    NoData(ref jobProcessResult);
                else
                    StartProcessForListOfPosts(queryInfo, ref jobProcessResult, posts);

                if (SkipTheUniqueOne(queryInfo, "", posts[0].ChannelId))
                    break;
            }
        }
    }
}
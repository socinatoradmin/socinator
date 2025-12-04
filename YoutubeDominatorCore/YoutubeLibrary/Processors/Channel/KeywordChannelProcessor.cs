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
    public class KeywordChannelProcessor : BaseYoutubeChannelProcessor
    {
        public KeywordChannelProcessor(IYdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService, IYoutubeFunctionality youtubeFunctionality) : base(jobProcess,
            blackWhiteListHandler, campaignService, youtubeFunctionality)
        {
        }

        protected override void Process(QueryInfo queryInfo)
        {
            if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                UsingBrowser(queryInfo);
            else
                UsingHttp(queryInfo);
        }

        private void UsingHttp(QueryInfo queryInfo)
        {
            var jobProcessResult = new JobProcessResult();
            var searchedChannels = new SearchChannelsResponseHandler();

            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                searchedChannels = YoutubeFunction.ScrapChannelsFromKeyword(JobProcess.DominatorAccountModel,
                    queryInfo.QueryValue,
                    searchedChannels.PostDataElements, searchedChannels.HeadersElements);

                if (searchedChannels.ListOfYoutubeChannels.Count == 0)
                {
                    NoData(ref jobProcessResult);
                }
                else
                {
                    jobProcessResult.maxId = searchedChannels.PostDataElements?.ContinuationToken;

                    StartProcessForListOfChannels(queryInfo, ref jobProcessResult,
                        searchedChannels.ListOfYoutubeChannels);

                    jobProcessResult.HasNoResult =
                        string.IsNullOrEmpty(searchedChannels.PostDataElements?.ContinuationToken);
                }
            }
        }

        private void UsingBrowser(QueryInfo queryInfo)
        {
            var jobProcessResult = new JobProcessResult();
            List<YoutubeChannel> posts = null;
            var index = 0;
            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                posts = BrowserManager.ScrapChannelsFromKeyword(queryInfo.QueryValue, ActType, ref index,
                    posts == null, 2);

                if (posts.Count == 0)
                    NoData(ref jobProcessResult);
                else
                    StartProcessForListOfChannels(queryInfo, ref jobProcessResult, posts);
            }
        }
    }
}
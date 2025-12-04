using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using System;
using System.Collections.Generic;
using YoutubeDominatorCore.Response;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeLibrary.Processes;
using YoutubeDominatorCore.YoutubeLibrary.YdFunctions;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.YoutubeLibrary.Processors.Post
{
    internal class KeywordPostProcessor : BaseYoutubePostProcessor
    {
        public KeywordPostProcessor(IYdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
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

        private SearchPostsResponseHandler GetLastPaginationData(QueryInfo queryInfo)
        {
            var pId = GetPaginationId(queryInfo);
            if (string.IsNullOrEmpty(pId))
                return null;
            var splitedParams = pId.Split('|');
            if (splitedParams.Length < 2)
                return null;
            var data = new SearchPostsResponseHandler
            {
                PostDataElements = new PostDataElements
                {
                    ContinuationToken = splitedParams[0],
                    Itct = splitedParams[1],
                    XsrfToken = splitedParams[2]
                },

                HeadersElements = new HeadersElements
                {
                    RefererUrl = splitedParams[3],
                    VariantsChecksum = splitedParams[4],
                    PageCl = splitedParams[5],
                    ClientName = splitedParams[6],
                    ClientVersion = splitedParams[7],
                    IdToken = splitedParams[8],
                    PageBuildLabel = splitedParams[9]
                }
            };

            return data;
        }

        private void SavePaginationData(QueryInfo queryInfo, SearchPostsResponseHandler data)
        {
            if (data == null || !data.HasMoreResults)
                return;

            var dataString =
                $"{data.PostDataElements.ContinuationToken}|{data.PostDataElements.Itct}|{data.PostDataElements.XsrfToken}|"
                + $"{data.HeadersElements.RefererUrl}|{data.HeadersElements.VariantsChecksum}|{data.HeadersElements.PageCl}|{data.HeadersElements.ClientName}|{data.HeadersElements.ClientVersion}|{data.HeadersElements.IdToken}|{data.HeadersElements.PageBuildLabel}";

            AddOrUpdatePaginationId(queryInfo, dataString);
        }

        private void UsingHttp(QueryInfo queryInfo)
        {
            var jobProcessResult = new JobProcessResult();
            var searchPostsResponseHandler = GetLastPaginationData(queryInfo);
            var dontSavePagination = true;
            try
            {
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    searchPostsResponseHandler = YoutubeFunction.ScrapPostsFromKeyword(
                        JobProcess.DominatorAccountModel, queryInfo.QueryValue, searchPostsResponseHandler,
                        SearchFilterUrlParam ?? "EgIQAQ%253D%253D", 3);
                    if (searchPostsResponseHandler == null || !searchPostsResponseHandler.Success
                                                           || searchPostsResponseHandler.ListOfYoutubePosts == null ||
                                                           searchPostsResponseHandler.ListOfYoutubePosts.Count == 0)
                    {
                        NoData(ref jobProcessResult);
                    }
                    else
                    {
                        jobProcessResult.maxId = searchPostsResponseHandler.PostDataElements.ContinuationToken;
                        StartProcessForListOfPosts(queryInfo, ref jobProcessResult,
                            searchPostsResponseHandler.ListOfYoutubePosts);
                        jobProcessResult.HasNoResult = !searchPostsResponseHandler.HasMoreResults;
                        dontSavePagination = false;
                    }
                }
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
                if (!dontSavePagination)
                    SavePaginationData(queryInfo, searchPostsResponseHandler);
            }
        }

        private void UsingBrowser(QueryInfo queryInfo)
        {
            var jobProcessResult = new JobProcessResult();
            List<YoutubePost> posts = null;
            BrowserManager.SearchByKeywordOrHashTag(JobProcess.DominatorAccountModel, queryInfo.QueryValue, searchFilterUrlParam: SearchFilterUrlParam);
            var searchCount = 0;
            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                posts = BrowserManager.ScrollWindowAndGetPosts(JobProcess.DominatorAccountModel, ActType, 5).ListOfYoutubePosts;
                searchCount++;
                if (posts.Count == 0 || searchCount > 2)
                    NoData(ref jobProcessResult);
                else
                    StartProcessForListOfPosts(queryInfo, ref jobProcessResult, posts);
            }
        }
    }
}
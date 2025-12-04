using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.Response;
using System;

namespace RedditDominatorCore.RDLibrary.Processors.Post
{
    internal class KeywordsPostProcessor : BaseRedditPostProcessor
    {
        private readonly IRdBrowserManager _browserManager;
        private RedditPostResponseHandler _redditPostResponseHandler;
        public KeywordsPostProcessor(IProcessScopeModel processScopeModel, IRdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IRedditFunction redditFunction, IRdBrowserManager browserManager)
            : base(processScopeModel, jobProcess, dbAccountService, globalService, campaignService, redditFunction,
                browserManager)
        {
            _redditPostResponseHandler = null;
            _browserManager = browserManager;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var urlScrapperModel = processScope.GetActivitySettingsAs<UrlScraperModel>();
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    _redditPostResponseHandler = RedditFunction.ScrapePostsByKeywords(JobProcess.DominatorAccountModel,
                        queryInfo.QueryValue, queryInfo, _redditPostResponseHandler,
                        GenerateUrlForSortSearchFilter(queryInfo.QueryValue, JobProcess.ActivityType,
                        urlScrapperModel, _redditPostResponseHandler, false));
                    jobProcessResult.maxId = _redditPostResponseHandler.PaginationParameter.LastPaginationId;
                }
                //For browser automation
                else
                {
                    if (_redditPostResponseHandler == null)
                    {
                        var response = _browserManager.TryAndGetResponse(JobProcess.DominatorAccountModel,
                            GenerateUrlForSortSearchFilter(queryInfo.QueryValue, JobProcess.ActivityType,
                            urlScrapperModel, _redditPostResponseHandler), 3,
                            string.Empty, false, false, urlScrapperModel.IsEnabledSafeSearch);
                        _redditPostResponseHandler = new RedditPostResponseHandler(response, false, null);
                    }
                    //For browser pagination
                    else
                    {
                        var response = _browserManager.ScrollWindowAndGetNextPageData(JobProcess.DominatorAccountModel);
                        _redditPostResponseHandler = new RedditPostResponseHandler(response, true, null);
                    }
                }

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (_redditPostResponseHandler == null || !_redditPostResponseHandler.Success ||
                    _redditPostResponseHandler.LstRedditPost.Count == 0)
                {
                    jobProcessResult.HasNoResult = true;
                    jobProcessResult.maxId = null;
                    return;
                }

                GlobusLogHelper.log.Info(Log.FoundXResults,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName,
                    _redditPostResponseHandler.LstRedditPost.Count,
                    queryInfo.QueryType, queryInfo.QueryValue, ActivityType);

                jobProcessResult.HasNoResult = !JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser
                                               && _redditPostResponseHandler.HasMoreResults;

                StartKeywordPostProcess(queryInfo, ref jobProcessResult, _redditPostResponseHandler.LstRedditPost);
            }
            catch (OperationCanceledException)
            {
                jobProcessResult.IsProcessCompleted = true;
                throw new OperationCanceledException();
            }
            catch (Exception) { jobProcessResult.IsProcessCompleted = true; }
            finally { if (_browserManager != null && _browserManager.BrowserWindow != null) _browserManager.CloseBrowser(); }
        }

        private string GenerateUrlForSortSearchFilter(string queryValue, ActivityType activityType, UrlScraperModel UrlScrapperModel, RedditPostResponseHandler responseHandler, bool IsBrowser = true)
        {
            var url = responseHandler == null || IsBrowser ? $"{RdConstants.GetRedditHomePageAPI}/search?q={queryValue}&type=link"
                    : responseHandler.PaginationParameter.LastPaginationId.Contains("community-more-posts") ?
                    $"{RdConstants.GetRedditHomePageAPI}{responseHandler.PaginationParameter.LastPaginationId}"
                    : $"https://gateway.reddit.com/desktopapi/v1/search?&q={queryValue}&type=link&after={responseHandler.PaginationParameter.LastPaginationId}";
            if (activityType == ActivityType.UrlScraper)
            {
                if (UrlScrapperModel != null && UrlScrapperModel.IsCheckedSearchPostFilter)
                {
                    if (UrlScrapperModel.IsCheckedSortByCategory)
                        url += "&sort=" + UrlScrapperModel.SelectedValue?.Replace("Most", "").ToLower()?.Trim();
                    if (UrlScrapperModel.IsCheckedSortByTime)
                        url += "&t=" + UrlScrapperModel.SelectedValueTimeFilter?.Replace("Time", "")?.Replace("Past", "")?.Replace("24 Hours", "day")?.ToLower()?.Trim();
                    if (!IsBrowser && !UrlScrapperModel.IsEnabledSafeSearch)
                        url += "&include_over_18=&allow_over18=";
                }
            }
            return url;
        }
    }
}
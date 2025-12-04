using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDUtility;
using RedditDominatorCore.Response;
using System;

namespace RedditDominatorCore.RDLibrary.Processors.Post
{
    internal class CommunityAndUserPostProcessor : BaseRedditPostProcessor
    {
        private readonly IRdBrowserManager _browserManager;
        private RedditPostResponseHandler _redditPostResponseHandler;
        private UpvoteModel _upvoteModel;
        private UrlScraperModel urlScraperModel;
        public CommunityAndUserPostProcessor(IProcessScopeModel processScopeModel, IRdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IRedditFunction redditFunction,
            IRdBrowserManager browserManager)
            : base(processScopeModel, jobProcess, dbAccountService, globalService, campaignService, redditFunction,
                browserManager)
        {
            _redditPostResponseHandler = null;
            _browserManager = browserManager;
            _upvoteModel = processScopeModel.GetActivitySettingsAs<UpvoteModel>();
            urlScraperModel = processScopeModel.GetActivitySettingsAs<UrlScraperModel>();
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var url = Utils.GetLastWordFromUrl(queryInfo.QueryValue);
                url = queryInfo.QueryType.Equals("Community Url") ? $"{RdConstants.GetCommunityUrlByUsername(url)}" :
                    queryInfo.QueryType.Equals("Specific User's Post") ? $"{RdConstants.UserProfileUrlByUsername(url)}/submitted" : queryInfo.QueryValue;

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    _redditPostResponseHandler = RedditFunction.ScrapePostsByUrl(JobProcess.DominatorAccountModel,
                        GenerateUrlForCommunityAndUserFilter(url, JobProcess.ActivityType,
                            urlScraperModel, _redditPostResponseHandler, false),
                        queryInfo, _redditPostResponseHandler, true, "new");
                    jobProcessResult.maxId = _redditPostResponseHandler.PaginationParameter.LastPaginationId;
                }
                //For browser automation
                else
                {
                    if (_redditPostResponseHandler == null)
                    {
                        var response = _browserManager.TryAndGetResponse(JobProcess.DominatorAccountModel,
                            GenerateUrlForCommunityAndUserFilter(url, JobProcess.ActivityType,
                            urlScraperModel, _redditPostResponseHandler), 3, string.Empty, false);
                        _redditPostResponseHandler = new RedditPostResponseHandler(response, false, null);
                    }
                    //For browser pagination
                    else
                    {
                        var response =
                            _browserManager.ScrollWindowAndGetNextPageDataForCustomUrlScraper(
                                JobProcess.DominatorAccountModel, 15);
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

                if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser &&
                    _redditPostResponseHandler.HasMoreResults == false)
                    jobProcessResult.HasNoResult = true;

                //To check with number of upvote per account on Subreddit and User's post
                if (ActivityType != ActivityType.UrlScraper)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    StartKeywordPostProcess(queryInfo, ref jobProcessResult, _redditPostResponseHandler.LstRedditPost);
                }
                else
                {
                    foreach (var redditPost in _redditPostResponseHandler.LstRedditPost)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        StartFinalPostProcess(ref jobProcessResult, redditPost, queryInfo);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
                // ignored
            }
            finally { if (_browserManager != null && _browserManager.BrowserWindow != null) _browserManager.CloseBrowser(); }
        }

        private string GenerateUrlForCommunityAndUserFilter(string queryValue, ActivityType activityType, UrlScraperModel UrlScrapperModel,
            RedditPostResponseHandler responseHandler, bool IsBrowser = true)
        {
            var IsBrowserSearch = responseHandler == null || IsBrowser;
            var IsCommunity = queryValue.Contains("/r/");
            var url = responseHandler == null || IsBrowser ? queryValue.TrimEnd('/') + "/" :
                $"{RdConstants.GetRedditHomePageAPI}{responseHandler.PaginationParameter.LastPaginationId}";

            if (activityType == ActivityType.UrlScraper)
            {
                if (UrlScrapperModel != null && UrlScrapperModel.IsEnabledCommunityOrUserPostFilter && IsBrowserSearch)
                {
                    if (UrlScrapperModel.IsCheckedSortByNew)
                    {
                        url += IsCommunity ? "new/".Trim() : "?sort=new".Trim();
                    }
                    if (UrlScrapperModel.IsCheckedSortByHot)
                    {
                        url += IsCommunity ? "hot/".Trim() : "?sort=hot".Trim();
                    }
                    if (UrlScrapperModel.IsCheckedSortByTop)
                    {
                        url += IsCommunity ? "top/".Trim() : "?sort=top".Trim();
                        url += (IsCommunity ? "?t=" : "&t=") + UrlScrapperModel.SelectedValueTopTimeFilter?.Replace("To", "")?.Replace("Now", "hour")?.Replace("This ", "")?.Replace("All Time", "all")?.ToLower()?.Trim();
                    }
                }
            }

            return url;
        }
    }
}
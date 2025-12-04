using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.Response;
using System;
using System.Linq;

namespace RedditDominatorCore.RDLibrary.Processors.Channel
{
    internal class UnsubscribeProcessor : BaseRedditChannelProcessor
    {
        private readonly IRdBrowserManager _browserManager;
        protected readonly IDbAccountServiceScoped DbAccountService;
        private SubredditResponseHandler _subredditResponseHandler;
        private readonly UnSubscribeModel UnSubscribeModel;

        public UnsubscribeProcessor(IProcessScopeModel processScopeModel, IRdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService,
            IRedditFunction redditFunction, IRdBrowserManager browserManager)
            : base(jobProcess, dbAccountService, campaignService, redditFunction, browserManager)
        {
            _subredditResponseHandler = null;
            DbAccountService = dbAccountService;
            _browserManager = browserManager;
            UnSubscribeModel = processScopeModel.GetActivitySettingsAs<UnSubscribeModel>();
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (UnSubscribeModel.IsChkCommunitySubscribedBySoftwareChecked && !jobProcessResult.IsProcessCompleted)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                FilterAndStartUnSubscribeBySoftware(ref jobProcessResult);
            }

            if (UnSubscribeModel.IsChkCustomCommunityListChecked)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                FilterAndStartUnSubscribeCustom(ref jobProcessResult);
            }

            if (UnSubscribeModel.IsChkCommunitySubscribedOutsideSoftwareChecked)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                FilterAndStartUnSubscribeByOutsideSoftware(ref jobProcessResult);
            }

            _browserManager.CloseBrowser();
            jobProcessResult.HasNoResult = true;
        }

        public void FilterAndStartUnSubscribeBySoftware(ref JobProcessResult jobProcessResult)
        {
            try
            {
                var interactedSubreddits = DbAccountService.GetInteractedSubreddits().ToList();

                for (var i = 0; i < interactedSubreddits.Count && !jobProcessResult.IsProcessCompleted; i++)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var url = interactedSubreddits[i].url;
                    var queryInfo = new QueryInfo { QueryValue = url, QueryType = "UnSubscribe By Software" };
                    if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        //Passing null value so _subredditResponseHandler has new value for next execution inside this for loop
                        _subredditResponseHandler =
                            RedditFunction.ScrapeSubRedditsByUrl(JobProcess.DominatorAccountModel, queryInfo.QueryValue,
                                queryInfo, null);
                    }
                    //For browser automation
                    else
                    {
                        var response = _browserManager.TryAndGetResponse(JobProcess.DominatorAccountModel, queryInfo.QueryValue, 3, string.Empty, true);
                        _subredditResponseHandler =
                            new SubredditResponseHandler(response, false, null, queryInfo.QueryValue);
                    }

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (_subredditResponseHandler != null && _subredditResponseHandler.LstSubReddit.Count > 0)
                        StartCustomSubscribeProcess(queryInfo, ref jobProcessResult, _subredditResponseHandler.LstSubReddit.FirstOrDefault());
                }
            }
            catch (OperationCanceledException)
            {
                _browserManager.CloseBrowser();
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void FilterAndStartUnSubscribeCustom(ref JobProcessResult jobProcessResult)
        {
            try
            {
                var lstCustomSubreddits = UnSubscribeModel.LstCustomCommunity;
                for (var i = 0; i < lstCustomSubreddits.Count && !jobProcessResult.IsProcessCompleted; i++)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var subreddit = lstCustomSubreddits[i];
                    var queryInfo = new QueryInfo { QueryValue = subreddit, QueryType = "UnSubscribe by Custom User" };
                    if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        //Passing null value so _subredditResponseHandler has new value for next execution inside this for loop
                        _subredditResponseHandler =
                            RedditFunction.ScrapeSubRedditsByUrl(JobProcess.DominatorAccountModel, queryInfo.QueryValue,
                                queryInfo, null);
                    }
                    //For browser automation
                    else
                    {
                        subreddit = _browserManager.GetUrlFromQuery(JobProcess.ActivityType, lstCustomSubreddits[i]);
                        queryInfo = new QueryInfo { QueryValue = subreddit, QueryType = "UnSubscribe by Custom User" };
                        var response = _browserManager.TryAndGetResponse(JobProcess.DominatorAccountModel, queryInfo.QueryValue, 3, string.Empty, true);
                        _subredditResponseHandler = new SubredditResponseHandler(response, false, null, queryInfo.QueryValue);
                    }

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (_subredditResponseHandler != null && _subredditResponseHandler.LstSubReddit.Count > 0)
                        StartCustomSubscribeProcess(queryInfo, ref jobProcessResult, _subredditResponseHandler.LstSubReddit.FirstOrDefault());
                }
            }
            catch (OperationCanceledException)
            {
                _browserManager.CloseBrowser();
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void FilterAndStartUnSubscribeByOutsideSoftware(ref JobProcessResult jobProcessResult)
        {
            try
            {
                var lstInteractedCommunity = DbAccountService.GetInteractedSubreddits().ToList();
                var subscriptionList = RedditFunction.SubRedditSubcription(JobProcess.DominatorAccountModel);
                //Removing the communities which is followed by software
                subscriptionList.RemoveAll(z => lstInteractedCommunity.Any(y => y.name.Equals(z)));
                for (var i = 0; i < subscriptionList.Count && !jobProcessResult.IsProcessCompleted; i++)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var subReddit = _browserManager.GetUrlFromQuery(JobProcess.ActivityType, subscriptionList[i]);

                    //if (DbAccountService.GetInteractedSubredditUrl(ActivityType, userName).Count > 0) continue;
                    var queryInfo = new QueryInfo
                    { QueryValue = subReddit, QueryType = "UnSubscribe By Outside Software" };

                    if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        // Passing null value so _subredditResponseHandler has new value for next execution inside this for loop
                        _subredditResponseHandler =
                            RedditFunction.ScrapeSubRedditsByUrl(JobProcess.DominatorAccountModel, queryInfo.QueryValue,
                                queryInfo, null);
                    }
                    //For browser automation
                    else
                    {
                        var url = _browserManager.GetUrlFromQuery(ActivityType, queryInfo.QueryValue);
                        var response = _browserManager.TryAndGetResponse(JobProcess.DominatorAccountModel, queryInfo.QueryValue, 3, string.Empty, true);
                        _subredditResponseHandler =
                            new SubredditResponseHandler(response, false, null, queryInfo.QueryValue);
                    }

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (_subredditResponseHandler != null && _subredditResponseHandler.LstSubReddit.Count > 0)
                        StartCustomSubscribeProcess(queryInfo, ref jobProcessResult, _subredditResponseHandler.LstSubReddit.FirstOrDefault());
                }
            }
            catch (OperationCanceledException)
            {
                _browserManager.CloseBrowser();
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using System;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrLibrary.TumblrProcesses;
using TumblrDominatorCore.TumblrResponseHandler;

namespace TumblrDominatorCore.TumblrLibrary.Processors.Posts
{
    internal class HashtagPostProcessor : BaseTumblrPostProcessor, IQueryProcessor
    {
        private SearchPostsResonseHandler _searchPostResponseHandler = new SearchPostsResonseHandler();
        private string MainUrl;
        private string Referer;
        private int _searchForPostCount;
        public HashtagPostProcessor(IProcessScopeModel processScopeModel,
            ITumblrJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService,
            ITumblrFunct tumblrFunct, IDbGlobalService globalService) : base(processScopeModel, jobProcess, dbAccountService, campaignService,
            tumblrFunct, globalService)
        {

        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var queryValue = "#" + queryInfo.QueryValue.TrimStart('#');
            if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
            {
                if (!string.IsNullOrEmpty(MainUrl))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                    $"Searching for NextPage => {queryInfo.QueryType} {queryInfo.QueryValue}");
                }
                if (string.IsNullOrEmpty(MainUrl))
                {
                    var Items = ConstantHelpDetails.GetSearchFilterUrl($"{Uri.EscapeDataString(queryInfo.QueryValue)}", _searchFilterModel, false, true);
                    MainUrl = Items.Item1;
                    Referer = Items.Item2;
                }
                _searchPostResponseHandler =
                    TumblrFunct.SearchPostsWithHashTag(JobProcess.DominatorAccountModel, MainUrl, Referer);
            }
            else
                _searchPostResponseHandler = _browser.GetTumblrPostsFromKeyWord(JobProcess.DominatorAccountModel, queryValue?.Replace("#", ""), _searchFilterModel, true);

            if (!string.IsNullOrEmpty(_searchPostResponseHandler.NextPageUrl))
                MainUrl = _searchPostResponseHandler.NextPageUrl;
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (_searchPostResponseHandler == null || _searchPostResponseHandler.LstTumblrPosts?.Count == 0)
            {
                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, ActivityType);
                jobProcessResult.HasNoResult = true;
                jobProcessResult.maxId = null;
                if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    return;
            }
            else
            {
                GlobusLogHelper.log.Info(Log.FoundXResults,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, _searchPostResponseHandler.LstTumblrPosts.Count,
                    queryInfo.QueryType, queryInfo.QueryValue, ActivityType);
                ProcessOnUserPosts(queryInfo, ref jobProcessResult, _searchPostResponseHandler.LstTumblrPosts, "");
                if (!jobProcessResult.IsProcessSuceessfull) _searchForPostCount++;
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    StartPagination(queryInfo, ref jobProcessResult, _searchPostResponseHandler);
            }

            if (string.IsNullOrEmpty(_searchPostResponseHandler.NextPageUrl) || (_searchForPostCount == 5 && !string.IsNullOrEmpty(_searchPostResponseHandler.NextPageUrl)))
            {
                GlobusLogHelper.log.Info(Log.ProcessCompleted,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, ActivityType);
                jobProcessResult.IsProcessCompleted = true;
            }
        }

        private void StartPagination(QueryInfo queryInfo, ref JobProcessResult jobProcessResult, SearchPostsResonseHandler _searchPostResponseHandler)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (_searchPostResponseHandler != null && _searchPostResponseHandler.hasMoreResults)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                        $"Checking For More Post.....");
                while (_searchPostResponseHandler != null && _searchPostResponseHandler.hasMoreResults)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    _searchPostResponseHandler =
                        TumblrFunct.SearchPostsWithHashTag(JobProcess.DominatorAccountModel, _searchPostResponseHandler.NextPageUrl);
                    if (!string.IsNullOrEmpty(_searchPostResponseHandler.NextPageUrl))
                        MainUrl = _searchPostResponseHandler.NextPageUrl;
                    var lstTumblrUsers = _searchPostResponseHandler.LstTumblrPosts;
                    GlobusLogHelper.log.Info(Log.FoundXResults,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserName, lstTumblrUsers.Count,
                        queryInfo.QueryType, queryInfo.QueryValue, ActivityType);
                    ProcessOnUserPosts(queryInfo, ref jobProcessResult, lstTumblrUsers, "");
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
            }
            catch (Exception e)
            { e.DebugLog(); }
            finally
            {
                jobProcessResult.IsProcessCompleted = true;
            }
        }
    }
}
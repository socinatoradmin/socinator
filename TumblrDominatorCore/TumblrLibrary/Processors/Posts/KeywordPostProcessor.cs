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
    public class KeywordPostProcessor : BaseTumblrPostProcessor, IQueryProcessor
    {
        private SearchPostsResonseHandler _searchPostResponseHandler = new SearchPostsResonseHandler();
        private string MainUrl;
        private string Referer;
        private int _searchForPostCount;
        private string xTumblrFormKey = string.Empty;
        public KeywordPostProcessor(IProcessScopeModel processScopeModel, ITumblrJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, ITumblrFunct tumblrFunct, IDbGlobalService globalService) :
            base(processScopeModel, jobProcess, dbAccountService, campaignService, tumblrFunct, globalService)
        {

        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
            {

                if (!string.IsNullOrEmpty(MainUrl))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                    $"Searching for More Post... => {queryInfo.QueryType} {queryInfo.QueryValue}");
                }
                if (string.IsNullOrEmpty(MainUrl))
                {
                    var Items = ConstantHelpDetails.GetSearchFilterUrl($"{Uri.EscapeDataString(queryInfo.QueryValue)}", _searchFilterModel);
                    MainUrl = Items.Item1;
                    Referer = Items.Item2;
                }

                _searchPostResponseHandler =
                    TumblrFunct.ScrapePostsfromkeyword(JobProcess.DominatorAccountModel, MainUrl, Referer);
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
            else
                _searchPostResponseHandler = _browser.GetTumblrPostsFromKeyWord(JobProcess.DominatorAccountModel, queryInfo.QueryValue, _searchFilterModel);

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
            }
            else
            {
                GlobusLogHelper.log.Info(Log.FoundXResults,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, _searchPostResponseHandler.LstTumblrPosts.Count,
                    queryInfo.QueryType, queryInfo.QueryValue, ActivityType);

                ProcessOnUserPosts(queryInfo, ref jobProcessResult, _searchPostResponseHandler.LstTumblrPosts,
                    xTumblrFormKey);
                if (!jobProcessResult.IsProcessSuceessfull) _searchForPostCount++;
            }
            if (string.IsNullOrEmpty(_searchPostResponseHandler.NextPageUrl) || (_searchForPostCount == 5 && !string.IsNullOrEmpty(_searchPostResponseHandler.NextPageUrl)))
            {
                GlobusLogHelper.log.Info(Log.ProcessCompleted,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, ActivityType);
                jobProcessResult.IsProcessCompleted = true;
            }


        }
    }
}
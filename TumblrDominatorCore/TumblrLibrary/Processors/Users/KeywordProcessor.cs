using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using System.Threading;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrLibrary.TumblrProcesses;
using TumblrDominatorCore.TumblrResponseHandler;

namespace TumblrDominatorCore.TumblrLibrary.Processors.Users
{
    public class KeywordsProcessor : BaseTumblrUserProcessor, IQueryProcessor
    {
        private int _currentPage;
        private SearchUsersForKeywordRespone _searchuserResponseHandler;
        private string _tumblrformkey = string.Empty;
        private string cursor = "";

        public KeywordsProcessor(IProcessScopeModel processScopeModel,
            ITumblrJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, ITumblrFunct tumblrFunct) : base(processScopeModel, jobProcess,
            dbAccountService, globalService, campaignService, tumblrFunct)
        {
            _searchuserResponseHandler = null;
        }

        /// <summary>
        ///     Extract Users and process them to Follow or Message, from given queryvalue
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <param name="jobProcessResult"></param>
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            string url, Referer;
            var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
            if (_currentPage == 0)
            {
                var Tuple = ConstantHelpDetails.GetSearchFilterUrl(queryInfo.QueryValue, new SearchFilterModel(), IsBrowser, false, string.Empty);
                url = Tuple.Item1;
                Referer = Tuple.Item2;
            }
            else
            {
                if (string.IsNullOrEmpty(cursor))
                {
                    jobProcessResult.HasNoResult = true;
                    return;
                }
                GlobusLogHelper.log.Info(Log.CustomMessage,
                JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                $"Searching for Next Page {queryInfo.QueryType} {queryInfo.QueryValue}");
                var Tuple = ConstantHelpDetails.GetSearchFilterUrl(queryInfo.QueryValue, new SearchFilterModel(), IsBrowser, false, cursor);
                url = Tuple.Item1;
                Referer = Tuple.Item2;
            }

            if (!IsBrowser)
            {
                var USerResponse =
                    TumblrFunct.GetApiResponse(JobProcess.DominatorAccountModel, url, ConstantHelpDetails.BearerToken, Referer);

                _searchuserResponseHandler = new SearchUsersForKeywordRespone(USerResponse.Response);
                if (!string.IsNullOrEmpty(_searchuserResponseHandler.Cursor))
                {
                    cursor = _searchuserResponseHandler.Cursor;
                }
                else
                    cursor = string.Empty;
                if (!string.IsNullOrEmpty(_searchuserResponseHandler.TumblrFormKey))
                    _tumblrformkey = _searchuserResponseHandler.TumblrFormKey;
            }
            else
                _searchuserResponseHandler = _browser.GetTumblrUsersFromKeyWord(JobProcess.DominatorAccountModel, queryInfo.QueryValue, _searchFilterModel);



            if (_searchuserResponseHandler == null || _searchuserResponseHandler.LstTumblrUser.Count == 0 &&
                !_searchuserResponseHandler.IsPagination)
            {
                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, ((BaseTumblrProcessor)this).ActivityType);
                jobProcessResult.HasNoResult = true;
                jobProcessResult.maxId = null;
                if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    return;
            }
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (_searchuserResponseHandler.Success && _searchuserResponseHandler.LstTumblrUser.Count == 0 &&
                _searchuserResponseHandler.IsPagination)
            {
                _currentPage++;
                return;
            }
            if (_searchuserResponseHandler == null || _searchuserResponseHandler.LstTumblrUser.Count == 0 &&
                !_searchuserResponseHandler.IsPagination)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                          JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                          JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                          $"Result in Searching for {queryInfo.QueryType} {queryInfo.QueryValue} => " + "No Users Found");
                jobProcessResult.HasNoResult = true;
                jobProcessResult.maxId = null;

                if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    return;
            }

            if (_searchuserResponseHandler.Success && _searchuserResponseHandler.LstTumblrUser.Count > 0)
            {
                GlobusLogHelper.log.Info(Log.FoundXResults,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, _searchuserResponseHandler.LstTumblrUser.Count,
                    queryInfo.QueryType, queryInfo.QueryValue, ((BaseTumblrProcessor)this).ActivityType);
                FilterAndStartFinalProcess(queryInfo, ref jobProcessResult, _searchuserResponseHandler.LstTumblrUser);
            }

            if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                jobProcessResult.IsProcessCompleted = true;
            _currentPage++;
            Thread.Sleep(100);
            //GlobusLogHelper.log.Info(Log.FoundXResults, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, lstTumblrUsers.Count, queryInfo.QueryType, queryInfo.QueryValue, ((BaseTumblrProcessor)this).ActivityType);
        }
    }
}
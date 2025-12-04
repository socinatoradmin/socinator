using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrLibrary.TumblrProcesses;
using TumblrDominatorCore.TumblrResponseHandler;

namespace TumblrDominatorCore.TumblrLibrary.Processors.Users
{
    public class HashtagProcessor : BaseTumblrUserProcessor, IQueryProcessor
    {
        private int _currentPage;
        private SearchUsersForKeywordRespone _searchuserResponseHandler;
        private int offset = 15;
        private string xTumblrFormKey = string.Empty;

        public HashtagProcessor(IProcessScopeModel processScopeModel,
            ITumblrJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, ITumblrFunct tumblrFunct) : base(processScopeModel, jobProcess,
            dbAccountService, globalService, campaignService, tumblrFunct)
        {
            _searchuserResponseHandler = null;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            var url = string.Empty;
            if (_currentPage == 0)
            {
                url = ConstantHelpDetails.GetUsersAPIByKeyWordOrHashTagValue(queryInfo.QueryValue);
            }
            else if (_currentPage == 1)
            {
                offset = 15;
                url = ConstantHelpDetails.GetUsersAPIByKeyWordOrHashTagValue(queryInfo.QueryValue) + $"&blog_offset={offset}";
            }
            else
            {
                offset += 11;
                url = ConstantHelpDetails.GetUsersAPIByKeyWordOrHashTagValue(queryInfo.QueryValue) + $"&blog_offset={offset}";
            }

            if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                _searchuserResponseHandler = new SearchUsersForKeywordRespone(TumblrFunct.GetApiResponse(JobProcess.DominatorAccountModel, url,
                    ConstantHelpDetails.BearerToken).Response);
            else
                _searchuserResponseHandler = _browser.GetTumblrUsersFromKeyWord(JobProcess.DominatorAccountModel, queryInfo.QueryValue?.Replace("#", ""), _searchFilterModel, true);


            if (_searchuserResponseHandler != null && _searchuserResponseHandler.LstTumblrUser.Count == 0 &&
                _searchuserResponseHandler.IsPagination)
            {
                _currentPage++;
                return;
            }

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
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
            if (_searchuserResponseHandler.LstTumblrUser.Count != 0)
            {
                GlobusLogHelper.log.Info(Log.FoundXResults,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, _searchuserResponseHandler.LstTumblrUser.Count,
                    queryInfo.QueryType, queryInfo.QueryValue, ((BaseTumblrProcessor)this).ActivityType);
            }
            if (_searchuserResponseHandler.LstTumblrUser.Count > 0)
                FilterAndStartFinalProcess(queryInfo, ref jobProcessResult,
                    _searchuserResponseHandler.LstTumblrUser);
            if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                jobProcessResult.IsProcessCompleted = true;
            _currentPage++;
        }
    }
}
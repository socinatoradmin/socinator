using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using System.Linq;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction.TumblrBrowserManager;
using TumblrDominatorCore.TumblrLibrary.TumblrProcesses;
using TumblrDominatorCore.TumblrResponseHandler;

namespace TumblrDominatorCore.TumblrLibrary.Processors.Users
{
    public class UserFollowingProcerssor : BaseTumblrUserProcessor, IQueryProcessor
    {
        private readonly ITumblrBrowserManager _browser;
        bool isShareFollowing;
        private int _currentPage;
        private string MainPageUrl = "";
        TumblrUser tumblrUser = new TumblrUser();
        private SearchforFollowingsorFollowersResponse _searchuserResponseHandler;

        public UserFollowingProcerssor(ITumblrBrowserManager browser, IProcessScopeModel processScopeModel,
            ITumblrJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, ITumblrFunct tumblrFunct) : base(processScopeModel, jobProcess,
            dbAccountService, globalService, campaignService, tumblrFunct)
        {
            _searchuserResponseHandler = null;
            _browser = browser;
        }

        /// <summary>
        ///     Extract Users from TumblrBlog followings and Process them Follow  or Message
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <param name="jobProcessResult"></param>
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var tumblrUser = TumblrUtility.getTumblrUserFromPostUrlorBlogUrlOrUsername(queryInfo.QueryValue);
            if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
            {
                if (string.IsNullOrEmpty(MainPageUrl))
                    MainPageUrl = ConstantHelpDetails.GetSomeOnesFollowingsAPIByUserName(tumblrUser.Username);
                _searchuserResponseHandler = TumblrFunct.SearchUsernameForFollowing(JobProcess.DominatorAccountModel,
                    tumblrUser.Username, _currentPage, MainPageUrl);
                if (!string.IsNullOrEmpty(_searchuserResponseHandler.NextPageUrl))
                    MainPageUrl = _searchuserResponseHandler.NextPageUrl;
            }
            else
            {
                _browser.SearchUserDetails(JobProcess.DominatorAccountModel, ref tumblrUser);
                if (tumblrUser.ShareFollowing)
                    _searchuserResponseHandler = _browser.GetSomeOnesFollowings(ConstantHelpDetails.SomeOnesFollowingsUrl(tumblrUser.Username), _currentPage);

            }
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (_searchuserResponseHandler != null && _searchuserResponseHandler.SearchResponse != null && _searchuserResponseHandler.SearchResponse.Response.Contains("{\"meta\":{\"status\":403,\"msg\":\"Forbidden\"}")
                )
            {
                GlobusLogHelper.log.Info(Log.CanNotAccessToThisAccout,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, ((BaseTumblrProcessor)this).ActivityType, " => " + tumblrUser.Username);
                jobProcessResult.HasNoResult = true;
                jobProcessResult.maxId = null;
            }
            if (_searchuserResponseHandler == null && _searchuserResponseHandler.SearchResponse != null && !_searchuserResponseHandler.SearchResponse.Response.Contains("{\"meta\":{\"status\":403,\"msg\":\"Forbidden\"}")
                && _searchuserResponseHandler.LstTumblrUser.Count == 0)
            {
                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, ((BaseTumblrProcessor)this).ActivityType);
                jobProcessResult.HasNoResult = true;
                jobProcessResult.maxId = null;
            }
            if (!_searchuserResponseHandler.LstTumblrUser.Any() && _searchuserResponseHandler != null && _searchuserResponseHandler.Success && _searchuserResponseHandler.TotalFollowings == 0)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                         JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                         JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                         $"{tumblrUser.Username} => Has 0 Followings.");
                jobProcessResult.IsProcessCompleted = true;
                return;
            }
            if (_searchuserResponseHandler != null && _searchuserResponseHandler.Success && _searchuserResponseHandler.LstTumblrUser.Count > 0)
            {
                GlobusLogHelper.log.Info(Log.FoundXResults,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, _searchuserResponseHandler.LstTumblrUser.Count,
                    queryInfo.QueryType, queryInfo.QueryValue, ((BaseTumblrProcessor)this).ActivityType);
                FilterAndStartFinalProcess(queryInfo, ref jobProcessResult, _searchuserResponseHandler.LstTumblrUser);
                if (!jobProcessResult.IsProcessSuceessfull)
                    _currentPage++;
            }
            if (string.IsNullOrEmpty(_searchuserResponseHandler.NextPageUrl) || (_currentPage == 5 && !string.IsNullOrEmpty(_searchuserResponseHandler.NextPageUrl)))
            {
                GlobusLogHelper.log.Info(Log.ProcessCompleted,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, ActivityType);
                jobProcessResult.IsProcessCompleted = true;
            }
        }
    }
}
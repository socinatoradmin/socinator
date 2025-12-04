using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction.TumblrBrowserManager;
using TumblrDominatorCore.TumblrLibrary.TumblrProcesses;
using TumblrDominatorCore.TumblrResponseHandler;

namespace TumblrDominatorCore.TumblrLibrary.Processors.Users
{
    public class OwnFollowerProcerssor : BaseTumblrUserProcessor, IQueryProcessor
    {
        private readonly ITumblrBrowserManager _browser;
        private int _currentPage;
        private string nextPageUrl;


        public OwnFollowerProcerssor(ITumblrBrowserManager browser, IProcessScopeModel processScopeModel,
            ITumblrJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, ITumblrFunct tumblrFunct) : base(processScopeModel, jobProcess,
            dbAccountService, globalService, campaignService, tumblrFunct)
        {
            _browser = browser;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            SearchforFollowingsorFollowersResponse _searchuserResponseHandler = new SearchforFollowingsorFollowersResponse();
            if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                _searchuserResponseHandler = new SearchforFollowingsorFollowersResponse(TumblrFunct.GetApiResponse(JobProcess.DominatorAccountModel,
                    ConstantHelpDetails.GetUserFollowersAPIByUserName(JobProcess.DominatorAccountModel.AccountBaseModel.UserId), ConstantHelpDetails.BearerToken).Response);
            else
                _searchuserResponseHandler = _browser.GetFollowingsOrFollowers(ConstantHelpDetails.OwnFollowersUrl(JobProcess.DominatorAccountModel.AccountBaseModel.UserId), _currentPage);
            if (_searchuserResponseHandler != null && _searchuserResponseHandler.Success && _searchuserResponseHandler.LstTumblrUser?.Count == 0)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                        $"No Follower Found {queryInfo.QueryType} {JobProcess.DominatorAccountModel.AccountBaseModel.UserId}");
                jobProcessResult.HasNoResult = true;
                return;
            }
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (_searchuserResponseHandler == null && _searchuserResponseHandler.LstTumblrUser?.Count == 0 && !_searchuserResponseHandler.Success)
            {
                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, ((BaseTumblrProcessor)this).ActivityType);
                jobProcessResult.HasNoResult = true;
                jobProcessResult.maxId = null;
                return;
            }


            if (_searchuserResponseHandler != null && _searchuserResponseHandler.Success)
            {
                var lstTumblrUsers = _searchuserResponseHandler.LstTumblrUser;
                GlobusLogHelper.log.Info(Log.FoundXResults,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, lstTumblrUsers.Count,
                    queryInfo.QueryType, queryInfo.QueryValue, ((BaseTumblrProcessor)this).ActivityType);
                FilterAndStartFinalProcess(queryInfo, ref jobProcessResult, _searchuserResponseHandler.LstTumblrUser);
                _currentPage++;
            }

            jobProcessResult.HasNoResult = true;
        }
    }
}
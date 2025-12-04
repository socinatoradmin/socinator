using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using System.Collections.Generic;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrLibrary.TumblrProcesses;
using TumblrDominatorCore.TumblrResponseHandler;

namespace TumblrDominatorCore.TumblrLibrary.Processors.Users
{
    public class PostOwnerProcessor : BaseTumblrUserProcessor, IQueryProcessor
    {
        private SearchUsersForPostOwnerResponse _searchUserForPostOwnerResponseHandler;

        private string MainUrl;
        private readonly string xTumblrFormKey = string.Empty;

        public PostOwnerProcessor(IProcessScopeModel processScopeModel,
            ITumblrJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, ITumblrFunct tumblrFunct) : base(processScopeModel, jobProcess,
            dbAccountService, globalService, campaignService, tumblrFunct)
        {
            _searchUserForPostOwnerResponseHandler = null;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            if (string.IsNullOrEmpty(queryInfo.QueryValue))
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                     JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                     JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                     $"No Query Value Found for {queryInfo.QueryType} {queryInfo.QueryValue}");
                jobProcessResult.HasNoResult = true;
                return;
            }
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var LstTumblrUser = new List<TumblrUser>();
            TumblrUser tumbrUser = TumblrUtility.getTumblrUserFromPostUrlorBlogUrlOrUsername(queryInfo.QueryValue);

            if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
            {
                var SearchUserResponse = TumblrFunct.GetUserDetailsResponseApi(JobProcess.DominatorAccountModel, tumbrUser.Username);
                _searchUserForPostOwnerResponseHandler = new SearchUsersForPostOwnerResponse(SearchUserResponse);
            }
            else
            {
                _browser.SearchUserDetails(JobProcess.DominatorAccountModel, ref tumbrUser);

                _searchUserForPostOwnerResponseHandler = new SearchUsersForPostOwnerResponse()
                {
                    user = tumbrUser,
                    PostOwnerResponse = new ResponseParameter() { Response = "" }
                };
            }
            if (_searchUserForPostOwnerResponseHandler != null && _searchUserForPostOwnerResponseHandler.Success)
            {
                jobProcessResult.IsProcessCompleted = true;
                if (LstTumblrUser.Count != 0)
                {

                    GlobusLogHelper.log.Info(Log.FoundXResults,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserName, LstTumblrUser.Count,
                        queryInfo.QueryType, queryInfo.QueryValue, ((BaseTumblrProcessor)this).ActivityType);

                }
                LstTumblrUser.Add(_searchUserForPostOwnerResponseHandler.user);
                FilterAndStartFinalProcess(queryInfo, ref jobProcessResult,
                           LstTumblrUser);
            }
            if (_searchUserForPostOwnerResponseHandler != null && !_searchUserForPostOwnerResponseHandler.Success)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                $"Searching for  {queryInfo.QueryType} {tumbrUser.Username}" + " Failed");
                jobProcessResult.IsProcessCompleted = true;
            }

            if (_searchUserForPostOwnerResponseHandler != null && LstTumblrUser.Count == 0 &&
                    _searchUserForPostOwnerResponseHandler.IsPagination) return;
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
        }
    }
}
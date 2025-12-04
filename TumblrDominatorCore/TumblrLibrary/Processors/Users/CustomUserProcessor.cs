using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrLibrary.TumblrProcesses;
using TumblrDominatorCore.TumblrResponseHandler;

namespace TumblrDominatorCore.TumblrLibrary.Processors.Users
{
    public class CustomUserProcessor : BaseTumblrUserProcessor, IQueryProcessor
    {
        private SearchUsersForCustomUserListResponse _customUserResponseHandler;
        public CustomUserProcessor(IProcessScopeModel processScopeModel,
            ITumblrJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, ITumblrFunct tumblrFunct) : base(processScopeModel, jobProcess,
            dbAccountService, globalService, campaignService, tumblrFunct)
        {

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
            var CanProcessUserlst = new List<TumblrUser>();
            Regex.Split(queryInfo.QueryValue, "\r\n").ToList().ForEach(x =>
                {
                    var tumbrUser = TumblrUtility.getTumblrUserFromPostUrlorBlogUrlOrUsername(x);
                    if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        var SearchUserResponse = TumblrFunct.GetUserDetailsResponseApi(JobProcess.DominatorAccountModel, tumbrUser.Username);
                        _customUserResponseHandler = new SearchUsersForCustomUserListResponse(SearchUserResponse);
                    }
                    else
                    {
                        _browser.SearchUserDetails(JobProcess.DominatorAccountModel, ref tumbrUser);
                        _customUserResponseHandler = new SearchUsersForCustomUserListResponse()
                        {
                            user = tumbrUser,
                            CustomUserResponse = new ResponseParameter() { Response = "" }
                        };
                    }
                    if (_customUserResponseHandler == null || _customUserResponseHandler.CustomUserResponse.Response.Contains("\"meta\":{\"status\":404,\"msg\":\"Not Found\""))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                       JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                        $"Error in Searching for {queryInfo.QueryType} {tumbrUser.Username}");
                    }
                    else
                    {
                        CanProcessUserlst.Add(_customUserResponseHandler.user);
                    }
                });
            if (CanProcessUserlst.Count > 0)
                FilterAndStartFinalProcess(queryInfo, ref jobProcessResult,
                                                             CanProcessUserlst);

            jobProcessResult.IsProcessCompleted = true;

        }
    }
}

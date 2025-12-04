using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDResponse.BrowserResponseHandler.UserResponseHandler;
using FaceDominatorCore.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor
{
    public class UserFrinedsProcessor : BaseFbUserProcessor
    {

        IResponseHandler responseHandler;


        public UserFrinedsProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
         IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
         IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            responseHandler = null;

        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            var ownfriendsList = new List<FacebookUser>();
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (queryInfo.QueryType == "Own Friends" && queryInfo.QueryValue == "NA")
                ownfriendsList = _dbAccountService.Get<Friends>().Select(x => JsonConvert.DeserializeObject<FacebookUser>(x.DetailedUserInfo)).ToList<FacebookUser>();
            else
                Browsermanager.SearchByFriendUrl(AccountModel, FbEntityType.User, AccountModel.AccountBaseModel.UserId);

            try
            {


                var noOfPagesToScroll = 0;

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        noOfPagesToScroll = 100;
                        if (queryInfo.QueryType == "Own Friends" && queryInfo.QueryValue == "NA")
                            responseHandler = new SearchPeopleResponseHandler(new ResponseParameter() { Response = "" }, true) { ObjFdScraperResponseParameters = new FdScraperResponseParameters() { ListUser = ownfriendsList } };
                        else
                            responseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetData(AccountModel, FbEntityType.Friends, noOfPagesToScroll, 0, FdConstants.FriendUser3Element)
                            : ObjFdRequestLibrary.GetFriendOfFriend(AccountModel, AccountModel.AccountBaseModel.UserId, responseHandler);

                        if (responseHandler.Status)
                        {
                            List<FacebookUser> lstFacebookUser = responseHandler.ObjFdScraperResponseParameters.ListUser;

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, lstFacebookUser.Count, queryInfo.QueryType,
                                        queryInfo.QueryValue, _ActivityType);

                            ProcessDataOfUsers(queryInfo, ref jobProcessResult, lstFacebookUser);
                            jobProcessResult.maxId = responseHandler.PageletData;

                            jobProcessResult.HasNoResult = !responseHandler.HasMoreResults;

                        }
                        else
                        {
                            jobProcessResult.HasNoResult = true;
                            jobProcessResult.maxId = null;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}

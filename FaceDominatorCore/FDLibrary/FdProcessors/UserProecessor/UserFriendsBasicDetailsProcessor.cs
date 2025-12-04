using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor
{
    public class UserFriendsBasicDetailsProcessor : BaseFbUserProcessor
    {
        IResponseHandler responseHandler;

        public UserFriendsBasicDetailsProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objRequestLibrary, IFdBrowserManager browserManager,
            IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objRequestLibrary, browserManager, processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            string url = queryInfo.QueryValue.Contains(FdConstants.FbHomeUrl)
                ? queryInfo.QueryValue
                : FdConstants.FbHomeUrl + queryInfo.QueryValue;

            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.SearchByFriendUrl(AccountModel, FbEntityType.User, url);

            var listAlreadyScrapedUsers = new List<FacebookUser>();

            try
            {


                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        responseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetData(AccountModel, FbEntityType.Friends, 5, 0, FdConstants.FriendUser3Element)
                            : ObjFdRequestLibrary.GetFriendOfFriend(AccountModel, queryInfo.QueryValue, responseHandler);

                        if (responseHandler.Status)
                        {
                            List<FacebookUser> lstFacebookUser = responseHandler.ObjFdScraperResponseParameters.ListUser;

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, lstFacebookUser.Count, queryInfo.QueryType,
                                        queryInfo.QueryValue, _ActivityType);

                            lstFacebookUser.RemoveAll(x => listAlreadyScrapedUsers.Any(y =>
                                     y.UserId == x.UserId));

                            foreach (var facebookUser in lstFacebookUser)
                            {
                                if (AlreadyInteractedUser(facebookUser))
                                    continue;

                                SendToPerformActivity(ref jobProcessResult, facebookUser, queryInfo);
                            }

                            listAlreadyScrapedUsers.AddRange(lstFacebookUser);

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

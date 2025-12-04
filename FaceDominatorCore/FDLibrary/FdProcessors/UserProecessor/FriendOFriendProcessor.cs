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
using FaceDominatorCore.FDResponse.CommonResponse;
using System;
using System.Collections.Generic;

namespace FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor
{
    public class FriendOFriendProcessor : BaseFbUserProcessor
    {
        public FriendOFriendProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {

        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            int noOfPagesToScroll = 0;

            string friendId = queryInfo.QueryValue;

            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.SearchByFriendUrl(AccountModel, FbEntityType.User, friendId);

            FdFriendOfFriendResponseHandler _objFdFriendOfFriendResponseHandler = null;

            try
            {
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        noOfPagesToScroll = 5;

                        var responseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetData(AccountModel, FbEntityType.Friends, noOfPagesToScroll, 0, FdConstants.FriendUser3Element)
                            : ObjFdRequestLibrary.GetFriendOfFriend(AccountModel, friendId, _objFdFriendOfFriendResponseHandler);

                        if (responseHandler != null && responseHandler.Status)
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
}

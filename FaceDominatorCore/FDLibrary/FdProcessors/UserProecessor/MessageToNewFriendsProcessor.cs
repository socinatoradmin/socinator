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

namespace FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor
{
    public class MessageToNewFriendsProcessor : BaseFbNonQueryUserProcessor
    {
        private IResponseHandler FdFriendsInfoNewResponseHandler;

        public MessageToNewFriendsProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            FdFriendsInfoNewResponseHandler = null;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.SearchByFriendRequests(AccountModel, FbEntityType.AddedFriends);


            try
            {
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {

                        FdFriendsInfoNewResponseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetFriends(AccountModel, FbEntityType.AddedFriends, 6, 0, FdConstants.AddedFriend3, FdConstants.SentFriendPaginationElement)
                            : ObjFdRequestLibrary.UpdateFriendsNewSync(AccountModel, FdFriendsInfoNewResponseHandler);

                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (FdFriendsInfoNewResponseHandler.Status)
                        {
                            List<FacebookUser> lstFacebookUser = FdFriendsInfoNewResponseHandler.ObjFdScraperResponseParameters.ListUser;

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, lstFacebookUser.Count, queryInfo.QueryType,
                                queryInfo.QueryValue, _ActivityType);

                            ProcessDataOfUsers(ref jobProcessResult, lstFacebookUser, string.Empty, string.Empty);

                            jobProcessResult.maxId = FdFriendsInfoNewResponseHandler.ObjFdScraperResponseParameters?.FriendsPager?.Data;

                            if (string.IsNullOrEmpty(FdFriendsInfoNewResponseHandler.ObjFdScraperResponseParameters?.FriendsPager?.Data)
                                || FdFriendsInfoNewResponseHandler.ObjFdScraperResponseParameters?.FriendsPager?.MaxDataKey ==
                                FdFriendsInfoNewResponseHandler.ObjFdScraperResponseParameters?.FriendsPager?.CurrentDataKey)
                                jobProcessResult.HasNoResult = true;

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
                        jobProcessResult.HasNoResult = true;
                        jobProcessResult.maxId = null;
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

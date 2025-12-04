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
using FaceDominatorCore.FDResponse.FriendsResponse;
using System;
using System.Collections.Generic;

namespace FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor
{
    public class SuggestedFriendsProcessor : BaseFbUserProcessor
    {
        private FriendSuggestedByAFriendResponseHandler _objFriendSuggestedByAFriendResponseHandler;

        public SuggestedFriendsProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            _objFriendSuggestedByAFriendResponseHandler = null;

        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {

            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.SearchByFriendRequests(AccountModel, FbEntityType.SuggestedFriends);


            try
            {

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {

                        var suggestFriendClass = FdConstants.SuggestedFriendsUserElement2;

                        var friendSuggestedByAFriendResponseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetFriends(AccountModel, FbEntityType.SuggestedFriends, 3, 0, suggestFriendClass)
                            : ObjFdRequestLibrary.GetFriendSuggestedByFacebook(AccountModel, _objFriendSuggestedByAFriendResponseHandler);

                        if (friendSuggestedByAFriendResponseHandler.Status)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            List<FacebookUser> lstFacebookUser = friendSuggestedByAFriendResponseHandler.ObjFdScraperResponseParameters.ListUser;

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, lstFacebookUser.Count, queryInfo.QueryType,
                                        queryInfo.QueryValue, _ActivityType);

                            ProcessDataOfUsers(queryInfo, ref jobProcessResult, lstFacebookUser);
                            jobProcessResult.maxId = _objFriendSuggestedByAFriendResponseHandler.ObjFdScraperResponseParameters.ExtraData;

                            if (_objFriendSuggestedByAFriendResponseHandler.HasMoreResults == false)
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


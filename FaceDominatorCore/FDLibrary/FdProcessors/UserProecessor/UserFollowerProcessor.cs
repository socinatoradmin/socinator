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
    public class UserFollowerProcessor : BaseFbUserProcessor
    {
        IResponseHandler _objFanpageLikersResponseHandler;
        public UserFollowerProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
         IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
         IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            _objFanpageLikersResponseHandler = null;

        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            string pageUrl = queryInfo.QueryValue;

            ObjFdRequestLibrary.GetLangugae(AccountModel);
            if (AccountModel.IsRunProcessThroughBrowser && !Browsermanager.SearchByFriendUrl(AccountModel, FbEntityType.Followers, pageUrl))
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                            AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, _ActivityType,
                            "Followers not found for this User");
                jobProcessResult.IsProcessCompleted = true;
                jobProcessResult.HasNoResult = true;
                return;
            }

            try
            {
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        _objFanpageLikersResponseHandler = AccountModel.IsRunProcessThroughBrowser
                           ? Browsermanager.ScrollWindowAndGetData(AccountModel, FbEntityType.Friends, 2, 0, FdConstants.FriendUser3Element)
                           : ObjFdRequestLibrary.GetUserFollowers(AccountModel, pageUrl, _objFanpageLikersResponseHandler);

                        if (_objFanpageLikersResponseHandler.Status || _objFanpageLikersResponseHandler.HasMoreResults)
                        {
                            List<FacebookUser> lstFacebookUser = _objFanpageLikersResponseHandler.
                                ObjFdScraperResponseParameters.ListUser;

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, lstFacebookUser.Count, queryInfo.QueryType,
                                        queryInfo.QueryValue, _ActivityType);

                            ProcessDataOfUsers(queryInfo, ref jobProcessResult, lstFacebookUser);
                            jobProcessResult.maxId = _objFanpageLikersResponseHandler.PageletData;

                            if (!_objFanpageLikersResponseHandler.HasMoreResults
                                            || string.IsNullOrEmpty(_objFanpageLikersResponseHandler.PageletData))
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

using DominatorHouseCore;
using DominatorHouseCore.Enums;
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
    public class WithdrawRequestProcessor : BaseFbNonQueryUserProcessor
    {

        public WithdrawRequestProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                while (!jobProcessResult.IsProcessCompleted)
                {

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (JobProcess.ModuleSetting.UnfriendOptionModel.IsAddedThroughSoftware
                        && JobProcess.ModuleSetting.UnfriendOptionModel.IsAddedOutsideSoftware)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        FilterAndStartFinalProcessForWithdraw(jobProcessResult, "Both", "WithdrawRequest");

                        jobProcessResult.IsProcessCompleted = true;
                    }
                    else
                    {
                        if (JobProcess.ModuleSetting.UnfriendOptionModel.IsAddedThroughSoftware)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            FilterAndStartFinalProcessForWithdraw(jobProcessResult, "Through Software",
                                "WithdrawRequest");
                            jobProcessResult.IsProcessCompleted = true;
                        }

                        if (JobProcess.ModuleSetting.UnfriendOptionModel.IsAddedOutsideSoftware)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            FilterAndStartFinalProcessForWithdraw(jobProcessResult, "Outside Software",
                                "WithdrawRequest");
                            jobProcessResult.IsProcessCompleted = true;
                        }
                    }

                    if (JobProcess.ModuleSetting.UnfriendOptionModel.IsCustomUserList)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        jobProcessResult = new JobProcessResult();

                        FilterAndStartFinalProcessForWithdraw(jobProcessResult, "Outside Software", "WithdrawRequest");
                    }
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
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

        public void FilterAndStartFinalProcessForWithdraw(JobProcessResult jobProcessResult, string query, string queryValue)
        {
            IResponseHandler SentRequestFriendListResponseHandler = null;

            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.SearchByFriendRequests(AccountModel, FbEntityType.SentFriendRequests);

            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                try
                {

                    SentRequestFriendListResponseHandler = AccountModel.IsRunProcessThroughBrowser
                        ? Browsermanager.ScrollWindowAndGetFriends(AccountModel, FbEntityType.SentFriendRequests, 50, 0, FdConstants.SentFriend3Element, FdConstants.SentFriendPaginationElement)
                        : ObjFdRequestLibrary.GetSentFriendRequestIdsNew(AccountModel, SentRequestFriendListResponseHandler);

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (SentRequestFriendListResponseHandler.Status)
                    {
                        List<FacebookUser> lstUserIds = SentRequestFriendListResponseHandler.ObjFdScraperResponseParameters.ListUser;

                        List<FacebookUser> lstFilteredUserIds = new List<FacebookUser>();

                        var lstUser = _dbAccountService.GetInteractedUsers(ActivityType.SendFriendRequest);

                        if (query.Contains("Both"))
                            lstFilteredUserIds = lstUserIds;

                        else if (query.Contains("Through Software"))
                        {
                            lstUserIds.ForEach(x =>
                            {
                                if (lstUser.FirstOrDefault(y => y.UserId == x.UserId) != null || lstUser.FirstOrDefault(y => y.UserProfileUrl == x.ScrapedProfileUrl) != null)
                                    lstFilteredUserIds.Add(x);
                            });
                        }
                        else if (query.Contains("Outside Software"))
                        {
                            lstUserIds.ForEach(x =>
                            {
                                if (lstUser.FirstOrDefault(y => y.UserId == x.UserId) == null)
                                    lstFilteredUserIds.Add(x);
                            });
                        }

                        if (lstFilteredUserIds.Count > 0)
                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, lstFilteredUserIds.Count, "Withdraw Request", "", _ActivityType);

                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (lstFilteredUserIds.Count == 0 &&
                            SentRequestFriendListResponseHandler.ObjFdScraperResponseParameters.FriendsPager?.CurrentDataKey == SentRequestFriendListResponseHandler.ObjFdScraperResponseParameters.FriendsPager?.MaxDataKey)
                        {
                            jobProcessResult.IsProcessCompleted = true;
                            jobProcessResult.HasNoResult = true;
                            jobProcessResult.maxId = null;
                        }
                        if (lstFilteredUserIds.Count > 0)
                            ProcessDataOfUsers(ref jobProcessResult, lstFilteredUserIds, query, queryValue);

                        jobProcessResult.maxId = SentRequestFriendListResponseHandler.ObjFdScraperResponseParameters.FriendsPager?.Data;

                        jobProcessResult.HasNoResult = !SentRequestFriendListResponseHandler.HasMoreResults;

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
                    jobProcessResult.IsProcessCompleted = true;
                    ex.DebugLog();
                }
            }
        }
    }
}

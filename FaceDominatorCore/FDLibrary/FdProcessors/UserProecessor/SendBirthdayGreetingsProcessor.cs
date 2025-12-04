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

namespace FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor
{
    public class SendBirthdayGreetingsProcessor : BaseFbNonQueryUserProcessor
    {
        private IResponseHandler responseHandler;

        public SendBirthdayGreetingsProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            responseHandler = null;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.SearchByBirthDayEvent(AccountModel, $"{FdConstants.FbHomeUrl}events/");

            try
            {

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        var className = FdConstants.SendGreetings2Element;

                        var paginationClass = string.Empty;

                        responseHandler = AccountModel.IsRunProcessThroughBrowser
                              ? Browsermanager.ScrollWindowAndGetFriends(AccountModel, FbEntityType.UserGreetings, 3, 0, className, paginationClass)
                              : ObjFdRequestLibrary.GetUsersBirtdayResponse(AccountModel, responseHandler);

                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (responseHandler.Status)
                        {
                            var lstFacebookUser = responseHandler.ObjFdScraperResponseParameters.ListUser;

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, lstFacebookUser.Count, queryInfo.QueryType,
                                        queryInfo.QueryValue, _ActivityType);

                            ProcessDataOfUsers(ref jobProcessResult, lstFacebookUser, string.Empty, string.Empty);

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
                        throw new OperationCanceledException("Requested Cancelled !");
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
                throw new OperationCanceledException("Requested Cancelled !");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}

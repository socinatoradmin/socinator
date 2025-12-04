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

    public class EventsUserProcessor : BaseFbUserProcessor
    {

        public EventsUserProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
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

                List<EventGuestType> lstGuestType = new List<EventGuestType> { EventGuestType.Going, EventGuestType.Interested, EventGuestType.Invited };

                lstGuestType.Shuffle();

                lstGuestType.ForEach(x =>
                {
                    ScrapEventProfileWithJobProcess(queryInfo, x);
                });

                jobProcessResult.IsProcessCompleted = true;

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
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

        private void ScrapEventProfileWithJobProcess(QueryInfo queryInfo, EventGuestType eventGuestType)
        {
            string eventUrl = queryInfo.QueryValue;
            JobProcessResult jobProcessResult = new JobProcessResult();

            IResponseHandler objEventGuestsResponseHandler = null;

            var noOfPagesToScroll = 0;

            try
            {
                if (AccountModel.IsRunProcessThroughBrowser)
                    Browsermanager.SearchByEventUrl(AccountModel, FbEntityType.User, eventUrl, eventGuestType);

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        noOfPagesToScroll = 2;

                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();


                        objEventGuestsResponseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetData(AccountModel, FbEntityType.EventGuests, noOfPagesToScroll, 0, FdConstants.EventGuestUserElement)
                            : ObjFdRequestLibrary.GetInterestedGuestsForEvents(AccountModel, objEventGuestsResponseHandler, eventUrl, eventGuestType);

                        if (objEventGuestsResponseHandler.Status)
                        {
                            List<FacebookUser> lstUserId = objEventGuestsResponseHandler.ObjFdScraperResponseParameters.ListUser.ToList();
                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, lstUserId.Count, queryInfo.QueryType, queryInfo.QueryValue, _ActivityType);

                            ProcessDataOfUsers(queryInfo, ref jobProcessResult, lstUserId);
                            jobProcessResult.maxId = objEventGuestsResponseHandler.PageletData;

                            jobProcessResult.HasNoResult = !objEventGuestsResponseHandler.HasMoreResults;

                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"No more data available for {eventGuestType} option of event url \"{eventUrl}\"");
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
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
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }
    }
}

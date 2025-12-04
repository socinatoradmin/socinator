using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.FbEvents;
using System;
using System.Collections.Generic;

namespace FaceDominatorCore.FDLibrary.FdProcessors.EventProcessors
{
    public class BaseFbEventProcessor : BaseFbProcessor
    {

        public static Dictionary<string, string> UniqueKeyValuePair = new Dictionary<string, string>();

        public IEventCreatorModel EventCreatorModel;

        public BaseFbEventProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            EventCreatorModel = processScopeModel.GetActivitySettingsAs<EventCreatorModel>();

        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {

        }

        public void ProcessDataOfEvents(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            var lstManageEventModel = JobProcess.ModuleSetting.LstManageEventModel;

            foreach (var events in lstManageEventModel)
            {
                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (AlreadyInteractedEvents(events.Id))
                        continue;

                    if (EventCreatorModel.IsUniqueEvents)
                    {
                        if (UniqueKeyValuePair.ContainsValue(events.Id))
                            continue;

                        UniqueKeyValuePair.Add(AccountModel.AccountId, events.Id);
                    }

                    jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew()
                    {
                        ResultEvent = events,
                        QueryInfo = queryInfo
                    });

                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            }

            jobProcessResult.HasNoResult = true;
        }

    }
}

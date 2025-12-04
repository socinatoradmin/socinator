using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using System;

namespace FaceDominatorCore.FDLibrary.FdProcessors.EventProcessors
{
    public class EventCreatorProcessors : BaseFbEventProcessor
    {
        public EventCreatorProcessors(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {

        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {

            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                AccountModel.AccountBaseModel.UserName, _ActivityType, "LangKeyStartedEventCreator".FromResourceDictionary());

            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                ProcessDataOfEvents(queryInfo, ref jobProcessResult);

                jobProcessResult.IsProcessCompleted = true;
                jobProcessResult.maxId = null;

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
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

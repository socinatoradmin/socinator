using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using System;
using System.Collections.Generic;

namespace FaceDominatorCore.FDLibrary.FdProcessors.GroupProcessor
{
    public class CustomGroupProcessor : BaseFbGroupProcessor
    {
        public CustomGroupProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
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
                GroupDetails objGroupDetails = FdConstants.getFacebookGroupFromUrlOrId(queryInfo.QueryValue);

                List<GroupDetails> lstGroupDetails = new List<GroupDetails> { objGroupDetails };

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                ProcessDataOfGroups(queryInfo, ref jobProcessResult, lstGroupDetails);
                jobProcessResult.maxId = null;
                jobProcessResult.HasNoResult = true;
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

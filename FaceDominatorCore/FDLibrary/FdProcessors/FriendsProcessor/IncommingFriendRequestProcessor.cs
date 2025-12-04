using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;

namespace FaceDominatorCore.FDLibrary.FdProcessors.FriendsProcessor
{
    public class IncommingFriendRequestProcessor : BaseIncommingFriendRequestProcessor
    {
        public IncommingFriendRequestProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            AddLocationFilterValues();

            while (!jobProcessResult.IsProcessCompleted)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                JobProcess.CheckProcessLimitsReached();

                if (JobProcess.ModuleSetting.ManageFriendsModel.IsAcceptRequest)
                    FilterAndStartFinalProcessForAcceptRequest(jobProcessResult, "Accept", FdConstants.IncommingFriend2Element, FdConstants.IncommingFriendPaginationElement);

                if (JobProcess.ModuleSetting.ManageFriendsModel.IsCancelReceivedRequest)
                    FilterAndStartFinalProcessForCancelRequest(jobProcessResult, "Cancel", FdConstants.IncommingFriend2Element, FdConstants.IncommingFriendPaginationElement);

                jobProcessResult.IsProcessCompleted = true;

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }

        }
    }
}

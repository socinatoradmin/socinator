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

namespace FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor
{
    public class CustomUserProcessor : BaseFbUserProcessor
    {
        public CustomUserProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            try
            {

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                FacebookUser fbUser = FdConstants.getFaceBookUserFromUrlOrIdOrUserName(queryInfo.QueryValue);

                if (AccountModel.IsRunProcessThroughBrowser)
                    fbUser = Browsermanager.GetFullUserDetails(JobProcess.AccountModel, fbUser, closeBrowser: false).ObjFdScraperResponseParameters?.FacebookUser;

                ProcessDataOfUsers(queryInfo, ref jobProcessResult, new List<FacebookUser>() { fbUser });

                jobProcessResult.IsProcessCompleted = true;

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

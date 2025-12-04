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

namespace FaceDominatorCore.FDLibrary.FdProcessors.FanpageProcessor
{
    public class CustomFanpageProcessor : BaseFbFanpageProcessor
    {
        public CustomFanpageProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            string pageUrl = queryInfo.QueryValue;

            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                FanpageDetails objFanpageDetails = new FanpageDetails();
                objFanpageDetails.FanPageUrl = pageUrl;
                if (AccountModel.IsRunProcessThroughBrowser)
                    objFanpageDetails = Browsermanager.GetFullPageDetails(AccountModel, objFanpageDetails, isNewWindow: false, isCloseBrowser: false).ObjFdScraperResponseParameters.FanpageDetails;
                else
                {
                    var pageId = ObjFdRequestLibrary.GetPageIdFromUrl(AccountModel, pageUrl);
                    objFanpageDetails.FanPageID = pageId;
                    objFanpageDetails.FanPageUrl = $"{FdConstants.FbHomeUrl}{pageId}";
                }

                List<FanpageDetails> lstFanpageDetails = new List<FanpageDetails> { objFanpageDetails };

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                ProcessDataOfPages(queryInfo, ref jobProcessResult, lstFanpageDetails);

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

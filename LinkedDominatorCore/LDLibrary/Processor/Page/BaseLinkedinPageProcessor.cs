using System;
using System.Collections.Generic;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;

namespace LinkedDominatorCore.LDLibrary.Processor.Page
{
    // BaseLinkedinPageProcessor
    public abstract class BaseLinkedinPageProcessor : BaseLinkedinProcessor
    {
        protected BaseLinkedinPageProcessor(ILdJobProcess ldJobProcess, IDbAccountService dbAccountService,
            IDbGlobalService globalService,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory, IDelayService delayService,
            IProcessScopeModel processScopeModel)
            : base(ldJobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
        }

        public void ProcessLinkedinPageFromPage(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<LinkedinPage> lstLinkedinPages)
        {
            try
            {
                foreach (var pages in lstLinkedinPages)
                {
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    SendToPerformActivity(ref jobProcessResult, pages, queryInfo);
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void SendToPerformActivity(ref JobProcessResult jobProcessResult, LinkedinPage LinkedinPages,
            QueryInfo queryInfo)
        {
            try
            {
                jobProcessResult = LdJobProcess.FinalProcess(new ScrapeResultNew
                {
                    ResultPage = LinkedinPages,
                    QueryInfo = queryInfo
                });
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
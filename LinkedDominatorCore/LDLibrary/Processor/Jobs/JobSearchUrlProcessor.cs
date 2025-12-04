using System;
using System.Collections.Generic;
using System.Linq;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;

namespace LinkedDominatorCore.LDLibrary.Processor.Jobs
{
    public class JobSearchUrlProcessor : BaseLinkedinJobProcessor, IQueryProcessor
    {
        public JobSearchUrlProcessor(ILdJobProcess jobProcess,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory, IDelayService delayService,
            IProcessScopeModel processScopeModel) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var start = 0;
                List<InteractedJobs> listInteractedJobsFromAccountDb = null;
                var dFlagship3SearchSrpPeople = "";
                listInteractedJobsFromAccountDb = DbAccountService.GetInteractedJobs(ActivityTypeString).ToList();
                var QueryValue = queryInfo.QueryValue;
                if (!QueryValue.Contains("https://www.linkedin.com") && (jobProcessResult.HasNoResult = true))
                    return;

                var constructedActionUrl = IsBrowser
                    ? QueryValue
                    : GetConstructedApiJobSearch(QueryValue, start);
                LdFunctions.SetWebRequestParametersforjobsearchurl(QueryValue, dFlagship3SearchSrpPeople);
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    var actionUrl = IsBrowser
                        ? start == 0 ? constructedActionUrl : $"{constructedActionUrl}&start={start}"
                        : GetConstructedApiJobSearch(QueryValue, start);

                    if (ActivityType != ActivityType.JobScraper && (jobProcessResult.HasNoResult = true))
                        return;

                    #region MyRegion
                    var jobSearchResponseHandler = LdFunctions.JobSearch(actionUrl);
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (jobSearchResponseHandler.Success)
                    {
                        if (jobSearchResponseHandler.JobsList.Count > 0)
                        {
                            // Filter Already Interacted
                            if (listInteractedJobsFromAccountDb != null)
                                jobSearchResponseHandler.JobsList.RemoveAll(x =>
                                    listInteractedJobsFromAccountDb.Any(y => y.JobPostUrl == x.JobPostUrl));

                            if (jobSearchResponseHandler.JobsList.Count > 0)
                                ProcessLinkedinJobFromJob(queryInfo, ref jobProcessResult,
                                    jobSearchResponseHandler.JobsList);
                        }

                        if (start >= 1000 && (jobProcessResult.HasNoResult = true))
                        {
                            GlobusLogHelper.log.Info("we have reached 100 pages from accounts --> " +
                                                     DominatorAccountModel.AccountBaseModel.UserName);
                            break;
                        }

                        #region Paginate ActionUrl
                        start += IsBrowser ? 25 : 25;

                        #endregion
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType);
                        jobProcessResult.HasNoResult = true;
                    }

                    #endregion
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                jobProcessResult.HasNoResult = true;
            }
        }
    }
}
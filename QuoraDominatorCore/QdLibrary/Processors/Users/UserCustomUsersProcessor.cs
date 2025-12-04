using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorCore.QdLibrary.Processors.Users
{
    public class UserCustomUsersProcessor : BaseQuoraProcessor
    {
        public UserCustomUsersProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IQuoraFunctions objQuoraFunct, IProcessScopeModel processScopeModel) :
            base(browser, jobProcess, dbAccountService, globalService, campaignService, objQuoraFunct,
                processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                string url;
                if (queryInfo.QueryValue.Contains("http"))
                    url = queryInfo.QueryValue;
                else
                    url = $"{QdConstants.HomePageUrl}/profile/" + queryInfo.QueryValue;

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                FilterAndStartFinalProcess(queryInfo, jobProcessResult, new List<string> { url });
                jobProcessResult.IsProcessCompleted = true;
            }
            catch (OperationCanceledException)
            {
                if (_browser.BrowserWindow != null) _browser.CloseBrowser();
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
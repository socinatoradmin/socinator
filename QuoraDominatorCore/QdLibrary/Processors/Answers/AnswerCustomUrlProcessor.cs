using System.Collections.Generic;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.Response;

namespace QuoraDominatorCore.QdLibrary.Processors.Answers
{
    public class AnswerCustomUrlProcessor : BaseQuoraProcessor
    {
        public AnswerCustomUrlProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IQuoraFunctions objQuoraFunct, IProcessScopeModel processScopeModel) :
            base(browser, jobProcess, dbAccountService, globalService, campaignService, objQuoraFunct,
                processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryinfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                FilterAndStartFinalProcessForAnswers(queryinfo, jobProcessResult, new List<AnswerDetails> { new AnswerDetails { AnswerUrl = queryinfo.QueryValue } });
            }
            finally
            {
                if (_browser?.BrowserWindow != null) _browser.CloseBrowser();
            }
        }
    }
}
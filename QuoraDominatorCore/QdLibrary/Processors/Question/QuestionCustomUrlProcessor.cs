using System.Collections.Generic;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;

namespace QuoraDominatorCore.QdLibrary.Processors.Question
{
    public class QuestionCustomUrlProcessor : BaseQuoraProcessor
    {
        public QuestionCustomUrlProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IQuoraFunctions objQuoraFunct, IProcessScopeModel processScopeModel) :
            base(browser, jobProcess, dbAccountService, globalService, campaignService, objQuoraFunct,
                processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryinfo, ref JobProcessResult jobProcessResult)
        {
            FilterAndStartFinalProcessWithQuestionUrl(queryinfo, jobProcessResult,
                new List<string> {queryinfo.QueryValue});
        }
    }
}
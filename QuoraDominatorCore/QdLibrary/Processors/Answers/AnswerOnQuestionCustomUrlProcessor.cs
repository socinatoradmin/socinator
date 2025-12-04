using System;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.Response;

namespace QuoraDominatorCore.QdLibrary.Processors.Answers
{
    public class AnswerOnQuestionCustomUrlProcessor : BaseQuoraProcessor
    {
        public AnswerOnQuestionCustomUrlProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
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
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                
                QuestionDetailsResponseHandler objQuestionDetailsResponseHandler = null;
                if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    objQuestionDetailsResponseHandler =
                        quoraFunct.QuestionDetails(JobProcess.DominatorAccountModel, queryInfo.QueryValue);
                }
                else
                {
                    var resp = _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, queryInfo.QueryValue);
                    objQuestionDetailsResponseHandler = new QuestionDetailsResponseHandler(resp);
                }
                var quoraUser = new QuoraUser
                {
                    Url = queryInfo.QueryValue,
                    Uid = objQuestionDetailsResponseHandler.Qid
                };
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                jobProcessResult =
                    FilterAndStartFinalProcessForAnswerOnQuestion(queryInfo, quoraUser,
                        objQuestionDetailsResponseHandler);
            }
            catch (OperationCanceledException)
            {
                if (_browser.BrowserWindow != null) _browser.CloseBrowser();
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
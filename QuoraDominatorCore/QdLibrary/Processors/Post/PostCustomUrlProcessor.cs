using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.Response;
using System;

namespace QuoraDominatorCore.QdLibrary.Processors.Post
{
    public class PostCustomUrlProcessor :BaseQuoraProcessor
    {
        public PostCustomUrlProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
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
                PostByKeywordResponseHandler objResponseHandler = null;
                IResponseParameter Response = null;
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                if (!IsBrowser)
                     Response = quoraFunct.CustomPostResponse(queryInfo.QueryValue);
                else
                     Response = _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, queryInfo.QueryValue);
               objResponseHandler = new PostByKeywordResponseHandler(Response, IsBrowser);
               FilterAndStartProcessForPosts(queryInfo, jobProcessResult, objResponseHandler.postDetailsList);
            }
            catch (Exception)
            {
            }
            finally { if (_browser != null && _browser.BrowserWindow != null) _browser.CloseBrowser();
                jobProcessResult.IsProcessCompleted = true;
            }
        }
    }
}

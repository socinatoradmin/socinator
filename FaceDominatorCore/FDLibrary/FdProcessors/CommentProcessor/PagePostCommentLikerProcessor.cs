using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDResponse.CommonResponse;
using FaceDominatorCore.Interface;
using System;

namespace FaceDominatorCore.FDLibrary.FdProcessors.CommentProcessor
{
    public class PagePostCommentLikerProcessor : BaseFbCommentLikerProcessor
    {
#pragma warning disable 414
        PostCommentorResponseHandler _objPostCommentorResponseHandler;
#pragma warning restore 414

        public PagePostCommentLikerProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            _objPostCommentorResponseHandler = null;

        }


        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {

            string pageUrl = queryInfo.QueryValue;

            IResponseHandler responseHandler = null;

            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.SearchPostsByPageUrl(AccountModel, FbEntityType.Fanpage, pageUrl);

            try
            {
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        responseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetDataForPost(AccountModel, FbEntityType.Post, 7, 0)
                            : ObjFdRequestLibrary.GetPostListFromFanpages(AccountModel, pageUrl, responseHandler);

                        if (responseHandler != null && responseHandler.Status)
                        {
                            var lstPostId = responseHandler.ObjFdScraperResponseParameters.ListPostDetails;

                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, lstPostId.Count, queryInfo.QueryType, queryInfo.QueryValue, _ActivityType);

                            FilterAndStartFinalProcessForEachPost(queryInfo, jobProcessResult, lstPostId);
                            jobProcessResult.maxId = responseHandler.PageletData;

                            jobProcessResult.HasNoResult = !responseHandler.HasMoreResults;
                        }
                        else
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            jobProcessResult.HasNoResult = true;
                            jobProcessResult.maxId = null;
                        }
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

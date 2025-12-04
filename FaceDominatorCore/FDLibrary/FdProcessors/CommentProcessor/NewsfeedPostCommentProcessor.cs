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
using System.Globalization;

namespace FaceDominatorCore.FDLibrary.FdProcessors.CommentProcessor
{
    public class NewsfeedPostCommentProcessor : BaseFbCommentLikerProcessor
    {
#pragma warning disable 414
        PostCommentorResponseHandler _objPostCommentorResponseHandler;
#pragma warning restore 414

        public NewsfeedPostCommentProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            _objPostCommentorResponseHandler = null;

        }


        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            IResponseHandler responseHandler = null;

            try
            {
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        responseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetDataForPost(AccountModel, FbEntityType.NewFeedPost, 7, 0)
                            : ObjFdRequestLibrary.GetPostListFromNewsFeed(AccountModel, responseHandler);

                        if (responseHandler.Status)
                        {
                            var lstPostId = responseHandler.ObjFdScraperResponseParameters.ListPostDetails;
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, lstPostId.Count, queryInfo.QueryType, queryInfo.QueryValue, _ActivityType);

                            FilterAndStartFinalProcessForEachPost(queryInfo, jobProcessResult, lstPostId, entityType: FbEntityType.NewFeedPost);
                            jobProcessResult.maxId = responseHandler.ObjFdScraperResponseParameters
                                .SectionId.ToString(CultureInfo.InvariantCulture);

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
                        jobProcessResult.HasNoResult = true;
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

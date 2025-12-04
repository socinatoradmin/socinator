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
using FaceDominatorCore.Interface;
using System;

namespace FaceDominatorCore.FDLibrary.FdProcessors.CommentProcessor
{
    public class GroupPostCommentLikerProcessor : BaseFbCommentLikerProcessor
    {
        public GroupPostCommentLikerProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            string groupUrl = queryInfo.QueryValue;

            IResponseHandler responseHandler = null;

            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.SearchPostsByGroupUrl(AccountModel, FbEntityType.Fanpage, groupUrl);

            try
            {
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        responseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetDataForPost(AccountModel, FbEntityType.Fanpage, 7, 0)
                            : ObjFdRequestLibrary.GetPostListFromGroupsNew(AccountModel, responseHandler, groupUrl);

                        if (responseHandler != null && responseHandler.Status)
                        {
                            var lstPostId = responseHandler.ObjFdScraperResponseParameters.ListPostDetails;
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, lstPostId.Count, queryInfo.QueryType, queryInfo.QueryValue, _ActivityType);
                            FilterAndStartFinalProcessForEachPost(queryInfo, jobProcessResult, lstPostId, entityType: FDEnums.FbEntityType.Groups);

                            jobProcessResult.maxId = responseHandler.PageletData;
                            if (!responseHandler.HasMoreResults)
                                jobProcessResult.HasNoResult = true;
                            else
                            {
                                jobProcessResult.HasNoResult = true;
                                jobProcessResult.maxId = null;
                            }
                        }
                        else
                        {
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

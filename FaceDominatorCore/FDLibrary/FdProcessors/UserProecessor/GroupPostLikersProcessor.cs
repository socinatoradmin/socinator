using DominatorHouseCore;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor
{
    public class GroupPostLikersProcessor : BaseFbUserProcessor
    {
        IResponseHandler ObjPostLikersResponseHandler { get; set; }

        public GroupPostLikersProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            ObjPostLikersResponseHandler = null;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            string groupUrl = queryInfo.QueryValue;

            IResponseHandler objScrapGroupPostListResponseHandler = null;

            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.SearchPostsByGroupUrl(AccountModel, FbEntityType.Fanpage, groupUrl);

            try
            {
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {

                        objScrapGroupPostListResponseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetDataForPost(AccountModel, FbEntityType.Fanpage, 10, 0)
                            : ObjFdRequestLibrary.GetPostListFromGroupsNew(AccountModel, objScrapGroupPostListResponseHandler, groupUrl);

                        if (objScrapGroupPostListResponseHandler.Status)
                        {
                            List<string> lstPostId = objScrapGroupPostListResponseHandler.ObjFdScraperResponseParameters.ListPostDetails.Select(x => x.Id).ToList();

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, lstPostId.Count, queryInfo.QueryType, queryInfo.QueryValue, _ActivityType);

                            FilterAndStartFinalProcessForEachGroups(queryInfo, jobProcessResult, lstPostId);
                            jobProcessResult.maxId = objScrapGroupPostListResponseHandler.PageletData;

                            jobProcessResult.HasNoResult = !objScrapGroupPostListResponseHandler.HasMoreResults;

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
                        throw;
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
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void FilterAndStartFinalProcessForEachGroups(QueryInfo queryInfo, JobProcessResult jobProcessResult, List<string> lstPostIds)
        {
            try
            {
                IResponseHandler objPostLikersResponseHandler = null;

                foreach (string postid in lstPostIds)
                {
                    if (AccountModel.IsRunProcessThroughBrowser)
                        Browsermanager.SearchPostReactions(AccountModel, BrowserReactionType.Like, $"{FdConstants.FbHomeUrl}{postid}");

                    Browsermanager.CheckGotReactions(AccountModel, ref jobProcessResult);

                    while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                    {
                        var noOfPagesToScroll = 5;

                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        try
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            objPostLikersResponseHandler = AccountModel.IsRunProcessThroughBrowser
                                ? Browsermanager.ScrollWindowAndGetData(AccountModel, FbEntityType.PostLikers, noOfPagesToScroll, 0, FdConstants.ReactionUserElement3)
                                : ObjFdRequestLibrary.GetPostLikers(AccountModel, postid, objPostLikersResponseHandler);

                            if (objPostLikersResponseHandler.Status)
                            {
                                List<FacebookUser> lstFacebookUser = objPostLikersResponseHandler.ObjFdScraperResponseParameters.ListUser;
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"Found {lstFacebookUser.Count} likers for post Url {FdConstants.FbHomeUrl}{postid}");

                                ProcessDataOfUsers(queryInfo, ref jobProcessResult, lstFacebookUser);

                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                                jobProcessResult.maxId = objPostLikersResponseHandler.ObjFdScraperResponseParameters.TotalCount;
                                jobProcessResult.HasNoResult = !objPostLikersResponseHandler.HasMoreResults;
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
                            throw;
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }

                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}

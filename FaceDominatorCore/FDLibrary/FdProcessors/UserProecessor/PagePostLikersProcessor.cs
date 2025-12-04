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
    public class PagePostLikersProcessor : BaseFbUserProcessor
    {
        public PagePostLikersProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {

        }


        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            string pageUrl = queryInfo.QueryValue;

            IResponseHandler objScrapPostListFromFanpageResponseHandler = null;

            var processedPostList = new List<string>();

            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.SearchPostsByPageUrl(AccountModel, FbEntityType.Fanpage, pageUrl);


            try
            {
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {

                        objScrapPostListFromFanpageResponseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetDataForPost(AccountModel, FbEntityType.Fanpage, 5, 0)
                            : ObjFdRequestLibrary.GetPostListFromFanpages(AccountModel, pageUrl, objScrapPostListFromFanpageResponseHandler);

                        if (objScrapPostListFromFanpageResponseHandler != null
                            && objScrapPostListFromFanpageResponseHandler.Status)
                        {
                            List<string> lstPostId = objScrapPostListFromFanpageResponseHandler.ObjFdScraperResponseParameters.ListPostDetails.Select(x => x.Id).ToList();

                            lstPostId.RemoveAll(x => processedPostList.Any(y => y == x));

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, lstPostId.Count, queryInfo.QueryType, queryInfo.QueryValue, _ActivityType);

                            processedPostList.AddRange(lstPostId);

                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            FilterAndStartFinalProcessForEachPage(queryInfo, jobProcessResult, lstPostId);
                            jobProcessResult.maxId = objScrapPostListFromFanpageResponseHandler.PageletData;

                            jobProcessResult.HasNoResult = !objScrapPostListFromFanpageResponseHandler.HasMoreResults;

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
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        private void FilterAndStartFinalProcessForEachPage(QueryInfo queryInfo, JobProcessResult jobProcessResult, List<string> lstPostIds)
        {
            IResponseHandler objPostLikersResponseHandler = null;

            try
            {
                foreach (string postid in lstPostIds)
                {
                    if (AccountModel.IsRunProcessThroughBrowser)
                        Browsermanager.SearchPostReactions(AccountModel, BrowserReactionType.Like, $"{FdConstants.FbHomeUrl}{postid}");

                    while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                    {
                        var noOfPagesToScroll = 5;

                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        try
                        {
                            objPostLikersResponseHandler = AccountModel.IsRunProcessThroughBrowser
                                ? Browsermanager.ScrollWindowAndGetData(AccountModel, FbEntityType.PostLikers, noOfPagesToScroll, 0, FdConstants.ReactionUserElement3)
                                : ObjFdRequestLibrary.GetPostLikers(AccountModel, postid, objPostLikersResponseHandler);

                            if (objPostLikersResponseHandler.Status)
                            {
                                List<FacebookUser> lstFacebookUser = objPostLikersResponseHandler.ObjFdScraperResponseParameters.ListUser;
                                GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                    AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyNoLikersFound".FromResourceDictionary(), $"{lstFacebookUser.Count}", $"{FdConstants.FbHomeUrl}{postid}"));

                                lstFacebookUser = CheckBlacklistUser(lstFacebookUser);

                                ProcessDataOfUsers(queryInfo, ref jobProcessResult, lstFacebookUser);
                                jobProcessResult.maxId = objPostLikersResponseHandler.ObjFdScraperResponseParameters.TotalCount;
                                if (!objPostLikersResponseHandler.HasMoreResults)
                                {
                                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                        AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LagKeyNoMoreLikersForPost".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{postid}"));
                                    jobProcessResult.HasNoResult = true;
                                }
                            }
                            else
                            {
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                    AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LagKeyNoMoreLikersForPost".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{postid}"));
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

                    jobProcessResult.HasNoResult = false;
                }


                if (AccountModel.IsRunProcessThroughBrowser)
                    Browsermanager.GoToNextPagination(AccountModel);


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

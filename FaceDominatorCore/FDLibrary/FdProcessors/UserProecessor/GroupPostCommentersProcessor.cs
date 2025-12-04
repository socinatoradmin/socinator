using DominatorHouseCore;
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

namespace FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor
{
    public class GroupPostCommentersProcessor : BaseFbUserProcessor
    {
        public GroupPostCommentersProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary, IFdBrowserManager browserManager,
            IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager,
                processScopeModel)
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
                            ? Browsermanager.ScrollWindowAndGetDataForPost(AccountModel, FbEntityType.Fanpage, 4, 0)
                            : ObjFdRequestLibrary.GetPostListFromGroupsNew(AccountModel, responseHandler, groupUrl);

                        if (responseHandler != null && responseHandler.Status)
                        {
                            var lstPostId = responseHandler.ObjFdScraperResponseParameters.ListPostDetails;
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, lstPostId.Count, queryInfo.QueryType, queryInfo.QueryValue, _ActivityType);
                            FilterAndStartFinalProcessForEachPost(queryInfo, jobProcessResult, lstPostId);

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

        public void FilterAndStartFinalProcessForEachPost(QueryInfo queryInfo, JobProcessResult jobProcessResult, List<FacebookPostDetails> lstPostId)
        {
            try
            {
                foreach (var post in lstPostId)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        IResponseHandler PostCommentorResponseHandler = null;

                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, _ActivityType,
                            FdFunctions.FdFunctions.IsIntegerOnly(post.Id)
                                ? string.Format("LangKeyGettingCommentsForPostUrl".FromResourceDictionary(), post.Id.Contains($"{FdConstants.FbHomeUrl}") ? post.Id : $"{FdConstants.FbHomeUrl}{post.Id}")
                                : string.Format("LangKeyGettingCommentsForPostUrl".FromResourceDictionary(), $"{post.Id}"));

                        jobProcessResult.HasNoResult = false;

                        while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            try
                            {
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                                //In browser for custom Url diffrent and for Other Diffrent
                                if (AccountModel.IsRunProcessThroughBrowser)
                                    PostCommentorResponseHandler = Browsermanager.ScrollWindowAndGetDataForCommentsForSinglePost(AccountModel, post, FbEntityType.Groups, 2, new List<FdPostCommentDetails>(), 0);
                                else
                                    PostCommentorResponseHandler = ObjFdRequestLibrary.GetPostCommentor(
                                        JobProcess.DominatorAccountModel, FdFunctions.FdFunctions.IsIntegerOnly(post.Id)
                                            ? $"{FdConstants.FbHomeUrl}{post.Id}" : $"{post.Id}", PostCommentorResponseHandler, JobProcess.JobCancellationTokenSource.Token);

                                var postDetails = PostCommentorResponseHandler.ObjFdScraperResponseParameters.PostDetails;

                                if (PostCommentorResponseHandler.Status)
                                {
                                    List<FdPostCommentDetails> lstFdCommentDetails = PostCommentorResponseHandler.ObjFdScraperResponseParameters.CommentList;

                                    List<FacebookUser> lstFacebookUser = new List<FacebookUser>();
                                    lstFdCommentDetails.ForEach(x => lstFacebookUser.Add(new FacebookUser() { UserId = x.CommenterID, ProfileUrl = FdConstants.FbHomeUrl + x.CommenterID }));

                                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyNoCommentsFound".FromResourceDictionary(), lstFdCommentDetails.Count, FdConstants.FbHomeUrl, postDetails.Id));

                                    ProcessDataOfUsers(queryInfo, ref jobProcessResult, lstFacebookUser);

                                    jobProcessResult.maxId = PostCommentorResponseHandler.ObjFdScraperResponseParameters.Offset.ToString();
                                    if (!PostCommentorResponseHandler.HasMoreResults)
                                    {
                                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                        jobProcessResult.HasNoResult = true;
                                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyNoCommentsForPostUrl".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{postDetails.Id}")); jobProcessResult.HasNoResult = true;
                                    }
                                }
                                else
                                {
                                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                    jobProcessResult.HasNoResult = true;
                                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyNoMoreCommentsForPostUrl".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{postDetails.Id}")); jobProcessResult.HasNoResult = true;
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

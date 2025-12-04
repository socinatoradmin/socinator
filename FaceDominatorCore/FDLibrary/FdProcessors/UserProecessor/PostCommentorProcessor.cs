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
using Unity;

namespace FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor
{
    public class PostCommentorProcessor : BaseFbUserProcessor
    {
        IResponseHandler _objPostCommentorResponseHandler;

        public PostCommentorProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            _objPostCommentorResponseHandler = null;

        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();


            string postUrl = queryInfo.QueryValue;

            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.GetCommentersFromPost(AccountModel, BrowserReactionType.Comment, postUrl);


            try
            {
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        FacebookPostDetails postDetails = new FacebookPostDetails() { Id = postUrl, PostUrl = postUrl };

                        if (AccountModel.IsRunProcessThroughBrowser)
                        {
                            var userSpecificWindow = _accountScopeFactory[$"{AccountModel.AccountId}{postDetails.Id}"]
                            .Resolve<IFdBrowserManager>();
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            var postDetailsResponseHandler = userSpecificWindow.GetFullPostDetails(AccountModel, postDetails);
                            userSpecificWindow.CloseBrowser(AccountModel);
                            postDetails = postDetailsResponseHandler.ObjFdScraperResponseParameters.PostDetails;
                        }

                        //          JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        _objPostCommentorResponseHandler = AccountModel.IsRunProcessThroughBrowser
                            ? Browsermanager.ScrollWindowAndGetDataForCommentsForSinglePost(AccountModel, postDetails, FbEntityType.Fanpage, 2, new List<FdPostCommentDetails>(), 0)
                            : ObjFdRequestLibrary.GetPostCommentor(AccountModel, postUrl, _objPostCommentorResponseHandler, JobProcess.JobCancellationTokenSource.Token);

                        if (_objPostCommentorResponseHandler != null && _objPostCommentorResponseHandler.Status)
                        {
                            //while scraping comments this on used
                            if (_objPostCommentorResponseHandler.ObjFdScraperResponseParameters.ListUser == null)
                                _objPostCommentorResponseHandler.ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();

                            foreach (var comment in _objPostCommentorResponseHandler.ObjFdScraperResponseParameters.CommentList)
                            {
                                _objPostCommentorResponseHandler.ObjFdScraperResponseParameters.ListUser.Add(
                                    new FacebookUser()
                                    {
                                        UserId = comment.CommenterID,
                                        ProfileUrl = $"{FdConstants.FbHomeUrl}{comment.CommenterID}",
                                        FullName = comment.CommenterName,
                                        Username = comment.CommenterName,
                                        ScrapedProfileUrl = $"{FdConstants.FbHomeUrl}{comment.CommenterID}"
                                    });
                            }

                            List<FacebookUser> lstPostCmmentorDetails =
                                        _objPostCommentorResponseHandler.ObjFdScraperResponseParameters.ListUser;

                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, lstPostCmmentorDetails.Count, queryInfo.QueryType,
                                        queryInfo.QueryValue, _ActivityType);

                            ProcessDataOfUsers(queryInfo, ref jobProcessResult, lstPostCmmentorDetails);
                            jobProcessResult.maxId = _objPostCommentorResponseHandler.ObjFdScraperResponseParameters.ScrollCount.ToString();

                            jobProcessResult.HasNoResult = !_objPostCommentorResponseHandler.HasMoreResults;
                        }
                        else
                        {
                            jobProcessResult.HasNoResult = true;
                            jobProcessResult.maxId = null;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        jobProcessResult.HasNoResult = true;
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
            catch (Exception)
            {
                //ex.DebugLog();
            }
        }

    }
}


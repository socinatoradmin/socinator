using DominatorHouseCore;
using DominatorHouseCore.Enums;
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
using System.Threading;

namespace FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor
{
    public class FanpageLikersProcessor : BaseFbUserProcessor
    {
        private readonly ActivityType _activityType;
        public FanpageLikersProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            _activityType = jobProcess.ActivityType;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            int noOfPagesToScroll = 0;
            string pageUrl = queryInfo.QueryValue;

            IResponseHandler scrapPostListFromFanpageResponseHandler = null;
            IResponseHandler postLikersResponseHandler = null;

            List<FacebookUser> listOfUser = new List<FacebookUser>();


            try
            {
                List<string> lstUser = new List<string>();

                List<FacebookPostDetails> facebookPostDetailslList = new List<FacebookPostDetails>();

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    noOfPagesToScroll += 5;

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {

                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        #region commented
                        //_objFanpageLikersResponseHandler = ObjFdRequestLibrary.GetPageLikers
                        //    (AccountModel, pageUrl, _objFanpageLikersResponseHandler);

                        //get page posts 
                        #endregion


                        if (AccountModel.IsRunProcessThroughBrowser)
                        {
                            Browsermanager.LoadPageSource(AccountModel, $"{pageUrl}".Replace("//", "/"));
                            scrapPostListFromFanpageResponseHandler = Browsermanager.ScrollWindowAndGetDataForPost(AccountModel, FbEntityType.Fanpage, noOfPagesToScroll);
                        }
                        else
                        {
                            scrapPostListFromFanpageResponseHandler = ObjFdRequestLibrary.GetPostListFromFanpages(AccountModel, pageUrl,
                                scrapPostListFromFanpageResponseHandler);

                        }


                        scrapPostListFromFanpageResponseHandler.ObjFdScraperResponseParameters
                                .ListPostDetails.RemoveAll(x => lstUser.Any(y => y == x.Id));

                        facebookPostDetailslList = scrapPostListFromFanpageResponseHandler.ObjFdScraperResponseParameters
                            .ListPostDetails;

                        lstUser.AddRange(scrapPostListFromFanpageResponseHandler.ObjFdScraperResponseParameters
                            .ListPostDetails.Select(x => x.Id).ToList());

                        facebookPostDetailslList.Shuffle();

                        if (facebookPostDetailslList.Count != 0)
                        {

                            if (AccountModel.IsRunProcessThroughBrowser)
                            {
                                foreach (var postDetails in facebookPostDetailslList)
                                {
                                    Browsermanager.SearchPostReactions(AccountModel, DominatorHouseCore.Enums.FdQuery.BrowserReactionType.Like, postDetails.PostUrl);

                                    postLikersResponseHandler = Browsermanager.ScrollWindowAndGetData(AccountModel,
                                        FbEntityType.PostLikers, noOfPagesToScroll, 0, FdConstants.ReactionUserElement3);

                                    postLikersResponseHandler.ObjFdScraperResponseParameters.ListUser.Shuffle();

                                    if (postLikersResponseHandler.ObjFdScraperResponseParameters.ListUser.Count == 0)
                                    {
                                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                                                AccountModel.AccountBaseModel.UserName, _activityType, string.Format("LagKeyNoMoreLikersForPost".FromResourceDictionary(), $"{postDetails.PostUrl}"));
                                        continue;
                                    }

                                    ProcessDataOfUsers(queryInfo, ref jobProcessResult, postLikersResponseHandler.ObjFdScraperResponseParameters.ListUser);
                                }
                            }
                            else
                            {
                                foreach (var postDetails in facebookPostDetailslList)
                                {
                                    postLikersResponseHandler = null;

                                    do
                                    {
                                        postLikersResponseHandler = ObjFdRequestLibrary.GetPostLikers
                                            (AccountModel, FdConstants.FbHomeUrl + postDetails.Id, postLikersResponseHandler);

                                        listOfUser.AddRange(postLikersResponseHandler.ObjFdScraperResponseParameters.ListUser);

                                        Thread.Sleep(1500);

                                        #region Post Commentor and liker
                                        //postCommentorResponseHandler = ObjFdRequestLibrary.GetPostCommentor
                                        //    (AccountModel, FdConstants.FbHomeUrl + postDetails.Id, postCommentorResponseHandler);

                                        //List<FacebookUser> user = new List<FacebookUser>();
                                        //foreach (var userValue in postCommentorResponseHandler.ObjFdScraperResponseParameters.CommentList)
                                        //{
                                        //    user.Add(new FacebookUser()
                                        //    {
                                        //        UserId = FdFunctions.FdFunctions.GetIntegerOnlyString(userValue.CommenterID),
                                        //        ProfileUrl = $"{FdConstants.FbHomeUrl}{userValue.CommenterID}"
                                        //    });
                                        //}

                                        //listOfUser.AddRange(user);
                                        //Thread.Sleep(1500);

                                        //postSharerResponseHandler = ObjFdRequestLibrary.GetPostSharer
                                        //    (AccountModel, FdConstants.FbHomeUrl + postDetails.Id, postSharerResponseHandler);
                                        //listOfUser.AddRange(postSharerResponseHandler.ObjFdScraperResponseParameters.ListUser);

                                        //listOfUser.Distinct();

                                        #endregion

                                        if (listOfUser.Count != 0)
                                        {
                                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                                                  AccountModel.AccountBaseModel.UserName, listOfUser.Count, queryInfo.QueryType,
                                                  queryInfo.QueryValue, _activityType);

                                            listOfUser.Shuffle();

                                            ProcessDataOfUsers(queryInfo, ref jobProcessResult, listOfUser);
                                        }

                                        listOfUser.Clear();

                                    } while (postLikersResponseHandler.HasMoreResults);


                                }
                            }

                            jobProcessResult.HasNoResult = !scrapPostListFromFanpageResponseHandler.HasMoreResults;
                        }
                        else
                        {
                            jobProcessResult.HasNoResult = true;
                            jobProcessResult.maxId = null;
                        }


                        #region Commented

                        //if (_objFanpageLikersResponseHandler.Success
                        //    || _objFanpageLikersResponseHandler.HasMoreResults)
                        //{
                        //    List<FacebookUser> lstFacebookUser = _objFanpageLikersResponseHandler
                        //        .ObjFdPageLikersParameters.LstFacebookUser;

                        //    GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                        //        AccountModel.AccountBaseModel.UserName, lstFacebookUser.Count, queryInfo.QueryType,
                        //        queryInfo.QueryValue, ActivityType);

                        //    ProcessDataOfUsers(queryInfo, ref jobProcessResult, lstFacebookUser);
                        //    jobProcessResult.maxId = _objFanpageLikersResponseHandler.ObjFdPageLikersParameters
                        //        .PaginationData;

                        //    if (_objFanpageLikersResponseHandler.HasMoreResults == false)
                        //        jobProcessResult.HasNoResult = true;

                        //    count++;

                        //}
                        //else
                        //{
                        //    jobProcessResult.HasNoResult = true;
                        //    jobProcessResult.maxId = null;
                        //} 

                        #endregion

                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        jobProcessResult.HasNoResult = true;
                        jobProcessResult.IsProcessCompleted = true;
                        ex.DebugLog();
                    }
                }

            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

    }
}


using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity;

namespace FaceDominatorCore.FDLibrary.FdProcessors.PostProcessor
{
    public class BaseFbPostProcessor : BaseFbProcessor
    {
        protected Dictionary<string, Func<DominatorAccountModel, IResponseHandler, string, IResponseHandler>> ScraperFunctionActionTables;

        private static Dictionary<string, string> DictOwnPage { get; set; }

        protected int SuccessCount { get; set; }

        private readonly FdJobProcess _objFdJobProcess;

        private readonly IAccountScopeFactory _accountScopeFactory;

        protected BaseFbPostProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)

        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _objFdJobProcess = (FdJobProcess)jobProcess;

            SetActivity(processScopeModel);

            _activitySettings = processScopeModel.GetActivitySettingsAs<PostLikerModel>();

            ScraperFunctionActionTables = new Dictionary<string, Func<DominatorAccountModel, IResponseHandler, string, IResponseHandler>>()
                        {
                            {$"{PostOptions.Group.ToString()}", ObjFdRequestLibrary.GetPostListFromGroupsNew},

                            {$"{PostOptions.OwnWall.ToString()}", ObjFdRequestLibrary.GetPostListFromTimeline},

                            {$"{PostOptions.Pages.ToString()}", ObjFdRequestLibrary.GetPostListFromFanpagesNew},

                            {$"{PostOptions.FriendWall.ToString()}", ObjFdRequestLibrary.GetPostListFromFriendTimelineNew},

                            {$"{PostOptions.NewsFeed.ToString()}", ObjFdRequestLibrary.GetPostListFromNewsFeed},

                            {$"{PostOptions.Albums.ToString()}", ObjFdRequestLibrary.GetPostListFromAlbums},

                            {$"{PostOptions.Keyword.ToString()}", ObjFdRequestLibrary.GetPostListFromKeyWords},

                            { $"{PostOptions.ProfileScraper.ToString()}", ObjFdRequestLibrary.GetPostListFromFriendTimelineNew}
                        };
        }


        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {

        }

        public void ProcessDataOfPosts(ref JobProcessResult jobProcessResult,
                    List<FacebookPostDetails> objLstFacebookPostDetails, string queryType, string queryValue,bool ShowLog=false)
        {
            List<FacebookPostDetails> listPreviousIncompletedPosts = null;

            if (_ActivityType == ActivityType.PostCommentor && _activitySettings.IschkAllowMultipleComment)
            {
                listPreviousIncompletedPosts = GetInCompletedPostComments();

                var listFilteredData = CheckIfCommentExists(listPreviousIncompletedPosts);

                objLstFacebookPostDetails.RemoveAll(x => listFilteredData.Any(y => y.Id == x.Id
                || y.PostUrl == x.PostUrl));

                objLstFacebookPostDetails.InsertRange(0, listFilteredData);

            }

            var filteredPosts = false;
            int actorChangeNumber = 0;
            foreach (var fbpost in objLstFacebookPostDetails)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var post = fbpost;
                if (ShowLog)
                {
                    GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                    AccountModel.AccountBaseModel.UserName, 1, "Custom Post", post.PostUrl,
                            _ActivityType);
                }
                try
                {

                    if (string.IsNullOrEmpty(post.Id) && string.IsNullOrEmpty(post.PostUrl))
                        continue;
                    if (AlreadyInteractedPosts(post))
                    { filteredPosts = true; continue; }
                    if (queryType == PostOptions.CustomPostList.ToString())
                        queryValue = post.QueryValue;

                    var userSpecificWindow = _accountScopeFactory[$"{AccountModel.AccountId}{post.Id}"]
                                .Resolve<IFdBrowserManager>();
                    IResponseHandler responseHandler = null;
                    post.actorChangeNumber = actorChangeNumber;
                    var iscommented = false;
                    if (AccountModel.IsRunProcessThroughBrowser)
                    {
                        if (queryType == PostOptions.CustomPostList.ToString() || queryType == PostOptions.PostScraperCampaign.ToString() || queryType == PostOptions.Campaign.ToString())
                            responseHandler = Browsermanager.GetFullPostDetails(AccountModel, post, true);
                        Browsermanager.CloseBrowserCustom(AccountModel);
                    }
                    else
                        responseHandler = ObjFdRequestLibrary.GetPostDetailsNew(AccountModel, post);
                    if (responseHandler != null && responseHandler.Status)
                    {
                        post = responseHandler?.ObjFdScraperResponseParameters?.PostDetails;
                        if (AlreadyInteractedPosts(post))
                        { filteredPosts = true; continue; }
                        iscommented = responseHandler.ObjFdScraperResponseParameters.IsCommentedOnPost ? true : false;
                    }

                    if (post.EntityType == FbEntityTypes.Page)
                        actorChangeNumber++;

                    if (post.IsPendingPost)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"Skipping Post with post url {post.PostUrl} because it is a pending post");
                        continue;
                    }

                    if (!CheckPagePostConditions(ref jobProcessResult, post, iscommented))
                        continue;

                    if (_ActivityType == ActivityType.PostCommentor && !post.CanComment)
                    { filteredPosts = true; continue; }


                    if (post.MediaType == MediaType.NoMedia && _ActivityType == ActivityType.DownloadScraper)
                        continue;

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    post.MaxCommentsOnEachPost = _maxCommentsPerPost;

                    if (!CheckPostUniqueNess(jobProcessResult, post))
                    {
                        Thread.Sleep(1000);
                        jobProcessResult.HasNoResult = true;
                        return;
                    }

                    if (JobProcess.CampaignDetails != null)
                    {
                        if (!ApplyCampaignLevelSettings(new QueryInfo() { QueryType = queryType, QueryValue = queryValue }, post.PostUrl, JobProcess.CampaignDetails))
                        {
                            Thread.Sleep(1000);
                            continue;
                        }
                    }


                    FilterData(queryType, queryValue, ref jobProcessResult, post, iscommented);

                    if ((_ActivityType == ActivityType.PostLiker || _ActivityType == ActivityType.PostCommentor)
                        && _activitySettings.IsPerEntityRangeChecked &&
                        SuccessCount >= _activitySettings.MaximumCountPerEntity.GetRandom())
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, _ActivityType,
                            $"Entity with url has reached limit of {SuccessCount}!");
                        jobProcessResult.HasNoResult = true;
                        break;
                    }

                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException("Operation Cancel request");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            if (filteredPosts)
                GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, _ActivityType,
                            "Succefully Skipped the Filtered Posts");
            //if (queryType == "CustomPostList" || queryType == "Campaign")
            //{
            //    jobProcessResult.HasNoResult = true;
            //    jobProcessResult.IsProcessCompleted = true;
            //}

        }

        private List<FacebookPostDetails> CheckIfCommentExists(List<FacebookPostDetails> listPosts)
        {
            var filteredResults = new List<FacebookPostDetails>();

            try
            {
                listPosts.ForEach(post =>
                    {
                        if (_activitySettings.LikerCommentorConfigModel.LstManageCommentModel.
                                FirstOrDefault(x => x.SelectedQuery.Any(y => y.Content.QueryType == post.QueryType &&
                                 y.Content.QueryValue == post.QueryValue)) != null)
                            filteredResults.Add(post);

                    });

                return filteredResults;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new List<FacebookPostDetails>();
        }

        private List<FacebookPostDetails> GetInCompletedPostComments()
        {
            var postList = _dbAccountService.GetInteractedPosts(_ActivityType, false).ToList();

            var totalPostList = _dbAccountService.GetInteractedPosts(_ActivityType);

            postList.RemoveAll(x => totalPostList.Any(y => y.PostId == x.PostId && y.IsMoreCommentsNeeded == false));

            var lstPostsYetToComment = new List<FacebookPostDetails>();

            postList.ForEach(x =>
            {
                lstPostsYetToComment.Add(new FacebookPostDetails()
                {
                    Id = x.PostId.ToString(),
                    PostUrl = x.PostUrl ?? $"{FdConstants.FbHomeUrl}{x.PostId}",
                    QueryType = x.QueryType,
                    QueryValue = x.QueryValue
                });
            });

            return lstPostsYetToComment;
        }

        private void FilterData(string queryType, string queryValue, ref JobProcessResult jobProcessResult,
            FacebookPostDetails objFacebookPostDetails, bool isPostByPageAvailable)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (JobProcess.ModuleSetting.PostFilterModel.IsFilterByLikeCountChkd)
            {
                try
                {
                    if (!JobProcess.ModuleSetting.PostFilterModel.PostLikerCount.InRange
                        (Int32.Parse(objFacebookPostDetails.LikersCount)))
                    {
                        _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"Post with url {FdConstants.FbHomeUrl}{objFacebookPostDetails.Id} Not Matched with filter condition");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (JobProcess.ModuleSetting.PostFilterModel.IsFilterByShareCountChkd)
            {
                try
                {
                    if (!JobProcess.ModuleSetting.PostFilterModel.PostSharerCount.InRange
                        (Int32.Parse(objFacebookPostDetails.SharerCount)))
                    {
                        _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"Post with url {FdConstants.FbHomeUrl}{objFacebookPostDetails.Id} Not Matched with filter condition");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (JobProcess.ModuleSetting.PostFilterModel.IsFilterByCommentCountChkd)
            {
                try
                {
                    if (!JobProcess.ModuleSetting.PostFilterModel.PostCommentorCount.InRange
                        (Int32.Parse(objFacebookPostDetails.CommentorCount)))
                    {
                        _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"Post with url {FdConstants.FbHomeUrl}{objFacebookPostDetails.Id} Not Matched with filter condition");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (JobProcess.ModuleSetting.PostFilterModel.IsFilterByPostedDateTimeChkd)
            {
                try
                {
                    if (!JobProcess.ModuleSetting.PostFilterModel.PostedDateTime.InRange
                        ((DateTime.Now - objFacebookPostDetails.PostedDateTime).Days))
                    {
                        _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"Post with url {FdConstants.FbHomeUrl}{objFacebookPostDetails.Id} Not Matched with filter condition");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (JobProcess.ModuleSetting.PostFilterModel.IsFilterPostCategory)
            {
                try
                {
                    if (JobProcess.ModuleSetting.PostFilterModel.IgnorePostImages &&
                        objFacebookPostDetails.MediaType == MediaType.Image)
                    {
                        _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"Post with url {FdConstants.FbHomeUrl}{objFacebookPostDetails.Id} Not Matched with filter condition");
                        return;
                    }

                    if (JobProcess.ModuleSetting.PostFilterModel.IgnorePostVideos &&
                        objFacebookPostDetails.MediaType == MediaType.Video)
                    {
                        _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"Post with url {FdConstants.FbHomeUrl}{objFacebookPostDetails.Id} Not Matched with filter condition");
                        return;
                    }

                    if (JobProcess.ModuleSetting.PostFilterModel.IgnoreNoMedia
                        && objFacebookPostDetails.MediaType == MediaType.NoMedia)
                    {
                        _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"Post with url {FdConstants.FbHomeUrl}{objFacebookPostDetails.Id} Not Matched with filter condition");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (JobProcess.ModuleSetting.PostFilterModel.FilterByBlacklistedWhitelistedWordsInCaption)
            {
                try
                {
                    if (JobProcess.ModuleSetting.PostFilterModel.FilterRestrictedPostCaptionList)
                    {
                        if (JobProcess.ModuleSetting.PostFilterModel.RestrictedPostCaptionList.
                                    Any(x => objFacebookPostDetails.Caption.Contains(x)))
                        {
                            _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"Post with url {FdConstants.FbHomeUrl}{objFacebookPostDetails.Id} Not Matched with filter condition");
                            return;
                        }
                    }

                    if (JobProcess.ModuleSetting.PostFilterModel.FilterAcceptedPostCaptionList)
                    {
                        if (!JobProcess.ModuleSetting.PostFilterModel.RestrictedPostCaptionList.
                            All(x => objFacebookPostDetails.Caption.Contains(x)))
                        {
                            _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"Post with url {FdConstants.FbHomeUrl}{objFacebookPostDetails.Id} Not Matched with filter condition");
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if ((isPostByPageAvailable || objFacebookPostDetails.EntityType == FbEntityTypes.Page) && _ActivityType != ActivityType.PostScraper
                && _ActivityType != ActivityType.DownloadScraper)
            {
                FilterAndStartFinalProcessForEachPost(ref jobProcessResult,
                                    objFacebookPostDetails, queryType, queryValue);
            }
            else
                SendToPerformActivity(ref jobProcessResult, objFacebookPostDetails, queryType, queryValue);

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (jobProcessResult.IsProcessSuceessfull)
                SuccessCount++;
        }

        public bool CheckPagePostConditions(ref JobProcessResult jobProcessResult, FacebookPostDetails objFacebookPostDetails, bool isCommentByPageAvailable)
        {
            if (_activitySettings.IsActionasPageChecked)
                objFacebookPostDetails.EntityType = FbEntityTypes.Page;
            if (_ActivityType != ActivityType.PostScraper && _ActivityType != ActivityType.DownloadScraper)
            {
                if (!_activitySettings.IsActionasOwnAccountChecked && _activitySettings.IsActionasPageChecked
                    && (objFacebookPostDetails.EntityType != FbEntityTypes.Page && !isCommentByPageAvailable))
                {
                    jobProcessResult = new JobProcessResult { IsProcessSuceessfull = false };

                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"Cannot like post with post url {FdConstants.FbHomeUrl}{objFacebookPostDetails.Id} because its not a page post");
                    return false;
                }
            }

            return true;
        }

        //private void FilterAndStartFinalProcessForEachPostComment(ref JobProcessResult jobProcessResult,
        // FacebookPostDetails postDetails, string queryType, string queryValue, int perOptionCount)
        //{
        //    try
        //    {
        //        FdRequestLibrary objLibrary = new FdRequestLibrary();

        //        var postCommentorModel = JsonConvert.DeserializeObject<PostCommentorModel>(TemplatesFileManager.Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId).ActivitySettings);

        //        if (postCommentorModel.IsActionasOwnAccountChecked)
        //        {
        //            SendToPerformActivity(ref jobProcessResult,
        //                        postDetails, queryType, queryValue);

        //            if (JobProcess.IsStopped())
        //                return;

        //            currentJobActionCount = JobProcess.GetCurrentJobCount();

        //            if (currentJobActionCount >= perOptionCount)
        //            {
        //                jobProcessResult.IsProcessCompleted = true;
        //                return;
        //            }
        //        }
        //        if (postCommentorModel.IsActionasPageChecked)
        //        {
        //            if (DictOwnPage == null)
        //                DictOwnPage = new Dictionary<string, string>();

        //            foreach (var pageUrl in postCommentorModel.ListOwnPageUrl)
        //            {
        //                var pageId = DictOwnPage.ContainsKey(pageUrl) ? DictOwnPage[pageUrl] : string.Empty;

        //                if (string.IsNullOrEmpty(pageId))
        //                {
        //                    pageId = ObjFdRequestLibrary.GetPageIdFromUrl(AccountModel, pageUrl);
        //                    DictOwnPage.Add(pageUrl, pageId);
        //                }

        //                //var ownpages = accountDbOperation.Get<OwnPages>();

        //                if (_dbAccountService.Any<OwnPages>(y => y.PageId == pageId))
        //                {
        //                     SendToPerformActivityByPage(ref jobProcessResult,
        //                        postDetails, queryType, queryValue, pageId);
        //                }

        //                if (jobProcessResult.IsProcessSuceessfull)
        //                    currentJobActionCount++;

        //                if (JobProcess.IsStopped())
        //                    return;

        //                if (currentJobActionCount >= perOptionCount)
        //                {
        //                    jobProcessResult.IsProcessCompleted = true;
        //                    return;
        //                }
        //            }
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        jobProcessResult = new JobProcessResult();
        //    }
        //}

        private void FilterAndStartFinalProcessForEachPost(ref JobProcessResult jobProcessResult,
          FacebookPostDetails postDetails, string queryType, string queryValue)
        {
            try
            {
                if (_activitySettings.IsActionasOwnAccountChecked)
                {
                    SendToPerformActivity(ref jobProcessResult,
                        postDetails, queryType, queryValue);

                    if (JobProcess.IsStopped())
                        return;

                }

                if (_activitySettings.IsActionasPageChecked)
                {
                    if (DictOwnPage == null)
                        DictOwnPage = new Dictionary<string, string>();

                    foreach (var pageUrl in _activitySettings.ListOwnPageUrl)
                    {
                        var pageId = DictOwnPage.ContainsKey(pageUrl) ? DictOwnPage[pageUrl] : string.Empty;

                        if (string.IsNullOrEmpty(pageId))
                        {
                            pageId = ObjFdRequestLibrary.GetPageIdFromUrl(AccountModel, pageUrl);

                            try
                            {
                                DictOwnPage.Add(pageUrl, pageId);
                            }
                            catch (Exception)
                            {
                            }

                        }

                        if (_dbAccountService.Any<OwnPages>(y => y.PageId == pageId))
                        {

                            postDetails.LikePostAsPageId = pageId;

                            SendToPerformActivityByPage(ref jobProcessResult,
                                postDetails, queryType, queryValue, pageId);
                        }

                        if (JobProcess.IsStopped())
                            return;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Caencel Requests");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                jobProcessResult = new JobProcessResult();
            }
        }


        // ReSharper disable once RedundantAssignment
        private void SendToPerformActivityByPage(ref JobProcessResult jobProcessResult,
            FacebookPostDetails postDetails, string queryType, string queryValue, string pageId)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            postDetails.LikePostAsPageId = pageId;
            postDetails.QueryType = queryType;
            postDetails.QueryValue = queryValue;
            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew()
            {
                ResultPost = postDetails,
                QueryInfo = new QueryInfo()
                {
                    QueryType = queryType,
                    QueryValue = queryValue
                }
            });
        }


        // ReSharper disable once RedundantAssignment
        public void SendToPerformActivity(ref JobProcessResult jobProcessResult,
            FacebookPostDetails objFacebookPostDetails, string queryType, string queryValue)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            objFacebookPostDetails.QueryType = queryType;
            objFacebookPostDetails.QueryValue = queryValue;

            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew()
            {
                ResultPost = objFacebookPostDetails,
                QueryInfo = new QueryInfo() { QueryType = queryType, QueryValue = queryValue }
            });
        }

    }
}


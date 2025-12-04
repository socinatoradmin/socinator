using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.YdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using YoutubeDominatorCore.Response;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeLibrary.Processes;
using YoutubeDominatorCore.YoutubeLibrary.YdFunctions;
using YoutubeDominatorCore.YoutubeModels;
using YoutubeDominatorCore.YoutubeModels.EngageModel;
using YoutubeDominatorCore.YoutubeModels.ScraperModel;
using YoutubeDominatorCore.YoutubeModels.WatchVideoModel;

namespace YoutubeDominatorCore.YoutubeLibrary.Processors.Post
{
    internal abstract class BaseYoutubePostProcessor : BaseYoutubeProcessor
    {
        protected BaseYoutubePostProcessor(IYdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService,
            IYoutubeFunctionality youtubeFunctionality) : base(jobProcess, blackWhiteListHandler,
            campaignService, youtubeFunctionality)
        {
            SetActivatedPostFilter();

            if (ActType == ActivityType.Comment)
                _commentModel = JsonConvert.DeserializeObject<CommentModel>(InstanceProvider
                    .GetInstance<ITemplatesFileManager>().Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)
                    ?.ActivitySettings);

            else if (ActType == ActivityType.LikeComment)
                _likeCommentModel = JsonConvert.DeserializeObject<LikeCommentModel>(InstanceProvider
                    .GetInstance<ITemplatesFileManager>().Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)
                    ?.ActivitySettings);
            else if (ActType == ActivityType.CommentScraper)
                _commentScraperModel = JsonConvert.DeserializeObject<CommentScraperModel>(InstanceProvider
                    .GetInstance<ITemplatesFileManager>().Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)
                    ?.ActivitySettings);

            if (ActType == ActivityType.ViewIncreaser)
                _viewIncreaserModel =
                    JsonConvert.DeserializeObject<ViewIncreaserModel>(TemplateModel.ActivitySettings);

            try
            {
                var _youtubeModel = InstanceProvider.GetInstance<IGenericFileManager>()
                    .GetModel<YoutubeModel>(ConstantVariable.GetOtherYoutubeSettingsFile());
                _commentAccountWiseUnique = _youtubeModel.IsAccountWiseUnique;
            }
            catch
            {
                _commentAccountWiseUnique = false;
            }
        }

        private CommentModel _commentModel { get; }
        private LikeCommentModel _likeCommentModel { get; }
        private CommentScraperModel _commentScraperModel { get; }
        private ViewIncreaserModel _viewIncreaserModel { get; }
        private string _providedSpecificCommentId { get; set; }
        private bool _commentAccountWiseUnique { get; }

        public void StartProcessForListOfPosts(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<YoutubePost> postsList)
        {
            CustomLog($"Found {postsList.Count} post for {queryInfo.QueryValue}");

            var blacklistedFiltred = SkipBlackListOrWhiteList(postsList);
            var pauseVideo = ActType != ActivityType.ViewIncreaser;

            foreach (var post in blacklistedFiltred)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (ReturnIfFilterFailedOrLimitReached(queryInfo, ref jobProcessResult))
                    return;

                if (SkipTheUniqueOne(queryInfo, post.Code, post.ChannelId))
                    continue;

                if ((ActType == ActivityType.PostScraper || ActType == ActivityType.ViewIncreaser ||
                     ActType == ActivityType.ReportVideo) && AlreadyDoneInDB(queryInfo, post))
                    continue;

                var url = PostUrlWithCommentId(post.Code);

                if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    BrowserManager.LoadPageSource(JobProcess.DominatorAccountModel, url, clearandNeedResource: true);

                var postInfo = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser
                    ? BrowserManager.PostInfo(ActType, url, post.VideoLength != 0, delayBeforeHit: 0,
                        ActType == ActivityType.LikeComment || ActType == ActivityType.CommentScraper || ActType == ActivityType.ViewIncreaser, pauseVideo: pauseVideo)
                    : YoutubeFunction.GetPostDetails(JobProcess.DominatorAccountModel, url, post.VideoLength != 0,
                        1.5);



                var reasonIfFailed = ReasonToFailedPostInfoFetch(postInfo);

                if (reasonIfFailed != null)
                {
                    CustomLog($"Skipped to process video ({url}). Reason : {reasonIfFailed}");
                    continue;
                }

                if (SkipTheUniqueOne(queryInfo, post.Code, post.ChannelId))
                    continue;

                if (postInfo.YoutubePost.VideoLength == 0)
                    postInfo.YoutubePost.VideoLength = post.VideoLength;

                SetProvidedCommentIdIntoPostInfo(queryInfo);
                StartCustomPostProcess(queryInfo, ref jobProcessResult, postInfo.YoutubePost);

                if (!string.IsNullOrEmpty(_providedSpecificCommentId))
                {
                    JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
                    return;
                }
            }
            if (FilterOrFailed > 0)
                GlobusLogHelper.log.Info(Log.FilterApplied,
                                JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActType,
                                FilterOrFailed);
            if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser &&
                    BrowserManager.CustomBrowserWindow != null)
                BrowserManager.CloseBrowserCustom();
        }

        protected string ReasonToFailedPostInfoFetch(PostInfoYdResponseHandler postInfo)
        {
            if (postInfo == null)
                return "May be any other error would be occurred.";
            if (postInfo.IsLiveVideo)
                return "It's a live video.";
            if (postInfo.IsEmptyResponse)
                return "Got empty pagesource.";
            if (postInfo.CaptchaFound)
                return "Solve Captcha.";
            if (SkipBlackListOrWhiteList(new List<YoutubePost> { postInfo.YoutubePost }).Count == 0)
                return
                    $"The video's channel({(!string.IsNullOrEmpty(postInfo.YoutubePost.ChannelId) ? postInfo.YoutubePost.ChannelId : postInfo.YoutubePost.ChannelUsername)}) has been blacklisted.";

            if (postInfo.Success)
                return null;
            return "May be any other error would be occurred.";
        }

        public void StartCustomPostProcess(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            YoutubePost postInfo, bool isCustom = false)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (!CheckPostUniqueNess(jobProcessResult, postInfo, queryInfo))
                    return;

                if (!ApplyCampaignLevelSettings(queryInfo, postInfo.PostUrl))
                    return;

                if (AlreadyInteracted(postInfo))
                    return;

                if (CommnetsRequiredAndCommentDisabled(postInfo, isCustom))
                    return;

                if (FilterPostApply(postInfo, 1) || FilterChannelApply(VideoChannelDetails(postInfo), 1))
                    return;

                if (AlreadyDoneInDB(queryInfo, postInfo, isCustom))
                    return;

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                postInfo.InteractingCommentId = _providedSpecificCommentId;

                var lstCommentIds = GetPostCommentInfo(postInfo);
            RepeatWatchCount:
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (ActType == ActivityType.LikeComment || ActType == ActivityType.CommentScraper ||
                    ActType == ActivityType.Comment && _commentModel.ReplyToComments)
                    StartProcessForComment(postInfo, ref jobProcessResult, queryInfo, lstCommentIds);
                else
                    StartFinalProcess(ref jobProcessResult, queryInfo, post: postInfo);
                if (ActType == ActivityType.ViewIncreaser && _viewIncreaserModel.IsChkRepeatVideo)
                {
                    while (!AlreadyDoneInDB(queryInfo, postInfo, isCustom))
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        goto RepeatWatchCount;
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

        public void StartProcessForComment(YoutubePost post, ref JobProcessResult jobProcessResult, QueryInfo queryInfo,
            PostCommentScraperResponseHandler lstCommentIds)
        {
            jobProcessResult.maxId = string.Empty;
            var lessThan10 = lstCommentIds.ListOfCommentsInfo.Count <= 20;

            int? commentsCountToInteract = null;

            var isLikeComment = _likeCommentModel != null;
            var act = isLikeComment ? "liking" : _commentScraperModel != null ? "scraping" : "replying to";
            if (_commentScraperModel?.IsCheckScrapeNComments ??
                ((_likeCommentModel?.IsCheckLikeNComments ?? false) ||
                 (_commentModel?.IsCheckReplyNCommentsOnly ?? false)) &&
                string.IsNullOrEmpty(_providedSpecificCommentId))
            {
                commentsCountToInteract =
                    (isLikeComment ? _likeCommentModel.LikeNComments :
                        _commentScraperModel != null ? _commentScraperModel.ScrapeNComments :
                        _commentModel.ReplyNComments).GetRandom();
                CustomLog(
                    $"Got random count [{commentsCountToInteract}] for {act} comments in the video [{post.Code}] for this running query [{queryInfo.QueryType} {queryInfo.QueryValue}]");
            }

            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (lstCommentIds.ListOfCommentsInfo == null || lstCommentIds.ListOfCommentsInfo.Count == 0)

                {
                    CustomLog($"No more comments are found in this video {post.Code}");
                    jobProcessResult.maxId = null;
                    return;
                }

                if (!string.IsNullOrWhiteSpace(lstCommentIds.PostDataElements.ContinuationToken))
                    post.PostDataElements.ContinuationToken = lstCommentIds.PostDataElements.ContinuationToken;
                jobProcessResult.maxId = post.PostDataElements?.ContinuationToken;
                StartProcessForListOfComments(queryInfo, ref jobProcessResult, post, lstCommentIds.ListOfCommentsInfo, ref commentsCountToInteract);

                if (!string.IsNullOrEmpty(_providedSpecificCommentId))
                {
                    JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
                    break;
                }

                if (commentsCountToInteract == 0)
                {
                    CustomLog(
                        $"{act} comments count limit reached in the video [{post.Code}] for this running query [{queryInfo.QueryType} {queryInfo.QueryValue}]\nGoing for next video post if any..");
                    break;
                }

                if (lessThan10)
                    break;

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        var lastCommentId = lstCommentIds.ListOfCommentsInfo?.LastOrDefault()?.CommentId ?? "";
                        lstCommentIds = new PostCommentScraperResponseHandler();
                        lstCommentIds.ListOfCommentsInfo = GetVideoCommentInfoByBrowser(post, lastCommentId);
                        lstCommentIds.HasMoreResults = lstCommentIds.ListOfCommentsInfo.Count > 0;
                    }
                    else
                    {
                        lstCommentIds =
                            YoutubeFunction.ScrapePostCommentsDetails(JobProcess.DominatorAccountModel, post);
                        lessThan10 = string.IsNullOrWhiteSpace(lstCommentIds.PostDataElements.ContinuationToken);
                    }
                }
            }

            if (LimitCountForThisQuery != null)
                --LimitCountForThisQuery;
        }

        public void StartProcessForListOfComments(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            YoutubePost newYoutubePost, List<YoutubePostCommentModel> comments, ref int? commentsCountToInteract)
        {
            newYoutubePost.CommentLikeIndex = newYoutubePost.CommentLikeIndex == 20 ? 0 : newYoutubePost.CommentLikeIndex;
            foreach (var post in comments)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                try
                {

                    if (ActType == ActivityType.LikeComment &&
                        (post.CommentLikeStatus == InteractedPosts.LikeStatus.Like || LikeCommentToSpecificCommentsContainingText(post.CommentText))
                        || post.CommenterChannelId == JobProcess.DominatorAccountModel.AccountBaseModel.UserId
                        || ReplyToSpecificCommentsContainingText(post.CommentText)
                        || ActType == ActivityType.Comment && AlreadyInteractedPostComment(queryInfo, post.CommentId, newYoutubePost.PostUrl))
                    {
                        if (ActType == ActivityType.LikeComment && !string.IsNullOrEmpty(_providedSpecificCommentId))
                        {
                            CustomLog($"Already liked highlighted comment [{post.CommentText.Substring(0, post.CommentText.Length < 30 ? post.CommentText.Length : 30)} ...]");
                            JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
                            return;
                        }

                        if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                        {
                            if (post.CommentLikeStatus == InteractedPosts.LikeStatus.Like)
                                newYoutubePost.CommentLikeIndex = newYoutubePost.CommentLikeIndex + 1;
                            else
                                newYoutubePost.CommentLikeIndex = newYoutubePost.CommentLikeIndex + 2;
                            BrowserManager.BrowserDispatcher(YDEnums.BrowserFuncts.MouseScroll, 3, YdStatic.GetScreenResolution().Key / 2, YdStatic.GetScreenResolution().Key / 2, 0, -100, 0, 1);
                        }

                        continue;
                    }

                    if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser && string.IsNullOrWhiteSpace(post.CreateReplyParams))
                    {
                        if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                        {
                            newYoutubePost.CommentLikeIndex = newYoutubePost.CommentLikeIndex + 1;
                            BrowserManager.ScrollScreen(100);
                        }

                        FilterOrFailed++;
                        if (ReturnIfFilterFailedOrLimitReached(queryInfo, ref jobProcessResult))
                            return;
                        continue;
                    }

                    SetParamToPostFromCommentModel(ref newYoutubePost, post);

                    if (ActType == ActivityType.LikeComment || ActType == ActivityType.CommentScraper)
                    {
                        newYoutubePost.LikeCount = post.CommentLikesCount;
                        newYoutubePost.Reaction = post.CommentLikeStatus;
                        newYoutubePost.PublishedDate = post.CommentTime;
                    }

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    StartFinalProcess(ref jobProcessResult, queryInfo, post: newYoutubePost);
                    if (string.IsNullOrEmpty(_providedSpecificCommentId) && commentsCountToInteract != null &&
                        jobProcessResult.IsProcessSuceessfull)
                    {
                        --commentsCountToInteract;
                        if (commentsCountToInteract == 0)
                            return;
                    }

                    if (!string.IsNullOrEmpty(_providedSpecificCommentId))
                    {
                        JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
                        return;
                    }

                    if (jobProcessResult.IsProcessCompleted)
                        break;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        private bool LikeCommentToSpecificCommentsContainingText(string commentText)
        {
            if (_likeCommentModel != null && _likeCommentModel.IsCommentShouldContainsWordPhraseChecked)
            {
                //bool exp = !_likeCommentModel.CommentShouldContainsWordPhrase.ToLower().Split(new string[] { Environment.NewLine },
                //StringSplitOptions.None).Any(x => commentText.ToLower().Contains(x.ToLower()));
                return !_likeCommentModel.CommentShouldContainsWordPhrase.ToLower().Split(new string[] { Environment.NewLine },
    StringSplitOptions.None).Any(x => commentText.ToLower().Contains(x.ToLower()));
            }

            return false;
        }

        private bool ReplyToSpecificCommentsContainingText(string commentText)
        {
            if (_commentModel != null && _commentModel.IsCheckFixCommentCountForPost)
                return !commentText.ToLower().Contains(_commentModel.CommentIdForReply.ToLower());
            return false;
        }

        protected bool AlreadyDoneInDB(QueryInfo queryInfo, YoutubePost postInfo, bool isCustom = false)
        {
            var lastWatchCount = 0;
            var IsInteracted = false;
            if (ActType == ActivityType.ViewIncreaser)
            {
                if (!_viewIncreaserModel.IsChkRepeatVideo)
                    _viewIncreaserModel.ViewCountForRepeatVideo = 0;
                if (AlreadyInteractedPost(queryInfo, postInfo, ref lastWatchCount, _viewIncreaserModel.IsChkRepeatVideo,
                    _viewIncreaserModel.ViewCountForRepeatVideo, _viewIncreaserModel.RunEveryday,
                    _viewIncreaserModel.RepeatWatchAfterMinutes.GetRandom()))
                {
                    if (isCustom && !_viewIncreaserModel.IsChkRepeatVideo)
                        JobProcess.DominatorAccountModel.IsNeedToSchedule = false;
                    IsInteracted = true;
                }
                postInfo.TotalWatchingCount = lastWatchCount;
                return IsInteracted;
            }
            IsInteracted = !(ActType == ActivityType.LikeComment || ActType == ActivityType.Comment || ActType == ActivityType.CommentScraper) &&
                 AlreadyInteractedPost(queryInfo, postInfo, ref lastWatchCount);
            return IsInteracted;
        }

        private PostCommentScraperResponseHandler GetPostCommentInfo(YoutubePost newYoutubePost)
        {
            if (ActType != ActivityType.LikeComment && ActType != ActivityType.Comment &&
                ActType != ActivityType.CommentScraper || ActType == ActivityType.Comment &&
                !(_commentModel.CommentAsReplyOnlyChecked || _commentModel.ReplyToComments))
                return new PostCommentScraperResponseHandler();

            var lstCommentIds = new PostCommentScraperResponseHandler();
            lstCommentIds.ListOfCommentsInfo = new List<YoutubePostCommentModel>();
            lstCommentIds.ListOfCommentsInfo.AddRange(newYoutubePost.ListOfCommentsInfo ??
                                                      new List<YoutubePostCommentModel>());

            SetSpecificCommentIdReplyParam(ref newYoutubePost, lstCommentIds);

            return lstCommentIds;
        }

        private List<YoutubePostCommentModel> GetVideoCommentInfoByBrowser(YoutubePost youtubePost,
            string lastCommentId)
        {
            if (ActType != ActivityType.LikeComment ||
                ActType == ActivityType.Comment && _commentModel.CommentAsReplyOnlyChecked)
                return new List<YoutubePostCommentModel>();
            var index = 5 + youtubePost.CommentLikeIndex;

            var lstCommentIds = BrowserManager.ScrapePostCommentsDetails(ActType, lastCommentId, ref index, 3);

            if (lstCommentIds == null)
                lstCommentIds = new List<YoutubePostCommentModel>();

            if (!string.IsNullOrEmpty(_providedSpecificCommentId))
            {
                var replyParam =
                    lstCommentIds.FirstOrDefault(x => x.CommentId.ToLower() == _providedSpecificCommentId.ToLower());
                if (replyParam != null)
                    youtubePost.InteractingCommentId = replyParam.CommentId;
            }

            return lstCommentIds;
        }

        private void SetSpecificCommentIdReplyParam(ref YoutubePost youtubePost,
            PostCommentScraperResponseHandler lstCommentIds)
        {
            if (!string.IsNullOrEmpty(_providedSpecificCommentId))
            {
                if (string.IsNullOrEmpty(youtubePost.PostDataElements.TrackingParams) &&
                    !string.IsNullOrEmpty(lstCommentIds.PostDataElements.TrackingParams))
                    youtubePost.PostDataElements.TrackingParams = lstCommentIds.PostDataElements.TrackingParams;
                var replyParam = lstCommentIds.ListOfCommentsInfo.FirstOrDefault(x =>
                    x.CommentId.ToLower() == _providedSpecificCommentId.ToLower());

                SetParamToPostFromCommentModel(ref youtubePost, replyParam);
            }
        }


        private void SetParamToPostFromCommentModel(ref YoutubePost youtubePost, YoutubePostCommentModel replyParam)
        {
            if (replyParam != null)
            {
                youtubePost.InteractingCommentId = replyParam.CommentId;
                youtubePost.InteractingCommenterChannelId = replyParam.CommenterChannelId;
                youtubePost.InteractingCommentText = replyParam.CommentText;
                youtubePost.InteractingCommentLikeStatus = replyParam.CommentLikeStatus;
                youtubePost.InteractingCommenterName = replyParam.CommenterChannelName;
                youtubePost.CommentActionParam = replyParam.CommentActionParam;
                if (!string.IsNullOrEmpty(replyParam.TrackingParams))
                    youtubePost.PostDataElements.TrackingParams = replyParam.TrackingParams;

                if (!string.IsNullOrEmpty(replyParam.CreateReplyParams))
                    youtubePost.PostDataElements.CreateReplyParams = replyParam.CreateReplyParams;
            }
        }

        private YoutubeChannel VideoChannelDetails(YoutubePost post)
        {
            return new YoutubeChannel()
            {
                ChannelId = post.ChannelId,
                ChannelName = post.ChannelTitle,
                SubscriberCount = post.ChannelSubscriberCount,
                ChannelUsername = post.ChannelUsername,
                IsSubscribed = post.IsChannelSubscribed,
                ViewsCount = post.ChannelViewsCount
            };
        }

        protected void SetActivatedPostFilter()
        {
            try
            {
                var ytPostFilt = new ScrapeFilter.VideoFilterModel(ModuleSetting);

                if (ModuleSetting.VideoFilterModel.IsViewsCountChecked)
                    (FilterPostActionList ?? (FilterPostActionList = new List<Func<YoutubePost, bool>>())).Add(
                        ytPostFilt.FilterViewCount);

                if (ModuleSetting.VideoFilterModel.IsCommentsCountChecked)
                    (FilterPostActionList ?? (FilterPostActionList = new List<Func<YoutubePost, bool>>())).Add(
                        ytPostFilt.FilterCommentsCount);

                if (ModuleSetting.VideoFilterModel.IsLikesCountChecked)
                    (FilterPostActionList ?? (FilterPostActionList = new List<Func<YoutubePost, bool>>())).Add(
                        ytPostFilt.FilterLikeCount);

                if (ModuleSetting.VideoFilterModel.IsDislikesCountChecked)
                    (FilterPostActionList ?? (FilterPostActionList = new List<Func<YoutubePost, bool>>())).Add(
                        ytPostFilt.FilterDislikeCount);

                if (ModuleSetting.VideoFilterModel.IsVideoLengthInSecondsChecked)
                    (FilterPostActionList ?? (FilterPostActionList = new List<Func<YoutubePost, bool>>())).Add(
                        ytPostFilt.FilterVideoLength);

                if (ModuleSetting.VideoFilterModel.IsTitleShouldContainsWordPhraseChecked &&
                    !string.IsNullOrEmpty(ModuleSetting.VideoFilterModel.TitleShouldContainsWordPhrase))
                    (FilterPostActionList ?? (FilterPostActionList = new List<Func<YoutubePost, bool>>())).Add(
                        ytPostFilt.FilterTitleShouldContains);

                if (ModuleSetting.VideoFilterModel.IsTitleShouldNotContainsWordPhraseChecked &&
                    !string.IsNullOrEmpty(ModuleSetting.VideoFilterModel.TitleShouldNotContainsWordPhrase))
                    (FilterPostActionList ?? (FilterPostActionList = new List<Func<YoutubePost, bool>>())).Add(
                        ytPostFilt.FilterTitleShouldNotContains);

                if (ModuleSetting.VideoFilterModel.IsDescriptionShouldContainsWordPhraseChecked &&
                    !string.IsNullOrEmpty(ModuleSetting.VideoFilterModel.DescriptionShouldContainsWordPhrase))
                    (FilterPostActionList ?? (FilterPostActionList = new List<Func<YoutubePost, bool>>())).Add(
                        ytPostFilt.FilterDescriptionShouldContains);

                if (ModuleSetting.VideoFilterModel.IsDescriptionShouldNotContainsWordPhraseChecked &&
                    !string.IsNullOrEmpty(ModuleSetting.VideoFilterModel.DescriptionShouldNotContainsWordPhrase))
                    (FilterPostActionList ?? (FilterPostActionList = new List<Func<YoutubePost, bool>>())).Add(
                        ytPostFilt.FilterDescriptionShouldNotContains);

                if (ModuleSetting.VideoFilterModel.IsPublishedOnDaysBeforeChecked)
                    (FilterPostActionList ?? (FilterPostActionList = new List<Func<YoutubePost, bool>>())).Add(
                        ytPostFilt.FilterShouldNotBePublishedBeforeDays);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public bool FilterPostApply(YoutubePost youtubePost, int numberOfScrapedResults)
        {
            var filtered = false;

            if (youtubePost == null)
                return true;

            if (FilterPostActionList?.Count > 0)
                //CustomLog("Filtering Post => " + youtubePost.Code);

                foreach (var filterMethod in FilterPostActionList)
                    try
                    {
                        if (filterMethod(youtubePost))
                        {
                            FilterOrFailed++;
                            filtered = true;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

            return filtered;
        }

        protected bool AlreadyInteractedPost(QueryInfo queryInfo, YoutubePost post, ref int lastWatchCount,
            bool isRepeating = false, int getDataCount = 0, bool runEveryday = false, int repeatWatchAfterMinutes = 0)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (_commentAccountWiseUnique || string.IsNullOrEmpty(((YdJobProcess)JobProcess).CampaignId))
            {
                var alreadyUsed = DbAccountService.GetInteractedPostsWithSameQuery(ActType,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserId, post.PostUrl).ToList();

                foreach (var x in alreadyUsed)
                {
                    if (ActType == ActivityType.ViewIncreaser && post.PostUrl.Contains(x.VideoUrl))
                    {
                        var dateTime = x.InteractionTimeStamp.EpochToDateTimeLocal();
                        if (isRepeating)
                        {
                            var addMinutes = dateTime.AddMinutes(repeatWatchAfterMinutes);
                            if (addMinutes <= dateTime) return true;
                        }

                        lastWatchCount = x.ViewingCountPerAccount;
                        if (!runEveryday)
                        {
                            if (x.ViewingCountPerAccount >= getDataCount)
                                return true;
                            return false;
                        }

                        var todayPlusOne = DateTime.Today + TimeSpan.FromDays(1);
                        if (lastWatchCount > 0 && dateTime > DateTime.Today && dateTime < todayPlusOne)
                        {
                            var currentDayWatchCount = lastWatchCount % getDataCount;
                            if (currentDayWatchCount == 0)
                                return true;
                            return false;
                        }
                    }

                    if (ActType != ActivityType.ViewIncreaser && post.PostUrl.Contains(x.VideoUrl))
                        return true;
                }
            }
            else
            {
                var alreadyUsed = CampaignService.GetSpecificInterectedPost(ActType,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserId, post.PostUrl).ToList();

                foreach (var x in alreadyUsed)
                {
                    if (ActType == ActivityType.ViewIncreaser && post.PostUrl.Contains(x.VideoUrl))
                    {
                        var dateTime = x.InteractionTimeStamp.EpochToDateTimeLocal();
                        if (isRepeating)
                        {
                            var addMinutes = dateTime.AddMinutes(repeatWatchAfterMinutes);
                            if (addMinutes <= dateTime) return true;
                        }

                        lastWatchCount = x.ViewingCountPerAccount;
                        if (!runEveryday)
                        {
                            if (x.ViewingCountPerAccount >= getDataCount)
                                return true;
                            return false;
                        }

                        var todayPlusOne = DateTime.Today + TimeSpan.FromDays(1);
                        if (lastWatchCount > 0 && dateTime > DateTime.Today && dateTime < todayPlusOne)
                        {
                            var currentDayWatchCount = lastWatchCount % getDataCount;
                            if (currentDayWatchCount == 0)
                                return true;
                            return false;
                        }
                    }

                    if (ActType != ActivityType.ViewIncreaser && post.PostUrl.Contains(x.VideoUrl))
                        return true;
                }
            }

            return false;
        }

        protected bool AlreadyInteractedPostComment(QueryInfo queryInfo, string commentId, string videoUrl)
        {
            var alreadyUsed = DbAccountService.GetInteractedPostsWithSameQuery(ActType,
                JobProcess.DominatorAccountModel.AccountBaseModel.UserId, videoUrl).ToList();

            var isActivityDoneBefore = false;
            alreadyUsed.ForEach(x =>
            {
                if (x.InteractedCommentUrl?.Contains(commentId) ?? false)
                    isActivityDoneBefore = true;
            });
            return isActivityDoneBefore;
        }

        /// <summary>
        ///     get to know Video is already interacted by web respond video info
        /// </summary>
        /// <param name="postInfo"> Video information inside</param>
        /// <returns></returns>
        protected bool AlreadyInteracted(YoutubePost postInfo)
        {
            if (ActType == ActivityType.Dislike && postInfo.Reaction == InteractedPosts.LikeStatus.Dislike
                || ActType == ActivityType.Like && postInfo.Reaction == InteractedPosts.LikeStatus.Like)
            {
                CustomLog($"Already {ActType.ToString().ToLower()}d the video ({postInfo.Code})");
                return true;
            }

            return false;
        }

        protected void SetProvidedCommentIdIntoPostInfo(QueryInfo queryInfo)
        {
            var commentIdFromUrl = UrlCommentId(queryInfo.QueryValue);
            if (!string.IsNullOrEmpty(commentIdFromUrl) && queryInfo.QueryTypeEnum == "CustomUrls" &&
                (_commentModel?.CommentAsReplyOnlyChecked ?? false) ||
                (_likeCommentModel?.IsCheckedLikeSpecificCommentId ?? false))
                _providedSpecificCommentId = commentIdFromUrl;
        }

        protected string UrlCommentId(string url)
        {
            return Utilities.GetBetween(url + "&", "&lc=", "&");
        }
    }
}
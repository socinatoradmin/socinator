using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.YdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using YoutubeDominatorCore.Request;
using YoutubeDominatorCore.YDFactories;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeLibrary.YdFunctions;
using YoutubeDominatorCore.YoutubeModels;
using YoutubeDominatorCore.YoutubeModels.EngageModel;

namespace YoutubeDominatorCore.YoutubeLibrary.Processes
{
    public class CommentProcess : YdJobProcessInteracted<InteractedPosts>
    {
        private readonly IDbAccountServiceScoped _accountServiceScoped;
        private readonly IDbCampaignService _campaignService;

        private readonly IYoutubeFunctionality _youtubeFunctionality;
        private readonly YoutubeModel _youtubeModel;

        public CommentProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager,
            IJobActivityConfigurationManager jobActivityConfigurationManager,
            IYdQueryScraperFactory queryScraperFactory, IYdHttpHelper ydHttpHelper, IYoutubeLogInProcess ydLogInProcess,
            IDbCampaignService campaignService, IYoutubeFunctionality youtubeFunctionality)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ydHttpHelper, ydLogInProcess)
        {
            _campaignService = campaignService;
            _accountServiceScoped = accountServiceScoped;
            _youtubeFunctionality = youtubeFunctionality;
            EngageCommentModel = processScopeModel.GetActivitySettingsAs<CommentModel>();
            _youtubeModel = processScopeModel.GetActivitySettingsAs<YoutubeModel>();
            var moduleSettings = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

            if (moduleSettings.IsTemplateMadeByCampaignMode && EngageCommentModel.IsChkCommentUnique &&
                !string.IsNullOrEmpty(CampaignId))
                try
                {
                    lock (YdStatic.LockInitializingCommentLockDict)
                    {
                        if (!YdStatic.LockUniqueCommentsDict.ContainsKey(CampaignId))
                            YdStatic.LockUniqueCommentsDict[CampaignId] = new object();

                        if (EngageCommentModel.IsPostUniqueCommentFromEachAccount)
                        {
                            if (!YdStatic.UniqueCommentsListWithVideoId.ContainsKey(CampaignId))
                                YdStatic.UniqueCommentsListWithVideoId[CampaignId] =
                                    new Dictionary<string, List<string>>();

                            if (!YdStatic.FirstTimeUniqueCommentListFromDb.ContainsKey(CampaignId))
                                YdStatic.FirstTimeUniqueCommentListFromDb[CampaignId] = new Dictionary<string, bool>();
                        }
                        else
                        {
                            if (!YdStatic.UniqueCommentsListUniqueCmntUniqueAccount.ContainsKey(CampaignId))
                                YdStatic.UniqueCommentsListUniqueCmntUniqueAccount[CampaignId] = new List<string>();

                            if (!YdStatic.FirstTimeUniqueCommentListFromDbUniqueCmntUniqueAccount.ContainsKey(
                                CampaignId))
                                YdStatic.FirstTimeUniqueCommentListFromDbUniqueCmntUniqueAccount[CampaignId] = true;
                        }
                    }

                    try
                    {
                        var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                        _youtubeModel =
                            genericFileManager.GetModel<YoutubeModel>(ConstantVariable.GetOtherYoutubeSettingsFile());
                    }
                    catch
                    {
                        _youtubeModel = new YoutubeModel();
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
        }

        public CommentModel EngageCommentModel { get; set; }
        private string CommentText { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var youtubePost = (YoutubePost)scrapeResult.ResultPost;

            var isReply = EngageCommentModel.CommentAsReplyOnlyChecked || EngageCommentModel.ReplyToComments;

            var jobProcessResult = new JobProcessResult();

            var processingVideoId = string.IsNullOrWhiteSpace(youtubePost.InteractingCommentId)
                ? youtubePost.Code
                : YdStatic.VideoUrl(youtubePost.Code, youtubePost.InteractingCommentId);

            if (_youtubeModel.IsAccountWiseUnique && !(EngageCommentModel.CommentAsReplyOnlyChecked ||
                                                       EngageCommentModel.IsChkAllowMultipleCommentsOnSamePost ||
                                                       EngageCommentModel.IsCheckReplyNCommentsOnly ||
                                                       EngageCommentModel.IsCheckFixCommentCountForPost))
            {
                var alreadyUsed = DbAccountService.GetInteractedPost(ActivityType,
                    DominatorAccountModel.AccountBaseModel.UserName, youtubePost.PostUrl).ToList();
                if (alreadyUsed.Count > 10)
                {
                    return jobProcessResult;
                }
            }

            try
            {
                var commentList = new List<string>();

                foreach (var item in EngageCommentModel.LstDisplayManageCommentModel)
                    if (item.SelectedQuery.Any(x =>
                        x.Content.QueryValue.Trim() == scrapeResult.QueryInfo.QueryValue.Trim() || x.Content.QueryValue.Split('\t').LastOrDefault().Trim() == scrapeResult.QueryInfo.QueryValue.Trim()))
                    {
                        if (string.IsNullOrEmpty(item.CommentText))
                            continue;
                        if (EngageCommentModel.IsSpintax)
                                commentList.AddRange(SpinTexHelper.GetSpinTextsCollection(item.CommentText));
                        else
                            commentList.Add(item.CommentText.Trim());
                    }

                if (!string.IsNullOrEmpty(CampaignId) && EngageCommentModel.IsChkCommentUnique)
                {
                    lock (YdStatic.LockUniqueCommentsDict[CampaignId])
                    {
                        if (EngageCommentModel.IsPostUniqueCommentFromEachAccount)
                        {
                            if (!YdStatic.UniqueCommentsListWithVideoId[CampaignId].ContainsKey(processingVideoId))
                                YdStatic.UniqueCommentsListWithVideoId[CampaignId][processingVideoId] =
                                    new List<string>();

                            if (!YdStatic.FirstTimeUniqueCommentListFromDb[CampaignId].ContainsKey(processingVideoId))
                                YdStatic.FirstTimeUniqueCommentListFromDb[CampaignId][processingVideoId] = true;

                            YdStatic.UnusedCommentsForUnique(DominatorAccountModel.AccountBaseModel.AccountId,
                                CampaignId, processingVideoId, commentList, _campaignService, _accountServiceScoped,
                                _youtubeModel.IsCampainWiseUnique, _youtubeModel.IsAccountWiseUnique,
                                !string.IsNullOrWhiteSpace(youtubePost.InteractingCommentId),
                                ActivityType.Comment.ToString());
                            var selectedComment = "";
                            YdStatic.RemoveUsedUniqueComment(ref selectedComment, CampaignId, processingVideoId);
                            CommentText = selectedComment;
                        }
                        else
                        {
                            YdStatic.UnusedCommentsForUniqueCommentFromUniqueUser(CampaignId, commentList,
                                _campaignService, _accountServiceScoped, _youtubeModel.IsCampainWiseUnique,
                                ActivityType.Comment.ToString());
                            var selectedComment = "";
                            YdStatic.RemoveUsedUniqueCommentFromUniqueUser(ref selectedComment, CampaignId);
                            CommentText = selectedComment;
                        }

                        var multipleCount = EngageCommentModel.IsChkAllowMultipleCommentsOnSamePost
                            ? EngageCommentModel.MultipleActionCount
                            : 1;

                        if (!string.IsNullOrEmpty(CommentText) && EngageCommentModel.IsChkCommentUnique)
                        {
                            if (_youtubeModel.IsAccountWiseUnique)
                            {
                                var getCount = DbAccountService.GetInteractedPost(ActivityType,
                                        DominatorAccountModel.AccountBaseModel.UserName, youtubePost.PostUrl).ToList()
                                    .Count;
                                if (getCount >= multipleCount)
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.YouTube,
                                        DominatorAccountModel.AccountBaseModel.UserName,
                                        ActivityType,
                                        $"Limit({getCount}) reached to comment on => {youtubePost.PostUrl} ");
                                    if (IsCustom)
                                        DominatorAccountModel.IsNeedToSchedule = false;
                                    ProcessFaliedSoReAddSelctedCmnt(processingVideoId);
                                    jobProcessResult.IsProcessCompleted = true;
                                    return jobProcessResult;
                                }
                            }
                            else
                            {
                                var getCount = DbCampaignService.GetSpecificInterectedPost(ActivityType,
                                    DominatorAccountModel.AccountBaseModel.UserName,
                                    DominatorAccountModel.AccountBaseModel.UserId, youtubePost.PostUrl).Count;
                                if (getCount >= multipleCount)
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.YouTube,
                                        DominatorAccountModel.AccountBaseModel.UserName,
                                        ActivityType,
                                        $"Limit({getCount}) reached to comment on => {youtubePost.PostUrl} ");
                                    if (IsCustom)
                                        DominatorAccountModel.IsNeedToSchedule = false;
                                    ProcessFaliedSoReAddSelctedCmnt(processingVideoId);
                                    jobProcessResult.IsProcessCompleted = true;
                                    return jobProcessResult;
                                }
                            }
                        }
                    }
                }
                else
                {
                    CommentText = commentList.GetRandomItem();
                    var post = youtubePost;

                    var dataGet = DbAccountService.Get<InteractedPosts>(x =>
                        x.ActivityType == "Comment" && x.VideoUrl == post.PostUrl && x.MyCommentedText == CommentText);

                    var alreadyPostedwithSameComment = isReply
                        ? dataGet.Where(y => y.InteractedCommentUrl == processingVideoId)?.FirstOrDefault()
                            ?.MyCommentedText
                        : dataGet.Where(y => y.VideoUrl.ToLower().Contains(post.Code.ToLower()))?.FirstOrDefault()
                            ?.MyCommentedText;

                    while (!string.IsNullOrEmpty(alreadyPostedwithSameComment))
                    {
                        commentList.Remove(alreadyPostedwithSameComment);
                        if (commentList.Count == 0)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.YouTube,
                                DominatorAccountModel.AccountBaseModel.UserName,
                                ActivityType,
                                $"Already commented \"{alreadyPostedwithSameComment}\" on the same post => {youtubePost.PostUrl}");
                            return jobProcessResult;
                        }

                        CommentText = commentList.GetRandomItem();

                        alreadyPostedwithSameComment = isReply
                            ? dataGet.Where(y => y.InteractedCommentUrl == processingVideoId)?.FirstOrDefault()
                                ?.MyCommentedText
                            : dataGet.Where(y => y.VideoUrl.ToLower().Contains(post.Code.ToLower()))?.FirstOrDefault()
                                ?.MyCommentedText;
                    }
                }

                if (string.IsNullOrEmpty(CommentText))
                {
                    jobProcessResult.IsProcessSuceessfull = false;
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.YouTube,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"No more comment texts found to comment So, stopped commenting. current url => {youtubePost.PostUrl}");

                    DominatorAccountModel.IsNeedToSchedule = false;
                    if (!EngageCommentModel.IsPostUniqueCommentFromEachAccount)
                        JobCancellationTokenSource.Cancel();

                    return jobProcessResult;
                }

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var mentionChannelInReply =
                    (EngageCommentModel.CommentAsReplyOnlyChecked || EngageCommentModel.ReplyToComments) &&
                    (youtubePost.InteractingCommentId?.Contains(".") ?? false)
                        ? "@" + $"{youtubePost.InteractingCommenterName} "
                        : "";

                var response = DominatorAccountModel.IsRunProcessThroughBrowser
                    ? BrowserManager.CommentOnVideo(youtubePost, CommentText, isReply, 2,
                        youtubePost.InteractingCommentId == null
                            ? 0
                            : (youtubePost.InteractingCommentId.Split('.').Count() - 1) * 2)
                    : _youtubeFunctionality.CommentOnVideo(DominatorAccountModel, ref youtubePost,
                        mentionChannelInReply + CommentText, isReply, 2);

                if (response.Success)
                {
                    var isReplyString = isReply ? " (as a reply) " : "";

                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, isReply ? ActivityType.Reply : ActivityType,
                        $"[{CommentText.Substring(0, CommentText.Length < 30 ? CommentText.Length : 30)}..]{isReplyString} in video {(isReply ? processingVideoId : youtubePost.PostUrl)}");
                    if (EngageCommentModel.IsCheckAfterCommentAction) //Like Post After Successful Comment
                        if (EngageCommentModel.IsCheckLikePostAfterComment)
                        {
                            var alreadyLikedThePost = DbAccountService.GetInteractedPost(ActivityType.Like,
                                DominatorAccountModel.AccountBaseModel.UserName, youtubePost.PostUrl).ToList();
                            if (alreadyLikedThePost.Count > 0)
                            {
                                GlobusLogHelper.log.Info(
                                    $"Already liked the same post [ {youtubePost.PostUrl} ] with user [ {DominatorAccountModel.AccountBaseModel.UserName} ]");
                                return jobProcessResult;
                            }

                            var _response = DominatorAccountModel.IsRunProcessThroughBrowser
                                ? _youtubeLogInProcess.BrowserManager.LikeDislikeVideoAfterComment(ActivityType.Like,
                                    youtubePost, 2)
                                : _youtubeFunctionality.LikeDislikeVideo(DominatorAccountModel, ActivityType.Like,
                                    ref youtubePost);

                            if (_response.Success)
                            {
                                var successMessage = $"Successful To Like on post {youtubePost.PostUrl}";
                                GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType.Like,
                                    successMessage);

                                IncrementCounters();
                                youtubePost.Reaction = InteractedPosts.LikeStatus.Like;
                                jobProcessResult.IsProcessSuceessfull = true;
                                if (IsCustom)
                                    DominatorAccountModel.IsNeedToSchedule = false;
                            }
                            else
                            {
                                GlobusLogHelper.log.Info($"Failed To Like on post {youtubePost.PostUrl}");
                            }
                        }

                    AddToDataBase(youtubePost, CommentText, response.CommentId, scrapeResult.QueryInfo);
                    jobProcessResult.IsProcessSuceessfull = true;
                    IncrementCounters();
                }
                else
                {
                    ProcessFaliedSoReAddSelctedCmnt(processingVideoId);
                    FailedLog(youtubePost.PostUrl);
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                if (EngageCommentModel != null && EngageCommentModel.EnableDelayBetweenPerformingActionOnSamePost)
                    DelayBeforeNextActivity(EngageCommentModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
                else
                    DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                ProcessFaliedSoReAddSelctedCmnt(processingVideoId);
            }
            catch (Exception e)
            {
                e.DebugLog();
            }

            return jobProcessResult;
        }

        private void ProcessFaliedSoReAddSelctedCmnt(string processingVideoId)
        {
            if (EngageCommentModel.IsChkCommentUnique && !string.IsNullOrEmpty(CommentText))
                lock (YdStatic.LockUniqueCommentsDict[CampaignId])
                {
                    if (EngageCommentModel.IsPostUniqueCommentFromEachAccount)
                        YdStatic.UniqueCommentsListWithVideoId[CampaignId][processingVideoId].Add(CommentText);
                    else
                        YdStatic.UniqueCommentsListUniqueCmntUniqueAccount[CampaignId].Add(CommentText);
                }
        }

        public void AddToDataBase(YoutubePost youtubePost, string commentText, string createdCommentId,
            QueryInfo queryInfo)
        {
            try
            {
                var interactedUrl = !string.IsNullOrWhiteSpace(youtubePost.InteractingCommentId)
                    ? YdStatic.VideoUrl(youtubePost.Code, youtubePost.InteractingCommentId)
                    : null;
                _accountServiceScoped.Add(new InteractedPosts
                {
                    AccountUsername = DominatorAccountModel.AccountBaseModel.UserName ?? "",
                    ActivityType = ActivityType.Comment.ToString(),
                    ChannelName = youtubePost.ChannelTitle ?? "",
                    ChannelId = youtubePost.ChannelId ?? "",
                    InteractedChannelUsername = youtubePost.ChannelUsername ?? "",
                    CommentCount = youtubePost.CommentCount ?? "",
                    CommentId = createdCommentId ?? "",
                    DislikeCount = youtubePost.DislikeCount ?? "",
                    InteractionTimeStamp = DateTime.UtcNow.GetCurrentEpochTime(),
                    LikeCount = youtubePost.LikeCount ?? "",
                    ReactionOnPost = youtubePost.Reaction,
                    PostDescription = youtubePost.Caption ?? "",
                    PublishedDate = youtubePost.PublishedDate ?? "",
                    QueryType = queryInfo.QueryType,
                    QueryValue = queryInfo.QueryValue,
                    SubscribeCount = youtubePost.ChannelSubscriberCount ?? "",
                    VideoLength = youtubePost.VideoLength,
                    VideoUrl = youtubePost.PostUrl ?? "",
                    ViewsCount = youtubePost.ViewsCount ?? "",
                    MyCommentedText = commentText ?? "",
                    MyChannelId = DominatorAccountModel.AccountBaseModel.UserId,
                    MyChannelPageId = DominatorAccountModel.AccountBaseModel.ProfileId,
                    PostTitle = youtubePost.Title,
                    CommenterChannelId = youtubePost.InteractingCommenterChannelId,
                    IsSubscribed = youtubePost.IsChannelSubscribed,
                    InteractedCommentUrl = interactedUrl
                });

                if (!string.IsNullOrEmpty(CampaignId))
                {
                    var existing =
                        _campaignService
                            .GetSingle<DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedPosts>(x =>
                                x.AccountUsername == DominatorAccountModel.AccountBaseModel.UserName && x.MyChannelId ==
                                                                                                     DominatorAccountModel
                                                                                                         .AccountBaseModel
                                                                                                         .UserId
                                                                                                     && x.VideoUrl ==
                                                                                                     youtubePost
                                                                                                         .PostUrl &&
                                                                                                     x.InteractedCommentUrl ==
                                                                                                     interactedUrl &&
                                                                                                     x.MyCommentedText ==
                                                                                                     commentText);
                    var existed = existing != null;
                    if (!existed)
                        existing = new DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedPosts();

                    existing.AccountUsername = DominatorAccountModel.AccountBaseModel.UserName ?? "";
                    existing.ActivityType = ActivityType.Comment.ToString();
                    existing.ChannelName = youtubePost.ChannelTitle ?? "";
                    existing.ChannelId = youtubePost.ChannelId ?? "";
                    existing.InteractedChannelUsername = youtubePost.ChannelUsername ?? "";
                    existing.CommentCount = youtubePost.CommentCount ?? "";
                    existing.CommentId = createdCommentId ?? "";
                    existing.DislikeCount = youtubePost.DislikeCount ?? "";
                    existing.InteractionTimeStamp = DateTime.UtcNow.GetCurrentEpochTime();
                    existing.LikeCount = youtubePost.LikeCount ?? "";
                    existing.ReactionOnPost = youtubePost.Reaction;
                    existing.PostDescription = youtubePost.Caption ?? "";
                    existing.PublishedDate = youtubePost.PublishedDate ?? "";
                    existing.QueryType = queryInfo.QueryType ?? "";
                    existing.QueryValue = queryInfo.QueryValue ?? "";
                    existing.SubscribeCount = youtubePost.ChannelSubscriberCount ?? "";
                    existing.VideoLength = youtubePost.VideoLength;
                    existing.VideoUrl = youtubePost.PostUrl ?? "";
                    existing.ViewsCount = youtubePost.ViewsCount ?? "";
                    existing.MyCommentedText = CommentText ?? "";
                    existing.MyChannelId = DominatorAccountModel.AccountBaseModel.UserId;
                    existing.MyChannelPageId = DominatorAccountModel.AccountBaseModel.ProfileId;
                    existing.Status = "Success";
                    existing.PostTitle = youtubePost.Title;
                    existing.CommenterChannelId = youtubePost.InteractingCommenterChannelId;
                    existing.IsSubscribed = youtubePost.IsChannelSubscribed;
                    existing.InteractedCommentUrl = interactedUrl;


                    if (existed)
                        _campaignService.Update(existing);
                    else
                        _campaignService.Add(existing);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
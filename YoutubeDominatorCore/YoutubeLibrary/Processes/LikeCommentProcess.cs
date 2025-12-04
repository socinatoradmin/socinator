using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.YdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using System;
using YoutubeDominatorCore.Request;
using YoutubeDominatorCore.Response;
using YoutubeDominatorCore.YDFactories;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeLibrary.YdFunctions;
using YoutubeDominatorCore.YoutubeModels;
using YoutubeDominatorCore.YoutubeModels.EngageModel;

namespace YoutubeDominatorCore.YoutubeLibrary.Processes
{
    public class LikeCommentProcess : YdJobProcessInteracted<InteractedPosts>
    {
        private readonly IDbAccountServiceScoped _accountServiceScoped;
        private readonly IDbCampaignService _campaignService;
        private readonly IYoutubeFunctionality _youtubeFunctionality;

        public LikeCommentProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, IYdQueryScraperFactory queryScraperFactory,
            IYdHttpHelper ydHttpHelper, IYoutubeLogInProcess ydLogInProcess, IDbCampaignService campaignService,
            IYoutubeFunctionality youtubeFunctionality)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ydHttpHelper, ydLogInProcess)
        {
            _campaignService = campaignService;
            _accountServiceScoped = accountServiceScoped;
            _youtubeFunctionality = youtubeFunctionality;
            EngageLikeCommentModel = processScopeModel.GetActivitySettingsAs<LikeCommentModel>();
        }

        public LikeCommentModel EngageLikeCommentModel { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var youtubePost = (YoutubePost)scrapeResult.ResultPost;

            var jobProcessResult = new JobProcessResult();
            try
            {
                var messageText = string.Empty;
                try
                {

                    if (EngageLikeCommentModel.LstDisplayManageCommentModel.Count > 0)
                        messageText = EngageLikeCommentModel.LstDisplayManageCommentModel.GetRandomItem().CommentText;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var response = DominatorAccountModel.IsRunProcessThroughBrowser
                    ? BrowserManager.LikeComments(youtubePost, messageText, 2)
                        ?
                        new LikeCommentResponseHandler { Success = true }
                        : new LikeCommentResponseHandler { Success = false }
                    : _youtubeFunctionality.LikeComments(DominatorAccountModel, ref youtubePost, messageText);

                if (response.Success)
                {
                    //      youtubePost.CommentLikeIndex = youtubePost.CommentLikeIndex + 2;
                    SuccessLog(
                        $"[{youtubePost.InteractingCommentText.Substring(0, youtubePost.InteractingCommentText.Length < 30 ? youtubePost.InteractingCommentText.Length : 30)}..] commented by {youtubePost.InteractingCommenterName}");

                    IncrementCounters();
                    AddToDataBase(youtubePost, scrapeResult.QueryInfo);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else if (!response.Success && youtubePost.InteractingCommentLikeStatus == InteractedPosts.LikeStatus.Like)
                {

                    CustomLog(
                         $"Successful to Skip As Already liked comment [{youtubePost.InteractingCommentText.Substring(0, youtubePost.InteractingCommentText.Length < 30 ? youtubePost.InteractingCommentText.Length : 30)}..] commented by {youtubePost.InteractingCommenterName} in video => {youtubePost.PostUrl}");
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                else
                {
                    FailedLog(
                        $"[{youtubePost.InteractingCommentText.Substring(0, youtubePost.InteractingCommentText.Length < 30 ? youtubePost.InteractingCommentText.Length : 30)}..] commented by {youtubePost.InteractingCommenterName}");
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                if (EngageLikeCommentModel != null && EngageLikeCommentModel.EnableDelayBetweenPerformingActionOnSamePost)
                    DelayBeforeNextActivity(EngageLikeCommentModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
                else
                    DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        public void AddToDataBase(YoutubePost youtubePost, QueryInfo queryInfo)
        {
            try
            {
                _accountServiceScoped.Add(new InteractedPosts
                {
                    AccountUsername = DominatorAccountModel.AccountBaseModel.UserName ?? "",
                    ActivityType = ActivityType.LikeComment.ToString(),
                    MyCommentedText = youtubePost.InteractingCommentText,
                    CommentId = youtubePost.InteractingCommentId,
                    ChannelId = youtubePost.InteractingCommenterChannelId ?? "",
                    InteractedChannelUsername = youtubePost.ChannelUsername ?? "",
                    ChannelName = youtubePost.InteractingCommenterName ?? "",
                    CommentCount = youtubePost.CommentCount ?? "",
                    LikeCount = youtubePost.LikeCount ?? "",
                    ReactionOnPost = InteractedPosts.LikeStatus.Like,
                    InteractionTimeStamp = DateTime.Now.GetCurrentEpochTime(),
                    PostDescription = youtubePost.Caption,
                    PublishedDate = youtubePost.PublishedDate ?? "",
                    QueryType = queryInfo.QueryType ?? "",
                    QueryValue = queryInfo.QueryValue ?? "",
                    VideoLength = youtubePost.VideoLength,
                    VideoUrl = youtubePost.PostUrl ?? "",
                    ViewsCount = youtubePost.ViewsCount ?? "",
                    MyChannelId = string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.UserId) ? YdStatic.DefaultChannel : DominatorAccountModel.AccountBaseModel.UserId,
                    MyChannelPageId = DominatorAccountModel.AccountBaseModel.ProfileId,
                    PostTitle = youtubePost.Title,
                    InteractedCommentUrl = !string.IsNullOrWhiteSpace(youtubePost.InteractingCommentId)
                        ? YdStatic.VideoUrl(youtubePost.Code, youtubePost.InteractingCommentId)
                        : "",
                    IsSubscribed = youtubePost.IsChannelSubscribed
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
                                                                                                     x.CommentId ==
                                                                                                     youtubePost
                                                                                                         .InteractingCommentId);
                    var existed = existing != null;
                    if (!existed)
                        existing = new DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedPosts();

                    existing.AccountUsername = DominatorAccountModel.AccountBaseModel.UserName ?? "";
                    existing.ActivityType = ActivityType.LikeComment.ToString();
                    existing.MyCommentedText = youtubePost.InteractingCommentText;
                    existing.CommentId = youtubePost.InteractingCommentId;
                    existing.CommenterChannelId = youtubePost.InteractingCommenterChannelId ?? "";
                    existing.ChannelName = youtubePost.InteractingCommenterName ?? "";
                    existing.ChannelId = youtubePost.ChannelId;
                    existing.InteractedChannelUsername = youtubePost.ChannelUsername ?? "";
                    existing.CommentCount = youtubePost.CommentCount ?? "";
                    existing.LikeCount = youtubePost.LikeCount ?? "";
                    existing.ReactionOnPost = youtubePost.Reaction;
                    existing.InteractionTimeStamp = DateTime.UtcNow.GetCurrentEpochTime();
                    existing.PublishedDate = youtubePost.PublishedDate ?? "";
                    existing.QueryType = queryInfo.QueryType ?? "";
                    existing.QueryValue = queryInfo.QueryValue ?? "";
                    existing.VideoLength = youtubePost.VideoLength;
                    existing.VideoUrl = youtubePost.PostUrl ?? "";
                    existing.ViewsCount = youtubePost.ViewsCount ?? "";
                    existing.MyChannelId = string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.UserId) ? YdStatic.DefaultChannel : DominatorAccountModel.AccountBaseModel.UserId;
                    existing.MyChannelPageId = DominatorAccountModel.AccountBaseModel.ProfileId;
                    existing.Status = "Success";
                    existing.PostTitle = youtubePost.Title;
                    existing.InteractedCommentUrl = !string.IsNullOrWhiteSpace(youtubePost.InteractingCommentId)
                        ? YdStatic.VideoUrl(youtubePost.Code, youtubePost.InteractingCommentId)
                        : "";
                    existing.IsSubscribed = youtubePost.IsChannelSubscribed;

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
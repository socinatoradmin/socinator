using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.YdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using System;
using YoutubeDominatorCore.Request;
using YoutubeDominatorCore.YDFactories;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeModels;
using YoutubeDominatorCore.YoutubeModels.ScraperModel;

namespace YoutubeDominatorCore.YoutubeLibrary.Processes
{
    public class CommentScraperProcess : YdJobProcessInteracted<InteractedPosts>
    {
        private readonly IDbAccountServiceScoped _accountServiceScoped;
        private readonly IDbCampaignService _campaignService;

        public CommentScraperProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, IYdQueryScraperFactory queryScraperFactory,
            IYdHttpHelper ydHttpHelper, IYoutubeLogInProcess ydLogInProcess, IDbCampaignService campaignService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ydHttpHelper, ydLogInProcess)
        {
            _campaignService = campaignService;
            _accountServiceScoped = accountServiceScoped;
            EngageCommentScraperModel = processScopeModel.GetActivitySettingsAs<CommentScraperModel>();
        }

        public CommentScraperModel EngageCommentScraperModel { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var youtubePost = (YoutubePost)scrapeResult.ResultPost;

            var jobProcessResult = new JobProcessResult();

            try
            {
                IncrementCounters();
                jobProcessResult.IsProcessSuceessfull = true;
                AddToDataBase(youtubePost, scrapeResult.QueryInfo);
                SuccessLog(
                    $"[{youtubePost.InteractingCommentText.Substring(0, youtubePost.InteractingCommentText.Length < 30 ? youtubePost.InteractingCommentText.Length : 30)}..] commented by {youtubePost.InteractingCommenterName}");

                if (IsCustom)
                    DominatorAccountModel.IsNeedToSchedule = false;
                if (EngageCommentScraperModel != null && EngageCommentScraperModel.EnableDelayBetweenPerformingActionOnSamePost)
                    DelayBeforeNextActivity(EngageCommentScraperModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
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
                    ActivityType = ActivityType.CommentScraper.ToString(),
                    MyCommentedText = youtubePost.InteractingCommentText,
                    CommentId = youtubePost.InteractingCommentId,
                    ChannelId = youtubePost.InteractingCommenterChannelId ?? "",
                    InteractedChannelUsername = youtubePost.ChannelUsername ?? "",
                    ChannelName = youtubePost.InteractingCommenterName ?? "",
                    CommentCount = youtubePost.CommentCount ?? "",
                    LikeCount = youtubePost.LikeCount ?? "",
                    ReactionOnPost = youtubePost.Reaction,
                    InteractionTimeStamp = DateTime.Now.GetCurrentEpochTime(),
                    PostDescription = youtubePost.Caption ?? "",
                    PublishedDate = youtubePost.PublishedDate ?? "",
                    QueryType = queryInfo.QueryType ?? "",
                    QueryValue = queryInfo.QueryValue ?? "",
                    VideoLength = youtubePost.VideoLength,
                    VideoUrl = youtubePost.PostUrl ?? "",
                    ViewsCount = youtubePost.ViewsCount ?? "",
                    MyChannelId = DominatorAccountModel.AccountBaseModel.UserId,
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
                    existing.ActivityType = ActivityType.CommentScraper.ToString();
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
                    existing.MyChannelId = DominatorAccountModel.AccountBaseModel.UserId;
                    existing.MyChannelPageId = DominatorAccountModel.AccountBaseModel.ProfileId;
                    existing.Status = "Success";
                    existing.PostTitle = youtubePost.Title;
                    existing.InteractedCommentUrl = !string.IsNullOrWhiteSpace(youtubePost.InteractingCommentId)
                        ? YdStatic.VideoUrl(youtubePost.Code, youtubePost.InteractingCommentId)
                        : "";
                    existing.IsSubscribed = youtubePost.IsChannelSubscribed;
                    existing.PostDescription = youtubePost.Caption ?? "";

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
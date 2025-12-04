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
    public class ReportVideoProcess : YdJobProcessInteracted<InteractedPosts>
    {
        private readonly IDbAccountServiceScoped _accountServiceScoped;
        private readonly IDbCampaignService _campaignService;

        private readonly IYoutubeFunctionality _youtubeFunctionality;
        private readonly YoutubeModel _youtubeModel;

        public ReportVideoProcess(IProcessScopeModel processScopeModel,
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
            Model = processScopeModel.GetActivitySettingsAs<ReportVideoModel>();
            _youtubeModel = processScopeModel.GetActivitySettingsAs<YoutubeModel>();
            var moduleSettings = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

            if (moduleSettings.IsTemplateMadeByCampaignMode && Model.IsChkTextUnique &&
                !string.IsNullOrEmpty(CampaignId))
                try
                {
                    lock (YdStatic.LockInitializingCommentLockDict)
                    {
                        if (!YdStatic.LockUniqueCommentsDict.ContainsKey(CampaignId))
                            YdStatic.LockUniqueCommentsDict[CampaignId] = new object();

                        if (Model.IsPostUniqueTextFromEachAccount)
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

        public ReportVideoModel Model { get; set; }
        private string Text { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var YoutubePost = (YoutubePost)scrapeResult.ResultPost;

            var jobProcessResult = new JobProcessResult();

            var processingVideoId = YoutubePost.Code;

            try
            {
                var commentList = new List<string>();
                int option = 0, subOption = 0, min = 0, sec = 0;
                var ReportText = string.Empty;
                foreach (var item in Model.ListReportDetailsModel)
                    if (item.SelectedQuery.Any(x =>
                        x.Content.QueryValue.Trim() == scrapeResult.QueryInfo.QueryValue.Trim()))
                    {
                        if (item.IsSpinTax && !string.IsNullOrWhiteSpace(item.CommentText))
                            commentList.AddRange(SpinTexHelper.GetSpinMessageCollection(item.CommentText));
                        else
                            commentList.Add(item.CommentText?.Trim() ?? "");
                        option = item.ReportOption == 8 ? item.ReportOption + 1 : item.ReportOption-1;
                        ReportText = item.ReportText;
                        subOption = item.ReportSubOption;
                        TimestampInitialization(YoutubePost.VideoLength, item.VideoTimestampPercentage, ref min,
                            ref sec);
                    }

                if (!string.IsNullOrEmpty(CampaignId) && Model.IsChkTextUnique)
                    lock (YdStatic.LockUniqueCommentsDict[CampaignId])
                    {
                        if (Model.IsPostUniqueTextFromEachAccount)
                        {
                            if (!YdStatic.UniqueCommentsListWithVideoId[CampaignId].ContainsKey(processingVideoId))
                                YdStatic.UniqueCommentsListWithVideoId[CampaignId][processingVideoId] =
                                    new List<string>();

                            if (!YdStatic.FirstTimeUniqueCommentListFromDb[CampaignId].ContainsKey(processingVideoId))
                                YdStatic.FirstTimeUniqueCommentListFromDb[CampaignId][processingVideoId] = true;

                            YdStatic.UnusedCommentsForUnique(DominatorAccountModel.AccountBaseModel.AccountId,
                                CampaignId, processingVideoId, commentList, _campaignService, _accountServiceScoped,
                                _youtubeModel.IsCampainWiseUnique, _youtubeModel.IsAccountWiseUnique, false,
                                ActivityType.ReportVideo.ToString());
                            var Selectedcomment = "";
                            YdStatic.RemoveUsedUniqueComment(ref Selectedcomment, CampaignId, processingVideoId);
                            Text = Selectedcomment;
                        }
                        else
                        {
                            YdStatic.UnusedCommentsForUniqueCommentFromUniqueUser(CampaignId, commentList,
                                _campaignService, _accountServiceScoped, _youtubeModel.IsCampainWiseUnique,
                                ActivityType.ReportVideo.ToString());
                            var selectedText = "";
                            YdStatic.RemoveUsedUniqueCommentFromUniqueUser(ref selectedText, CampaignId);
                            Text = selectedText;
                        }
                    }
                else
                    Text = commentList.GetRandomItem();

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (option == 9 || (option == 10 && subOption == 0))
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"This Option cannot be Automated as it can lead to compromise account ");
                    jobProcessResult.IsProcessSuceessfull = false;
                    return jobProcessResult;
                }

                var response = DominatorAccountModel.IsRunProcessThroughBrowser
                    ? BrowserManager.ReportToVideo(DominatorAccountModel, YoutubePost, option, subOption,
                        Text.Substring(0, Text.Length < 499 ? Text.Length : 499), min, sec, 2,ReportText)
                    : _youtubeFunctionality.ReportToVideo(DominatorAccountModel, YoutubePost, option, subOption,
                        Text.Substring(0, Text.Length < 499 ? Text.Length : 499), min, sec, 2);

                if (response.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"{response.SelectedOptionToVideoReport} | Timestamp: {min}:{sec} |Text: {Text.Substring(0, Text.Length < 10 ? Text.Length : 10)}..] in video {YoutubePost.PostUrl}");

                    AddToDataBase(YoutubePost, Text, response.SelectedOptionToVideoReport, $"{min}:{sec}",
                        scrapeResult.QueryInfo);
                    jobProcessResult.IsProcessSuceessfull = true;
                    IncrementCounters();
                }
                else
                {
                    ProcessFaliedSoReAddSelctedCmnt(processingVideoId);
                    FailedLog(YoutubePost.PostUrl);
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                if (Model != null && Model.EnableDelayBetweenPerformingActionOnSamePost)
                    DelayBeforeNextActivity(Model.DelayBetweenPerformingActionOnSamePost.GetRandom());
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
            if (Model.IsChkTextUnique && !string.IsNullOrEmpty(Text))
                lock (YdStatic.LockUniqueCommentsDict[CampaignId])
                {
                    if (Model.IsPostUniqueTextFromEachAccount)
                        YdStatic.UniqueCommentsListWithVideoId[CampaignId][processingVideoId].Add(Text);
                    else
                        YdStatic.UniqueCommentsListUniqueCmntUniqueAccount[CampaignId].Add(Text);
                }
        }

        private void TimestampInitialization(int totalVideoLength, int percentageToGet, ref int min, ref int sec)
        {
            try
            {
                var lenghtAtThePercent = (int)(totalVideoLength / 100.00 * percentageToGet);
                min = lenghtAtThePercent / 60;
                sec = lenghtAtThePercent % 60;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AddToDataBase(YoutubePost youtubePost, string commentText, string selectedOptionToVideoReport,
            string selectedTimeStampToVideoReport, QueryInfo queryInfo)
        {
            try
            {
                _accountServiceScoped.Add(new InteractedPosts
                {
                    AccountUsername = DominatorAccountModel.AccountBaseModel.UserName ?? "",
                    ActivityType = ActivityType.ReportVideo.ToString(),
                    ChannelName = youtubePost.ChannelTitle ?? "",
                    ChannelId = youtubePost.ChannelId ?? "",
                    InteractedChannelUsername = youtubePost.ChannelUsername ?? "",
                    CommentCount = youtubePost.CommentCount ?? "",
                    CommentId = "",
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
                    SelectedOptionToVideoReport = selectedOptionToVideoReport,
                    SelectedTimeStampToVideoReport = selectedTimeStampToVideoReport
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
                                                                                                         .PostUrl);
                    var existed = existing != null;
                    if (!existed)
                        existing = new DominatorHouseCore.DatabaseHandler.YdTables.Campaign.InteractedPosts();

                    existing.AccountUsername = DominatorAccountModel.AccountBaseModel.UserName ?? "";
                    existing.ActivityType = ActivityType.Comment.ToString();
                    existing.ChannelName = youtubePost.ChannelTitle ?? "";
                    existing.ChannelId = youtubePost.ChannelId ?? "";
                    existing.InteractedChannelUsername = youtubePost.ChannelUsername ?? "";
                    existing.CommentCount = youtubePost.CommentCount ?? "";
                    existing.CommentId = "";
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
                    existing.MyCommentedText = Text ?? "";
                    existing.MyChannelId = DominatorAccountModel.AccountBaseModel.UserId;
                    existing.MyChannelPageId = DominatorAccountModel.AccountBaseModel.ProfileId;
                    existing.Status = "Success";
                    existing.PostTitle = youtubePost.Title;
                    existing.CommenterChannelId = youtubePost.InteractingCommenterChannelId;
                    existing.IsSubscribed = youtubePost.IsChannelSubscribed;
                    existing.SelectedOptionToVideoReport = selectedOptionToVideoReport;
                    existing.SelectedTimeStampToVideoReport = selectedTimeStampToVideoReport;

                    if (existed)
                        _campaignService.Update(existing);
                    else
                        _campaignService.Add(existing);
                }
            }
            catch (Exception Ex)
            {
                Ex.DebugLog();
            }
        }
    }
}